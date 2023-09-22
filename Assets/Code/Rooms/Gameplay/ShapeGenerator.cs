using System;
using System.Collections.Generic;
using UnityEngine;


public class ShapeGenerator : MonoBehaviour
{
    [SerializeField] private List<Shape> _availableShapes;

    [SerializeField] private List<Shape> _feverModeShapes;
    [SerializeField] private Shape _feverModeStartPrefab;
    public bool HasFeverModeStartPrefab => (null != _feverModeStartPrefab);

    [SerializeField] private Shape _obstaclePrefab;

    [SerializeField] private Shape _preFinalPrefab;
    [SerializeField] private Shape _finalPrefab;

    [SerializeField] private GameObject paperForFinalShape;

    [SerializeField] private int _minCountInSet = 3;
    [SerializeField] private int _maxCountInSet = 7;

    public float AdditionalInterval = 0;
    private float _requiredInterval = 0.3785841f * 2.0f; //strawberry size
    private float _obstacleInterval = 0.2f;

    public Conveyor ParentConveyor { get; set; }
    public event Action<ShapesChunk> ChunkReady;

    private int _shapesBeforeObstacleCounter = 0;
    private int _shapesBeforeObstacleCount = 0;

    private List<Shape> _prefabs;
    private List<Shape> _shapeSet;
    private Shape _nextPrefab;
    private Shape _currentShape;

    private float _distanceToNext;

    private bool _isActive;
    private bool _isFinishing = false;

    private int fatalObstaclesRight = 0;

    private bool isObstaclesAllowToUse = false;
    private float maxShapeLength = float.MaxValue;

    private bool feverModeEnabled = false;
    private LevelBalanceData levelBalanceData;
    
    private void Awake()
    {
        int currentLevel = Env.Instance.Inventory.CurrentLevelIndex;
        GameplayRoomConfig config = Env.Instance.Rooms.GameplayRoom.Config as GameplayRoomConfig;
        maxShapeLength = config.MaxShapeLengthOnLevel(currentLevel);

        _shapesBeforeObstacleCount = UnityEngine.Random.Range(3, 5);
        FillPrefabs();

        if (!levelBalanceData.IsNeedObstacles(currentLevel))
        {
            _obstaclePrefab = null;
        }
    }

    public void AllowObstacles()
    {
        isObstaclesAllowToUse = true;
    }

    private void FillPrefabs()
    {
        _prefabs = new List<Shape>();
        _shapeSet = new List<Shape>();

        if ((null != _availableShapes) && (0 < _availableShapes.Count))
        {
            foreach (Shape shape in _availableShapes)
            {
                if (shape.HasSet)
                    _shapeSet.Add(shape);
                _prefabs.Add(shape);
            }
        }
        else
        {
            foreach (var shapeName in Env.Instance.Inventory.AvailableShapes)
            {
                var shape = Env.Instance.Content.LoadContentAsset<Shape>(ContentAsset.AssetType.Shape, shapeName);

                if (shape.LengthSize <= maxShapeLength)
                {
                    if (shape.HasSet)
                        _shapeSet.Add(shape);
                    _prefabs.Add(shape);
                }

                if (null != shape.FinalShape)
                {
                    _finalPrefab = shape.FinalShape;
                }
            }
        }
    }

    public void StartGeneration()
    {
        if (!_nextPrefab)
        {
            var prefab = _prefabs[UnityEngine.Random.Range(0, _prefabs.Count)];
            Generate(prefab, false);
        }

        _isActive = true;
    }

    public void FillTape(float from, float to)
    {
        Shape shapePrefab = _prefabs[UnityEngine.Random.Range(0, _prefabs.Count)];

        from += shapePrefab.WidthHalf;
        transform.position = transform.position.SetX(from);

        Generate(shapePrefab, true);
        while (transform.position.x < to)
        {
            transform.position += new Vector3(0.1f, 0, 0);
            GenerateByDistance(_nextPrefab, true);
        }
    }

    private void GenerateByDistance(Shape prefab, bool initialFilling)
    {
        if (_currentShape.transform.position.x > transform.position.x - _distanceToNext)
            return;

       Generate(prefab, initialFilling);
    }

    private int feverShapesCounter = 0;
    private Shape GetNextFeverModeShape()
    {
        Shape feverShape = _feverModeShapes[feverShapesCounter];
        feverShapesCounter = (feverShapesCounter + 1) % _feverModeShapes.Count;
        return feverShape;
    }

    private void Generate(Shape prefab, bool initialFilling)
    {
        if (!feverModeEnabled && ParentConveyor.FeverModeEnabled)
        {
            feverModeEnabled = true;
            prefab = _feverModeStartPrefab;
            _distanceToNext = _currentShape.WidthHalf + prefab.WidthHalf + _obstacleInterval;
        }

        if (feverModeEnabled && !ParentConveyor.FeverModeEnabled)
        {
            feverModeEnabled = false;
        }
        
        if (feverModeEnabled && (prefab != _feverModeStartPrefab))
        {
            prefab = GetNextFeverModeShape();

        }
        
        if (!feverModeEnabled)
        {
            if (isObstaclesAllowToUse && (_shapesBeforeObstacleCounter >= _shapesBeforeObstacleCount) && !_isFinishing)
            {
                if (null != _obstaclePrefab)
                {
                    prefab = _obstaclePrefab;
                    _shapesBeforeObstacleCount = UnityEngine.Random.Range(4, 6);
                    _shapesBeforeObstacleCounter = 0;
                    _distanceToNext = _currentShape.WidthHalf + prefab.WidthHalf + _obstacleInterval;
                }
            }
        }

        if (_shapeSet.Contains(prefab) && !feverModeEnabled)
        {
            GenerateSet(prefab);
            IncrementObstacleCounter();
            return;
        }

        Vector3 nextItemPosition = (null == _currentShape) ? transform.position : (_currentShape.transform.position + Vector3.right * _distanceToNext);
        _currentShape = Instantiate(prefab, nextItemPosition, Quaternion.identity);
        ChunkReady?.Invoke(new ShapesChunk(feverModeEnabled, _currentShape));

        if (_isFinishing)
        {
            _isActive = false;
            return;
        }

        _nextPrefab = GetNextShape();
        IncrementObstacleCounter();

        float intervalBetweenShapes =  (prefab == _obstaclePrefab) ? _obstacleInterval : (_requiredInterval + AdditionalInterval);
        if (!ParentConveyor.FeverModeEnabled)
            _distanceToNext = prefab.WidthHalf + _nextPrefab.WidthHalf + intervalBetweenShapes;
        else
            _distanceToNext = prefab.WidthHalf * 3.0f;
    }

    private Shape GetNextShape()
    {
        GameplayRoomConfig config = Env.Instance.Rooms.GameplayRoom.Config as GameplayRoomConfig;

        float param = config.ShapesInverseCDF.Evaluate(UnityEngine.Random.Range(0.0f, 1.0f));
        int resultIndex = Mathf.Min(Mathf.Max((int)(param * _prefabs.Count), 0), _prefabs.Count - 1);

        return _prefabs[resultIndex];
    }

    private void IncrementObstacleCounter()
    {
        if (!isObstaclesAllowToUse) return;

        if(Env.Instance.Inventory.CurrentLevelIndex > 0)
            _shapesBeforeObstacleCounter++;
    }

    private void GenerateSet(Shape prefab)
    {
        Vector3 nextItemPosition = (null == _currentShape) ? transform.position : _currentShape.transform.position;
        float distance = (null == _currentShape) ? 0.0f : _distanceToNext;

        _currentShape = Instantiate(prefab, nextItemPosition + Vector3.right * distance, Quaternion.identity);

        List<Shape> newShapes = new List<Shape>
        {
            _currentShape
        };

        int currentSetCount = UnityEngine.Random.Range(_minCountInSet, _maxCountInSet);
        float accumulatedLength = _currentShape.LengthSize;

        for (int i = 0; i < currentSetCount; i++)
        {
            _nextPrefab = _shapeSet[UnityEngine.Random.Range(0, _shapeSet.Count)];
            accumulatedLength += _nextPrefab.LengthSize;
            if (accumulatedLength > maxShapeLength) break;

            distance += (_currentShape.WidthHalf + _nextPrefab.WidthHalf);
            _currentShape = Instantiate(_nextPrefab, nextItemPosition + Vector3.right * distance, Quaternion.identity);
            newShapes.Add(_currentShape);
        }

        ChunkReady?.Invoke(new ShapesChunk(feverModeEnabled, newShapes.ToArray()));

        _nextPrefab = _prefabs[UnityEngine.Random.Range(0, _prefabs.Count)];
        _distanceToNext = _currentShape.WidthHalf + _nextPrefab.WidthHalf + _requiredInterval + AdditionalInterval;
    }

    private void Update()
    {
        if (!_isActive)
            return;

        if (_isFinishing)
            return;

        GenerateByDistance(_nextPrefab, false);
    }

    public void GenerateFinish()
    {
        if (_currentShape == _obstaclePrefab)
            return;

        _isFinishing = true;
        _distanceToNext -= (_nextPrefab.WidthHalf + _requiredInterval + AdditionalInterval);
        _distanceToNext += _preFinalPrefab.WidthHalf;
        Generate(_preFinalPrefab, false);
        _distanceToNext = _preFinalPrefab.WidthHalf + _finalPrefab.WidthHalf;
        Generate(_finalPrefab, false);

        if (null != paperForFinalShape)
        {
            GameObject paperGo = GameObject.Instantiate(paperForFinalShape);
            paperGo.transform.parent = _currentShape.transform;
            paperGo.transform.localPosition = Vector3.zero;
            paperGo.transform.localRotation = Quaternion.identity;
            paperGo.transform.localScale = Vector3.one;
            paperGo.transform.localPosition = Vector3.zero;
        }
    }

    public void SetBalance(LevelBalanceData levelBalanceData)
    {
        this.levelBalanceData = levelBalanceData;
    }
}

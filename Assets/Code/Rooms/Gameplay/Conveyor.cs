using Modules.General;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    public int DestroyPoint { get { return -8; } }
    //public float InitialFillingPoint { get { return 2.5f; } }
    public int StartPoint { get { return 5; } }
    public Shape LastShape { get; private set; }

    [SerializeField] private ShineAnimation shapeShineOriginal;
    [SerializeField] private Color baseLineColor;
    [SerializeField] private Color feverLineColor;
    [SerializeField] private MeshRenderer lineMeshRenerer;
    private Material lineMaterial;
    private Color targetLineColor;
    private Color lineColor;

    [SerializeField] Transform _tape;
    [SerializeField] Collider _tapeSegment;
    private List<Transform> _tapeSegments;

    public float Speed = 1;

    public Dictionary<int, CollisionDetector> Colliders { get; private set; }

    public List<Shape> Shapes;
    public List<ShapesChunk> Chunks;

    public bool IsStopped { get; private set; } = false;

    private CreamCreator _creamCreator;

    private ShapeGenerator _generator;
    private bool _isFinishReady = false;

    private System.Action<ShapesChunk, float> onChunkFinished;
    private System.Action<GameObject> onKeyTouched;
    private System.Action<GameObject> onCoinTouched;

    private int maxKeysCount = 0;
    private int keysOnTape = 0;
    private int chunksPassed = 0;
    private int chunksPassedOnStart = 0;
    private float averageChunksCount;
    private bool canPlaceKeys = false;

    private int maxCoinsCount = 0;
    private int coinsOnTape = 0;
    private bool canPlaceCoins = false;

    public bool FeverModeEnabled { get; private set; } = false;
    private int chunksBeforeFeverMode = -1;
    public int FeverModeChunksCount { get; private set; } = -1;

    public float IntervalBetweenShapes {
        get {
            return _generator.AdditionalInterval;
        } set {
            _generator.AdditionalInterval = value;
        }
    }

    public void Init(ShapeGenerator generator, CreamCreator creamCreator, float averageChunksCount, 
                     System.Action<ShapesChunk, float> onChunkFinished, 
                     System.Action<GameObject> onKeyTouched, 
                     System.Action<GameObject> onCoinTouched)
    {
        this.averageChunksCount = averageChunksCount;
        this.onChunkFinished = onChunkFinished;
        this.onKeyTouched = onKeyTouched;
        this.onCoinTouched = onCoinTouched;
        
        lineMaterial = Material.Instantiate(lineMeshRenerer.material);
        lineMeshRenerer.material = lineMaterial;
        targetLineColor = baseLineColor;
        lineColor = targetLineColor;
        UpdateLineColor();

        _generator = generator;
        _creamCreator = creamCreator;
        _generator.transform.position = new Vector3(DestroyPoint, 0, 0);

        Chunks = new List<ShapesChunk>();
        Colliders = new Dictionary<int, CollisionDetector>();

        CreateTape();

        keysOnTape = 0;
        coinsOnTape = 0;

        maxKeysCount = 0;
        int availableKeys = BalanceDataProvider.Instance.InGameKeysCount - Env.Instance.Inventory.CurrentLevelPickedUpKeys;
        for (int i = 0; i < availableKeys; i++)
        {
            if (Random.Range(0.0f, 1.0f) <= BalanceDataProvider.Instance.InGameKeysChanceAppear)
            {
                maxKeysCount++;
            }
        }

        maxCoinsCount = 0;
        int availableCoins = BalanceDataProvider.Instance.InGameCoinsCount - Env.Instance.Inventory.CurrentLevelPickedUpCoins;
        for (int i = 0; i < availableCoins; i++)
        {
            if (Random.Range(0.0f, 1.0f) <= BalanceDataProvider.Instance.InGameCoinsChanceAppear)
            {
                maxCoinsCount++;
            }
        }

        _generator.ParentConveyor = this;
        _generator.ChunkReady += AddChunk;
        _generator.FillTape(DestroyPoint, StartPoint);
        _generator.StartGeneration();
    }

    private void CreateTape()
    {
        _tapeSegments = new List<Transform>(StartPoint - DestroyPoint);
        for (int i = DestroyPoint; i < StartPoint; i++)
        {
            Collider segment = Instantiate(_tapeSegment, _tape);
            segment.transform.localPosition = new Vector3(i, 0, 0);
            _tapeSegments.Add(segment.transform);
            Colliders.Add(segment.GetInstanceID(), CollisionDetector.GetAuto(segment));
        }
    }

    private void AddChunk(ShapesChunk chunk)
    {
        foreach (Shape shape in chunk.Shapes)
        {
            Shapes.Add(shape);

            foreach (Collider cldr in shape.Colliders)
            {
                Colliders.Add(cldr.GetInstanceID(), CollisionDetector.GetAuto(cldr));
            }
        }
        
        if (chunk.IsFeverChunk)
        {
            if ((Chunks.Count > 0) && (Chunks.Last().IsFeverChunk))
            {
                chunk.FeverChunkIndex = Chunks.Last().FeverChunkIndex + 1;
            }
            else
            {
                chunk.FeverChunkIndex = 0;
            }
        }
        else
        {
            if ((Chunks.Count > 0) && (Chunks.Last().IsFeverChunk))
            {
                Chunks.Last().isLastFeverChunk = true;
            }
        }

        if (chunk.IsSimpleShape())
        {
            chunksPassed++;

            Chunks.Add(chunk);

            if (!FeverModeEnabled)
            {
                if (!TryToAddKey(chunk))
                    TryToAddCoin(chunk);
            }

            if (chunksBeforeFeverMode != -1)
            {
                if (!FeverModeEnabled && (chunksPassed >= chunksBeforeFeverMode))
                {
                    FeverModeEnabled = true;
                }

                if (FeverModeEnabled && (chunksPassed >= (chunksBeforeFeverMode + FeverModeChunksCount)))
                {
                    FeverModeEnabled = false;
                }
            }
        }
    }

    private bool TryToAddKey(ShapesChunk chunk)
    {
        if (!canPlaceKeys) 
            return false;

        if (!Env.Instance.Rules.Keys.Value)
            return false;

        if (keysOnTape >= maxKeysCount) 
            return false;

        if (Mathf.Approximately(averageChunksCount, 0.0f)) 
            return false;

        if (Env.Instance.Inventory.CurrentLevelPickedUpKeys >= BalanceDataProvider.Instance.InGameKeysCount)
            return false;

        float fullChunksCount = averageChunksCount;// + ((-1 == FeverModeChunksCount) ? 0 : FeverModeChunksCount);
        float chunksForKey = fullChunksCount / (float)maxKeysCount;
        float firstChunkForKey = chunksForKey * (float)keysOnTape;
        int chunksPassedAfterStarting = chunksPassed - chunksPassedOnStart;
        float chanceToKey = (chunksPassedAfterStarting - firstChunkForKey) / chunksForKey;

        if (Random.Range(0.0f, 1.0f) > chanceToKey) 
            return false; 

        keysOnTape++;
        chunk.AddKey(onKeyTouched);

        return true;
    }

    private bool TryToAddCoin(ShapesChunk chunk)
    {
        if (!canPlaceCoins) 
            return false;

        if (!BalanceDataProvider.Instance.IsCoinsBoxEnabled)
            return false;

        if (Env.Instance.Inventory.CurrentLevelIndex < BalanceDataProvider.Instance.InGameCoinsMinLevel)
            return false;

        if (Env.Instance.Inventory.CurrentLevelPickedUpCoins >= BalanceDataProvider.Instance.InGameCoinsCount)
            return false;

        if (coinsOnTape >= maxCoinsCount) 
            return false;

        if (Mathf.Approximately(averageChunksCount, 0.0f)) 
            return false;

        float fullChunksCount = averageChunksCount;// + ((-1 == FeverModeChunksCount) ? 0 : FeverModeChunksCount);
        float chunksForCoin = fullChunksCount / (float)maxCoinsCount;
        float firstChunkForCoin = chunksForCoin * (float)coinsOnTape;
        int chunksPassedAfterStarting = chunksPassed - chunksPassedOnStart;
        float chanceToCoin = (chunksPassedAfterStarting - firstChunkForCoin) / chunksForCoin;

        if (Random.Range(0.0f, 1.0f) > chanceToCoin) 
            return false;

        coinsOnTape++;
        chunk.AddCoin(onCoinTouched);

        return true;
    }

    public void UpdateSelf(float speedScale)
    {
        if (!GameplayController.IsGameplayActive)
            return;

        Shape finishedShape = null;
        foreach (var s in Shapes)
        {
            if (s.transform.position.x < DestroyPoint)
            {
                finishedShape = s;
                break;
            }
        }
        if (finishedShape)
        {
            Shapes.Remove(finishedShape);
            UpdateChunks(finishedShape);
            Destroy(finishedShape.gameObject);
            
            foreach (var c in finishedShape.Colliders)
            {
                foreach (Collider cldr in finishedShape.Colliders)
                {
                    Colliders.Remove(cldr.GetInstanceID());
                }
            }
        }

        Vector3 deltaPos = Vector3.zero;
        if (!IsStopped)
        {
            Move(speedScale, out deltaPos);
            CheckChunksFinishing();
        }

        foreach (CollisionDetector cd in Colliders.Values)
        {
            cd.UpdateValues();
        }

        _creamCreator.UpdateSelf(deltaPos, speedScale);
    }

    public void Go()
    {
        canPlaceKeys = true;
        canPlaceCoins = true;
        chunksPassedOnStart = chunksPassed;
        _generator.AllowObstacles();

        bool isFeverModeAvailable = Env.Instance.Inventory.CurrentLevelFeverCompletions < 1;
        float feverModeChance = BalanceDataProvider.Instance.FeverModeChance;
        if (Env.Instance.Inventory.CurrentLevelIndex == BalanceDataProvider.Instance.FeverModeMinLevel) feverModeChance = 1.0f;
        if (Env.Instance.Inventory.CurrentLevelIndex < BalanceDataProvider.Instance.FeverModeMinLevel) feverModeChance = 0.0f;

        if (isFeverModeAvailable && _generator.HasFeverModeStartPrefab && (Random.Range(0.0f, 0.9999f) < feverModeChance))
        {
            FeverModeChunksCount = Random.Range(BalanceDataProvider.Instance.FeverModeShapesMin, BalanceDataProvider.Instance.FeverModeShapesMax + 1);
            FeverModeChunksCount = Mathf.Max(0, FeverModeChunksCount);
            chunksBeforeFeverMode = chunksPassed + Mathf.Max(1, ((int)averageChunksCount - FeverModeChunksCount) / 2);
        }
    }

    public void Stop()
    {
        IsStopped = true;
    }

    private void UpdateChunks(Shape deletedShape)
    {
        foreach (ShapesChunk chunk in Chunks)
        {
            if (chunk.Shapes.Contains(deletedShape))
            {
                Chunks.Remove(chunk);
                break;
            }
        }
    }

    private void Move(float speedScale, out Vector3 deltaPos)
    {
        deltaPos = new Vector3(speedScale * Speed * Time.deltaTime, 0, 0);
        foreach (var t in _tapeSegments)
        {
            t.localPosition -= deltaPos;
            if (t.localPosition.x <= DestroyPoint)
            {
                t.SetLocalPositionX(StartPoint - (t.localPosition.x - DestroyPoint));
            }
        }
        foreach (var s in Shapes)
        {
            s.Move(deltaPos);
            if (s.HasAnimation && Mathf.Abs(s.transform.position.x) <= deltaPos.x)
            {
                Stop();
            }
        }
    }

    public void CreateFinish()
    {
        if (_isFinishReady)
            return;

        _generator.GenerateFinish();
        _isFinishReady = true;
    }

    public void OnShapeCollidedByCream(Shape shape, Vector3 worldPoint)
    {
        foreach (ShapesChunk chunk in Chunks)
        {
            if (chunk.Shapes.Contains(shape))
            {
                chunk.OnShapeCollidedByCream(shape, worldPoint);
                break;
            }
        }
    }

    private void CheckChunksFinishing()
    {
        foreach (ShapesChunk chunk in Chunks)
        {
            if (!chunk.IsSimpleShape()) continue;
            if (!chunk.IsFinishedOnThisUpdate(out float filling)) continue;

            onChunkFinished?.Invoke(chunk, filling);            
        }
    }

    public void AddShine(ShapesChunk chunk)
    {
        if (Env.Instance.Rules.Effects.Value)
            ShineAnimation.AddTo(shapeShineOriginal, chunk.GetFirstShape().transform);
    }

    private void Update()
    {
        UpdateLineColor();
    }

    private void UpdateLineColor()
    {
        lineColor = Color.Lerp(lineColor, targetLineColor, 0.1f);
        lineMaterial.SetColor("_MainColor", lineColor);
    }

    public void SetNormalColor()
    {
        targetLineColor = baseLineColor;
    }

    public void SetFeverColor()
    {
        targetLineColor = feverLineColor;
    }

    private void OnDrawGizmos()
    {
        foreach (ShapesChunk chunk in Chunks)
        {
            chunk.DrawGizmos();
        }
    }
}

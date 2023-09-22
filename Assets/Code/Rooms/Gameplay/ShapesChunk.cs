using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapesChunk
{
    private const float OBJ_TOUCH_DISTANCE = 0.15f;
    private const float ONE_SEGMENT_LENGTH = 0.07f;
    private const float ONE_SEGMENT_LENGTH_HALF = 0.5f * ONE_SEGMENT_LENGTH;

    public HashSet<Shape> Shapes { get; private set; }
    public List<Shape> ShapesList { get; private set; }

    private Shape firstShape = null;
    private float minLocalX = 0.0f;
    private float maxLocalX = 0.0f;
    private RangesFloat rangesFloat = new RangesFloat();
    private bool finished = false;

    private System.Action<GameObject> onKeyTouched = null;
    private System.Action<GameObject> onCoinTouched = null;
    private GameObject keyObject = null;
    private GameObject coinObject = null;

    public bool IsFeverChunk  { get; private set; } = false;
    public int FeverChunkIndex { get; set; } = -1;
    public bool isLastFeverChunk { get; set; } = false;

    public ShapesChunk(bool isFeverChunk, params Shape[] controlledShapes)
    {
        IsFeverChunk = isFeverChunk;
        Shapes = new HashSet<Shape>(controlledShapes);
        ShapesList = new List<Shape>(controlledShapes);

        firstShape = ShapesList[0];

        InitMinMaxLocalX();
    }

    public void AddKey(System.Action<GameObject> onKeyTouched)
    {
        if (!firstShape.GetCollidersTopY(out float maxy)) return;

        GameplayRoomConfig config = Env.Instance.Rooms.CurrentRoom.Config as GameplayRoomConfig;

        keyObject = GameObject.Instantiate(
            config.IngameKeyPrefab, 
            new Vector3(GetChunkPosition() + 0.5f * (maxLocalX + minLocalX), maxy, 0.0f),
            Quaternion.identity,
            firstShape.transform);

        this.onKeyTouched = onKeyTouched;
    }

    public void AddCoin(System.Action<GameObject> onCoinTouched)
    {
        if (!firstShape.GetCollidersTopY(out float maxy)) return;

        GameplayRoomConfig config = Env.Instance.Rooms.CurrentRoom.Config as GameplayRoomConfig;

        coinObject = GameObject.Instantiate(
            config.IngameCoinPrefab, 
            new Vector3(GetChunkPosition() + 0.5f * (maxLocalX + minLocalX), maxy, 0.0f),
            Quaternion.identity,
            firstShape.transform);

        this.onCoinTouched = onCoinTouched;
    }

    public float GetChunkPosition()
    {
        return firstShape.transform.position.x;
    }

    private void InitMinMaxLocalX()
    {
        maxLocalX = firstShape.LengthSize * 0.5f;
        minLocalX = -maxLocalX;
        float chunkPos = GetChunkPosition();

        foreach (Shape shape in Shapes)
        {
            if (shape == firstShape) continue;

            float shapePos = shape.transform.position.x;
            float relativePos = shapePos - chunkPos;

            maxLocalX = Mathf.Max(maxLocalX, relativePos + shape.LengthSize * 0.5f);
            minLocalX = Mathf.Min(minLocalX, relativePos - shape.LengthSize * 0.5f);
        }
    }

    public bool IsSimpleShape()
    {
        foreach (Shape shape in Shapes)
        {
            if (shape.GetSelfType() != Shape.Type.Shape) return false;
        }

        return true;
    }

    public void DrawGizmos()
    {
        float currentPos = GetChunkPosition();
        float minx = minLocalX + currentPos;
        float maxx = maxLocalX + currentPos;

        Vector3 center = new Vector3(0.5f * (maxx + minx), 0.0f, 0.0f);
        Vector3 size = new Vector3(maxx - minx, 1.0f, 1.0f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, size);
    }

    public void OnShapeCollidedByCream(Shape shape, Vector3 worldPoint)
    {
        float localPoint = worldPoint.x - GetChunkPosition();

        rangesFloat.Add(localPoint - ONE_SEGMENT_LENGTH_HALF, localPoint + ONE_SEGMENT_LENGTH_HALF);

        if (null != keyObject)
        {
            float keyPoint = 0.5f * (maxLocalX + minLocalX);
            float toKeyDistance = localPoint - keyPoint;

            if (Mathf.Abs(toKeyDistance) < OBJ_TOUCH_DISTANCE)
            {
                onKeyTouched?.Invoke(keyObject);
                keyObject = null;
            }
        }

        if (coinObject != null)
        {
            float coinPoint = 0.5f * (maxLocalX + minLocalX);
            float toCoinDistance = localPoint - coinPoint;

            if (Mathf.Abs(toCoinDistance) < OBJ_TOUCH_DISTANCE)
            {
                onCoinTouched?.Invoke(coinObject);
                coinObject = null;
            }
        }
    }

    public bool IsFinishedOnThisUpdate(out float filling)
    {
        filling = Mathf.Clamp01(rangesFloat.GetSummLength() / (maxLocalX - minLocalX));
        if (finished) return false;

        float finishinfWorldPosition = maxLocalX + GetChunkPosition();
        finished = (finishinfWorldPosition < 0.0f);
        return finished;
    }

    public Shape GetFirstShape()
    {
        return firstShape;
    }
}

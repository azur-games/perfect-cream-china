using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreamParticle : MonoBehaviour
{
    public bool IsFree { get; private set; }
    public bool IsFreezed;

    public int Generation { get; private set; }

    private KeyValuePair<CreamParticle, float>? connectionToNextPart;
    private KeyValuePair<CreamParticle, float>? connectionToPrevPart;

    private const int perimeterSegs = 12;
    private const float unPerimeterSegs = 1.0f / (float)perimeterSegs;

    public float[] perimeter = new float[perimeterSegs];
    private float commonPerimeter = 0.0f;
    private float[] sin = new float[perimeterSegs];
    private float[] cos = new float[perimeterSegs];
    public float[] angle = new float[perimeterSegs];
    private bool[] perimeterStops = new bool[perimeterSegs];
    private bool allPerimeterStops = false;
    public Vector3[] perimeterPoints = new Vector3[perimeterSegs];
    private List<int> nearestColliderIDs = null;
    private Dictionary<int, CollisionDetector> plateColliders;
    private CreamCreator creamCreator;
    public float targetScale;
    private float strFunction;

    private System.Action<Collider, CreamParticle, Vector3> onCollide = (c, p, v) => { };

    private CreamSkinAsset creamSkinAsset;

    private bool inTouchedRegion = false;
    private bool stillRotate = true;
    private Vector3[] dirs = null;

    private Vector3 _position;
    public Vector3 Position
    {
        get
        {
            return _position;
        }

        private set
        {
            _position = value;
        }
    }

    public void Init(CreamCreator creamCreator, int generation, Vector3 pos, float targetScale, float strFunction, Dictionary<int, CollisionDetector> plateColliders, System.Action<Collider, CreamParticle, Vector3> onCollide, CreamSkinAsset creamSkinAsset)
    {
        this.onCollide = onCollide;
        
        this.creamCreator = creamCreator;
        this.targetScale = targetScale;
        this.strFunction = strFunction;
        this.creamSkinAsset = creamSkinAsset;

        Generation = generation;
        IsFree = false;
        IsFreezed = false;
        stillRotate = true;
        inTouchedRegion = false;

        nearestColliderIDs = null;

        Position = pos;
        this.transform.position = pos;
        dirs = new Vector3[perimeterSegs];
        CalculateDirs();

        onPlatePoint = Vector3.zero;
        touchedPlateTransform = null;
        touchedCollider = null;

        connectionToNextPart = null;
        connectionToPrevPart = null;

        additionalSpeedVector = Vector3.zero;
        additionalOrthoVector = Vector3.zero;

        creamSkinAsset.PrecalculateAmps(perimeterSegs);

        this.plateColliders = plateColliders;
        nearestColliderIDs = new List<int>();
        float posx = _position.x;
        foreach (KeyValuePair<int, CollisionDetector> cldr in plateColliders)
        {
            if ((cldr.Value.Min.x - posx) > 1.5f) continue;
            if ((posx - cldr.Value.Max.x) > 0.2f) continue;

            int id = cldr.Key;
            nearestColliderIDs.Add(id);
        }

        for (int i = 0; i < perimeterSegs; i++)
        {
            perimeter[i] = 0.0f;
            commonPerimeter = 0.0f;
            perimeterStops[i] = false;
            allPerimeterStops = false;
            perimeterPoints[i] = Position;

            angle[i] = (2.0f * Mathf.PI * unPerimeterSegs * (float)i) + GetGenerationMeshAngleOffset();

            float amp = creamSkinAsset.precalculatedAmps[i];
            cos[i] = Mathf.Cos(angle[i]) * amp;
            sin[i] = Mathf.Sin(angle[i]) * amp;
        }
    }

    public float GetGenerationMeshAngleOffset()
    {
        return Generation * creamSkinAsset.MeshTwistingAngle;
    }

    public float GetGenerationTextureAngleOffset()
    {
        return Generation * creamSkinAsset.TextureTwistingAngle;
    }

    public float GetGenerationTextureV()
    {
        return Generation * creamSkinAsset.TextureLengthScale;
    }

    public float GetTextureUScale()
    {
        return creamSkinAsset.TextureWidthScale;
    }

    public void SetConnectionToNext(CreamParticle otherPart)
    {
        connectionToNextPart = new KeyValuePair<CreamParticle, float>(otherPart, (Position - otherPart.Position).magnitude);
    }

    public void SetConnectionToPrev(CreamParticle otherPart)
    {
        connectionToPrevPart = new KeyValuePair<CreamParticle, float>(otherPart, (Position - otherPart.Position).magnitude);
    }

    public void DestroyConnectionToPrev()
    {
        connectionToPrevPart = null;
    }

    public void DestroyConnectionToNext()
    {
        connectionToNextPart = null;
    }

    public void MoveTo(Vector3 newPosition)
    {
        Position = newPosition;
        this.transform.position = newPosition;
    }

    public void SetTargetScaleWithFadeWithTail(float scale, float fade = 0.0f)
    {
        if (fade > 1.0f) return;

        float fadeFunc = Mathf.Sin(fade * Mathf.PI - Mathf.PI * 0.5f) * 0.5f + 0.5f;
        fadeFunc *= fadeFunc;
        targetScale = scale * (1.0f - fadeFunc) + targetScale * fadeFunc;

        CreamParticle prevItem = PrevItem;
        if (null != prevItem)
        {
            prevItem.SetTargetScaleWithFadeWithTail(scale, fade + 0.1f);
        }
    }

    public void SetTargetScaleWithFade(float scale)
    {
        float scaleChange = Mathf.Abs(targetScale - scale);
        if (scaleChange < 0.0125f) return;

        CreamParticle prevItem = PrevItem;
        if (null != prevItem)
        {
            prevItem.SetTargetScaleWithFade(targetScale * 0.5f + scale * 0.5f);
        }

        targetScale = scale;
    }

    public void SetStrFunction(float strFunction)
    {
        this.strFunction = strFunction;

        CreamParticle prevItem = PrevItem;
        if (null != prevItem)
        {
            prevItem.SetStrFunction(strFunction);
        }
    }

    public void SetFree()
    {
        IsFree = true;
    }

    public CreamParticle PrevItem
    {
        get
        {
            return connectionToPrevPart.HasValue ? connectionToPrevPart.Value.Key : null;
        }
    }

    public CreamParticle NextItem
    {
        get
        {
            return connectionToNextPart.HasValue ? connectionToNextPart.Value.Key : null;
        }
    }

    public bool HasConnectionToNext
    {
        get
        {
            return connectionToNextPart.HasValue;
        }
    }

    public bool HasConnectionToPrev
    {
        get
        {
            return connectionToPrevPart.HasValue;
        }
    }

    private Vector3 CalculateTargetPointToPart(Vector3 cpPosition, float targetDistance)
    {
        Vector3 vec = Position - cpPosition;
        float redist = targetDistance / (vec.magnitude + 0.0001f);
        return new Vector3(
            cpPosition.x + vec.x * redist,
            cpPosition.y + vec.y * redist,
            cpPosition.z + vec.z * redist);
    }

    public void SetAdditionalSpeedVector(float asv = 0.0075f)
    {
        if (null != touchedPlateTransform) return;

        additionalSpeedVector = Vector3.right * asv;

        if (null != PrevItem)
        {
            PrevItem.SetAdditionalSpeedVector(asv);
        }
    }

    public void SetAdditionalSpeedVectorWithGradient(float asv, float delta)
    {
        if (null != touchedPlateTransform) return;

        additionalSpeedVector = Vector3.right * asv;

        if (null != PrevItem)
        {
            PrevItem.SetAdditionalSpeedVectorWithGradient(asv + delta, delta);
        }
    }

    public void SetAdditionalOrthoVector(float aov)
    {
        additionalOrthoVector = Vector3.forward * aov;
    }

    public void ResetAdditionalOrthoVector()
    {
        if (null != touchedPlateTransform) return;
        additionalOrthoVector *= 0.5f;
        if (connectionToPrevPart.HasValue)
        {
            connectionToPrevPart.Value.Key.ResetAdditionalOrthoVector();
        }
    }

    private Vector3 additionalSpeedVector = Vector3.zero;
    private Vector3 additionalOrthoVector = Vector3.zero;
    private Vector3 precalculatedPosition = Vector3.zero;
    public void Precalculate(float gravityScale, Vector3 tapeDeltaPos)
    {
        if (!IsFree) return;

        if (null != touchedPlateTransform)
        {
            precalculatedPosition = Position - tapeDeltaPos;//  touchedPlateTransform.localToWorldMatrix.MultiplyPoint(onPlatePoint);
            return;
        }

        precalculatedPosition = Position;

        if (!IsFreezed)
        {
            //precalculatedPosition += gravityScale * Vector3.down * creamCreator.gravity + additionalSpeedVector + additionalOrthoVector;
            precalculatedPosition.x = precalculatedPosition.x + additionalSpeedVector.x + additionalOrthoVector.x;
            precalculatedPosition.y = precalculatedPosition.y + additionalSpeedVector.y + additionalOrthoVector.y - gravityScale * creamCreator.gravity;
            precalculatedPosition.z = precalculatedPosition.z + additionalSpeedVector.z + additionalOrthoVector.z;

            Vector3 vecTargets = Vector3.zero;
            float dividor = 0.0f;

            if (connectionToNextPart.HasValue)
            {
                vecTargets += CalculateTargetPointToPart(connectionToNextPart.Value.Key.Position, connectionToNextPart.Value.Value);
                dividor += 1.0f;
            }

            if (connectionToPrevPart.HasValue)
            {
                vecTargets += CalculateTargetPointToPart(connectionToPrevPart.Value.Key.Position, connectionToPrevPart.Value.Value);
                dividor += 1.0f;
            }

            if (dividor > 0.5f)
            {
                vecTargets = vecTargets / dividor;
                precalculatedPosition = vecTargets * strFunction + precalculatedPosition * (1.0f - strFunction);
            }

            /*if (connectionToPrevPart.HasValue)
            {
                precalculatedPosition.x = Mathf.Max(precalculatedPosition.x, connectionToPrevPart.Value.Key.Position.x);
            }*/
        }

        //if (precalculatedPosition.y < 0.0f) precalculatedPosition = new Vector3(precalculatedPosition.x, 0.0f, precalculatedPosition.z);
    }

    public void ApplyPositionAndRotation()
    {
        //if ((null == touchedPlateTransform) && connectionToNextPart.HasValue && connectionToPrevPart.HasValue)
        this.transform.position = Position;

        Vector3 aprevUp = Vector3.zero;
        Vector3 aprevRight = Vector3.zero;
        Vector3 aprevForward = Vector3.zero;

        if (stillRotate && inTouchedRegion)
        {
            aprevUp = this.transform.up;
            aprevRight = this.transform.right;
            aprevForward = this.transform.forward;
        }

        if (stillRotate && connectionToNextPart.HasValue && connectionToPrevPart.HasValue)
        {
            CreamParticle prevPart = connectionToPrevPart.Value.Key;
            CreamParticle nextPart = connectionToNextPart.Value.Key;

            Vector3 lookFromPrev = Position - prevPart.Position;
            Vector3 prevLeft = (new Vector3(-lookFromPrev.z, 0.0f, lookFromPrev.x + 0.001f));
            Vector3 prevUp = Vector3.Cross(Vector3.forward, lookFromPrev);
            Quaternion prevRot = Quaternion.LookRotation(lookFromPrev, prevUp);

            Vector3 lookToNext = nextPart.Position - Position;
            Vector3 nextLeft = (new Vector3(-lookToNext.z, 0.0f, lookToNext.x + 0.001f));
            Vector3 nextUp = Vector3.Cross(Vector3.forward, lookToNext);
            Quaternion nextRot = Quaternion.LookRotation(lookToNext, nextUp);

            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Lerp(prevRot, nextRot, 0.5f), 0.2f);//0.025f

            CalculateDirs();
        }

        if (stillRotate && inTouchedRegion)
        {
            Vector3 bprevUp = this.transform.up;
            Vector3 bprevRight = this.transform.right;
            Vector3 bprevForward = this.transform.forward;

            float ang = Mathf.Abs(Vector3.Angle(aprevUp, bprevUp)) + Mathf.Abs(Vector3.Angle(aprevRight, bprevRight)) + Mathf.Abs(Vector3.Angle(aprevForward, bprevForward));
            if ((null != touchedPlateTransform) && (ang < 0.01f))
            {
                stillRotate = false;
            }
        }
    }

    public void ApplyPrecalculations()
    {
        Position = precalculatedPosition;
    }

    private void CalculateDirs()
    {
        Vector3 right = this.transform.right;
        Vector3 up = this.transform.up;

        for (int i = 0; i < perimeterSegs; i++)
        {
            dirs[i] = (cos[i] * right + sin[i] * up);
        }
    }

    public IEnumerable<CreamParticle> GetConnections()
    {
        List<CreamParticle> connections = new List<CreamParticle>(2);
        if (connectionToNextPart.HasValue) connections.Add(connectionToNextPart.Value.Key);
        if (connectionToPrevPart.HasValue) connections.Add(connectionToPrevPart.Value.Key);
        return connections;
    }

    private Vector3 onPlatePoint = Vector3.zero;
    private Transform touchedPlateTransform;
    private CollisionDetector touchedCollider;

    private void TriggerEntered(Collider collider, CollisionDetector cd)
    {
        if (null != touchedPlateTransform) return;

        touchedCollider = cd;
        touchedPlateTransform = collider.gameObject.transform;
        onPlatePoint = touchedPlateTransform.worldToLocalMatrix.MultiplyPoint(Position);

        SendInTouchedRegionEvent();
    }

    private void SendInTouchedRegionEvent()
    {
        if (inTouchedRegion) return;
        inTouchedRegion = true;
        if (HasConnectionToPrev)
        {
            connectionToPrevPart.Value.Key.SendInTouchedRegionEvent();
        }
    }

    private float puffSpeed = 0.0185f;
    public void UpdateSize(float minCreamXPosition, bool checkCollisions, float speedScale)
    {
        //CreamCreator.sw1.Restart();

        if (!IsFree)
        {
            puffSpeed = 0.02f;
        }
        else
        {
            float swapSpeed = 0.82f - (speedScale - 1.0f) * 0.2f;
            //puffSpeed = puffSpeed * 0.72f * speedScale + 0.070f * 0.28f * 1.5f;
            puffSpeed = puffSpeed * swapSpeed + 0.070f * (1.0f - swapSpeed) * speedScale;
        }

        if (!allPerimeterStops)
        {
            float newDist = commonPerimeter * (1.0f - puffSpeed) + targetScale * puffSpeed;
            if (((newDist - commonPerimeter) < 0.0004f) && (null != touchedPlateTransform))
            {
                allPerimeterStops = true;
            }

            commonPerimeter = newDist;

            for (int i = 0; i < perimeterSegs; i++)
            {
                if (!perimeterStops[i])
                {
                    perimeter[i] = commonPerimeter;
                    perimeterStops[i] = allPerimeterStops;
                }
            }
        }

        for (int i = 0; i < perimeterSegs; i++)
        {
            Vector3 point = dirs[i];
            float perim = perimeter[i];
            point.x = point.x * perim + Position.x;
            point.y = point.y * perim + Position.y;
            point.z = point.z * perim + Position.z;
            perimeterPoints[i] = point;
        }

        //CreamCreator.sw1.Stop();
        //CreamCreator.max1 += (float)CreamCreator.sw1.Elapsed.TotalMilliseconds;

        if (!allPerimeterStops && checkCollisions)
        {
            float posx = Position.x;
            float posy = Position.y;

            //CreamCreator.sw2.Restart();
            List<CollisionDetector> nearestColliders = new List<CollisionDetector>();
            if (null != touchedPlateTransform)
            {
                nearestColliders.Add(touchedCollider);
            }
            else
            {
                foreach (int cldrID in nearestColliderIDs)
                {
                    if (!plateColliders.TryGetValue(cldrID, out CollisionDetector cd)) continue;

                    if (!cd.IsNear(posx, posy, commonPerimeter)) continue;

                    nearestColliders.Add(cd);
                }
            }

            //CreamCreator.sw2.Stop();
            //CreamCreator.max2 += (float)CreamCreator.sw2.Elapsed.TotalMilliseconds;

            //CreamCreator.sw3.Restart();

            for (int i = 0; i < perimeterSegs; i++)
            {
                Vector3 point = perimeterPoints[i];

                if (!allPerimeterStops && checkCollisions && !perimeterStops[i] && (dirs[i].y < 0.01f))
                {
                    foreach (CollisionDetector cd in nearestColliders)
                    {
                        if (cd.IsPointInside(point))
                        {
                            perimeterStops[i] = true;
                            if (null == touchedPlateTransform)
                            {
                                onCollide(cd.collider, this, point);
                                TriggerEntered(cd.collider, cd);
                            }
                            break;
                        }
                    }
                }
            }
            //CreamCreator.sw3.Stop();
            //CreamCreator.max3 += (float)CreamCreator.sw3.Elapsed.TotalMilliseconds;
        }

        //IsFreezed = (freezed && (null != touchedPlateTransform));

        if (Position.x < minCreamXPosition)
        //if ((Position.x < -2.8f) || (Position.y < -5.0f))
        {
            IsFreezed = true;
        }
    }

    public void DrawOwnGizmos()
    {
        Gizmos.color = Color.cyan;
        for (int i = 0; i < perimeterSegs; i++)
        {
            int next = (i + 1) % perimeterSegs;
            Gizmos.DrawLine(perimeterPoints[i], perimeterPoints[next]);
        }

        //Gizmos.DrawWireSphere(Position, 0.1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(Position, Position + transform.forward * 0.4f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(Position, Position + transform.up * 0.4f);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Position, Position + transform.right * 0.4f);
    }

    public bool IsTouchGround()
    {
        if (null != touchedPlateTransform) return true;

        if (!connectionToPrevPart.HasValue) return false;

        return connectionToPrevPart.Value.Key.IsTouchGround();
    }

    public bool CanBeDestroyed()
    {
        if (IsFreezed)
        {
            if (connectionToNextPart.HasValue) return connectionToNextPart.Value.Key.IsFreezed;
            return true;
        }

        return false;
    }

    public void AddFamily(HashSet<CreamParticle> family)
    {
        family.Add(this);
        if (null != connectionToPrevPart)
        {
            connectionToPrevPart.Value.Key.AddFamily(family);
        }
    }
}

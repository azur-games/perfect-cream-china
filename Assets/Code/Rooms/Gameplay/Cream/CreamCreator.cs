using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreamCreator : MonoBehaviour
{
    public enum CreamForm
    {
        None,
        Sine,
        Straight,
    }

    private const int IMPRESSIVE_PUSHING = 22;
    private const int UNIMPRESSIVE_PUSHING = 15;
    private const int VERY_FIRST_PUSHING = 15;

    [SerializeField] private float speed = 0.5f;
    [SerializeField] private float newLayerPeriod = 0.5f;

    public float gravity = 0.1f;

    private CreamForm currentCreamForm = CreamForm.Straight;
    public CreamForm CurrentCreamForm
    {
        get
        {
            return currentCreamForm;
        }

        set
        {
            currentCreamForm = value;
            Env.Instance.Rules.CreamForm.Value = value.ToString();
        }
    }

    private List<CreamParticle> partsPool = new List<CreamParticle>();
    private List<CreamParticle> allParts = new List<CreamParticle>();
    private List<CreamParticle> newCreatedParts = null;
    private float elapsed = 0.0f;
    private int generation = 0;

    public GameObject CreamMeshObject;
    private MeshFilter creamMFilter;
    private Conveyor conveyor;

    [SerializeField] private float minCreamXPosition = 4.0f;

    private CreamSkinAsset creamSkinAsset;
    private string creamSkinName = "";
    public string CreamSkinName
    {
        get
        {
            return creamSkinName;
        }

        set
        {
            SetCreamSkinWithoutSaving(value);
            Env.Instance.Rules.CreamSkin.Value = creamSkinName;
        }
    }

    private void Start()
    {
        //CreateNewLayer();
    }

    public void SetCreamSkinWithoutSaving(string skinName)
    {
        creamSkinName = skinName;
        creamSkinAsset = Env.Instance.Content.LoadContentAsset<CreamSkinAsset>(ContentAsset.AssetType.CreamSkin, creamSkinName);
        CreamMeshObject.GetComponent<MeshRenderer>().material = creamSkinAsset.Material;

        creamSkinAsset.SetMainColorToParticles(Env.Instance.Rooms.GameplayRoom.Controller.Valve.gameObject);
    }

    private System.Action<Collider, CreamParticle, Vector3> onCollide = (c, p, v) => { };
    public void Init(Conveyor conveyor, System.Action<Collider, CreamParticle, Vector3> onCollide)
    {
        this.conveyor = conveyor;
        this.onCollide = onCollide;
        currentCreamForm = (CreamForm)System.Enum.Parse(typeof(CreamForm), Env.Instance.Rules.CreamForm.Value);
        minCreamXPosition = conveyor.DestroyPoint + 4.25f;

        creamMFilter = CreamMeshObject.GetComponent<MeshFilter>();
        creamMFilter.mesh = new Mesh();

        CreamSkinName = Env.Instance.Rules.CreamSkin.Value;
    }

    public CreamSkinAsset GetCurrentSkin()
    {
        return this.creamSkinAsset;
    }

    public void UpdateSelf(Vector3 tapeDeltaPos, float speedScale)
    {
        if (!GameplayController.IsGameplayActive)
            return;

        if (!this.gameObject.activeSelf)
            return;

        float dTimeRescale = 0.0166666f / Time.deltaTime;
        dTimeRescale = Mathf.Sqrt(dTimeRescale);
        float gravity = 1.0f / Mathf.Pow(dTimeRescale, 0.33334f);
        
        FixedUpdate2(allParts, Mathf.RoundToInt(10.0f * speedScale), true, gravity, dTimeRescale, tapeDeltaPos, speedScale);

        UpdateTimings(true, speedScale, 1.0f / speedScale);
    }

    private void UpdateTimings(bool recreateMesh, float speedScale, float newLayerPeriodScale)
    {
        elapsed += Time.deltaTime * speedScale;

        if (null != prevPart)
        {
            prevPart.MoveTo(prevPart.Position + Mathf.Sqrt(speedScale) * Vector3.down * Time.deltaTime * speed);
        }

        float nlp = 1.0f;
        switch (CurrentCreamForm)
        {
            case CreamForm.Sine:
                nlp = newLayerPeriod * 0.5f * newLayerPeriodScale;
                break;

            case CreamForm.Straight:
                nlp = newLayerPeriod * newLayerPeriodScale;
                break;
        }

        if (elapsed > nlp)
        {
            elapsed -= nlp;

            CreateNewLayer(speedScale);
        }

        if (recreateMesh && (allParts.Count > 1))
        {
            UpdateMesh();
        }
    }

    public void UpdateMesh()
    {
        creamSkinAsset.SkinMeshGenerator.CreateCreamMesh(creamMFilter.mesh, allParts);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void StopSound()
    {
        if (playedCreamLoopSoungGUID.HasValue)
        {
            Env.Instance.Sound.StopSound(playedCreamLoopSoungGUID.Value);
            playedCreamLoopSoungGUID = null;
        }
    }

    public void StartSound()
    {
        if (!playedCreamLoopSoungGUID.HasValue)
        {
            playedCreamLoopSoungGUID = Env.Instance.Sound.PlaySound(AudioKeys.Gameplay.CreamLoop, true);
        }
    }

    public void TurnOff()
    {
        turnedOn = false;
    }

    private System.Guid? playedCreamLoopSoungGUID = null;
    private bool veryFirstValving = true;

    public void Turn(bool on, bool impressivePushing)
    {
        if (on == turnedOn) return;
        if (!gameObject.activeSelf) return;
        //if (!on && (null != prevPart) && (lastFamilyLength < 7)) return;

        if (on)
        {
            StartSound();
        }
        else
        {
            StopSound();
        }

        turnedOn = on;
        if (turnedOn)
        {
            newCreatedParts = new List<CreamParticle>();
            float dtimeRescale = 0.0166666f / Time.deltaTime;
            dtimeRescale = Mathf.Sqrt(dtimeRescale);

            int forcedUpdateTimes = impressivePushing ? IMPRESSIVE_PUSHING : UNIMPRESSIVE_PUSHING;
            if (veryFirstValving) forcedUpdateTimes = VERY_FIRST_PUSHING;
            veryFirstValving = false;

            float fps60 = dtimeRescale;
            float gravityOn60FPS = 10.0f * dtimeRescale * dtimeRescale;
            float gravityOn15FPS = 10.0f;
            float gravity = MathUtils.LerpFloat(1.0f, gravityOn60FPS, 0.25f, gravityOn15FPS, fps60, true);
            forcedUpdateTimes = Mathf.RoundToInt(MathUtils.LerpFloat(1.0f, forcedUpdateTimes, 0.25f, 0.65f * (float)forcedUpdateTimes, fps60, true));

            for (int i = 0; i < forcedUpdateTimes; i++)
            {
                FixedUpdate2(newCreatedParts, 2, false, gravity, dtimeRescale, Vector3.zero, 1.0f);
                UpdateTimings(false, 1.5f, 1.0f);
            }
            newCreatedParts = null;
        }
    }

    public bool CanTurnedOff()
    {
        if (null == prevPart) return true;
        return (lastFamilyLength > 5);
    }

    private int lastFamilyLength = 0;
    private bool turnedOn = false;
    private CreamParticle prevPart = null;
    private float targetScale = 0.01f;
    private float sineAmp = 0.0f;
    public float strFunction = 0.65f;
    private float str = 1.0f;

    private void CreateNewLayer(float speedScale)
    {
        if (!turnedOn)
        {
            if (null != prevPart)
            {
                prevPart.SetFree();
                prevPart.SetTargetScaleWithFade(0.00f);
                prevPart.SetStrFunction(strFunction);

                float dtimeRescale = 1.0f;
                float fps60 = 0.0166666f / Time.deltaTime;

                if (!conveyor.IsStopped)
                {
                    dtimeRescale = MathUtils.LerpFloat(1.0f, 1.0f, 0.25f, 0.0f, fps60, true);
                }
                else
                {
                    dtimeRescale = MathUtils.LerpFloat(1.0f, 1.1f, 0.25f, 0.5f, fps60, true);
                }

                prevPart.ResetAdditionalOrthoVector();

                if (prevPart.IsTouchGround())
                {
                    prevPart.SetAdditionalSpeedVector(dtimeRescale * 0.0075f);
                }
                else
                {
                    prevPart.SetAdditionalSpeedVectorWithGradient(dtimeRescale * 0.005f, -0.01f * dtimeRescale / ((float)lastFamilyLength));
                }
            }

            lastFamilyLength = 0;
            targetScale = 0.01f;
            sineAmp = 0.0f;
            str = strFunction;
            prevPart = null;
            return;
        }

        CreamParticle cp = GetPartObject();

        //cp.transform.rotation = (null == prevPart) ? Quaternion.LookRotation(Vector3.up, Vector3.left) : prevPart.transform.rotation;
        cp.transform.rotation = Quaternion.LookRotation(Vector3.up, Vector3.left);

        cp.Init(
            this, 
            generation, 
            this.transform.position, 
            targetScale,
            str,
            conveyor.Colliders, 
            onCollide, 
            creamSkinAsset);

        if (null != prevPart)
        {
            if (prevPart.IsTouchGround())
                cp.SetAdditionalOrthoVector(Mathf.Sin(10.0f * Time.realtimeSinceStartup) * sineAmp);

            prevPart.SetConnectionToNext(cp);
            cp.SetConnectionToPrev(prevPart);
            prevPart.SetFree();
        }

        allParts.Add(cp);
        if (null != newCreatedParts) newCreatedParts.Add(cp);

        lastFamilyLength++;
        generation++;
        prevPart = cp;
        targetScale = targetScale * 0.6f + 0.26f * 0.4f;
        str = str * 0.75f + 0.65f * 0.25f;

        switch (CurrentCreamForm)
        {
            case CreamForm.Straight:
                sineAmp = 0.0f;
                break;

            case CreamForm.Sine:
                sineAmp = sineAmp * 0.95f + 0.02f * 0.05f;
                break;
        }
    }

    private CreamParticle GetPartObject()
    {
        if (partsPool.Count > 0)
        {
            CreamParticle part = partsPool[partsPool.Count - 1];
            partsPool.RemoveAt(partsPool.Count - 1);
            return part;
        }

        GameObject newPartObject = new GameObject("Part");
        newPartObject.transform.parent = this.transform;
        CreamParticle newPart = newPartObject.AddComponent<CreamParticle>();
        return newPart;
    }

    public void OnPartDestroyed(CreamParticle cp)
    {
        cp.gameObject.SetActive(false);

        if (cp.HasConnectionToNext)
        {
            cp.NextItem.DestroyConnectionToPrev();
        }

        if (cp.HasConnectionToPrev)
        {
            cp.PrevItem.DestroyConnectionToNext();
        }

        allParts.Remove(cp);
        partsPool.Add(cp);
    }

    private void FixedUpdate2(List<CreamParticle> parts, int physicTimes, bool checkCollisions, float gravityScale, float dTimeRescale, Vector3 tapeDeltaPos, float speedScale)
    {
        //Debug.Log("gravityScale = " + gravityScale.ToString());
        //Debug.Log("dTimeRescale = " + dTimeRescale.ToString());
        //gravityScale /= Mathf.Pow(dTimeRescale, 1.0f / 3.0f);
        float speedRescale = speedScale / dTimeRescale;
        foreach (CreamParticle cp in parts)
        {
            cp.UpdateSize(minCreamXPosition, checkCollisions, speedRescale);
        }

        List<CreamParticle> newList = new List<CreamParticle>(allParts);
        foreach (CreamParticle part in newList)
        {
            if (part.CanBeDestroyed())
            {
                OnPartDestroyed(part);
            }
        }

        Vector3 oneStepTapeDeltaPos = tapeDeltaPos / (float)physicTimes;

        for (int i = 0; i < physicTimes; i++)
        {
            RefreshPhysics(parts, gravityScale, oneStepTapeDeltaPos);
        }

        foreach (CreamParticle cp in parts)
        {
            if (cp.IsFree) cp.ApplyPositionAndRotation();
        }
    }

    private void RefreshPhysics(List<CreamParticle> parts, float gravityScale, Vector3 oneStepTapeDeltaPos)
    {
        foreach (CreamParticle cp in parts)
        {
            cp.Precalculate(gravityScale, oneStepTapeDeltaPos);
        }

        foreach (CreamParticle cp in parts)
        {
            if (cp.IsFree) cp.ApplyPrecalculations();
        }
    }

    public void DestroyChildrenIslands()
    {
        HashSet<CreamParticle> lastFamily = new HashSet<CreamParticle>();
        allParts[allParts.Count - 1].AddFamily(lastFamily);

        CreamParticle[] allPartsCopy = allParts.ToArray();

        foreach (CreamParticle part in allPartsCopy)
        {
            if (!lastFamily.Contains(part))
            {
                allParts.Remove(part);
            }
        }
    }

    public GameObject DetachCreamMesh()
    {
        bool cachedTurnedOn = turnedOn;

        turnedOn = false;
        CreateNewLayer(1.0f);
        UpdateMesh();

        partsPool.AddRange(allParts);
        allParts.Clear();

        GameObject go = GameObject.Instantiate(CreamMeshObject.gameObject);
        go.GetComponent<MeshRenderer>().material = Material.Instantiate(go.GetComponent<MeshRenderer>().material);
        go.GetComponent<MeshFilter>().mesh = Mesh.Instantiate(go.GetComponent<MeshFilter>().mesh);

        UpdateMesh();

        turnedOn = cachedTurnedOn;

        return go;
    }

    private void OnDrawGizmos()
    {
        return;
        foreach (CreamParticle cp1 in allParts)
        {
            Gizmos.color = Color.yellow;
            foreach (CreamParticle cp2 in cp1.GetConnections())
            {
                Gizmos.DrawLine(cp1.Position, cp2.Position);
            }
            cp1.DrawOwnGizmos();
        }
    }
}

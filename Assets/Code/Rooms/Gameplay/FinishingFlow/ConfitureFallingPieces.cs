using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfitureFallingPieces : MonoBehaviour
{
    public static void Create(bool needSmallParts, bool needMainPart, Transform parent, int layer, FinishingFlowConfig finishingFlowConfig, GameObject surfaceMesh, MeshSurfacePoints meshSurfacePoints, out bool hasSmallPieces)
    {
        GameObject cobject = new GameObject("ConfitureFallingPieces");
        cobject.transform.parent = parent;
        cobject.transform.localPosition = Vector3.zero;
        cobject.transform.localRotation = Quaternion.identity;
        cobject.transform.localScale = Vector3.one;
        cobject.layer = layer;
        ConfitureFallingPieces confiture = cobject.AddComponent<ConfitureFallingPieces>();
        confiture.Init(needSmallParts, needMainPart, finishingFlowConfig, surfaceMesh, meshSurfacePoints, out hasSmallPieces);
    }

    private const int ItemsCount = 8;
    private const int PowsCount = 10;
    private const float RandomInitialRadius = 1.0f;

    public bool Reset = false;

    //private Material staticConfitureMaterial;
    //private Material material;
    private List<DelitiousSmallPiece> pieces = new List<DelitiousSmallPiece>();
    private DelitiousSmallPiece mainPiece = null;

    private MeshSurfacePoints meshSurfacePoints;

    private Vector4[] pows = new Vector4[PowsCount];
    private int powsIndex = 0;

    private void Init(bool needSmallParts, bool needMainPart, FinishingFlowConfig finishingFlowConfig, GameObject surfaceMesh, MeshSurfacePoints meshSurfacePoints, out bool hasSmallPieces)
    {
        this.meshSurfacePoints = meshSurfacePoints;

        ConfitureAsset confitureAsset = (false == needSmallParts) ? null : Env.Instance.Content.LoadContentAsset<ConfitureAsset>(ContentAsset.AssetType.Confiture, Env.Instance.Inventory.CurrentConfiture);

        DelitiousSmallPiece smallPieces = confitureAsset?.GetComponentInChildren<DelitiousSmallPiece>();

        InstantiateDelitious(smallPieces, needMainPart, finishingFlowConfig.DelitiousMainPieces, ItemsCount);

        ResetPieces(RandomInitialRadius);

        hasSmallPieces = (null != smallPieces);

        float timeoutBeforeMainPiece = hasSmallPieces ? 1.0f : 0.0f;

        if (needMainPart)
        {
            Vector3 mainPieceTargetPoint = finishingFlowConfig.MainPieceTargetPosition;
            mainPieceTargetPoint.x = meshSurfacePoints.MinX * 0.22f + meshSurfacePoints.MaxX * 0.78f;
            mainPieceTargetPoint.y = meshSurfacePoints.MinY * 0.22f + meshSurfacePoints.MaxY * 0.78f;

            mainPiece.Init(
                mainPieceTargetPoint + Vector3.up * 3.0f,
                mainPieceTargetPoint,
                -timeoutBeforeMainPiece,
                OnPieceFinished);
            mainPiece.SetTargetRotation(finishingFlowConfig.MainPieceTargetRotation);
        }

        InitPows();
    }

    private void InitPows()
    {
        for (int i = 0; i < PowsCount; i++)
        {
            pows[i] = Vector4.zero;
        }
    }

    private void PassPows()
    {
        const float speed = 0.45f;
        float add = speed * Time.deltaTime;

        for (int i = 0; i < PowsCount; i++)
        {
            float w = pows[i].w;
            if (Mathf.Approximately(w, 0.0f)) continue;
            pows[i].w = w + add;
        }

        //material.SetVectorArray("pows", pows);
        //staticConfitureMaterial.SetVectorArray("pows", pows);
    }

    private void ResetPieces(float randomRarius)
    {
        if (null == pieces) return;

        Vector3 center = meshSurfacePoints.GetCenter();
        List<Vector3> surfacePoints = meshSurfacePoints.GetPoints(0.667f, ItemsCount);

        for (int i = 0; i < ItemsCount; i++)
        {
            pieces[i].Init(
                surfacePoints[i] + Vector3.up * 3.0f,
                surfacePoints[i],
                //-0.25f - 1.75f * (float)i / (float)ItemsCount,
                0.0f - 1.0f * (float)i / (float)ItemsCount,
                //Random.Range(-1.5f, -0.25f),
                OnPieceFinished);
        }

        /*material.SetFloat("minHeight", meshSurfacePoints.GetPercentHeight(0.15f));
        staticConfitureMaterial.SetFloat("minHeight", meshSurfacePoints.GetPercentHeight(0.15f));

        for (int i = 0; i < PowsCount; i++) pows[i] = Vector4.zero;
        material.SetVectorArray("pows", pows);
        staticConfitureMaterial.SetVectorArray("pows", pows);*/
    }

    private void OnPieceFinished(Vector3 point)
    {
        pows[powsIndex] = new Vector4(point.x, point.y - 0.2f, point.z, 0.001f);
        powsIndex = (powsIndex + 1) % PowsCount;
    }

    private void InstantiateDelitious(DelitiousSmallPiece smallPrefabs, bool needMainPart, List<DelitiousSmallPiece> mainPrefabs, int count)
    {
        if (null != smallPrefabs)
        {
            for (int i = 0; i < count; i++)
            {
                DelitiousSmallPiece piece = GameObject.Instantiate(smallPrefabs, this.transform);
                pieces.Add(piece);
            }
        }
        else
        {
            pieces = null;
        }

        if (needMainPart)
            mainPiece = GameObject.Instantiate(mainPrefabs[Random.Range(0, mainPrefabs.Count)], this.transform);
    }

    private void Update()
    {
        PassPows();

        if (Reset)
        {
            Reset = false;
            ResetPieces(RandomInitialRadius);
        }
    }
}

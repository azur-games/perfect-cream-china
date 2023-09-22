using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Confiture : MonoBehaviour
{
    public static void Create(Transform parent, int layer, FinishingFlowConfig finishingFlowConfig, MeshSurfacePoints meshSurfacePoints)
    {
        GameObject cobject = new GameObject("Confiture");
        cobject.transform.parent = parent;
        cobject.transform.localPosition = Vector3.zero;
        cobject.transform.localRotation = Quaternion.identity;
        cobject.transform.localScale = Vector3.one;
        cobject.layer = layer;
        Confiture confiture = cobject.AddComponent<Confiture>();
        confiture.Init(finishingFlowConfig, meshSurfacePoints);
    }

    private const int ItemsCount = 50;
    private class MeshInfo
    {
        public Vector3[] vertices;
        public Vector3[] normals;
        public Color[] colors;
        public Vector2[] uvs;
        public int[] indices;
    }

    public bool Reset = false;

    private List<MeshInfo> meshInfos;
    private Material material;
    private float[] scales = new float[ItemsCount];

    private void Init(FinishingFlowConfig finishingFlowConfig, MeshSurfacePoints meshSurfacePoints)
    {
        CreateMeshInfos();
        List<Vector3> surfacePoints = meshSurfacePoints.GetPoints(0.333f, 0.1f, 0.9f, ItemsCount);
        MeshInfo minfo = CreateMeshResultInfo(ItemsCount, surfacePoints, finishingFlowConfig);

        this.material = finishingFlowConfig.ConfitureMaterial;

        MeshRenderer mRenderer = this.gameObject.AddComponent<MeshRenderer>();
        mRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mRenderer.material = this.material;
        this.gameObject.AddComponent<MeshFilter>().mesh = CreateResultMesh(minfo);

        FillScales();
        SendScales();
    }

    private void Update()
    {
        if (Reset)
        {
            Reset = false;
            FillScales();
        }

        ProcessScales();
        SendScales();
    }

    private void FillScales()
    {
        for (int i = 0; i < ItemsCount; i++)
        {
            scales[i] = Random.Range(0.8f, 1.5f);
        }
    }

    private void ProcessScales()
    {
        float add = Time.deltaTime;
        for (int i = 0; i < ItemsCount; i++)
        {
            scales[i] -= add;
        }
    }

    private void SendScales()
    {
        material.SetFloatArray("scales", scales);
    }

    private Mesh CreateResultMesh(MeshInfo minfo)
    {
        Mesh mesh = new Mesh
        {
            vertices = minfo.vertices,
            normals = minfo.normals,
            colors = minfo.colors,
            uv = minfo.uvs,
            triangles = minfo.indices
        };

        return mesh;
    }

    private Vector3 RotateAround(Vector3 pnt, float anglex, float angley)
    {
        float sn = Mathf.Sin(angley);
        float cs = Mathf.Cos(angley);
        pnt = new Vector3(cs * pnt.x + sn * pnt.z, pnt.y, cs * pnt.z - sn * pnt.x);

        sn = Mathf.Sin(anglex);
        cs = Mathf.Cos(anglex);
        pnt = new Vector3(pnt.x, cs * pnt.y + sn * pnt.z, cs * pnt.z - sn * pnt.y);

        return pnt;
    }

    private MeshInfo CreateMeshResultInfo(int meshesCount, List<Vector3> surfacePoints, FinishingFlowConfig finishingFlowConfig)
    {
        int vcount = 0;
        int icount = 0;

        for (int i = 0; i < meshesCount; i++)
        {
            MeshInfo mInfo = meshInfos[i % meshInfos.Count];
            vcount += mInfo.vertices.Length;
            icount += mInfo.indices.Length;
        }

        MeshInfo resultMInfo = new MeshInfo
        {
            vertices = new Vector3[vcount],
            normals = new Vector3[vcount],
            colors = new Color[vcount],
            uvs = new Vector2[vcount],
            indices = new int[icount]
        };

        int vIndex = 0;
        int iIndex = 0;
        float unCount = 1.0f / (float)(meshesCount - 1);

        for (int i = 0; i < meshesCount; i++)
        {
            MeshInfo mInfo = meshInfos[i % meshInfos.Count];
            Vector3 position = surfacePoints[i];
            
            int firstIndex = vIndex;
            float anglex = position.x * 100.0f;
            float angley = position.z * 100.0f;
            Color clr = finishingFlowConfig.ConfitureColors[i % finishingFlowConfig.ConfitureColors.Count];
            clr.a = unCount * (float)i;

            for (int j = 0; j < mInfo.vertices.Length; j++)
            {
                resultMInfo.vertices[vIndex] = RotateAround(mInfo.vertices[j], anglex, angley) + position;
                resultMInfo.normals[vIndex] = RotateAround(mInfo.normals[j], anglex, angley);
                resultMInfo.colors[vIndex] = clr;
                resultMInfo.uvs[vIndex++] = Vector2.zero;
            }

            for (int j = 0; j < mInfo.indices.Length; j++)
            {
                resultMInfo.indices[iIndex++] = mInfo.indices[j] + firstIndex;
            }
        }

        return resultMInfo;
    }

    private void CreateMeshInfos()
    {
        meshInfos = new List<MeshInfo>();
        meshInfos.Add(CreateMeshInfo_Box(new Vector3(0.14f, 0.03f, 0.03f)));
    }

    private MeshInfo CreateMeshInfo_Box(Vector3 size)
    {
        MeshInfo meshInfo = new MeshInfo();

        Vector3 halfSize = size * 0.5f;

        meshInfo.vertices = new Vector3[]
        {
            new Vector3(halfSize.x, halfSize.y, -halfSize.z),
            new Vector3(-halfSize.x, halfSize.y, -halfSize.z),
            new Vector3(-halfSize.x, halfSize.y, halfSize.z),
            new Vector3(halfSize.x, halfSize.y, halfSize.z),

            new Vector3(-halfSize.x, -halfSize.y, -halfSize.z),
            new Vector3(halfSize.x, -halfSize.y, -halfSize.z),
            new Vector3(halfSize.x, -halfSize.y, halfSize.z),
            new Vector3(-halfSize.x, -halfSize.y, halfSize.z),
        };

        meshInfo.normals = new Vector3[]
        {
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),

            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, 0.5f),
        };

        meshInfo.indices = new int[]
        {
            0, 1, 2, 0, 2, 3,
            4, 5, 6, 4, 6, 7,

            0, 3, 6, 0, 6, 5,
            2, 1, 4, 2, 4, 7,

            1, 0, 5, 1, 5, 4,
            3, 2, 7, 3, 7, 6,
        };

        return meshInfo;
    }
}

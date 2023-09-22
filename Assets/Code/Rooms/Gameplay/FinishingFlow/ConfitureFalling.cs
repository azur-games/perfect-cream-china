using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfitureFalling : MonoBehaviour
{
    public static void Create(Transform offsetTransform, int layer, FinishingFlowConfig finishingFlowConfig)
    {
        GameObject cfobject = new GameObject("ConfitureFalling");
        cfobject.transform.localPosition = offsetTransform.position + Vector3.up;
        cfobject.transform.localRotation = Quaternion.identity;
        cfobject.transform.localScale = Vector3.one;
        cfobject.layer = layer;
        ConfitureFalling confitureFalling = cfobject.AddComponent<ConfitureFalling>();
        confitureFalling.Init(finishingFlowConfig);
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
    private float[] heights = new float[ItemsCount];

    private void Init(FinishingFlowConfig finishingFlowConfig)
    {
        CreateMeshInfos();
        MeshInfo minfo = CreateMeshResultInfo(ItemsCount, new Vector2(1.1f, 0.4f), finishingFlowConfig);

        this.material = finishingFlowConfig.ConfitureFallingMaterial;
        this.gameObject.AddComponent<MeshRenderer>().material = this.material;
        this.gameObject.AddComponent<MeshFilter>().mesh = CreateResultMesh(minfo);

        FillHeights();
    }

    private void Update()
    {
        if (Reset)
        {
            Reset = false;
            FillHeights();
        }

        ProcessHeights();
        SendHeights();
    }

    private void FillHeights()
    {
        for (int i = 0; i < ItemsCount; i++)
        {
            heights[i] = Random.Range(-1.0f, 0.5f);
        }
    }

    private void ProcessHeights()
    {
        float add = Time.deltaTime * 1.4f;
        for (int i = 0; i < ItemsCount; i++)
        {
            heights[i] -= add;
        }
    }

    private void SendHeights()
    {
        material.SetFloatArray("heights", heights);
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

    private MeshInfo CreateMeshResultInfo(int meshesCount, Vector2 initialBox, FinishingFlowConfig finishingFlowConfig)
    {
        int vcount = 0;
        int icount = 0;
        Vector2 halfInitialBox = initialBox * 0.5f;

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
            int firstIndex = vIndex;
            Vector2 uv = new Vector2(Random.Range(-halfInitialBox.x, halfInitialBox.x), Random.Range(-halfInitialBox.y, halfInitialBox.y));
            float anglex = uv.x * 100.0f;
            float angley = uv.y * 100.0f;

            Color clr = finishingFlowConfig.ConfitureColors[i % finishingFlowConfig.ConfitureColors.Count];
            clr.a = unCount * (float)i;

            for (int j = 0; j < mInfo.vertices.Length; j++)
            {
                resultMInfo.vertices[vIndex] = RotateAround(mInfo.vertices[j], anglex, angley);
                resultMInfo.normals[vIndex] = RotateAround(mInfo.normals[j], anglex, angley);
                resultMInfo.colors[vIndex] = clr;
                resultMInfo.uvs[vIndex++] = uv;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreamSkinMeshGenerator_Cylinder : CreamSkinMeshGenerator
{
    public override void CreateCreamMesh(Mesh mesh, List<CreamParticle> parts)
    {
        if (parts.Count == 0)
        {
            if (0 == mesh.vertexCount) return;
            mesh.triangles = (new List<int>()).ToArray();
            mesh.vertices = (new List<Vector3>()).ToArray();
            mesh.uv = (new List<Vector2>()).ToArray();
            return;
        }

        int anglesCount = parts[0].angle.Length;
        int meshRadialSegmentsCount = anglesCount;// * 3;
        int pcountm1 = parts.Count - 1;

        int vertsCount = (meshRadialSegmentsCount + 1) * parts.Count;
        List<Vector3> vertices = new List<Vector3>(vertsCount + 100);
        List<Vector2> uvs = new List<Vector2>(vertsCount + 100);
        List<int> indices = new List<int>(meshRadialSegmentsCount * pcountm1 * 6);

        float unAnglesCount = 1.0f / anglesCount;

        for (int p = 0; p < parts.Count; p++)
        {
            CreamParticle part = parts[p];
            Vector3 partCenter = part.Position;

            float textureTwisting = part.GetGenerationTextureAngleOffset();
            float textureV = part.GetGenerationTextureV();

            int firstVertexID = vertices.Count;

            for (int a = 0; a < anglesCount; a++)
            {
                int nextA = a + 1;
                if (nextA == anglesCount) nextA = 0;

                Vector3 p0 = part.perimeterPoints[a];
                /*float p0len = part.perimeter[a];
                Vector3 p1 = part.perimeterPoints[nextA];
                float p1len = part.perimeter[nextA];

                Vector3 p0a = (p0 * 0.6667f + p1 * 0.3333f);
                Vector3 to_p0a = (p0a - partCenter).normalized;
                p0a = partCenter + to_p0a * (p0len * 0.6667f + p1len * 0.3333f);

                Vector3 p0b = (p0 * 0.3333f + p1 * 0.6667f);
                Vector3 to_p0b = (p0b - partCenter).normalized;
                p0b = partCenter + to_p0b * (p0len * 0.3333f + p1len * 0.6667f);*/

                vertices.Add(p0);
                //vertices.Add(p0a);
                //vertices.Add(p0b);

                uvs.Add(new Vector2(textureTwisting + unAnglesCount * (float)a, textureV));
                //uvs.Add(new Vector2(textureTwisting + unAnglesCount * (float)a + 0.3333f * unAnglesCount, textureV));
                //uvs.Add(new Vector2(textureTwisting + unAnglesCount * (float)a + 0.6667f * unAnglesCount, textureV));
            }

            vertices.Add(vertices[firstVertexID]);
            uvs.Add(new Vector2(textureTwisting + unAnglesCount + unAnglesCount * (float)(anglesCount - 1), textureV));

            if (part.HasConnectionToNext && (p != pcountm1))
            {
                if (!cachedIndices.TryGetValue(p, out int[] lineIndices))
                {
                    lineIndices = GenerateIndices(p, meshRadialSegmentsCount);
                    cachedIndices.Add(p, lineIndices);
                }

                indices.AddRange(lineIndices);
            }
        }

        if (mesh.vertices.Length >= 3)
        {
            mesh.triangles = ZeroIndices;
        }

        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    private static Dictionary<int, int[]> cachedIndices = new Dictionary<int, int[]>();

    private int[] GenerateIndices(int lengthIndex, int radialSegsCount)
    {
        int[] indices = new int[radialSegsCount * 6];
        int index = 0;

        int y = lengthIndex * (radialSegsCount + 1);
        for (int j = 0; j < radialSegsCount; j++)
        {
            int baseIndex = y + j;
            int base2Index = baseIndex + radialSegsCount + 1;
            indices[index + 0] = baseIndex + 0;
            indices[index + 1] = baseIndex + 1;
            indices[index + 2] = base2Index + 1;

            indices[index + 3] = baseIndex + 0;
            indices[index + 4] = base2Index + 1;
            indices[index + 5] = base2Index;
            index += 6;
        }

        return indices;
    }
}

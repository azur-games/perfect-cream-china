using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CreamMesh
{
    public static void CreateCreamMesh(List<CreamParticle> parts, out Mesh mesh)
    {
        int anglesCount = parts[0].angle.Length;
        int meshRadialSegmentsCount = anglesCount * 4;
        int pcountm1 = parts.Count - 1;

        int vertsCount = (meshRadialSegmentsCount + 1) * parts.Count;
        List<Vector3> vertices = new List<Vector3>(vertsCount + 100);
        List<Vector2> uvs = new List<Vector2>(vertsCount + 100);
        List<Vector3> oneVertsLine = new List<Vector3>(100);
        List<int> indices = new List<int>(meshRadialSegmentsCount * pcountm1 * 6);

        for (int p = 0; p < parts.Count; p++)
        {
            CreamParticle part = parts[p];
            Vector3 partCenter = part.Position;

            for (int a = 0; a < anglesCount; a++)
            {
                int nextA = a + 1;
                if (nextA == anglesCount) nextA = 0;

                Vector3 p0 = part.perimeterPoints[a];
                Vector3 p1 = part.perimeterPoints[nextA];

                oneVertsLine.Add(p0 * 0.93f + p1 * 0.07f);
                oneVertsLine.Add((p0 * 0.55f + p1 * 0.45f) * 0.93f + partCenter * 0.07f);
                oneVertsLine.Add((p0 * 0.45f + p1 * 0.55f) * 0.93f + partCenter * 0.07f);
                oneVertsLine.Add(p0 * 0.07f + p1 * 0.93f);

                uvs.Add(new Vector2(a + 0.05f, 0.0f));
                uvs.Add(new Vector2(a + 0.48f, 0.0f));
                uvs.Add(new Vector2(a + 0.52f, 0.0f));
                uvs.Add(new Vector2(a + 0.95f, 0.0f));
            }

            oneVertsLine.Add(oneVertsLine[0]);
            vertices.AddRange(oneVertsLine);
            oneVertsLine.Clear();

            uvs.Add(new Vector2(anglesCount, 0.0f));

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

        mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    private static Dictionary<int, int[]> cachedIndices = new Dictionary<int, int[]>();

    private static int[] GenerateIndices(int lengthIndex, int radialSegsCount)
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

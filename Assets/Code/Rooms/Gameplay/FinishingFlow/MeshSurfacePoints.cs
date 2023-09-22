using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSurfacePoints
{
    public float MinX { get; private set; }
    public float MaxX { get; private set; }

    public float MinY { get; private set; }
    public float MaxY { get; private set; }

    public float MinZ { get; private set; }
    public float MaxZ { get; private set; }

    public List<Vector3> Points { get; private set; }

    public MeshSurfacePoints(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        MinX = float.MaxValue;
        MaxX = float.MinValue;
        MinY = float.MaxValue;
        MaxY = float.MinValue;
        MinZ = float.MaxValue;
        MaxZ = float.MinValue;
        foreach (Vector3 v in vertices)
        {
            MinX = Mathf.Min(v.x, MinX);
            MaxX = Mathf.Max(v.x, MaxX);
            MinY = Mathf.Min(v.y, MinY);
            MaxY = Mathf.Max(v.y, MaxY);
            MinZ = Mathf.Min(v.z, MinZ);
            MaxZ = Mathf.Max(v.z, MaxZ);
        }

        int[] indices = mesh.triangles;
        int trisCount = indices.Length / 3;
        Points = new List<Vector3>(trisCount);

        for (int i = 0; i < trisCount; i++)
        {
            int index = i * 3;
            Vector3 point = 0.3333f * (vertices[indices[index + 0]] + vertices[indices[index + 1]] + vertices[indices[index + 2]]);
            Points.Add(point);
        }
    }

    public Vector3 GetCenter()
    {
        return new Vector3(0.5f * (MaxX + MinX), 0.5f * (MaxY + MinY), 0.5f * (MaxZ + MinZ));
    }

    public float GetPercentHeight(float percents)
    {
        return MinY + (MaxY - MinY) * percents;
    }

    public List<Vector3> GetPoints(float higherThanPart, float minPartX, float maxPartX)
    {
        List<Vector3> resList = new List<Vector3>(Points.Count);
        float minAvailY = MinY + (MaxY - MinY) * higherThanPart;
        float minAvailX = MinX + minPartX * (MaxX - MinX);
        float maxAvailX = MinX + maxPartX * (MaxX - MinX);

        foreach (Vector3 vec in Points)
        {
            if (vec.y < minAvailY) continue;

            if (vec.x < minAvailX) continue;
            if (vec.x > maxAvailX) continue;

            resList.Add(vec);
        }

        return resList;
    }

    public List<Vector3> GetPoints(float higherThanPart)
    {
        List<Vector3> resList = new List<Vector3>(Points.Count);
        float minAvailY = MinY  + (MaxY - MinY) * higherThanPart;

        foreach (Vector3 vec in Points)
        {
            if (vec.y < minAvailY) continue;
            resList.Add(vec);
        }

        return resList;
    }

    public List<Vector3> GetPoints(float higherThanPart, float minPartX, float maxPartX, int count)
    {
        List<Vector3> resList = new List<Vector3>(count);
        List<Vector3> points = GetPoints(higherThanPart, minPartX, maxPartX);
        List<Vector3> pointsCopy = new List<Vector3>(points);
        if (0 == points.Count) return new List<Vector3>() { Vector3.zero };

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, points.Count);
            resList.Add(points[index]);
            points.RemoveAt(index);

            if (0 == points.Count)
            {
                points.AddRange(pointsCopy);
            }
        }

        return resList;
    }

    public List<Vector3> GetPoints(float higherThanPart, int count)
    {
        List<Vector3> resList = new List<Vector3>(count);
        List<Vector3> points = GetPoints(higherThanPart);
        List<Vector3> pointsCopy = new List<Vector3>(points);
        if (0 == points.Count) return new List<Vector3>() { Vector3.zero };

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, points.Count);
            resList.Add(points[index]);
            points.RemoveAt(index);

            if (0 == points.Count)
            {
                points.AddRange(pointsCopy);
            }
        }

        return resList;
    }
}

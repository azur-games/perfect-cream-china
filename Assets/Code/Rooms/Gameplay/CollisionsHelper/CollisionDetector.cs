using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector
{
    public static CollisionDetector GetAuto(Collider collider)
    {
        if (collider is BoxCollider)
        {
            return new CollisionDetectorBox(collider as BoxCollider);
        }

        if (collider is SphereCollider)
        {
            return new CollisionDetectorSphere(collider as SphereCollider);
        }

        return new CollisionDetector(collider);
    }

    public Collider collider;
    public Vector3 Min;
    public Vector3 Max;
    public Vector3 Center;
    public int ID;

    public CollisionDetector(Collider collider)
    {
        this.collider = collider;
        ID = collider.GetInstanceID();

        UpdateValues();
    }

    public void UpdateValues()
    {
        Bounds bounds = collider.bounds;
        Min = bounds.min;
        Max = bounds.max;
        Center = 0.5f * (Min + Max);
    }

    public virtual bool IsPointInside(Vector3 point)
    {
        return (collider.ClosestPoint(point) - point).sqrMagnitude < 0.0001f;
    }

    public virtual bool IsNear(float posx, float posy, float dist)
    {
        if ((posy - Max.y) > dist) return false;

        if ((Min.x - posx) > dist) return false;
        if ((posx - Max.x) > dist) return false;

        if ((Min.y - posy) > dist) return false;

        return true;
    }
}

public class CollisionDetectorBox : CollisionDetector
{
    public CollisionDetectorBox(BoxCollider collider) : base(collider)
    {
        
    }

    public override bool IsPointInside(Vector3 point)
    {
        if (Min.x > point.x) return false;
        if (Max.x < point.x) return false;
        if (Min.y > point.y) return false;
        if (Max.y < point.y) return false;

        return true;
    }
}

public class CollisionDetectorSphere : CollisionDetector
{
    public float Radius;
    public float RadiusSqr;

    public CollisionDetectorSphere(SphereCollider collider) : base(collider)
    {
        Radius = (Max.x - Min.x) * 0.5f;
        RadiusSqr = Radius * Radius;
    }

    public override bool IsPointInside(Vector3 point)
    {
        return (Center - point).sqrMagnitude <= RadiusSqr;
    }
}


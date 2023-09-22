using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalCreamBehaviour : MonoBehaviour
{
    private Vector3 capsuleFrom;
    private Vector3 capsuleTo;
    private float capsuleRadius;

    private void Awake()
    {
        FindCapsuleParams();

        CreateDebugCapsule();
    }

    private void FindCapsuleParams()
    {
        Bounds bounds = this.gameObject.GetComponent<MeshFilter>().mesh.bounds;
        capsuleRadius = bounds.size.z * 0.5f - 0.02f;
        float halfLength = (bounds.size.x * 0.5f - capsuleRadius) - 0.025f;
        capsuleFrom = bounds.center + Vector3.left * halfLength + Vector3.down * 0.04f;
        capsuleTo = bounds.center + Vector3.right * halfLength + Vector3.down * 0.04f;
    }

    private void CreateDebugCapsule()
    {
        Transform capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule).transform;
        capsule.transform.parent = this.transform;
        GameObject.Destroy(capsule.GetComponent<MeshRenderer>());
        GameObject.Destroy(capsule.GetComponent<MeshFilter>());
        CapsuleCollider cc = capsule.GetComponent<CapsuleCollider>();

        capsule.position = (capsuleFrom + capsuleTo) * 0.5f;
        capsule.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);

        cc.height = capsuleTo.x - capsuleFrom.x + 2.0f * capsuleRadius;
        cc.radius = capsuleRadius;
    }
}

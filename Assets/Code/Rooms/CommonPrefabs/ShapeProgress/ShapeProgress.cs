using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeProgress : MonoBehaviour
{
    [SerializeField] private MeshRenderer mRenderer;

    void Start()
    {
        UpdateAspect();
    }

    public void SetImage(Sprite sprite)
    {
        UpdateAspect();
    }

    private void UpdateAspect()
    {
        Texture2D texture = mRenderer.material.GetTexture("_MainTex") as Texture2D;
        if (null == texture) return;

        mRenderer.transform.localScale = new Vector3(
            mRenderer.transform.localScale.x,
            (float)mRenderer.transform.localScale.x *
            (float)texture.height /
            (float)texture.width);

    }
}

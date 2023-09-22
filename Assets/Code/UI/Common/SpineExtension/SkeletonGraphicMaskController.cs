using UnityEngine;
using Spine.Unity;


public class SkeletonGraphicMaskController : MonoBehaviour
{
    #region Fields

    [SerializeField] SkeletonGraphic skeletonGraphic;
    [SerializeField] RectTransform maskRectTransform;

    #endregion



    #region Unity lifecycle

    void Update()
    {
        Camera graphicCamera = skeletonGraphic.canvas.worldCamera;
        Matrix4x4 maskToClipTransform = graphicCamera.projectionMatrix * 
                                        graphicCamera.worldToCameraMatrix * 
                                        maskRectTransform.localToWorldMatrix;
        Matrix4x4 clipToMaskTransform = maskToClipTransform.inverse;
        
        Vector4 maskBounds = new Vector4(maskRectTransform.rect.xMin, 
                                         maskRectTransform.rect.yMin, 
                                         maskRectTransform.rect.xMax, 
                                         maskRectTransform.rect.yMax);

        skeletonGraphic.material.SetMatrix("_ClipToMaskTransform", clipToMaskTransform);
        skeletonGraphic.material.SetVector("_MaskBounds", maskBounds);
    }

    #endregion
}

using UnityEngine;
using UnityEngine.UI;


public class GraphicMaskController : MonoBehaviour
{
    #region Fields

    [SerializeField] Graphic graphic;
    [SerializeField] RectTransform maskRectTransform;

    #endregion



    #region Unity lifecycle

    void Update()
    {
        Camera graphicCamera = graphic.canvas.worldCamera;
        if(graphicCamera == null)
        {
            return;
        }

        Matrix4x4 maskToClipTransform = graphicCamera.projectionMatrix * 
                                        graphicCamera.worldToCameraMatrix * 
                                        maskRectTransform.localToWorldMatrix;
        Matrix4x4 clipToMaskTransform = maskToClipTransform.inverse;
        
        Vector4 maskBounds = new Vector4(maskRectTransform.rect.xMin, 
                                         maskRectTransform.rect.yMin, 
                                         maskRectTransform.rect.xMax, 
                                         maskRectTransform.rect.yMax);

        graphic.material.SetMatrix("_ClipToMaskTransform", clipToMaskTransform);
        graphic.material.SetVector("_MaskBounds", maskBounds);
    }

    #endregion
}
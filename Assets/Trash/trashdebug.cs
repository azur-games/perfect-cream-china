using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class trashdebug : MonoBehaviour
{
    public bool OverlayEnable = false;
    public Color Color = Color.red;

    public bool Add = false;
    public bool Remove = false;

    private List<UIOverlay.Instance> overlays = new List<UIOverlay.Instance>();

    private static bool alreadyLoaded = false;

    private void Awake()
    {
        if (alreadyLoaded)
        {
            GameObject.Destroy(this.gameObject);
            return;
        }

        alreadyLoaded = true;
        GameObject.DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if (Add)
        {
            Add = false;

            UIOverlay.Instance overlayInstance = Env.Instance.UI.Overlay.Set(
                this,
                OverlayEnable ? Color : (Color?)null,
                (oi) =>
                {
                    Debug.Log("FINISHED!");
                });

            overlays.Add(overlayInstance);
        }

        if (Remove)
        {
            Remove = false;

            if (overlays.Count > 0)
            {
                UIOverlay.Instance lastTarget = overlays[overlays.Count - 1];
                overlays.RemoveAt(overlays.Count - 1);
                Env.Instance.UI.Overlay.Remove(lastTarget);
            }
        }
    }
}
*/
using UnityEngine;


[RequireComponent(typeof(RectTransform))]
public class SafeArea : MonoBehaviour
{
    #region Fields

    static ScreenOrientation lastScreenOrientation;

    RectTransform safeAreaRect;

    #endregion



    #region Unity lifecycle

    void Awake ()
    {
        safeAreaRect = GetComponent<RectTransform> ();
        lastScreenOrientation = Screen.orientation;

        Refresh ();
    }


    void Update ()
    {
        ScreenOrientation currentScreenOrientation = Screen.orientation;
        if (lastScreenOrientation != Screen.orientation)
        {
            //GameController.Instance.SetNewSafeArea(Screen.safeArea);
            Refresh();
            lastScreenOrientation = currentScreenOrientation;
        }
    }

    #endregion



    #region Private methods

    void Refresh ()
    {
        //Rect safeArea = GetSafeArea ();
        //ApplySafeArea (safeArea);
    }


    /*Rect GetSafeArea ()
    {
        return GameController.Instance.SafeArea;
    }*/


    void ApplySafeArea (Rect r)
    {
        Vector2 anchorMin = r.position;
        Vector2 anchorMax = r.position + r.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
        safeAreaRect.anchorMin = anchorMin;
        safeAreaRect.anchorMax = anchorMax;
    }

    #endregion
}
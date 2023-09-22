using System;
using UnityEngine;
using UnityEngine.UI;


public class MetagamePiggyBubble : MonoBehaviour
{
    #region Fields

    [SerializeField] private Transform contentRoot;
    [SerializeField] private Button button;
    [SerializeField] [Min(0.0f)] private float showingDelay = 2.0f;

    private Action onClicked = null;
    private float timer = 0.0f;
    private bool isShowRequired = false;

    #endregion



    #region Properties

    public bool IsShowing
    {
        get
        {
            return contentRoot.gameObject.activeSelf;
        }
        private set
        {
            contentRoot.gameObject.SetActive(value);
        }
    }

    #endregion


    
    #region Unity lifecycle

    private void Start()
    {
        button.onClick.AddListener(Button_OnClick);
        button.interactable = false;

        IsShowing = false;
    }


    private void Update()
    {
        if (!isShowRequired)
            return;

        if (timer < showingDelay)
        {
            timer += Time.deltaTime;
            return;
        }

        if (!IsShowing && Env.Instance.Rooms.MetagameRoom.Controller != null)
        {
            Show();
        }
    }


    private void OnDestroy()
    {
        button.onClick.RemoveListener(Button_OnClick);
    }

    #endregion



    #region Show / Hide

    public void Show(Action onClicked)
    {
        this.onClicked = onClicked;

        if (!isShowRequired)
        {
            isShowRequired = true;
            timer = 0.0f;
        }
    }


    private void Show()
    {
        isShowRequired = false;

        Vector3? bubblePivotPoint = ScenePivot.GetPivotViewPoint(Env.Instance.Rooms.MetagameRoom.Controller.Pivots, ScenePivot.Subject.MetaPiggyBubble);
        if (!bubblePivotPoint.HasValue) return;

        MetagameRoomUI metagameUI = this.gameObject.GetComponentInParent<MetagameRoomUI>();
        Vector3 bubblePoint = metagameUI.UICamera.ViewportToScreenPoint(new Vector3(bubblePivotPoint.Value.x - 0.5f, bubblePivotPoint.Value.y - 0.5f, bubblePivotPoint.Value.z));

        Env.Instance.Sound.PlaySound(AudioKeys.UI.PopupCloud);

        this.transform.localPosition = new Vector3(bubblePoint.x, bubblePoint.y, this.transform.localPosition.z);

        button.interactable = true;
        IsShowing = true;
    }


    public void Hide()
    {
        IsShowing = false;
        isShowRequired = false;
    }

    #endregion



    #region Events handling

    private void Button_OnClick()
    {
        button.interactable = false;
        IsShowing = false;

        onClicked?.Invoke();
    }

    #endregion
}

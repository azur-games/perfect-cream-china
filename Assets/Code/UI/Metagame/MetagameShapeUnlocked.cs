using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetagameShapeUnlocked : MonoBehaviour
{
    public static void CreateMetagameShapeUnlockedPopup(MetagameShapeUnlocked original, System.Action onFinish)
    {
        GameObject go = GameObject.Instantiate(original.gameObject);

        RectTransform rt = (go.transform as RectTransform);
        RectTransform rtOriginal = (original.transform as RectTransform);

        rt.SetParent(original.transform.parent);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rtOriginal.rect.width);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rtOriginal.rect.height);
        go.transform.localScale = original.transform.localScale;
        go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0.0f);
        MetagameShapeUnlocked msu = go.GetComponent<MetagameShapeUnlocked>();

        msu.Show(onFinish);
    }

    private enum State
    {
        None,
        Awaitig,
        Moving,
    }

    public UnityEngine.UI.Image Icon;
    public UnityEngine.UI.Image GrayBack;
    public Color StartBackColor;
    public Color EndBackColor;
    public Transform TargetIconPosition;

    public float AwaitingTime = 1.0f;
    public float MovingTime = 0.5f;
    public float TargetRescale = 0.5f;

    private State currentState = State.None;
    private float timer;
    private Vector2 initialSize;
    private Vector3 initialPosition;

    private System.Action onFinish;

    private void Show(System.Action onFinish)
    {
        string lastAvailableShape = Env.Instance.Inventory.GetLastAvailableShape();
        Sprite icon = Env.Instance.Content.LoadContentItemIconRef(ContentAsset.AssetType.Shape, lastAvailableShape).Icon;

        Icon.sprite = icon;

        initialSize = new Vector2(Icon.rectTransform.rect.width,
            (float)Icon.rectTransform.rect.width *
            (float)icon.rect.height /
            (float)icon.rect.width);
        Icon.rectTransform.sizeDelta = initialSize;

        this.gameObject.SetActive(true);

        initialPosition = Icon.transform.localPosition;
        this.onFinish = onFinish;
        timer = 0.0f;
        currentState = State.Awaitig;
        UpdateView();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        UpdateView();
    }

    private void UpdateView()
    {
        switch (currentState)
        {
            case State.Awaitig:
                if (timer > AwaitingTime)
                {
                    timer = 0.0f;
                    currentState = State.Moving;
                }

                break;

            case State.Moving:
                float step = Mathf.Clamp01(timer / MovingTime);

                Icon.transform.localPosition = Vector3.Lerp(initialPosition, TargetIconPosition.localPosition, step);
                Icon.rectTransform.sizeDelta = Vector2.Lerp(initialSize, initialSize * TargetRescale, step);
                GrayBack.color = Color.Lerp(StartBackColor, EndBackColor, step);

                if (timer > MovingTime)
                {
                    timer = 0.0f;
                    currentState = State.None;
                    onFinish?.Invoke();

                    GameObject.Destroy(this.gameObject);
                }

                break;
        }
    }
}

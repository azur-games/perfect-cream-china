using UnityEngine;

public class SupplyAnimation : MonoBehaviour
{

    [SerializeField] float _heightVal = -0.1f;

    [Space(5)][Header("Start")]
    [SerializeField] float _startScaleVal = -0.15f;
    [SerializeField] private AnimationCurve _scaleCurve;
    [SerializeField] private AnimationCurve _HeightCurve;
    [SerializeField] private float _animTime = 0.4f;
    [Space(5)] [Header("Stop")]
    [SerializeField] float _stopScaleVal = 0.15f;
    [SerializeField] private AnimationCurve _scaleCurveStop;
    [SerializeField] private AnimationCurve _HeightCurveStop;
    [SerializeField] private float _animTimeStop = 0.4f;

    public bool Ready { get { return _isSupplying ? _animTimer > _animTime : _animTimer > _animTimeStop; } }

    bool _isAnimating;
    private Vector3 _defaultScale;
    private Vector3 _defaulPos;

    float _animTimer = 0;

    bool _isSupplying = false;

    public float TimeScale { get; set; } = 1.0f;

    private void Awake()
    {
        _defaultScale = transform.localScale;
        _defaulPos = transform.position;
    }


    public void Animate(bool isSupplying)
    {
        if (_isSupplying == isSupplying)
            return;

        _isSupplying = isSupplying;
        _animTimer = 0;
        _isAnimating = true;
    }

    public void ForcedDownAndStop()
    {
        TimeScale = 1.0f;
        _isSupplying = true;
        _animTimer = 0;
        _isAnimating = true;

        UpdateSelf(_animTime);

        _animTimer = float.MaxValue;

        this.enabled = false;
    }

    void Update()
    {
        UpdateSelf(Time.deltaTime);
    }

    public float GetPushingHeight()
    {
        return _heightVal * _HeightCurve.Evaluate(1.0f);
    }

    private void UpdateSelf(float dTime)
    {
        if (failAnimationTimer >= 0.0f)
        {
            failAnimationTimer += dTime;
            this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, failAnimationRotaion.Evaluate(failAnimationTimer) * 360.0f);
            this.transform.localPosition = new Vector3(
                failAnimationMoveX.Evaluate(failAnimationTimer),
                failAnimationMoveY.Evaluate(failAnimationTimer),
                failAnimationMoveZ.Evaluate(failAnimationTimer));
            return;
        }

        if (TimeScale < 0.001f)
            return;

        if (!_isAnimating)
            return;

        if (_animTimer > (_isSupplying ? _animTime : _animTimeStop))
            return;

         _animTimer += dTime * TimeScale;
        float t = _animTimer / (_isSupplying ? _animTime : _animTimeStop);
        float scale = _isSupplying? _startScaleVal * _scaleCurve.Evaluate(t) : _stopScaleVal * _scaleCurveStop.Evaluate(t);
        float currentHeight = _isSupplying ? _HeightCurve.Evaluate(t) : _HeightCurveStop.Evaluate(t);
        float height = currentHeight * _heightVal;
        transform.localScale = _defaultScale + new Vector3(scale, 0, scale);
        transform.position = _defaulPos + new Vector3(0, height, 0);

        if (_animTimer > (_isSupplying ? _animTime : _animTimeStop))
        {
            _isAnimating = false;
            TimeScale = 1.0f;
        }
    }

    [SerializeField] private AnimationCurve failAnimationRotaion;
    [SerializeField] private AnimationCurve failAnimationMoveX;
    [SerializeField] private AnimationCurve failAnimationMoveY;
    [SerializeField] private AnimationCurve failAnimationMoveZ;

    private float failAnimationTimer = -1.0f;
    private Quaternion rotationBeforeFail;
    private Vector3 positionBeforeFail;

    public void PlayFailAnimation()
    {
        failAnimationTimer = 0.0f;
    }

    public void StopFailAnimation()
    {
        if (failAnimationTimer < 0.0f) return;
        failAnimationTimer = -1.0f;
        this.transform.rotation = rotationBeforeFail;
        this.transform.localPosition = positionBeforeFail;
    }
}

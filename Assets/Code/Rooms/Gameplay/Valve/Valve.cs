using System;
using UnityEngine;

public class Valve : MonoBehaviour
{
    private const int HAPTIC_PERIOD_ANDROID = 3;
    [SerializeField] CreamCreator creamCreator;
    [SerializeField] GameObject particles;
    [SerializeField] SupplyAnimation _supplyAnimation;

    public bool IsSupplying { get; private set; }

    public ValveAsset Asset { get; private set; }
    private GameObject _oldAsset;


    public Vector3 SpawnPos
    {
        get
        { 
            return creamCreator.transform.position;
        } 
    }

    private Conveyor _conveyor;

    private Shape _animatedShape;
    private Vector3 _deltaPosForAnimation;
    private float _startDistance = 0;
    private float _distance = 0;
    private float _startYPos = 0;

    public void Init(Conveyor conveyor, string valveNameOverride, Action<Collider, CreamParticle, Vector3> onCreamCollision)
    {
        _conveyor = conveyor;

        creamCreator.Init(conveyor, onCreamCollision);

        _deltaPosForAnimation = transform.parent.position - SpawnPos;

        if (string.IsNullOrEmpty(valveNameOverride)) valveNameOverride = Env.Instance.Inventory.ValveName;
        var valveasset = Env.Instance.Content.LoadContentAsset<ValveAsset>(ContentAsset.AssetType.Valve, valveNameOverride);

        Asset = Instantiate(valveasset, _supplyAnimation.transform);
    }

    public bool IsSupplyingInProcess()
    {
        return IsSupplying && _supplyAnimation.Ready;
    }

    public void Supply(bool isOn, bool impressivePushing = false)
    {
        if (creamCreator)
        {
            if (isOn)
            {
                if (_supplyAnimation.Ready && IsSupplying == isOn)
                {
                    creamCreator.Turn(isOn, impressivePushing);
                    if (OptionsPanel.IsVibroEnabled)
                        MakeHaptic();
                }
            }
            else
            {
                if (!creamCreator.CanTurnedOff()) return;
                creamCreator.Turn(isOn, impressivePushing);
            }
        }

        if (isOn == IsSupplying)
            return;

        IsSupplying = isOn;
        _supplyAnimation.Animate(IsSupplying);

        TurnParticles(isOn);
    }

    public void TurnParticles(bool isOn)
    {
        if (Env.Instance.Rules.Effects.Value)
            particles.SetActive(isOn);
    }

    private int haptic_period_counter = 0;
    private void MakeHaptic()
    {
        #if UNITY_ANDROID
        haptic_period_counter++;
        if (haptic_period_counter < HAPTIC_PERIOD_ANDROID) return;
        haptic_period_counter = 0;
        #endif

        MoreMountains.NiceVibrations.MMVibrationManager.Haptic(MoreMountains.NiceVibrations.HapticTypes.Selection);
    }

    public float GetPushingHeight()
    {
        return _supplyAnimation.GetPushingHeight();
    }

    public void ForcedDownAndStop()
    {
        _supplyAnimation.ForcedDownAndStop();
    }

    public void SetSupplyAnimationTimeScale(float timeScale)
    {
        _supplyAnimation.TimeScale = timeScale;
    }

    public void Animate()
    {
        Supply(false);

        if (_startDistance > 0)
            return;

        if (_animatedShape.IsAnimating)
            return;

        _animatedShape.Animate();
    }

    private ValveAsset understudyValveAsset;
    public void SetUnderstudySkin(ValveAsset asset)
    {
        understudyValveAsset = Instantiate(asset, _supplyAnimation.transform);
        understudyValveAsset.gameObject.SetActive(false);
    }

    public void SwapToUnderstudySkin()
    {
        _oldAsset = Asset.gameObject;
        Asset = understudyValveAsset;
        Asset.gameObject.SetActive(true);
        _oldAsset.SafeDestroy();
    }

    public void UpdateSkin(ValveAsset asset)
    {
        _oldAsset = Asset.gameObject;
        Asset = Instantiate(asset, _supplyAnimation.transform);
        _oldAsset.SafeDestroy();
    }


    private void Update()
    {
        if (_startDistance > 0)
        {
            _distance += Time.deltaTime * _conveyor.Speed;
            float t = _distance/ _startDistance;
            transform.parent.SetGlobalPositionY(Mathf.Lerp(_startYPos, (_animatedShape.CreamSpawnPoint.position + _deltaPosForAnimation).y, t));
            if (t >= 1)
                _startDistance = 0;
        }
        else
        {
            if(_animatedShape)
                transform.parent.position = _animatedShape.CreamSpawnPoint.position + _deltaPosForAnimation;
        }
    }


    public CreamCreator GetCreamCreator()
    {
        return creamCreator;
    }
}

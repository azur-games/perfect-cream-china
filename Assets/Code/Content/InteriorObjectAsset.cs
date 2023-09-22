using UnityEngine;


public class InteriorObjectAsset : ContentAsset
{
    public ParticleSystem FX;
    public GameObject PreviewModel;
    public override AssetType GetAssetType() { return AssetType.InteriorObject; }

    private void Awake()
    {
        FX.gameObject.SetActive(false);
    }

    public void PlayCreating(System.Action onFinish)
    {
        MakeFX(FX);

        InteriorUpgradeAnimation upAnim = this.gameObject.GetComponentInChildren<InteriorUpgradeAnimation>();

        if (null == upAnim)
        {
            onFinish?.Invoke();
            return;
        }

        upAnim.Play(InteriorUpgradeAnimation.State.Creating, onFinish);
    }

    public void PlayDestroying(System.Action onFinish)
    {
        InteriorUpgradeAnimation upAnim = this.gameObject.GetComponentInChildren<InteriorUpgradeAnimation>();

        if (null == upAnim)
        {
            onFinish?.Invoke();
            return;
        }

        upAnim.Play(InteriorUpgradeAnimation.State.Destroying, onFinish);
    }

    private void MakeFX(ParticleSystem fx)
    {
        if (null == fx) return;
        GameObject fxGo = new GameObject("FX");
        fxGo.transform.parent = null;
        fxGo.transform.position = fx.transform.position;
        fxGo.transform.rotation = fx.transform.rotation;
        fxGo.transform.localScale = fx.transform.localScale;
        fx.transform.parent = fxGo.transform;
        fx.gameObject.SetActive(true);

        AutonomousTimer.Create(2.0f, () =>
        {
            GameObject.Destroy(fx.gameObject);
        });
    }
}

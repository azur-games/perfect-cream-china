using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersCharacter : MonoBehaviour
{
    public string BARMAN_DELIVERING_ANIMATION_TRIGGER = "Delivery";
    public string APPLAUSE_ANIMATION_TRIGGER = "Applause";
    public string UPGRADE_ANIMATION_TRIGGER = "Upgrade1";
    public string CRY_ANIMATION_TRIGGER = "Cry";

    public enum Model
    {
        None = 0,
        Boy,
        Girl,
        Barman,
        Boy2,
        Girl2,
    }

    [SerializeField] private Model modelOnStart;

    [SerializeField] private GameObject modelBoy;
    [SerializeField] private GameObject modelBoy2;
    [SerializeField] private GameObject modelGirl;
    [SerializeField] private GameObject modelGirl2;
    [SerializeField] private GameObject modelBarman;

    [SerializeField] private GameObject modelBox;
    [SerializeField] private GameObject modelBoxCommon;
    [SerializeField] private GameObject modelBoxPlastic;
    [SerializeField] private GameObject modelBoxMetall;
    [SerializeField] private GameObject modelBoxFastfood;

    private Animator animator;

    private void Start()
    {
        if (modelOnStart != Model.None) SwitchModel(modelOnStart);
    }

    public void SwitchModel(Model model)
    {
        modelBoy.SetActive(model == Model.Boy);
        modelBoy2.SetActive(model == Model.Boy2);
        modelGirl.SetActive(model == Model.Girl);
        modelGirl2.SetActive(model == Model.Girl2);
        modelBarman.SetActive(model == Model.Barman);
    }

    private Model GetCurrentModel()
    {
        if (modelBoy.activeSelf) return Model.Boy;
        if (modelBoy2.activeSelf) return Model.Boy2;
        if (modelGirl.activeSelf) return Model.Girl;
        if (modelGirl2.activeSelf) return Model.Girl2;
        return Model.Barman;
    }

    public void SwitchModelUsingAll()
    {
        switch (GetCurrentModel())
        {
            case Model.Boy: SwitchModel(Model.Boy2); break;
            case Model.Boy2: SwitchModel(Model.Girl); break;
            case Model.Girl: SwitchModel(Model.Girl2); break;
            case Model.Girl2: SwitchModel(Model.Barman); break;
            case Model.Barman: SwitchModel(Model.Boy); break;
        }
    }

    public void StartDelivering(LevelAsset.DeliveredObjectType typeOfDeliveredObject, Color deliveredObjectColor1, Color deliveredObjectColor2)
    {
        StartAnimation(BARMAN_DELIVERING_ANIMATION_TRIGGER, false, null);

        Animator boxAnimator = modelBox.GetComponent<Animator>();
        boxAnimator.SetTrigger(BARMAN_DELIVERING_ANIMATION_TRIGGER);

        modelBoxCommon.SetActive(typeOfDeliveredObject == LevelAsset.DeliveredObjectType.CommonBox);
        modelBoxPlastic.SetActive(typeOfDeliveredObject == LevelAsset.DeliveredObjectType.PlasticBox);
        modelBoxMetall.SetActive(typeOfDeliveredObject == LevelAsset.DeliveredObjectType.MetallicBox);
        modelBoxFastfood.SetActive(typeOfDeliveredObject == LevelAsset.DeliveredObjectType.FastFoodBox);

        if (typeOfDeliveredObject == LevelAsset.DeliveredObjectType.CommonBox)
        {
            Material[] mats = modelBoxCommon.GetComponentInChildren<MeshRenderer>().materials;
            mats[0].SetColor("_MainColor", deliveredObjectColor1);
            mats[1].SetColor("_MainColor", deliveredObjectColor2);
        }
    }

    public void StartApplause(bool withSound)
    {
        StartAnimation(APPLAUSE_ANIMATION_TRIGGER, withSound, AudioKeys.UI.Applause);
    }

    public void StartCry(bool withSound)
    {
        StartAnimation(CRY_ANIMATION_TRIGGER, withSound, null);
    }

    public void StartUpgrade(bool withSound)
    {
        StartAnimation(UPGRADE_ANIMATION_TRIGGER, withSound, null);
    }

    private void StartAnimation(string animationTrigger, bool withSound, string soundName)
    {
        if (null == animator)
        {
            animator = this.gameObject.GetComponentInChildren<Animator>();
        }

        animator.SetTrigger(animationTrigger);

        if (withSound && !string.IsNullOrEmpty(soundName))
        {
            Env.Instance.Sound.PlaySound(soundName);
        }
    }
}

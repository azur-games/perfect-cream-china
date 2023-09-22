using System;
using UnityEngine;
using UnityEngine.UI;

public class ChestItem : MonoBehaviour
{
    [SerializeField] private AnimationEventsListener animationEventsListener;
    [SerializeField] private Safe _safe;
    [SerializeField] private Image _prizeIcon;
    [SerializeField] private Text _coinsAmount;
    [SerializeField] private Image coinsImage;
    [SerializeField] private ParticleSystem confetti;

    public Button Button;

    public Transform CoinsRoot => coinsImage.transform;

    public Safe Safe => _safe;

    public void TurnOffButtonUntilAnimationEvent()
    {
        if (null == animationEventsListener) return;

        Button.interactable = false;
        animationEventsListener.AddListener(() =>
        {
            Button.interactable = true;
        });
    }

    public void Open(int coins, System.Action onAnimationOpened)
    {
        Env.Instance.Sound.PlaySound(AudioKeys.UI.ChestOpen);

        Button.interactable = false;
        _safe.Open();

        AutonomousTimer.Create(1.0f, () =>
        {
            Env.Instance.Sound.PlaySound(AudioKeys.UI.ConfettiDrop);
            if (Env.Instance.Rules.Effects.Value)
                confetti.Play();
        });

        AutonomousTimer.Create(1.8f, () =>
        {
            onAnimationOpened?.Invoke();

            _coinsAmount.text = coins.ToString();
            _coinsAmount.gameObject.SetActive(true);
        });
    }


    public void Open(Sprite prize, System.Action onAnimationOpened)
    {
        Env.Instance.Sound.PlaySound(AudioKeys.UI.ChestOpen);

        Button.interactable = false;
        _safe.Open();

        AutonomousTimer.Create(1.0f, () =>
        {
            Env.Instance.Sound.PlaySound(AudioKeys.UI.ConfettiDrop);
            if (Env.Instance.Rules.Effects.Value)
                confetti.Play();

            onAnimationOpened?.Invoke();

            _prizeIcon.sprite = prize;
            _prizeIcon.rectTransform.sizeDelta = new Vector2(_prizeIcon.rectTransform.rect.width,
                (float)_prizeIcon.rectTransform.rect.width *
                (float)prize.rect.height /
                (float)prize.rect.width);

            _prizeIcon.gameObject.SetActive(true);
        });       
    }
}

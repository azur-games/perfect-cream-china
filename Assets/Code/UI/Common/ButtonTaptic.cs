using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTaptic : MonoBehaviour
{
    [SerializeField] private MoreMountains.NiceVibrations.HapticTypes hapticType;

    void Start()
    {
        UnityEngine.UI.Button button = this.gameObject.GetComponentInChildren<UnityEngine.UI.Button>();
        button.onClick.AddListener(() =>
        {
            if (OptionsPanel.IsVibroEnabled)
                MoreMountains.NiceVibrations.MMVibrationManager.Haptic(hapticType);
        });
    }
}

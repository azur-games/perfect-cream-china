using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GDPRWindowController : MonoBehaviour
{
   [SerializeField] private TextMeshProUGUI _headerText;
   [SerializeField] private TextMeshProUGUI _gdprText;
   [SerializeField] private TextMeshProUGUI _termsText;
   [SerializeField] private TextMeshProUGUI _privacyText;
   [SerializeField] private TextMeshProUGUI _acceptText;
   
   [SerializeField] private Button _acceptButton;
   [SerializeField] private Button _termsButton;
   [SerializeField] private Button _privacyButton;

   public void Initialize(UnityAction onAccept, string headerText, string mainText, string termsButton, string privacyButton, string linkTerms, string linkPrivacy, string accept)
   {
      _acceptButton.onClick.AddListener(onAccept);
      _headerText.SetText(headerText);
      _gdprText.SetText(mainText);
      _termsText.SetText(termsButton);
      _privacyText.SetText(privacyButton);
      _acceptText.SetText(accept);
      
      _privacyButton.onClick.AddListener(() => Application.OpenURL(linkPrivacy));
      _termsButton.onClick.AddListener(() => Application.OpenURL(linkTerms));
   }

   public void AddOnClickListener(UnityAction onConfirmed) => _acceptButton.onClick.AddListener(onConfirmed);

}


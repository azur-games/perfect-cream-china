using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GDPRWindowController : MonoBehaviour
{
   [SerializeField] private Text _headerTextUnity;
   [SerializeField] private Text _gdprTextUnity;
   [SerializeField] private Text _termsTextUnity;
   [SerializeField] private Text _privacyTextUnity;
   [SerializeField] private Text _acceptTextUnity;
   [SerializeField] private GameObject hindiBlock;
   
   [SerializeField] private Button _acceptButton;
   [SerializeField] private Button _termsButton;
   [SerializeField] private Button _privacyButton;

   [SerializeField] private Font thaiFont;
   [SerializeField] private Font hindiFontChankaya;
   [SerializeField] private Font defaultFont;

   public void Initialize(UnityAction onAccept, string headerText, string mainText, string termsButton, string privacyButton, string linkTerms, string linkPrivacy, string accept, bool isHindi, bool goesOverflow, bool isThai)
   {
      if (isHindi)
      {
         _headerTextUnity.font = hindiFontChankaya;
         _termsTextUnity.font = hindiFontChankaya;
         _privacyTextUnity.font = hindiFontChankaya;
         _acceptTextUnity.font = hindiFontChankaya;
         headerText = UnicodeToKruti.Convert(headerText);
         termsButton = UnicodeToKruti.Convert(termsButton);
         privacyButton = UnicodeToKruti.Convert(privacyButton);
         accept = UnicodeToKruti.Convert(accept);
         
         hindiBlock.SetActive(true);
         _gdprTextUnity.gameObject.SetActive(false);
      }
      else if (isThai)
      {
         _headerTextUnity.font = thaiFont;
         _gdprTextUnity.font = thaiFont;
         _termsTextUnity.font = thaiFont;
         _privacyTextUnity.font = thaiFont;
         _acceptTextUnity.font = thaiFont;
         
         _headerTextUnity.resizeTextMaxSize = 80;
         _gdprTextUnity.resizeTextMaxSize = 120;
         _termsTextUnity.fontSize = 80;
         _privacyTextUnity.fontSize = 80;
         _acceptTextUnity.resizeTextMaxSize = 80;
      }
      else
      {
         _headerTextUnity.font = defaultFont;
         _gdprTextUnity.font = defaultFont;
         _termsTextUnity.font = defaultFont;
         _privacyTextUnity.font = defaultFont;
         _acceptTextUnity.font = defaultFont;
      }

      if (goesOverflow)
      {
         _gdprTextUnity.horizontalOverflow = HorizontalWrapMode.Overflow;
      }
      _headerTextUnity.text = headerText;
      _gdprTextUnity.text = mainText;
      _termsTextUnity.text = termsButton;
      _privacyTextUnity.text = privacyButton;
      _acceptTextUnity.text = accept;
       
      _acceptButton.onClick.AddListener(onAccept);
      _privacyButton.onClick.AddListener(() => Application.OpenURL(linkPrivacy));
      _termsButton.onClick.AddListener(() => Application.OpenURL(linkTerms));
   }
   
   public void InitializeWithoutLocalization(UnityAction onAccept, string linkTerms, string linkPrivacy)
   {
      _acceptButton.onClick.AddListener(onAccept);

      _privacyButton.onClick.AddListener(() => Application.OpenURL(linkPrivacy));
      _termsButton.onClick.AddListener(() => Application.OpenURL(linkTerms));
   }

   public void AddOnClickListener(UnityAction onConfirmed) => _acceptButton.onClick.AddListener(onConfirmed);

}


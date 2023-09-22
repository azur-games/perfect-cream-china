using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code
{
    public class TextLinkHandler : MonoBehaviour, IPointerClickHandler
    {
        private const string TermsOfUseLink = "https://aigames.ae/policy#terms";
        private const string PrivacyPolicyLink = "https://aigames.ae/policy#privacy";
        private TMP_Text _text;

        private void OnEnable()
        {
            _text = GetComponent<TMP_Text>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var linkIndex = TMP_TextUtilities.FindIntersectingLink(_text, Input.mousePosition, null);

            if (linkIndex >= 0)
            {
                var linkId = _text.textInfo.linkInfo[linkIndex].GetLinkID();
                var url = linkId switch
                {
                    "TermsOfUse" => TermsOfUseLink,
                    "PrivacyPolicy" => PrivacyPolicyLink,
                    _ => throw new ArgumentOutOfRangeException()
                };

                Application.OpenURL(url);
            }
        }
    }
}
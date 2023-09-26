using UnityEngine;
using UnityEngine.UI;

namespace Code.Subscriptions
{
    public class CustomSubscriptionPopUp : MonoBehaviour
    {
        [SerializeField] private Button _closeButton;

        public void Show()
        {
            _closeButton.onClick.AddListener(Hide);

            gameObject.SetActive(true);
            Env.Instance.Inventory.AddBucks(1000);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
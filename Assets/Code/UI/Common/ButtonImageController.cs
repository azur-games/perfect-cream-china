using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class ButtonImageController : MonoBehaviour
{
    #region Fields

    [SerializeField] Sprite nonInteractableImage;

    Sprite defaultImage = null;
    Button button = null;
    Image image = null;

    bool lastInteractableCheck = false;

    #endregion



    #region Unity lifecycle

    void Start()
    {
        button = gameObject.GetComponent<Button>();
        image = gameObject.GetComponent<Image>();

        if (button != null)
        {
            defaultImage = image.sprite;
            
            UpdateImage();
        }
    }


    void Update()
    {
        if (button != null && lastInteractableCheck != button.interactable)
        {
            UpdateImage();
        }
    }

    #endregion



    #region Update image

    void UpdateImage()
    {
        lastInteractableCheck = button.interactable;

        if (image != null)
        {
            if (lastInteractableCheck)
            {
                image.sprite = defaultImage;
            }
            else
            {
                image.sprite = nonInteractableImage;
            }
        }
    }

    #endregion
}
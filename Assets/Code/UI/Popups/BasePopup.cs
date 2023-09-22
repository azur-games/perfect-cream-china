using System;
using UnityEngine;
using UnityEngine.UI;


namespace Modules.General
{
    public class BasePopup : MonoBehaviour
    {
        #region Fields

        [Header("Base Popup Settings")]
        [SerializeField] protected Button closeButton = null;
        [SerializeField] protected Animator animator = null;
        [SerializeField] protected AnimationClip showPopupClip = null;
        [SerializeField] protected AnimationClip idlePopupClip = null;
        [SerializeField] protected AnimationClip hidePopupClip = null;

        protected Action<bool> closeCallback = null;

        protected bool closeResult = false;

        #endregion



        #region Unity lifecycle

        protected virtual void OnEnable()
        {
            closeButton?.onClick.AddListener(CloseButton_OnClick);
        }


        protected virtual void OnDisable()
        {
            closeButton?.onClick.RemoveListener(CloseButton_OnClick);
        }

        #endregion



        #region Methods

        public virtual void Show(Action<bool> callback)
        {
            animator.Play(showPopupClip.name);
            closeCallback = callback;
        }


        protected virtual void Hide(bool result)
        {
            closeResult = result;
            animator.Play(hidePopupClip.name);
        }

        #endregion



        #region Events handlers

        protected virtual void CloseButton_OnClick()
        {
            Hide(false);
        }


        // Must coincide with animation event name or redeclared
        public virtual void PopupShown()
        {
            animator.Play(idlePopupClip.name);
        }


        // Must coincide with animation event name or redeclared
        public virtual void PopupHiden()
        {
            closeCallback?.Invoke(closeResult);
            closeCallback = null;

            Destroy(this.gameObject);
        }

        #endregion
    }
}

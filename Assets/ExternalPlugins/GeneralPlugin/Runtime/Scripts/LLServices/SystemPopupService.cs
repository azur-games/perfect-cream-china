using Modules.General.Abstraction;
using Modules.General.InitializationQueue;
using Modules.General.ServicesInitialization;
using Modules.Hive.Ioc;
using System;


namespace Modules.General
{
    [InitQueueService(-300, bindTo: typeof(ISystemPopupService))]
    public class SystemPopupService : ISystemPopupService, IInitializableService
    {
        #region Methods

        public void ShowPopupWithoutButtons(string title, string message)
        {

        }


        public void HidePopupWithoutButtons()
        {

        }


        public void ShowPopUp(string title, string message, string buttonTitle, System.Action callback = null)
        {

        }


        public void ShowPopUpWithTwoButtons(string title, string message, string firstButtonTitle,
            string secondButtonTitle, Action firstButtonCallback = null, Action secondButtonCallback = null,
            bool isVerticalLayout = true)
        {

        }

        #endregion



        #region IServiceInitializable

        public void InitializeService(
            IServiceContainer container,
            Action onCompleteCallback,
            Action<IInitializableService, InitializationStatus> onErrorCallback)
        {
            onCompleteCallback?.Invoke();
        }

        #endregion
    }
}

using Modules.General.Abstraction;
using System;
using System.Collections.Generic;


namespace Modules.Advertising
{
    public interface IEventAds
    {
        #region Fields

        event Action<IAdvertisingService, AdModule, string, string, float> OnImpressionDataReceive;
        event Action<IAdvertisingService, AdModule, string> OnAdRequested;
        event Action<IAdvertisingService, AdModule, int, AdActionResultType, string, string> OnAdRespond;

        event Action<IAdvertisingService, AdModule, AdActionResultType, int, string, string> OnAdShow;
        event Action<IAdvertisingService, AdModule, AdActionResultType, int, string, string, Dictionary<string, object>> OnAdStarted;

        event Action<IAdvertisingService, AdModule, AdActionResultType, string, string, string, string, Dictionary<string, object>> OnAdHide;
        event Action<IAdvertisingService, AdModule, string> OnAdClick;
        event Action<IAdvertisingService, AdModule, int, string> OnAdExpire;

        #endregion



        #region Methods

        void Invoke_OnImpressionDataReceive(
            string impressionJsonData, 
            string adIdentifier, 
            float revenue);
        
        
        void Invoke_OnAdRequested(
            string adIdentifier);
        
        
        void Invoke_OnAdRespond(
            int delay, 
            AdActionResultType responseResultType, 
            string errorDescription, 
            string adIdentifier);
        
        
        void Invoke_OnAdShow(
            AdActionResultType responseResultType, 
            int delay, 
            string errorDescription, 
            string adIdentifier);
        
        
        void Invoke_OnAdHide(
            AdActionResultType responseResultType, 
            string errorDescription, 
            string adIdentifier,
            string adPlacement, 
            string result,
            Dictionary<string, object> data = null);
        
        
        void Invoke_OnAdClick(
            string adIdentifier);
        
        
        void Invoke_OnAdExpire(
            int delay, 
            string adIdentifier);

        #endregion
    }
}

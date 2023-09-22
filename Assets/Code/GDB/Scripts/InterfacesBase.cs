using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BoGD
{
    public interface IId
    {
        string Id
        {
            get;
        }
    } 

    /// <summary>
    /// Синглтон
    /// </summary>
    public interface IStatic : ISender, ISubscriber
    {
        bool IsEmpty
        {
            get;
        }

        StaticType StaticType
        {
            get;
        }

        void SaveInstance();
        void DeleteInstance();
    }

    /// <summary>
    /// Отправитель
    /// </summary>
    public interface ISender
    {
        string Description
        {
            get;
            set;
        }

        List<ISubscriber> Subscribers
        {
            get;
        }

        void AddSubscriber(ISubscriber subscriber);
        void RemoveSubscriber(ISubscriber subscriber);
        void Event(Message type, params object[] parameters);
    }

    /// <summary>
    /// Подписчик
    /// </summary>
    public interface ISubscriber
    {
        string Description
        {
            get;
            set;
        }

        void Reaction(Message message, params object[] parameters);
    }


    public interface IDestroyObject
    {
        System.Action OnDestroyObject
        {
            get;
            set;
        }
    }  

    public interface IUpdateObject
    {
        Action<float> OnUpdate
        {
            get;
            set;
        }
    }

    public interface IDevelopmentInfo : IStatic
    {
        float AvgFPS
        {
            get;
        }

        string NetVersion
        {
            get;
        }

        string Version
        {
            get;
        }

        string Platform
        {
            get;
        }
        bool IsHighTier
        {
            get;
        }

        void StartAVG();
        void EndAVG();
        void ChangeTimeScale(float target, bool force = false);
        int ControlInternetConnect();
    }  
       
    public interface ILocalization : IStatic
    {
        bool TryLocalize(string key, out string value);
        List<SystemLanguage> GetAvailableLanguages();
        void SetSystemLanguage(SystemLanguage language);
        SystemLanguage CurrentLanguage
        {
            get;
        }
    }
    
    /// <summary>
    /// Внутриигровая покупка
    /// </summary>
    public interface IInAppItem
    {
        string ID
        {
            get;
        }

        string Type
        {
            get;
            set;
        }

        string Title
        {
            get;
        }

        decimal LocalizedPrice
        {
            get;
        }

        string Price
        {
            get;
        }

        string ISO
        {
            get;
        }

        string TransactionID
        {
            get;
            set;
        }

        string Receipt
        {
            get;
            set;
        }
    }
}

# README #

### Версия плагина HuaweiMobileServices - 2.0.10 ###

## Ссылки ##
* git репозиторий Unity плагина - `https://github.com/EvilMindDevs/hms-unity-plugin`
* Документация HMS Core - `https://developer.huawei.com/consumer/en/doc/overview/HMS`


## Тестирование ##
### Ключи для AB тестирования рекламы : ###
* "huaweiAdsKitInterstitialModuleId": "testb4znbuh3n2",
* "huaweiAdsKitBannerModuleId": "testw6vs28auh3",
* "huaweiAdsKitRewardedVideoModuleId":"testx9dtjwj8hp"


## Изменения в `ExternalDependencies/hms-unity-plugin` ##

### 1.0.10 
* Папка `Resources` перенесена в `Huawei/Resources`, чтобы `HuaweiMobileServicesPluginHierarchy` имел к ней доступ

### 1.0.0
* В `Huawei/Editor/Utils/HMSGradleWorker.cs` в метод `OnPreprocessBuild` перед исполняемым кодом добавлен return -
  вместо него используется `Editor/HMSGradleWorker.cs`. Переделанный `HMSGradleWorker` получает настройки не из
  `HMSSettings` (устанавливается из окна эдитора `HMSMainWindow`), а из `LLHuaweiKitsSettings`
* В `Huawei/Editor/Utils/HMSGradleFixer.cs` в метод `OnPostGenerateGradleAndroidProject` перед исполняемым кодом добавлен return -
  вместо него используется `Editor/HMSGradleFixer.cs`
  
* Добавлен `Huawei/HuaweiMobileServicesPluginHierarchy.cs`
* В `HuaweiMobileServices.Editor.asmdef` добавлен референс на `Modules.Hive.Editor`
* Заменено получение путей на использование `HuaweiMobileServicesPluginHierarchy` :
  * `Huawei/Editor/Utils/HMSEditorUtils.cs`
  * `Huawei/Editor/View/MainWindow/HMSMainKitsTabFactory.cs`
  * `Huawei/Editor/View/InAppPurchaseTab/HMSIAPSettingsDrawer.cs`
  
* Закомментирован `Huawei/Editor/Utils/HMSPluginUpdaterInit.cs`
* Закомментирован `Huawei/Editor/Utils/HMSPackageChecker.cs`
* В `Huawei/Editor/View/MainWindow/HMSMainWindow.cs` закомментированы методы `CheckForUpdates` и `ShowWindow`, 
  закомментировано использование в `WindowEditRedirect`
* Закомментирован код внутри методов `Huawei/Scripts/Settings/HMSMainEditorSettings.cs`, чтобы избежать генерацию ассетов настроек  

* В `Huawei/Editor/View/MainWindow/HMSMainKitsTabFactory.cs` убрана генерация всех тоглов кроме EnablePlugin, Ads, Account, IAP
* Убрана генерация синглтонов при выборе тогла в следующих скриптах :
  * `Huawei/Editor/View/KitSettingsTab/AdsToggleEditor.cs`
  * `Huawei/Editor/View/KitSettingsTab/AccountToggleEditor.cs`
  * `Huawei/Editor/View/KitSettingsTab/IAPToggleEditor.cs`
* Убрано открытие дополнительных вкладок при выборе тогла в следующих скриптах :
  * `Huawei/Editor/View/KitSettingsTab/AdsToggleEditor.cs`
  * `Huawei/Editor/View/KitSettingsTab/IAPToggleEditor.cs`
* Удалена папка `StreamingAssets`


## Переработанные скрипты из `ExternalDependencies/hms-unity-plugin`(оригинальные использовать не рекомендуется) ##
* `ExternalDependencies/hms-unity-plugin/Huawei/Runtime/Scripts/IAP/UnityPurchase/HuaweiPurchasingModule.cs` -> 
`Runtime/InAppPurchasing/HuaweiPurchasingModule.cs`
* `ExternalDependencies/hms-unity-plugin/Huawei/Runtime/Scripts/IAP/UnityPurchase/HuaweiStore.cs` ->
    `Runtime/InAppPurchasing/HuaweiStore.cs`
* `ExternalDependencies/hms-unity-plugin/Huawei/Editor/Utils/HMSGradleWorker.cs` -> `Editor/HMSGradleWorker.cs`
* `ExternalDependencies/hms-unity-plugin/Huawei/Editor/Utils/HMSGradleFixer.cs` -> `Editor/HMSGradleFixer.cs`
* `ExternalDependencies/hms-unity-plugin/Huawei/Scripts/Ads/HMSAdsKitManager.cs` -> `Runtime/Advertising/AdsKit/HuaweiAdsKitManager.cs`

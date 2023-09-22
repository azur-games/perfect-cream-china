### Версия плагина ###

iOS SDK versions           6.22.0

Android SDK versions
 Framework                    | Version
------------------------------|----------
Firebase Core                 | 17.3.0
Remote Config                 | 19.1.3 
Firebase Auth                 | 19.3.0
Remote Database               | 19.2.1 

### iOS SDK Description ###
## Analytics
  - FirebaseAnalytics.framework
  - FIRAnalyticsConnector.framework
  - FirebaseCoreDiagnostics.framework
  - FirebaseCore.framework
  - FirebaseInstallations.framework
  - FirebaseInstanceID.framework
  - GoogleAppMeasurement.framework
  - GoogleUtilities.framework
  - nanopb.framework
  - PromisesObjC.framework
## ABTesting (~> Analytics)
  - FirebaseABTesting.framework
  - Protobuf.framework
## RemoteConfig (~> Analytics)
  - FirebaseABTesting.framework
  - FirebaseRemoteConfig.framework
  - Protobuf.framework

  The frameworks in this directory map to these versions of the iOS Firebase SDKs in CocoaPods.

           CocoaPod           | Version
----------------------------- | -------
FIRAnalyticsConnector         | 6.1.8
FirebaseABTesting             | 3.2.0
FirebaseAnalytics             | 6.4.1
FirebaseAuth                  | 6.5.1
FirebaseCore                  | 6.6.6
FirebaseCoreDiagnostics       | 1.2.3
FirebaseDatabase              | 6.1.4
FirebaseInstallations         | 1.1.1
FirebaseInstanceID            | 4.3.3
FirebaseRemoteConfig          | 4.4.9
GoogleAppMeasurement          | 6.4.1  ДОЛЖНЕН СОВПАДАТЬ С ВЕРСИЕЙ В IRONSOURCE!!!!!
GoogleDataTransport           | 5.1.1
GoogleDataTransportCCTSupport | 2.0.2
GoogleUtilities               | 6.5.2
GTMSessionFetcher             | 1.3.1
leveldb-library               | 1.22
nanopb                        | 0.3.9011
PromisesObjC                  | 1.2.8
Protobuf                      | 3.11.4

### Особенности реализации плагина ###
В Firebase имена некоторых параметров уже зарезервированы, причем значения, соответствующие этим параметрам, должны иметь определённый тип. Подробнее по ссылке: https://clck.ru/HiWDs


### Описание скрипта firebase_fix_dependencies.py ###
Скрипт используется после обновления плагина. Запускать через консоль. Чистит данные о зависимостях плагина в `manifest.json` и define symbols в `ProjectSettings.asset`.

### Описание обновления зависимостей ###
Обновить версию в классе `ModulesConstants`.
Залить новые `.tgz` пакеты в папку `Dependencies`.
Если надо, в продуктовом проекте, после обновления плагина, в меню вызвать `Modules/Firebase/Update version`.
При обновлении пакетов важно обратить внимание на зависимость `com.google.firebase:firebase-analytics` (находится внутри пакета в файле `Firebase/Editor/...Dependencies.xml`). Версия этого пакета, а так же его зависимости (например `play-services-measurement`) должны совпадать с теми версиями, от которых зависим адаптер Google Ads в плагине ApplovinMax. Текущая версия 18.0.3.
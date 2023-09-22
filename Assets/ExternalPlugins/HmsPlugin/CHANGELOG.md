# Hms plugin

## 1.0.11-transfer.1 - 2022-03-23
* Удалено обращение к серверному таймеру.

## 1.0.11 - 2022-01-13
* Рефакторинг рекламных скриптов.
* Метод `ReloadWithTestAd` перенесен в `HuaweiAdvertisingServiceImplementor`, `HuaweiAdsKitManager` удален.

## 1.0.10 - 2022-01-11
* Для не Huawei билдов используется `PluginDisabler` из `HmsAndroidBuildPreprocess`.
* В билд не попадают демо файлы `hms-unity-plugin`.

## 1.0.9 - 2021-12-28
* Исправлена ошибка при сворачивании игры во время показа интера и рв.

## 1.0.8 - 2021-12-27
* Добавлен метод `ReloadWithTestAd` в `HuaweiAdsKitManager`, чтобы была возможность показывать тестовую рекламу из чит панели.

## 1.0.7 - 2021-12-22
* Не создавать receipt для просроченных подписок.
* Обновлена зависимость от InAppPurchase plugin до 1.4.9.

## 1.0.6 - 2021-12-21
* Прерывание покупки при сворачивании игры в окне покупки.

## 1.0.5 - 2021-12-10
* Исправлена ошибка получения IDFA в редакторе.

## 1.0.4 - 2021-12-08
* Добавлена более подробная информация в логах о некоторых ошибках.

## 1.0.3 - 2021-12-03
* Добавлено bindTo в атрибут InitQueueService у HuaweiServices.

## 1.0.2 - 2021-12-02
* Отключено автоматическое отображение баннера после загрузки.

## 1.0.1 - 2021-11-30
* Исправлена выдача награды при прерванной RewardedVideo.

## 1.0.0 - 2021-11-01
* Добавлен основной контент плагина в папку `ExternalDependencies/hms-unity-plugin`
* Добавлены скрипты для работы InAppPlugin с Huawei IAP
* Добавлены скрипты для работы AdvertisingPlugin с Huawei Ads Kit
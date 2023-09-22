# Скрипт используется после обновления плагина. Запускать через консоль.
# Чистит данные о зависимостях плагина в `manifest.json` и define symbols в `ProjectSettings.asset`.
# В терминале macOS нужно запускать под python3.

import json
import pathlib
from os.path import abspath


# Функция для замены нескольких значений
def multiple_replace(target_str, replace_values):
    for i, j in replace_values.items():
        target_str = target_str.replace(i, j)
    return target_str


isAssets = False
dir = pathlib.Path(abspath(__file__)).parent

while not(isAssets):
    files = [file.name for file in dir.iterdir() if file.is_dir() and file.name == "Assets"]
    
    isAssets = len(files) > 0

    if(isAssets):
        manifestFiles = [manifestFile.name for manifestFile in pathlib.Path(dir, "Packages").iterdir() if manifestFile.is_file() and manifestFile.name == "manifest.json"]
        isAssets = len(manifestFiles) > 0

    if not(isAssets):
        dir = dir.parent

        if(dir.name == dir.parent.name):
            print("Assets path not found")
            quit()
    else:
        pathProjectInput = dir


pathProject = pathlib.Path(pathProjectInput)

if not(pathProject.exists() and pathProject.is_dir()):
    print("The path does not exist or is not a folder")
    quit()


pathManifest = pathlib.Path(pathProjectInput, "Packages", "manifest.json")

if not(pathManifest.exists() and pathManifest.is_file()):
    print("manifest.json file not found on the path " + pathManifest.name)
    quit()


pathProjectSettings = pathlib.Path(pathProjectInput, "ProjectSettings", "ProjectSettings.asset")

if not(pathProjectSettings.exists() and pathProjectSettings.is_file()):
    print("manifest.json file not found on the path " + pathProjectSettings.name)
    quit()


replace_firebase_values = {
    "FIREBASE_ANALYTICS": "", 
    "FIREBASE_CORE": "",
    "FIREBASE_REMOTE_CONFIG": "",
    "FIREBASE_DATABASE": "",
    "FIREBASE_CRASHLYTICS": "",
    "FIREBASE_AUTHENTICATION": "",
    "FIREBASE_INSTALLATIONS": ""
}

with open(pathProjectSettings, 'r+', encoding = 'utf-8') as f:
    settings = f.read()
    settings = multiple_replace(settings, replace_firebase_values)
    f.seek(0)
    f.write(settings)
    f.close()


firebase_dependencies_keys = [
    "com.google.firebase.analytics",
    "com.google.firebase.remote-config",
    "com.google.external-dependency-manager",
    "com.google.firebase.database",
    "com.google.firebase.crashlytics",
    "com.google.firebase.app",
    "com.google.firebase.auth",
    "com.google.firebase.installations"
]

with open(pathManifest, 'r+', encoding = 'utf-8') as f:
    data = json.load(f)
    dependencies = data['dependencies']

    for key in firebase_dependencies_keys:
        if key in dependencies:
            del dependencies[key]

    data['dependencies'] = dependencies
    f.seek(0)
    json.dump(data, f, indent = 2)
    f.truncate()
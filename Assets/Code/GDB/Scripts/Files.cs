using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

namespace BoGD
{
    public class Files : StaticBehaviour, IFiles
    {
        [SerializeField]
        private List<FileSettings>  settings = null;
        [SerializeField]
        private bool                debug = false;
        [SerializeField]
        private string              phSec = "";

        [SerializeField]
        private string              subFolder = "/Saves/";

        private Dictionary<FileType, FileSettings> allFiles = null;

        public override StaticType StaticType => StaticType.Files;

        private Dictionary<FileType, FileSettings> AllFiles
        {
            get
            {
                if (allFiles == null)
                {
                    allFiles = new Dictionary<FileType, FileSettings>();

                    foreach (var file in settings)
                    {
                        AllFiles[file.Type] = file;
                    }
                }

                return allFiles;
            }
        }


        protected override void Awake()
        {
            base.Awake();
            RequestPermission();
        }

        private void Start()
        {
            RequestPermission();
            CheckFolder();
        }

        private void Reset()
        {
            settings = new List<FileSettings>();
        }

        private void CheckFolder()
        {
            if (!System.IO.Directory.Exists(Application.persistentDataPath + subFolder))
            {
                System.IO.Directory.CreateDirectory(Application.persistentDataPath + subFolder);
            }
        }

        public void RequestPermission()
        {
#if PLATFORM_ANDROID
            //if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            //{
            //    Permission.RequestUserPermission(Permission.ExternalStorageWrite);
            //}
            //
            //if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
            //{
            //    Permission.RequestUserPermission(Permission.ExternalStorageRead);
            //}
#endif
        }

        public string ToJSON(Dictionary<string, object> dictionary)
        {
            return dictionary.ToJSON();
        }

        public Dictionary<string, object> FromJSON(string json)
        {
            return json.FromJSON();
        }

        public Dictionary<string, object> GetFromFile(FileType file)
        {
            CheckFolder();
            FileSettings result = null;
            if (!AllFiles.TryGetValue(file, out result))
            {
                Debug.LogErrorFormat("File '{0}' does not exist!", file);
                return null;
            }
            result.Load(Application.persistentDataPath + subFolder, phSec);

            return result.Dictionary;
        }


        public bool Exists(FileType file)
        {
            CheckFolder();
            FileSettings result = null;
            if (!AllFiles.TryGetValue(file, out result))
            {
                if (debug)
                {
                    Debug.LogErrorFormat("File '{0}' does not exist!", file);
                }
                return false;
            }

            return System.IO.File.Exists(Application.persistentDataPath + subFolder + "/" + result.Path);
        }

        public void PutIntoFile(FileType file, Dictionary<string, object> data)
        {
            CheckFolder();
            RequestPermission();
            FileSettings result = null;
            if (!AllFiles.TryGetValue(file, out result))
            {
                Debug.LogErrorFormat("File '{0}' does not exist!", file);
                return;
            }

            if (debug)
            {
                Debug.LogFormat("Save {0} tp {1}", file.ToString().FormatString("color:cyan"), (Application.persistentDataPath + subFolder + "/" + result.Path).FormatString("color:yellow"));
            }

            result.Save(Application.persistentDataPath + subFolder, data, phSec);
        }

        public void Clear()
        {
            if (!System.IO.Directory.Exists(Application.persistentDataPath + subFolder))
            {
                return;
            }

            //foreach (var file in settings)
            //{
            //    if(file.Reference == ReferenceType.None)
            //    {
            //        continue;
            //    }
            //
            //    System.IO.File.Delete(Application.persistentDataPath + subFolder + "/"+ file.Path);
            //    System.IO.File.Delete(Application.persistentDataPath + "/" + file.Path);
            //}

            //System.IO.Directory.Delete(Application.persistentDataPath + subFolder);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace BoGD
{    
    public interface ICheatController : IStatic
    {
        bool CheatBuid
        {
            get;
        }

        bool HasCheat(CONSOLE.Cheat cheat);


        System.Action OnChangeOptions
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Файлы
    /// </summary>
    public interface IFiles : IStatic
    {
        void PutIntoFile(FileType file, Dictionary<string, object> data);

        bool Exists(FileType file);
        Dictionary<string, object> GetFromFile(FileType file);

        Dictionary<string, object> FromJSON(string json);
        string ToJSON(Dictionary<string, object> dictionary);
    }
}
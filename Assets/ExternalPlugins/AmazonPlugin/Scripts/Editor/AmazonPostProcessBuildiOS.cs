#if UNITY_IOS || UNITY_IPHONE
#if UNITY_2019_3_OR_NEWER
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
#endif

namespace Amazon.Scripts.Editor
{
    public class AmazonPostProcessBuildiOS
    {
        [PostProcessBuild(int.MaxValue)]
        public static void PostProcessPlist(BuildTarget buildTarget, string path)
        {
            var plistPath = Path.Combine(path, "Info.plist");
            var plist = new PlistDocument();
            
            plist.ReadFromFile(plistPath);
            AddSkAdNetworkInfo(plist);
            plist.WriteToFile(plistPath);
        }

        private static void AddSkAdNetworkInfo(PlistDocument plist)
        {
            plist.root.values.TryGetValue("SKAdNetworkItems", out var skAdNetworkItems);
            var existingSkAdNetworkIds = new HashSet<string>();
            
            if (skAdNetworkItems != null && skAdNetworkItems.GetType() == typeof(PlistElementArray))
            {
                var plistElementDictionaries = skAdNetworkItems.AsArray().values
                    .Where(plistElement => plistElement.GetType() == typeof(PlistElementDict));
                
                foreach (var plistElement in plistElementDictionaries)
                {
                    plistElement.AsDict().values.TryGetValue("SKAdNetworkIdentifier", out var existingId);
            
                    if (existingId == null 
                        || existingId.GetType() != typeof(PlistElementString) 
                        || string.IsNullOrEmpty(existingId.AsString()))
                        continue;
                    
                    existingSkAdNetworkIds.Add(existingId.AsString());
                }
            }
            else
                skAdNetworkItems = plist.root.CreateArray("SKAdNetworkItems");

            var skAdNetworkItemDict = skAdNetworkItems.AsArray().AddDict();
            skAdNetworkItemDict.SetString("SKAdNetworkIdentifier", "p78axxw29g.skadnetwork");
        }
    }
}
#endif
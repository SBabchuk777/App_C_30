using System;
using UnityEngine;

namespace Prototype.MagicApps_Client
{
    public static class GameSettings
    {
        public static void Update()
        {
            PlayerPrefs.SetString(Constants.ApiVersion, Version.apiVersion.ToString());

            PlayerPrefs.SetString(Constants.UniqueAppID, Guid.NewGuid().ToString());

            PlayerPrefs.SetString(Constants.PackageName, Application.identifier);

            PlayerPrefs.SetString(Constants.CodeVersion, Version.codeVersion);

            PlayerPrefs.SetString(Constants.AppVersion, Application.version);
            
            PlayerPrefs.SetString(Constants.StartUrl, "");
            
            PlayerPrefs.Save();
        }

        public static string GetParam(string key)
        {
            return PlayerPrefs.GetString(key);
        }
        
        public static void SetParam(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }
    }
}

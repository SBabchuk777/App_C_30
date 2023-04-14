using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Prototype.SceneLoaderCore.Helpers;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Prototype.MagicApps_Client
{
    [System.Serializable]
    public class Request
    {
        protected string body;
        
        protected Action _finished;

        public virtual void GeneralParametersGenerate()
        {
            body = "\"" + Constants.ApiVersion + "\":" + int.Parse(GameSettings.GetParam("apiVersion")) +
                   ",\"" + Constants.UniqueAppID + "\":\"" + GameSettings.GetParam("uniqueAppID") + "\"" +
                   ",\"" + Constants.PackageName + "\":\"" + GameSettings.GetParam("packageName") + "\"" +
                   ",\"" + Constants.CodeVersion + "\":\"" + GameSettings.GetParam("codeVersion") + "\"" +
                   ",\"" + Constants.AppVersion + "\":\"" + GameSettings.GetParam("appVersion") + "\"";
        }

        public string Json()
        {
            return "{"+ body + "}";
        }
        
        public virtual void Respone(UnityWebRequest response, Action finished)
        {
            _finished = finished;
            
            if (response.isNetworkError || response.isHttpError)
            {
                Debug.Log("Error While Sending: " + response.error);

                ShowWhiteApp();
            }
            else
            {
                ProcessResponse(response);
            }
        }
        
        public virtual void ProcessResponse(UnityWebRequest response)
        {
            try
            {
                var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(XBase64.Decode(response.downloadHandler.text));

                Debug.Log($"Decode: {XBase64.Decode(response.downloadHandler.text)}");

                ResponseHanding(dictionary);
            }
            catch (Exception e)
            {
                Debug.Log("------------------------------------------");
                Debug.LogError(Json());
                Debug.LogError(e);
                Debug.Log("------------------------------------------");
                    
                Debug.Log("Error While Responsing: " + response.error);

                ShowWhiteApp();
            }
        }

        protected virtual void ResponseHanding(Dictionary<string, string> dictionary)
        {
            foreach (var (key, value) in dictionary)
                PlayerPrefs.SetString(key, value);
            
            PlayerPrefs.Save();
            
            _finished?.Invoke();
        }

        private void ShowWhiteApp()
        {
            Debug.Log("ShowWhiteApp");
            
            if (SceneLoader.Instance)
                SceneLoader.Instance.SwitchToScene(SceneLoader.Instance.mainScene);
            else
                SceneManager.LoadScene("Game");
        }
    }
}

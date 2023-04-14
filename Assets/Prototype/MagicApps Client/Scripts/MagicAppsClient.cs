using System;
using System.Collections.Generic;
using Prototype.SceneLoaderCore.Helpers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Prototype.MagicApps_Client
{
    public class MagicAppsClient : MonoBehaviour
    {
        public static MagicAppsClient Instance;
        
        public bool isIgnoreFirstRunApp;
        
        private List<Request> requests = new List<Request>();
        
        protected void Awake()
        {
            if (Instance != null)
                Destroy(gameObject);
            else
                Instance = this;
        }
        
        public void Initialize()
        {
            if(PlayerPrefs.HasKey(Constants.IsFirstRunApp) && !isIgnoreFirstRunApp)
            {
                if (PlayerPrefs.GetInt("newToken", 0) == 1)
                {
                    requests.Add(new UpdateRequest());
            
                    Send(requests[0]);
                }
                else
                {
                    CheckBinomIsNullOrEmpty();
                }
                
                return;
            }

            GameSettings.Update();

            requests.Add(new InitRequest());
            requests.Add(new StartRequest());
            requests.Add(new UpdateRequest());
            
            Send(requests[0]);
        }

        private void Send(Request request)
        {
            requests.Remove(request);

            StartCoroutine(SenderRequest.Send(request, CheckRequests));
        }

        private void CheckRequests()
        {
            if (requests.Count != 0)
                Send(requests[0]);
            else
                CheckBinomIsNullOrEmpty();
        }
        
        private void CheckBinomIsNullOrEmpty()
        {
            var binom = GameSettings.GetParam(Constants.StartUrl);
        
            Debug.Log($"@@@ binom: {binom}");

            var isNullOrEmpty = String.IsNullOrEmpty(binom);

            var scene = isNullOrEmpty ? SceneLoader.Instance.mainScene : "Webview";
            
            if (SceneLoader.Instance)
                SceneLoader.Instance.SwitchToScene(scene);
            else
                SceneManager.LoadScene(scene);
        }
    }
}

using Prototype.VerifyInternet;
using Prototype.MagicApps_Client;
using Prototype.Preloader;
using Prototype.SceneLoaderCore.Helpers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Prototype.Content
{
   public class Content : MonoBehaviour
   {
      private void Start()
      { 
         LoadingProgress.Instance.Visible(true);
         
         CheckInternetConnection(ConnectivityManager.Instance.IsConnected);
      }

      private void CheckInternetConnection(bool? isConnection)
      {
         if (isConnection == null)
         {
            ConnectivityManager.Instance.OnChangedInternetConnection.AddListener(CheckInternetConnection);
            
            return;
         }
         else
         {
            ConnectivityManager.Instance.OnChangedInternetConnection.RemoveListener(CheckInternetConnection);
         }
         
         Debug.Log("Content -> ChangedInternetConnection: " + isConnection);
         
         if (isConnection == true)
         {
            Debug.Log("Internet connected");

            MagicAppsClient.Instance.Initialize();
         }
         else if (isConnection == false)
         {
            Debug.Log("Internet Not connected");

            if (SceneLoader.Instance)
               SceneLoader.Instance.SwitchToScene(SceneLoader.Instance.mainScene);
            else
               SceneManager.LoadScene("Game");
         }
      }
   }
}

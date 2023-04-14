using System;
using System.Collections;
using DG.Tweening;
using Prototype.Logger;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Prototype.VerifyInternet
{
    [Serializable]
    public enum Mode
    {
        None,
        Ping,
        UnityWebRequest,
        NetworkReachability
    }
    
    public class VerifyInternet : MonoBehaviour
    {
        [Header("Mode")]
        [SerializeField] private Mode _mode = Mode.None;
        
        [SerializeField, Range(1, 20), Header("Time re-checking")]
        private int timeout = 10;

        [SerializeField, Space(10)] private UnityEvent InternetAvailable;

        [SerializeField] private UnityEvent InternetNotAvailable;

        private UnityWebRequest webRequest;
        
        private void Start()
        {
            CheckInternetConnection(CheckRequest);
        }

        public void LoseInternetConnection()
        {
            InternetNotAvailable?.Invoke();
        }

        private void CheckInternetConnection()
        {
            DOVirtual.DelayedCall(timeout, () => CheckInternetConnection(CheckRequest));
        }

        private void CheckRequest(bool isConnected)
        {
            if (isConnected)
            {
                DLogger.Log("Internet Available");
                
                InternetAvailable?.Invoke();
            }
            else
            {
                DLogger.Log("Internet Not Available");
                
                InternetNotAvailable?.Invoke();
            }
        }

        private void CheckInternetConnection(Action<bool> action)
        {
            if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork
                || Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                switch (_mode)
                {
                    case Mode.Ping:
                        PingCheckInternetConnection(action);
                        break;
                    case Mode.UnityWebRequest:
                        StartCoroutine(UnityWebRequestCheckInternetConnection(action));
                        break;
                    case Mode.NetworkReachability:
                        action(true);
                        break;
                    default:
                        action(true);
                        break;
                }
            }
            else
            {
                action(false);
            }
        }

        private void PingCheckInternetConnection(Action<bool> action)
        {
            DLogger.Log($"Application.internetReachabilityP: {Application.internetReachability}");

            var ping = new Ping("https://www.google.com");
            
            DLogger.Log($"ping.isDone {ping.isDone}");
            
            action(ping.isDone);
        }

        private IEnumerator UnityWebRequestCheckInternetConnection(Action<bool> action)
        {
            webRequest = new UnityWebRequest("https://google.com");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                action(true);
            } 
            else if (webRequest.result != UnityWebRequest.Result.InProgress)
            {
                action(false);
            }
        }
        
        public void OnConnectivityChange(bool isConnected, string errorMsg)
        {
            DLogger.Log($"OnConnectivityChange {isConnected}");
            
            CheckRequest(isConnected);
            
            if (!string.IsNullOrEmpty(errorMsg))
            {
                DLogger.LogWarning(errorMsg);
            }
        }
    }
}
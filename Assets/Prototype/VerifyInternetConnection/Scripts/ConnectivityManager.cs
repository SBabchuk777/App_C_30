using System;
using System.Collections;
using Prototype.Logger;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Prototype.VerifyInternet
{
    [DefaultExecutionOrder(-50)]
    public class ConnectivityManager : MonoBehaviour
    {
        [SerializeField] private bool printDebugMessages;
        
        [SerializeField] private bool dontDestroyOnLoad;
        
        [SerializeField] private bool StartOnAwake = true;
        
        [SerializeField] private string pingUrl = "https://google.com";

        [SerializeField] private float pingInterval = 5f;
        
        [HideInInspector] public UnityEvent<bool?> OnChangedInternetConnection;
        
        public static ConnectivityManager Instance;
        
        private UnityWebRequest webRequest;
        private bool? isConnected;
        private Coroutine connectionTestCoroutine;
        
        public bool? IsConnected
        {
            get => isConnected;
            private set
            {
                PrintDebugMessage($"isConnected: {isConnected} => value: {value}", MessageType.Verbose);

                if (isConnected == value) return;

                isConnected = value;
                
                OnChangedInternetConnection?.Invoke(isConnected == true);
                
                PrintDebugMessage($"Is Connected:: {isConnected}", MessageType.Verbose);
            }
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                
                if(dontDestroyOnLoad)
                    DontDestroyOnLoad(Instance.gameObject);
            }
            
            if (StartOnAwake) StartConnectionCheck();
        }

        private void StartConnectionCheck()
        {
            if (connectionTestCoroutine == null)
            {
                connectionTestCoroutine = StartCoroutine(TestConnection());
            }
            else
            {
                PrintDebugMessage("Connection check already started!", MessageType.Warning);
            }
        }

        public void StopConnectionCheck()
        {
            if (connectionTestCoroutine != null)
            {
                StopCoroutine(connectionTestCoroutine);
                
                connectionTestCoroutine = null;
            }
            else
            {
                PrintDebugMessage("No active Connection check!", MessageType.Warning);
            }
        }

        public void CheckErrorReceived()
        {
            if (IsConnected == true)
                IsConnected = false;
        }
        
        private IEnumerator TestConnection()
        {
            while (true)
            {
                if(Application.internetReachability == NetworkReachability.NotReachable)
                {
                    IsConnected = false;
                }
                else
                {
                    if (!string.IsNullOrEmpty(pingUrl))
                    {
                        webRequest = new UnityWebRequest(pingUrl);

                        yield return webRequest.SendWebRequest();

                        if (webRequest.result == UnityWebRequest.Result.Success)
                        {
                            IsConnected = true;
                        }
                        else if (webRequest.result != UnityWebRequest.Result.InProgress)
                        {
                            IsConnected = false;

                            PrintDebugMessage($"Connection Error::{webRequest.error}", MessageType.Warning);
                        }

                    }
                    else
                    {
                        IsConnected = false;
                        PrintDebugMessage($"Ping in Connectivity Manager (Inspector) is missing", MessageType.Error);
                    }
                }

                yield return new WaitForSeconds(pingInterval);
            }
        }

        private void PrintDebugMessage(string msg, MessageType msgType)
        {
            if (!printDebugMessages) return;
            
            switch (msgType)
            {
                case MessageType.Warning:
                    Debug.LogWarning($"ConnectivityManager:: {msg}");
                    break;
                case MessageType.Error:
                    Debug.LogError($"ConnectivityManager:: {msg}");
                    break;
                case MessageType.Verbose:
                    Debug.Log($"ConnectivityManager:: {msg}");
                    break;
                default:
                    Debug.Log($"ConnectivityManager:: {msg}");
                    break;
            }
        }

        private enum MessageType
        {
            Verbose,
            Warning,
            Error
        }
    }
}

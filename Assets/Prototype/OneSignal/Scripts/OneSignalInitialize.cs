using OneSignalSDK;
using UnityEngine;

public class OneSignalInitialize : MonoBehaviour
{
    [SerializeField] private string appIdIOS;
    
    [SerializeField] private string appIdAndroid;
    private void Start () 
    {
#if UNITY_ANDROID
        Debug.Log($"Initializing with appId <b>{appIdAndroid}</b>");
        
        OneSignal.Default.Initialize(appIdAndroid);
#elif UNITY_IOS
        Debug.Log($"Initializing with appId <b>{appIdIOS}</b>");
        
        OneSignal.Default.Initialize(appIdIOS);
#endif
        Debug.Log($"Initializing with appId <b>{appIdAndroid}</b>");
        
        OneSignal.Default.Initialize(appIdAndroid);
       
    }
}

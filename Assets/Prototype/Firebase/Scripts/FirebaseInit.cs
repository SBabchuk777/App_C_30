using Prototype.MagicApps_Client;
using UnityEngine;

public class FirebaseInit : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("FirebaseInit -> Start");
        
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => 
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available) 
                Init();
            else 
                Debug.LogError(System.String.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
        });
    }

    private void Init()
    {
        Debug.Log("FirebaseInit -> Init");
      
        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        
        //var token = Firebase.Messaging.FirebaseMessaging.GetTokenAsync().Result;
    }

    private void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        Debug.Log("Received Registration Token: " + token.Token);
        
        if (PlayerPrefs.GetString("updateFirebase", "") == token.Token) return;
        
        PlayerPrefs.SetInt("newToken", 1);
        PlayerPrefs.SetString("updateFirebase", token.Token);
        PlayerPrefs.Save();
    }

    private void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e) 
    {
        Debug.Log("-------------------------notification--------------------------");
        Debug.Log("Received a new message from: " + e.Message.From);
        
        var notification = e.Message.Notification;
        
        if (notification != null) 
        {
            Debug.Log("title: " + notification.Title);
            Debug.Log("body: " + notification.Body);
            var android = notification.Android;
            
            if (android != null)
            {
                Debug.Log("android channel_id: " + android.ChannelId);
            }
        }
        else
        {
            Debug.Log("notification == null");
        }
        
        if (e.Message.From.Length > 0)
            Debug.Log("from: " + e.Message.From);
        if (e.Message.Link != null) 
            Debug.Log("link: " + e.Message.Link.ToString());
        
        if (e.Message.Data.Count > 0) 
        {
            Debug.Log("data:");

            foreach (var (key, value) in e.Message.Data) 
            {
                Debug.Log(" - " + key + ": " + value);

                PlayerPrefs.SetString(key, value);
            }
            
            PlayerPrefs.Save();
            
            StartCoroutine(SenderRequest.Send(new TrackRequest(),null));
        }
        
        Debug.Log("-------------------------_______________--------------------------");
    }
    
    public void OnDestroy()
    {
        Firebase.Messaging.FirebaseMessaging.MessageReceived -= OnMessageReceived;
        Firebase.Messaging.FirebaseMessaging.TokenReceived -= OnTokenReceived;
    }
}

using Firebase.Extensions;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseInitialize : MonoBehaviour
{
    private string logText = "";
    const int kMaxLogSize = 16382;
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
    protected bool isFirebaseInitialized = false;
    private string topic = "TestTopic";
    private bool UIEnabled = true;
    
    protected bool LogTaskCompletion(Task task, string operation) 
    {
        bool complete = false;
        if (task.IsCanceled) 
        {
            DebugLog(operation + " canceled.");
        } else if (task.IsFaulted) 
        {
            DebugLog(operation + " encounted an error.");
            foreach (Exception exception in task.Exception.Flatten().InnerExceptions) {
                string errorCode = "";
                Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                if (firebaseEx != null) {
                    errorCode = String.Format("Error.{0}: ",
                        ((Firebase.Messaging.Error)firebaseEx.ErrorCode).ToString());
                }
                DebugLog(errorCode + exception.ToString());
            }
        } else if (task.IsCompleted) 
        {
            DebugLog(operation + " completed");
            complete = true;
        }
        
        return complete;
    }
    
    protected virtual void Start() 
    {
        Debug.Log("FirebaseInitialize->Start");
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available) 
            {
                InitializeFirebase();
            } 
            else 
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void InitializeFirebase() 
    {
        Debug.Log("FirebaseInitialize->InitializeFirebase");
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.SubscribeAsync(topic).ContinueWithOnMainThread(task => {
            LogTaskCompletion(task, "SubscribeAsync");
        });

        DebugLog("Firebase Messaging Initialized");

        Firebase.Messaging.FirebaseMessaging.RequestPermissionAsync().ContinueWithOnMainThread(
            task => {
                LogTaskCompletion(task, "RequestPermissionAsync");
            }
        );
        
        isFirebaseInitialized = true;

        GetTokenAsync();
    }
    
    private async void GetTokenAsync()
    {
        var task = Firebase.Messaging.FirebaseMessaging.GetTokenAsync();

        await task;

        if (task.IsCompleted)
        {
            Debug.Log("GET TOKEN ASYNC "+ task.Result);
        }
    }

    protected virtual void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e) 
    {
      DebugLog("Received a new message");
      var notification = e.Message.Notification;
      if (notification != null) {
        DebugLog("title: " + notification.Title);
        DebugLog("body: " + notification.Body);
        var android = notification.Android;
        if (android != null) {
            DebugLog("android channel_id: " + android.ChannelId);
        }
      }
      if (e.Message.From.Length > 0)
        DebugLog("from: " + e.Message.From);
      if (e.Message.Link != null) {
        DebugLog("link: " + e.Message.Link.ToString());
      }
      if (e.Message.Data.Count > 0) {
        DebugLog("data:");
        foreach (System.Collections.Generic.KeyValuePair<string, string> iter in
                 e.Message.Data) {
          DebugLog("  " + iter.Key + ": " + iter.Value);
        }
      }
    }

    protected virtual void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token) 
    {
      DebugLog("##### Received Registration Token: " + token.Token);
    }

    public void ToggleTokenOnInit() 
    {
      bool newValue = !Firebase.Messaging.FirebaseMessaging.TokenRegistrationOnInitEnabled;
      Firebase.Messaging.FirebaseMessaging.TokenRegistrationOnInitEnabled = newValue;
      DebugLog("Set TokenRegistrationOnInitEnabled to " + newValue);
    }

    public void OnDestroy()
    {
      Firebase.Messaging.FirebaseMessaging.MessageReceived -= OnMessageReceived;
      Firebase.Messaging.FirebaseMessaging.TokenReceived -= OnTokenReceived;
    }

    private void DebugLog(string s) 
    {
      print(s);
      logText += s + "\n";

      while (logText.Length > kMaxLogSize) {
        int index = logText.IndexOf("\n");
        logText = logText.Substring(index + 1);
      }
    }
}

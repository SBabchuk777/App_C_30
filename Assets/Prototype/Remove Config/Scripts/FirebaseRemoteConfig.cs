using System;
using System.Threading.Tasks;
using Firebase.Extensions;
using Prototype.Logger;
using UnityEngine;

public class FirebaseRemoteConfig : MonoBehaviour
{
	[SerializeField, Header("Test mode")] private bool testMode;

	[SerializeField, Header("Settings")] private Settings _testSettings;
	[SerializeField] private Settings _iosSettings;
	[SerializeField] private Settings _androidSettings;
	
	private Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;

	private Action _callback;
	
	public void InitRemoveConfig(Action callback)
	{
		DLogger.Log("@@@ InitRemoveConfig");
		
		_callback = callback;
		
		Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
			dependencyStatus = task.Result;
			if (dependencyStatus == Firebase.DependencyStatus.Available)
			{
				InitializeFirebase();

				FetchFireBase();
			}
			else
			{
				DLogger.LogError($"dependencyStatus: {dependencyStatus}");
			}
		});
	}

	private void InitializeFirebase()
	{
		var defaults = new System.Collections.Generic.Dictionary<string, object>();
		
		defaults.Add(GetFirebaseKey(), GetDefaultBinom());

		Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults);
	}

	private void FetchFireBase()
	{
		FetchDataAsync();
	}

	public string GetURL()
	{
		return Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(GetFirebaseKey()).StringValue;
	}

	private string GetDefaultBinom()
	{
#if UNITY_ANDROID
		return testMode? _testSettings.defaultBinom : _androidSettings.defaultBinom;
#elif UNITY_IOS
		return testMode? _testSettings.defaultBinom : _iosSettings.defaultBinom;
#endif
		return _testSettings.defaultBinom;
	}
	
	private string GetFirebaseKey()
	{
#if UNITY_ANDROID
		return testMode? _testSettings.firebaseKey : _androidSettings.firebaseKey;
#elif UNITY_IOS
		return testMode? _testSettings.firebaseKey : _iosSettings.firebaseKey;
#endif
		return _testSettings.firebaseKey;
	}
	
	public bool isUrlDefaultBinom(string url)
	{
		DLogger.Log($"isUrlDefaultBinom {url} ?= {GetDefaultBinom()}");
		
		var domainUrl = new Uri(url).Host;
		
		var domainUDefaultBinom = new Uri(GetDefaultBinom()).Host;

		return string.CompareOrdinal(domainUrl, domainUDefaultBinom) == 0;
	}

    private Task FetchDataAsync()
	{
		var fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
		
		return fetchTask.ContinueWithOnMainThread(FetchComplete);
	}

    private void FetchComplete(Task fetchTask)
	{
		if (fetchTask.IsCanceled)
		{
			DLogger.Log("Fetch canceled.");
		}
		else if (fetchTask.IsFaulted)
		{
			DLogger.Log("Fetch encountered an error.");
		}
		else if (fetchTask.IsCompleted)
		{
			DLogger.Log("Fetch completed successfully!");
		}

		var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
		
		switch (info.LastFetchStatus)
		{
			case Firebase.RemoteConfig.LastFetchStatus.Success:
				Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync()
					.ContinueWithOnMainThread(task =>
					{
						DLogger.Log(String.Format("Remote data loaded and ready (last fetch time {0}).",
							info.FetchTime));

						_callback?.Invoke();
					});
				break;
			case Firebase.RemoteConfig.LastFetchStatus.Failure:
				switch (info.LastFetchFailureReason)
				{
					case Firebase.RemoteConfig.FetchFailureReason.Error:
						DLogger.Log("Fetch failed for unknown reason");
						break;
					case Firebase.RemoteConfig.FetchFailureReason.Throttled:
						DLogger.Log("Fetch throttled until " + info.ThrottledEndTime);
						break;
				}
				break;
			case Firebase.RemoteConfig.LastFetchStatus.Pending:
				DLogger.Log("Latest Fetch call still pending.");
				break;
		}
	}
}
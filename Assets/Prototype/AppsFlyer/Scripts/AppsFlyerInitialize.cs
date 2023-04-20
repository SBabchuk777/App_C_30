using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AppsFlyerSDK;
using Castle.Core.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class AppsFlyerInitialize : MonoBehaviour, IAppsFlyerConversionData
{
    public static AppsFlyerInitialize Instance;
    
    // These fields are set from the editor so do not modify!
    public string devKey;
    public string appID;
    public bool isDebug;
    public bool getConversionData;

    private Action _callback;
    
    protected void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void Initialize(Action callback)
    {
        _callback = callback;
        
        devKey = PlayerPrefs.GetString(Constants.InitApfDevKey, "");
        
        getConversionData = PlayerPrefs.GetString(Constants.InitAskApfConv, "false") == "true";
        
        AppsFlyer.setIsDebug(isDebug);
        
        Debug.Log($" AppsFlyer.initSDK(devKey, appID, getConversionData): {devKey}, {appID}, {getConversionData}");
        
        AppsFlyer.initSDK(devKey, appID, getConversionData ? this : null);

        AppsFlyer.startSDK();
        
        if(!getConversionData)
            _callback?.Invoke();
        else
        {
#if UNITY_EDITOR
            _callback?.Invoke();
#endif
        }
    }
    
    public void onConversionDataSuccess(string conversionData)
    {
        Debug.Log("@@@@@ onConversionDataSuccess: " + conversionData);

        SetStartApfConvData(conversionData);
        
        AppsFlyer.AFLog("didReceiveConversionData", conversionData);
        
        Dictionary<string, object> conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(conversionData);
        // add deferred deeplink logic here
        
        _callback?.Invoke();
    }

    public void onConversionDataFail(string error)
    {
        AppsFlyer.AFLog("didReceiveConversionDataWithError", error);
        
        _callback?.Invoke();
    }

    public void onAppOpenAttribution(string attributionData)
    {
        AppsFlyer.AFLog("onAppOpenAttribution", attributionData);
        Dictionary<string, object> attributionDataDictionary = AppsFlyer.CallbackStringToDictionary(attributionData);
        // add direct deeplink logic here
    }

    public void onAppOpenAttributionFailure(string error)
    {
        AppsFlyer.AFLog("onAppOpenAttributionFailure", error);
    }

    private void SetStartApfConvData(string conversionData, string status = "success", string type = "onInstallConversionDataLoaded")
    {
        conversionData = ConvertJsonToString(conversionData);

        Debug.Log("conversionData (replace from strings): " + conversionData);

        var prefix = "{\"status\":\"" +status+ "\",\"type\":\"" + type + "\",\"data\":";

        var suffix = "}";
        
        Debug.Log($"prefix: {prefix}");
        
        var startApfConvData = prefix + conversionData + suffix;
        
        startApfConvData = startApfConvData.Replace("\"", "\\\"");
        
        Debug.Log($"startApfConvData: {startApfConvData}");
        
        PlayerPrefs.SetString(Constants.StartApfConvData, startApfConvData);
        PlayerPrefs.Save();
    }
    
    private string ConvertJsonToString(string jsonString)
    {
        var jsonObject = JObject.Parse(jsonString);
        
        foreach (var property in jsonObject.Descendants().OfType<JProperty>().ToList())
        {
            if (property.Value == null || property.Value.Type == JTokenType.Null)
            {
                property.Value = "null";
            }
            else if (property.Value.Type == JTokenType.Object)
            {
                ConvertJsonToString(property.Value.ToString());
            }
            else
            {
                property.Value = property.Value.ToString();
            }
        }

        var str = jsonObject.ToString();

        return str.Replace(Environment.NewLine, "");
    }
}

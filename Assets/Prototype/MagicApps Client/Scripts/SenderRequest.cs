using System;
using System.Collections;
using Prototype.MagicApps_Client;
using UnityEngine;
using UnityEngine.Networking;

public static class SenderRequest
{
    private static string _url = "https://plotyuire.site";
    
    public static IEnumerator Send(Request request, Action finished)
    {
        request.GeneralParametersGenerate();
           
        Debug.Log($"Json: {request.Json()}");
        
        var uwr = new UnityWebRequest(_url, UnityWebRequest.kHttpVerbPOST);
            
        var encoded = XBase64.Encode(request.Json());
        var bytes = new System.Text.UTF8Encoding().GetBytes(encoded);

        uwr.uploadHandler = new UploadHandlerRaw(bytes);
        uwr.downloadHandler = new DownloadHandlerBuffer();

        uwr.SetRequestHeader("User-Agent", "runscope/0.1");
            
        yield return uwr.SendWebRequest();
            
        request.Respone(uwr, finished);
        
        uwr.Dispose();
    }
}

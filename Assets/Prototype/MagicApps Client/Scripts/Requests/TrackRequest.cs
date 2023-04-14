using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Prototype.MagicApps_Client
{
    [System.Serializable]
    public class TrackRequest : Request
    {
        public override void GeneralParametersGenerate()
        {
            base.GeneralParametersGenerate();
            
            body += ",\"" + Constants.ReqType + "\":\"" + ReqType.track.ToString() + "\"";
            body += ",\"" + "trackID" + "\":\"" + PlayerPrefs.GetString("custom_id", "") + "\"";
        }
        
        public override void Respone(UnityWebRequest response, Action finished)
        {
            
        }
    }
}

using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Prototype.MagicApps_Client
{
    [System.Serializable]
    public class UpdateRequest : Request
    {
        public override void GeneralParametersGenerate()
        {
            base.GeneralParametersGenerate();
            
            body += ",\"" + Constants.ReqType + "\":\"" + ReqType.update.ToString() + "\"";
            body += ",\"" + "updateFirebase" + "\":\"" + PlayerPrefs.GetString("updateFirebase", "") + "\"";
            
            PlayerPrefs.SetInt("newToken", 0);
            PlayerPrefs.Save();
        }
        
        public override void Respone(UnityWebRequest response, Action finished)
        {
            finished?.Invoke();
        }
    }
}

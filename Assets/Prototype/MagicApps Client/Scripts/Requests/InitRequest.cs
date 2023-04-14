using System.Collections.Generic;
using UnityEngine;

namespace Prototype.MagicApps_Client
{
    [System.Serializable]
    public class InitRequest : Request
    {
        public override void GeneralParametersGenerate()
        {
            base.GeneralParametersGenerate();
            
            body += ",\"" + Constants.ReqType + "\":\"" + ReqType.init.ToString() + "\"";
        }
        
        protected override void ResponseHanding(Dictionary<string, string> dictionary)
        {
            foreach (var (key, value) in dictionary)
            {
                //Debug.Log($"{key} : {value}");
                PlayerPrefs.SetString(key, value);
            }
            
            PlayerPrefs.Save();
                
            if(AppsFlyerInitialize.Instance)
                AppsFlyerInitialize.Instance.Initialize(_finished);
        }
    }
}

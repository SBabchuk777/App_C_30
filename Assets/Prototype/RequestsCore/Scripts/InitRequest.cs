using System;
using System.Linq;
using UnityEngine;

namespace Prototype.RequestsCore
{
    [CreateAssetMenu(menuName = "RequestsCore/Requests/Create init request", fileName = "InitRequest")]
    public class InitRequest : Request
    {
        private void OnValidate()
        {
            if(IsCreatedParam(ReqType.init.ToString()))
                parameters.Add(new Pair("reqType", ReqType.init.ToString()));
        }

        private bool IsCreatedParam(string reqType)
        {
            return parameters.Count != 0 && parameters.Any(parameter => parameter.Key == reqType);
        }
    }
}

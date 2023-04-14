using System.Linq;
using UnityEngine;

namespace Prototype.RequestsCore
{
    [CreateAssetMenu(menuName = "RequestsCore/Requests/Create start request", fileName = "StartRequest")]
    public class StartRequest : Request
    {
        private void OnValidate()
        {
            if(IsCreatedParam(ReqType.start.ToString()))
                parameters.Add(new Pair("reqType", ReqType.start.ToString()));
        }

        private bool IsCreatedParam(string reqType)
        {
            return parameters.Count != 0 && parameters.Any(parameter => parameter.Key == reqType);
        }
    }
}

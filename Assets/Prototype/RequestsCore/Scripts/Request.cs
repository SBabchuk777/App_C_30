using System.Collections.Generic;
using UnityEngine;

namespace Prototype.RequestsCore
{
    public class Request : ScriptableObject
    {
        public List<Pair> headers = new List<Pair>();
        
        public List<Pair> parameters = new List<Pair>();
    }
}


using System;
using System.Collections.Generic;
using System.Text;
using Random = System.Random;

namespace Prototype.MagicApps_Client
{
    public class XBase64
    {
        public static string Encode(string data = "123")
        {
            var base64Encoded = Base64Encode(data);
            
            var outStr = "";
            
            var rand = new Random();
            
            foreach (var c in base64Encoded)
            {
                outStr += c;
                
                var randomNum = rand.Next(98, 122);
                
                var randomChar = (char)randomNum;
                
                RandomUpperChar(randomNum, ref randomChar);
                
                outStr += randomChar;
            }
            
            return outStr;
        }

        public static string Decode(string data = "123") 
        {
            var dataBytes = Encoding.UTF8.GetBytes(data);
            
            var outBytes = new List<byte>();

            for (var k = 0; k < dataBytes.Length / 2; k++) {
                outBytes.Add((byte)Encoding.UTF8.GetString(new byte[] { dataBytes[2 * k] }).ToCharArray()[0]);
            }

            while (outBytes.Count % 4 != 0) {
                outBytes.Add((byte)'=');
            }

            var outStr = Encoding.UTF8.GetString(outBytes.ToArray());
            
            var decoded = Convert.FromBase64String(outStr);
            
            return Encoding.UTF8.GetString(decoded);
        }
        
        private static string Base64Encode(string data)
        {
            var dataBytes = Encoding.UTF8.GetBytes(data);

            var base64Encoded = Convert.ToBase64String(dataBytes);

            base64Encoded = base64Encoded.TrimEnd('=');
            
            return base64Encoded;
        }
        
        private static void RandomUpperChar(int randomNum, ref char randomChar)
        {
            randomChar = randomNum % 2 == 0 ? Char.ToUpper(randomChar) : randomChar;
        }
    }
}

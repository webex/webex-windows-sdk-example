using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitchenSink
{
    public class StringExtention
    {
        public static string Base64UrlDecode(string input)
        {
            var Output = input;
            Output = Output.Replace('-', '+'); // 62nd char of encoding
            Output = Output.Replace('_', '/'); // 63rd char of encoding
            switch (Output.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 1: Output += "==="; break; // Three pad chars
                case 2: Output += "=="; break; // Two pad chars
                case 3: Output += "="; break; // One pad char
                default: throw new System.Exception("Illegal base64url string!");
            }
            var converted = Convert.FromBase64String(Output); // Standard base64 decoder

            return System.Text.Encoding.UTF8.GetString(converted);
        }
    }
}

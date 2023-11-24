using System;
using System.Text;

namespace DynamicInvoke
{
    public static class Encrypt
    {
        //using XOR for simplicity, these can be any encryption you like

        public static string Encyrpt(byte[] data, string key)
        {
            int keyindex = 0;
            byte[] keybytes = Encoding.UTF8.GetBytes(key);
            byte[] outbytes = new byte[data.Length];

            for(int i = 0; i < data.Length; i++)
            {
                outbytes[i] = (byte)((int)data[i] ^ (int)keybytes[keyindex]);
                keyindex++;
                if (keyindex >= keybytes.Length) keyindex = 0;
            }

            string b64 = Convert.ToBase64String(outbytes);
            return b64;
        }

        public static byte[] Decrypt(string b64, string key)
        {
            int keyindex = 0;
            byte[] keybytes = Encoding.UTF8.GetBytes(key);
            byte[] encodedbytes  = Convert.FromBase64String(b64);
            byte[] outbytes = new byte[encodedbytes.Length];

            for (int i = 0; i < encodedbytes.Length; i++)
            {
                outbytes[i] = (byte)((int)encodedbytes[i] ^ (int)keybytes[keyindex]);
                keyindex++;
                if (keyindex >= keybytes.Length) keyindex = 0;
            }

            return outbytes;
        }
    }
}

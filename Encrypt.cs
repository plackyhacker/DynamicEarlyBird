using System;
using System.Text;

namespace DynamicInvoke
{
    public static class Encrypt
    {
            public static byte[] Decrypt(string aes_base64, string key)
            {
                byte[] tempKey = Encoding.ASCII.GetBytes(key);
                tempKey = SHA256.Create().ComputeHash(tempKey);
        
                byte[] data = Convert.FromBase64String(aes_base64);
        
                // decrypt data
                Aes aes = new AesManaged();
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                ICryptoTransform dec = aes.CreateDecryptor(tempKey, SubArray(tempKey, 16));
        
                using (MemoryStream msDecrypt = new MemoryStream())
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, dec, CryptoStreamMode.Write))
                    {
        
                        csDecrypt.Write(data, 0, data.Length);
        
                        return msDecrypt.ToArray();
                    }
                }
            }
        
            static byte[] SubArray(byte[] a, int length)
            {
                byte[] b = new byte[length];
                for (int i = 0; i < length; i++)
                {
                    b[i] = a[i];
                }
                return b;
            }
        }
    }
}

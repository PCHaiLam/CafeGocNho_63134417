using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CafeGocNho_63134417.Helper
{
    public class Md5Helper
    {
        public static string EncryptMD5(string input)
        {
            var md5 = System.Security.Cryptography.MD5.Create();

            var inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);

            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString().ToLower();
        }
    }
}
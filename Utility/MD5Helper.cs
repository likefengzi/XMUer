using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace XMUer.Utility
{
    public class MD5Helper
    {
        /// <summary>
        /// MD5加密字符串（32位大写）
        /// </summary>
        /// <param name="password"></param>
        /// <returns>加密后的字符串</returns>
        public static string MD5Encryption(string password)
        {
            string res = "";
            MD5 md5 = MD5.Create();
            byte[] byteStr = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
            foreach (byte ch in byteStr) res += ch.ToString("X2");
            return res;
        }
    }
}

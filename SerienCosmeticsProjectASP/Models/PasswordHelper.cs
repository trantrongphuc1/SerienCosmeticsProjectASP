using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SerienCosmeticsProjectASP.Models
{
    public class PasswordHelper
    {
        public string GetMD5(string password)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] encrypt;
            UTF8Encoding encode = new UTF8Encoding();
            encrypt = md5.ComputeHash(encode.GetBytes(password));
            StringBuilder encryptdata = new StringBuilder();
            for (int i = 0; i < encrypt.Length; i++)
            {
                encryptdata.Append(encrypt[i].ToString());
            }
            return encryptdata.ToString();
        }
        public bool VerifyPasswordMD5(string enteredPassword, string storedHashedPassword)
        {
            // mã hóa mật khẩu người dùng nhập
            string hashedEnteredPassword = GetMD5(enteredPassword);

            // So sánh mật khẩu đã mã hóa với mật khẩu đã lưu
            return string.Equals(hashedEnteredPassword, storedHashedPassword, StringComparison.OrdinalIgnoreCase);
        }
    }
}
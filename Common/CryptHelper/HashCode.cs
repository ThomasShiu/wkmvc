using System;
using System.Text;
using System.Security.Cryptography;
namespace Common.CryptHelper
{
    /// <summary>
    /// 得到隨機安全碼（雜湊加密）。
    /// </summary>
    public class HashEncode
    {
        public HashEncode()
        {
            //
            // TODO: 在此處添加構造函數邏輯
            //
        }
        /// <summary>
        /// 得到隨機雜湊加密字串
        /// </summary>
        /// <returns></returns>
        public static string GetSecurity()
        {
            string Security = HashEncoding(GetRandomValue());
            return Security;
        }
        /// <summary>
        /// 得到一個亂數值
        /// </summary>
        /// <returns></returns>
        public static string GetRandomValue()
        {
            Random Seed = new Random();
            string RandomVaule = Seed.Next(1, int.MaxValue).ToString();
            return RandomVaule;
        }
        /// <summary>
        /// 雜湊加密一個字串
        /// </summary>
        /// <param name="Security"></param>
        /// <returns></returns>
        public static string HashEncoding(string Security)
        {
            byte[] Value;
            UnicodeEncoding Code = new UnicodeEncoding();
            byte[] Message = Code.GetBytes(Security);
            SHA512Managed Arithmetic = new SHA512Managed();
            Value = Arithmetic.ComputeHash(Message);
            Security = "";
            foreach (byte o in Value)
            {
                Security += (int)o + "O";
            }
            return Security;
        }
    }
}

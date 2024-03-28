using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Confidential
{
    /// <summary>
    /// 建议客户端自己实现
    /// </summary>
    public abstract class ConfidentialServiceBase : IConfidentialService
    {
        #region Bidirectional 双向的实现, 这里默认使用AES
        /// <summary>
        /// AesPassword 一般是：128位、192位、256位三种情况下的字节数组的base64编码
        /// </summary>
        public abstract string AesPassword { get; }


        private string _CreateAesIV()
        {
            byte[] randomBytes = new byte[16];
            RandomNumberGenerator.Fill(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
        /// <summary>
        /// 加密
        /// IV将于密文放一起 使用|分隔
        /// </summary>
        /// <param name="paintext"></param>
        /// <returns></returns>
        public virtual Task<string> BidirectionalEncrypt(string paintext)
        {
            using Aes myAes = Aes.Create();
            myAes.Key = Convert.FromBase64String(AesPassword);
            var aesIV = _CreateAesIV();

            myAes.IV = Convert.FromBase64String(aesIV);

            // Encrypt the string to an array of bytes.
            byte[] encrypted = EncryptStringToBytes_Aes(paintext, myAes.Key, myAes.IV);

            return Task.FromResult(aesIV + "|" + Convert.ToBase64String(encrypted));
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="encryptedText"></param>
        /// <returns></returns>
        public virtual Task<string> BidirectionalDecrypt(string encryptedText)
        {
            using Aes myAes = Aes.Create();
            myAes.Key = Convert.FromBase64String(AesPassword);

            if(encryptedText.Split("|").Length != 2)
            {
                throw new Exception("无效的密文");
            }
            var aesIv = encryptedText.Split("|")[0];
            myAes.IV = Convert.FromBase64String(aesIv);

            // Encrypt the string to an array of bytes.
            string paintext = DecryptStringFromBytes_Aes(Convert.FromBase64String(encryptedText.Split("|")[1]), myAes.Key, myAes.IV);

            return Task.FromResult(paintext);
        }

        byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string? plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
        #endregion

        #region Unidirectional 单向的实现，默认使用SHA256



        public Task<string> UnbidirectionalEncrypt(string paintext)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(paintext));
                StringBuilder hex = new StringBuilder(bytes.Length * 2);
                foreach (byte b in bytes)
                {
                    hex.AppendFormat("{0:x2}", b);
                }
                return Task.FromResult(hex.ToString());
            }
        }

        public async Task<bool> UnbidirectionalEncryptValidate(string paintext, string encryptedText)
        {
            // 重新计算哈希值与存储的哈希值进行比较  
            string computedHash = await UnbidirectionalEncrypt(paintext);
            return computedHash.Equals(encryptedText);
        }
        #endregion

    }
}

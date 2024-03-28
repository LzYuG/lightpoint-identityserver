using SkiaSharp.QrCode.Image;
using SkiaSharp;
using System.Security.Cryptography;
using System.Text;

namespace LightPoint.IdentityServer.ServerInfrastructure.Security.MFA.GoogleAuthenticator
{
    public class GoogleAuthenticator
     {
         private readonly static DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
         private TimeSpan DefaultClockDriftTolerance { get; set; }
 
         public GoogleAuthenticator()
         {
             DefaultClockDriftTolerance = TimeSpan.FromSeconds(30);
        }
        public GoogleAuthenticator(TimeSpan timeSpan)
        {
            DefaultClockDriftTolerance = timeSpan;
        }



        /// <summary>
        /// Generate a setup code for a Google Authenticator user to scan
        /// </summary>
        /// <param name="issuer">Issuer ID (the name of the system, i.e. 'MyApp'), can be omitted but not recommended https://github.com/google/google-authenticator/wiki/Key-Uri-Format </param>
        /// <param name="accountTitleNoSpaces">Account Title (no spaces)</param>
        /// <param name="accountSecretKey">Account Secret Key</param>
        /// <param name="accountSecretKey">Number of pixels per QR Module (2 pixels give ~ 100x100px QRCode)</param>
        /// <returns>SetupCode object</returns>
        public SetupCode GenerateSetupCode(string issuer, string accountTitleNoSpaces, string accountSecretKey, int qrCodeWidth)
         {
             byte[] key = Encoding.UTF8.GetBytes(accountSecretKey);
             return GenerateSetupCode(issuer, accountTitleNoSpaces, key, qrCodeWidth);
         }

        /// <summary>
        /// Generate a setup code for a Google Authenticator user to scan
        /// </summary>
        /// <param name="issuer">Issuer ID (the name of the system, i.e. 'MyApp'), can be omitted but not recommended https://github.com/google/google-authenticator/wiki/Key-Uri-Format </param>
        /// <param name="accountTitleNoSpaces">Account Title (no spaces)</param>
        /// <param name="accountSecretKey">Account Secret Key as byte[]</param>
        /// <returns>SetupCode object</returns>
        private SetupCode GenerateSetupCode(string issuer, string accountTitleNoSpaces, byte[] accountSecretKey, int qrCodeWidth)
        {
            if (accountTitleNoSpaces == null) { throw new NullReferenceException("Account Title is null"); }
            accountTitleNoSpaces = RemoveWhitespace(accountTitleNoSpaces);
            string encodedSecretKey = Base32Encoding.ToString(accountSecretKey);
            string? provisionUrl = null;
            provisionUrl = String.Format("otpauth://totp/{2}:{0}?secret={1}&issuer={2}", accountTitleNoSpaces, encodedSecretKey.Replace("=", ""), UrlEncode(issuer));

            using (MemoryStream ms = new MemoryStream())
            {
                // generate QRCode
                var qrCode = new QrCode(provisionUrl, new Vector2Slim(qrCodeWidth, qrCodeWidth), SKEncodedImageFormat.Png);

                // output to MemoryStream
                qrCode.GenerateImage(ms);

                return new SetupCode(accountTitleNoSpaces, encodedSecretKey, String.Format("data:image/png;base64,{0}", Convert.ToBase64String(ms.ToArray())));
            }
        }


        private static string RemoveWhitespace(string str)
         {
             return new string(str.Where(c => !Char.IsWhiteSpace(c)).ToArray());
         }
 
         private string UrlEncode(string value)
         {
             StringBuilder result = new StringBuilder();
             string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
 
             foreach (char symbol in value)
             {
                 if (validChars.IndexOf(symbol) != -1)
                 {
                     result.Append(symbol);
                 }
                 else
                 {
                     result.Append('%' + String.Format("{0:X2}", (int)symbol));
                 }
             }
 
             return result.ToString().Replace(" ", "%20");
         }

         private string GeneratePINAtInterval(string accountSecretKey, long counter, int digits = 6)
         {
             return GenerateHashedCode(accountSecretKey, counter, digits);
         }
 
         internal string GenerateHashedCode(string secret, long iterationNumber, int digits = 6)
         {
             byte[] key = Encoding.UTF8.GetBytes(secret);
             return GenerateHashedCode(key, iterationNumber, digits);
         }
 
         internal string GenerateHashedCode(byte[] key, long iterationNumber, int digits = 6)
         {
             byte[] counter = BitConverter.GetBytes(iterationNumber);
 
             if (BitConverter.IsLittleEndian)
             {
                 Array.Reverse(counter);
             }
 
             HMACSHA1 hmac = new HMACSHA1(key);
 
             byte[] hash = hmac.ComputeHash(counter);
 
             int offset = hash[hash.Length - 1] & 0xf;
 
             // Convert the 4 bytes into an integer, ignoring the sign.
             int binary =
                 ((hash[offset] & 0x7f) << 24)
                 | (hash[offset + 1] << 16)
                 | (hash[offset + 2] << 8)
                 | (hash[offset + 3]);
 
             int password = binary % (int)Math.Pow(10, digits);
             return password.ToString(new string('0', digits));
         }
 
         private long GetCurrentCounter()
         {
             return GetCurrentCounter(DateTime.UtcNow, _epoch, 30);
         }
 
         private long GetCurrentCounter(DateTime now, DateTime epoch, int timeStep)
         {
             return (long)(now - epoch).TotalSeconds / timeStep;
         }
 
         public bool ValidateTwoFactorPIN(string accountSecretKey, string twoFactorCodeFromClient)
         {
             return ValidateTwoFactorPIN(accountSecretKey, twoFactorCodeFromClient, DefaultClockDriftTolerance);
         }
 
         public bool ValidateTwoFactorPIN(string accountSecretKey, string twoFactorCodeFromClient, TimeSpan timeTolerance)
         {
             var codes = GetCurrentPINs(accountSecretKey, timeTolerance);
             return codes.Any(c => c == twoFactorCodeFromClient);
         }
 
         public string[] GetCurrentPINs(string accountSecretKey, TimeSpan timeTolerance)
         {
             List<string> codes = new List<string>();
             long iterationCounter = GetCurrentCounter();
             int iterationOffset = 0;
 
             if (timeTolerance.TotalSeconds > 30)
             {
                 iterationOffset = Convert.ToInt32(timeTolerance.TotalSeconds / 30.00);
             }
 
             long iterationStart = iterationCounter - iterationOffset;
             long iterationEnd = iterationCounter + iterationOffset;
 
             for (long counter = iterationStart; counter <= iterationEnd; counter++)
             {
                 codes.Add(GeneratePINAtInterval(accountSecretKey, counter));
             }
 
             return codes.ToArray();
         }

        /// <summary>
        /// Writes a string into a bitmap
        /// </summary>
        /// <param name="qrCodeSetupImageUrl"></param>
        /// <returns></returns>
        public static SKBitmap GetQRCodeImage(string qrCodeSetupImageUrl)
        {
            // data:image/png;base64,
            qrCodeSetupImageUrl = qrCodeSetupImageUrl.Replace("data:image/png;base64,", "");
            byte[] buffer = Convert.FromBase64String(qrCodeSetupImageUrl);
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                using (var skStream = new SKManagedStream(ms))
                {
                    return SKBitmap.Decode(skStream);
                }
            }
        }

    }

    public class Base32Encoding
     {
         /// <summary>
         /// Base32 encoded string to byte[]
         /// </summary>
         /// <param name="input">Base32 encoded string</param>
         /// <returns>byte[]</returns>
         public static byte[] ToBytes(string input)
         {
             if (string.IsNullOrEmpty(input))
             {
                 throw new ArgumentNullException("input");
             }
 
             input = input.TrimEnd('='); //remove padding characters
             int byteCount = input.Length * 5 / 8; //this must be TRUNCATED
             byte[] returnArray = new byte[byteCount];
 
             byte curByte = 0, bitsRemaining = 8;
             int mask = 0, arrayIndex = 0;
 
             foreach (char c in input)
             {
                 int cValue = CharToValue(c);
 
                 if (bitsRemaining > 5)
                 {
                     mask = cValue << (bitsRemaining - 5);
                     curByte = (byte)(curByte | mask);
                     bitsRemaining -= 5;
                 }
                 else
                 {
                     mask = cValue >> (5 - bitsRemaining);
                     curByte = (byte)(curByte | mask);
                     returnArray[arrayIndex++] = curByte;
                     curByte = (byte)(cValue << (3 + bitsRemaining));
                     bitsRemaining += 3;
                 }
             }
 
             //if we didn't end with a full byte
             if (arrayIndex != byteCount)
             {
                 returnArray[arrayIndex] = curByte;
             }
 
             return returnArray;
         }
 
         /// <summary>
         /// byte[] to Base32 string, if starting from an ordinary string use Encoding.UTF8.GetBytes() to convert it to a byte[]
         /// </summary>
         /// <param name="input">byte[] of data to be Base32 encoded</param>
         /// <returns>Base32 String</returns>
         public static string ToString(byte[] input)
         {
             if (input == null || input.Length == 0)
             {
                 throw new ArgumentNullException("input");
             }
 
             int charCount = (int)Math.Ceiling(input.Length / 5d) * 8;
             char[] returnArray = new char[charCount];
 
             byte nextChar = 0, bitsRemaining = 5;
             int arrayIndex = 0;
 
             foreach (byte b in input)
             {
                 nextChar = (byte)(nextChar | (b >> (8 - bitsRemaining)));
                 returnArray[arrayIndex++] = ValueToChar(nextChar);
 
                 if (bitsRemaining < 4)
                 {
                     nextChar = (byte)((b >> (3 - bitsRemaining)) & 31);
                     returnArray[arrayIndex++] = ValueToChar(nextChar);
                     bitsRemaining += 5;
                 }
 
                 bitsRemaining -= 3;
                 nextChar = (byte)((b << bitsRemaining) & 31);
             }
 
             //if we didn't end with a full char
             if (arrayIndex != charCount)
             {
                 returnArray[arrayIndex++] = ValueToChar(nextChar);
                 while (arrayIndex != charCount) returnArray[arrayIndex++] = '='; //padding
             }
 
             return new string(returnArray);
         }
 
         private static int CharToValue(char c)
         {
             int value = (int)c;
 
             //65-90 == uppercase letters
             if (value < 91 && value > 64)
             {
                 return value - 65;
             }
             //50-55 == numbers 2-7
             if (value < 56 && value > 49)
             {
                 return value - 24;
             }
             //97-122 == lowercase letters
             if (value < 123 && value > 96)
             {
                 return value - 97;
             }
 
             throw new ArgumentException("Character is not a Base32 character.", "c");
         }
 
         private static char ValueToChar(byte b)
         {
             if (b < 26)
             {
                 return (char)(b + 65);
             }
 
             if (b < 32)
             {
                 return (char)(b + 24);
             }
 
             throw new ArgumentException("Byte is not a value Base32 value.", "b");
         }
     } 

}

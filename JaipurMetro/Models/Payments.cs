using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace RPPSampleMVC.Models
{
    public class PaymentHelper
    {
        const string MERCHANTCODE = "testMerchant2";
        const string CHECKSUMKEY = "WFsdaY28Pf";
        const string ENCRYPTIONKEY = "9759E1886FB5766DA58FF17FF8DD4";
        const string SUCCESSURL = "http://localhost:50263/Payment/PaymentResponse";
        const string FAILUREURL = "http://localhost:50263/Payment/PaymentResponse";
        const string CANCELURL = "http://localhost:50263/Payment/PaymentResponse";        
       
        public static PaymentRequest SendRequest(string PRN, string AMOUNT, string PURPOSE, string USERNAME, string USERMOBILE, string USEREMAIL)
        {
            string REQTIMESTAMP = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string CHECKSUM = MD5HASHING(MERCHANTCODE + "|" + PRN + "|" + AMOUNT + "|" + CHECKSUMKEY);
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            RequestParameters REQUESTPARAMS = new RequestParameters
            {
                MERCHANTCODE = MERCHANTCODE,
                PRN = PRN,
                REQTIMESTAMP = REQTIMESTAMP,
                PURPOSE = PURPOSE,
                AMOUNT = AMOUNT,
                SUCCESSURL = SUCCESSURL,
                FAILUREURL = FAILUREURL,
                CANCELURL = CANCELURL,
                USERNAME = USERNAME,
                USERMOBILE = USERMOBILE,
                USEREMAIL = USEREMAIL,
                UDF1 = "PARAM1",
                UDF2 = "PARAM2",
                UDF3 = "PARAM3",
                OFFICECODE = "",
                REVENUEHEAD = "AMOUNT=" + AMOUNT.ToString(),
                CHECKSUM = CHECKSUM
            };

            string REQUESTJSON = serializer.Serialize(REQUESTPARAMS);
            string ENCDATA = AESEncrypt(REQUESTJSON, ENCRYPTIONKEY);
            PaymentRequest PAYMENTREQUEST = new PaymentRequest
            {
                MERCHANTCODE = MERCHANTCODE,
                REQUESTJSON = REQUESTJSON,
                REQUESTPARAMETERS = REQUESTPARAMS,
                ENCDATA = ENCDATA
            };

            return PAYMENTREQUEST;
        }
      
        private static readonly Encoding encoding = Encoding.UTF8;

        public static PaymentResponse GetResponse(string STATUS, string ENCDATA)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string RESPONSEJSON = AESDecrypt(ENCDATA, ENCRYPTIONKEY);
            ResponseParameters RESPONSEPARAMS = serializer.Deserialize<ResponseParameters>(RESPONSEJSON);
            string CHECKSUM = MD5HASHING(MERCHANTCODE + "|" + RESPONSEPARAMS.PRN + "|" + RESPONSEPARAMS.RPPTXNID + "|" + RESPONSEPARAMS.PAYMENTAMOUNT + "|" + CHECKSUMKEY);

            PaymentResponse PAYMENTRESPONSE = new PaymentResponse();
            if (CHECKSUM == RESPONSEPARAMS.CHECKSUM.ToUpper())
            {
                PAYMENTRESPONSE = new PaymentResponse
                {
                    RESPONSEJSON = RESPONSEJSON,
                    ENCDATA = ENCDATA,
                    RESPONSEPARAMETERS = RESPONSEPARAMS,
                    STATUS = STATUS,
                    CHECKSUMVALID = true
                };
            }
            else
            {
                PAYMENTRESPONSE = new PaymentResponse
                {
                    RESPONSEJSON = RESPONSEJSON,
                    ENCDATA = ENCDATA,
                    RESPONSEPARAMETERS = RESPONSEPARAMS,
                    STATUS = STATUS,
                    CHECKSUMVALID = false
                };
            }

            return PAYMENTRESPONSE;
        }

        public static string AESEncrypt(string textToEncrypt, string encryptionKey)
        {
            try
            {
                RijndaelManaged aes = new RijndaelManaged();
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;
                aes.Key = SHA256.Create().ComputeHash(encoding.GetBytes(encryptionKey));
                aes.IV = MD5.Create().ComputeHash(encoding.GetBytes(encryptionKey));
                ICryptoTransform AESEncrypt = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] buffer = encoding.GetBytes(textToEncrypt);
                return Convert.ToBase64String(AESEncrypt.TransformFinalBlock(buffer, 0, buffer.Length));
            }
            catch (Exception e)
            {
                throw new Exception("Error encrypting: " + e.Message);
            }
        }
        
        public static string AESDecrypt(string textToDecrypt, string encryptionKey)
        {
            try
            {
                RijndaelManaged aes = new RijndaelManaged();
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;
                aes.Key = SHA256.Create().ComputeHash(encoding.GetBytes(encryptionKey));
                aes.IV = MD5.Create().ComputeHash(encoding.GetBytes(encryptionKey));
                ICryptoTransform AESDecrypt = aes.CreateDecryptor(aes.Key, aes.IV);
                byte[] buffer = Convert.FromBase64String(textToDecrypt);
                return encoding.GetString(AESDecrypt.TransformFinalBlock(buffer, 0, buffer.Length));
            }
            catch (Exception e)
            {
                throw new Exception("Error decrypting: " + e.Message);
            }
        }

        public static string MD5HASHING(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

    }

    public class RequestParameters
    {
        public string MERCHANTCODE { get; set; }
        public string PRN { get; set; }
        public string REQTIMESTAMP { get; set; }
        public string PURPOSE { get; set; }
        public string AMOUNT { get; set; }
        public string SUCCESSURL { get; set; }
        public string FAILUREURL { get; set; }
        public string CANCELURL { get; set; }
        public string USERNAME { get; set; }
        public string USERMOBILE { get; set; }
        public string USEREMAIL { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string OFFICECODE { get; set; }
        public string REVENUEHEAD { get; set; }
        public string CHECKSUM { get; set; }
    }

    public class ResponseParameters
    {
        public string MERCHANTCODE { get; set; }
        public string REQTIMESTAMP { get; set; }
        public string PRN { get; set; }
        public decimal? AMOUNT { get; set; }
        public string RPPTXNID { get; set; }
        public string RPPTIMESTAMP { get; set; }
        public string PAYMENTAMOUNT { get; set; }
        public string STATUS { get; set; }
        public string PAYMENTMODE { get; set; }
        public string PAYMENTMODEBID { get; set; }
        public string PAYMENTMODETIMESTAMP { get; set; }
        public string RESPONSECODE { get; set; }
        public string RESPONSEMESSAGE { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string CHECKSUM { get; set; }
    }

    public class PaymentRequest
    {
        public string MERCHANTCODE { get; set; }
        public RequestParameters REQUESTPARAMETERS { get; set; }
        public string REQUESTJSON { get; set; }
        public string ENCDATA { get; set; }
    }

    public class PaymentResponse
    {
        public ResponseParameters RESPONSEPARAMETERS { get; set; }
        public string RESPONSEJSON { get; set; }
        public string STATUS { get; set; }
        public string ENCDATA { get; set; }
        public bool CHECKSUMVALID { get; set; }
    }
}
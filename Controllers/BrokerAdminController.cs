using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ETradeAPI.Models;
using ETradeAPI.Helpers;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using MicroClear.EnterpriseSolutions.CryptographyServices;
using MicroClear.EnterpriseSolutions.ServiceFactories;

namespace ETradeAPI.Controllers
{
    // [AllowAnonymous]
    // [RequireHttps]
    [RoutePrefix("api/BrokerAdmin")]
    public class BrokerAdminController : ApiController
    {

        [Route("GetBrokerSubOrdinateDetails")]
        [HttpPost]
        public HttpResponseMessage GetBrokerSubOrdinateDetails([FromBody] BrokerSubOrdinateReq data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.GetBrokerSubOrdinateDetails(data), System.Text.Encoding.UTF8, "application/json")
            };
        }

        [Route("test")]
        [HttpGet]
        public HttpResponseMessage test(string pwd)
        {

            SymmetricEncryption CgServices = ServiceFactory.GetSymmetricEncryptionInstance();
            string test =CgServices.DecryptData(pwd);
            
            //CryptographyKGAC.CryptographyKGAC ed = new CryptographyKGAC.CryptographyKGAC();
           

            //string test = ed.EncryptStringToBytes(pwd,
            return new HttpResponseMessage()
            {
                Content = new StringContent(test, System.Text.Encoding.UTF8, "application/json")
            };
        }

        public string Encrypt(string clearText)
        {
            string EncryptionKey = getPrivateKey();// dsConfig.Tables[0].Rows[0]["configvalue"].ToString();
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Rijndael encryptor = Rijndael.Create())
            {
                //encryptor.Padding = PaddingMode.;
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        private string Decrypt(string cipherText)
        {

            //SaveLog("Decrypt", cipherText, EventLogEntryType.Information);

            string EncryptionKey = getPrivateKey(); //"MAKقصV2ضSPBعهخحNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText); //Encoding.Unicode.GetBytes(cipherText); 
            using (Rijndael encryptor = Rijndael.Create())
            {
                //encryptor.Padding = PaddingMode.PKCS7;

                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        //cs.Close();
                        cs.FlushFinalBlock();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        private String getPrivateKey()
        {
            String privateKey = "";
            //try
            {
                String connectStr = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;
                using (SqlConnection connect = new SqlConnection(connectStr))
                {
                    using (SqlCommand command = new SqlCommand("getPrivateKey", connect))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@privateKey", SqlDbType.NVarChar, 150).Direction = ParameterDirection.Output;

                        connect.Open();

                        command.ExecuteNonQuery();


                        if (command.Parameters["@privateKey"].Value != null)
                        {
                            privateKey = command.Parameters["@privateKey"].Value.ToString();
                        }

                    }
                }
            }
            //catch (Exception ex)
            //{
            //    SaveLog("privateKeyGCS", " Error => " + ex.ToString(), EventLogEntryType.Error);
            //}

            return privateKey;
        }
    }

}

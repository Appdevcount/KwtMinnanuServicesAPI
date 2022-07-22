using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using ETradeAPI.Filters;
using ETradeAPI.Models;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using ETradeAPI.Helpers;
using System.Net.Http.Headers;
using WebApplication1.Models;

using System.Web;

namespace ETradeAPI.Controllers
{
    [RoutePrefix("api/Docs")]
    [RequireHttps]
    public class FileController : ApiController
    {
        private static readonly string ServerUploadFolder = ConfigurationManager.AppSettings["DocsShareServer"].ToString(); //Path.GetTempPath();
        
        //[HttpPost, Route("StreamUpload")]
        //public async Task<IHttpActionResult> Upload()
        //{
        //    if (!Request.Content.IsMimeMultipartContent())
        //        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

        //    var provider = new MultipartMemoryStreamProvider();
        //    await Request.Content.ReadAsMultipartAsync(provider);
        //    foreach (var file in provider.Contents)
        //    {
        //        var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
        //        var buffer = await file.ReadAsByteArrayAsync();
        //        //Do whatever you want with filename and its binaray data.
        //    }

        //    return Ok();
        //}

        
        [Route("Upload")]
        [HttpPost]
        public HttpResponseMessage UploadSingleFile()
        {
            string login = ConfigurationManager.AppSettings["UploadAccessUserId"].ToString();
            string domain = ConfigurationManager.AppSettings["UploadAccessDomainName"].ToString();
            string password = Crypt.DecryptData(ConfigurationManager.AppSettings["UploadAccessPassword"].ToString());
            
            
            string tokenId = "";
            string mUserid = "";
            string OrgReqId = "";
            string DocumentName = "";
            string DocumentType = "";
            string eservicerequestid = "";
            tokenId = Convert.ToString(System.Web.HttpContext.Current.Request.Form["tokenId"]);
            mUserid = Convert.ToString(System.Web.HttpContext.Current.Request.Form["mUserid"]);
            DocumentName = Convert.ToString(System.Web.HttpContext.Current.Request.Form["DocumentName"]);
            DocumentType = Convert.ToString(System.Web.HttpContext.Current.Request.Form["DocumentType"]);


            eservicerequestid= Convert.ToString(System.Web.HttpContext.Current.Request.Form["eservicerequestid"]);
            OrgReqId = Convert.ToString(System.Web.HttpContext.Current.Request.Form["OrgReqId"]);
            if (!eservicerequestid.All(char.IsDigit))
            {
                eservicerequestid = CommonFunctions.CsUploadDecrypt(eservicerequestid.ToString());
            }
            if (!OrgReqId.All(char.IsDigit))
            {

                OrgReqId = CommonFunctions.CsUploadDecrypt(OrgReqId.ToString());
            }



            if (!DocumentType.All(char.IsDigit))
            {
                DocumentType = CommonFunctions.CsUploadDecrypt(DocumentType);
            }
          

            MobileDataBase.Result rslt = MobileDataBase.GetValidUserDetails(tokenId, mUserid);
            rslt.Data = null;
          
            try
            {
                rslt.status = "0";
                if (rslt.status == "0")
                {

                    if (ConfigurationManager.AppSettings["UploadAccessImpersinationRequired"].ToString() == "Y")
                    {
                        using (UserImpersonation user = new UserImpersonation(login, domain, password))
                        {
                            if (user.ImpersonateValidUser())
                            {                               
                                UploadFile(rslt, DocumentName, DocumentType, OrgReqId);
                            }
                            else
                            {
                                throw new UnauthorizedAccessException("Access failed while uploading.");
                            }
                        }
                    }
                    else
                    {
                        UploadFile(rslt, DocumentName, DocumentType, OrgReqId);                        
                    }                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(rslt, Formatting.None)//Frdata
                , System.Text.Encoding.UTF8, "application/json")
            };
        }



        private MobileDataBase.Result UploadFile(MobileDataBase.Result rslt,string DocumentName, string DocumentType, string OrgReqId)
        {
            MobileDataBase m = new MobileDataBase();
            string ID = m.GetNewIntKey("ScanRequestUploadDocs");

            string sPath = "";
            string UploadedFrom= Convert.ToString(System.Web.HttpContext.Current.Request.Form["UploadedFrom"]);

            string ShareFolderPath1 = Path.Combine(UploadedFrom, DateTime.Now.Year + "\\" + DateTime.Now.Month.ToString("00") + "\\" + DateTime.Now.Day.ToString("00") + '\\' + OrgReqId + '\\' + ID);// System.Web.Hosting.HostingEnvironment.MapPath("~/locker/");


            sPath = Path.Combine(UploadedFrom, DateTime.Now.Year + "\\" + DateTime.Now.Month.ToString("00") + "\\" + DateTime.Now.Day.ToString("00") + '\\' + OrgReqId+'\\'+ ID);// System.Web.Hosting.HostingEnvironment.MapPath("~/locker/");


         //   string ShareFolderPath1 = UploadedFrom + "\\" + year + "\\" + month + "\\" + day + "\\" + sProfileReferenceId;// +DeclarationId;
           


            System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
            FileResult Frdata = new FileResult();
            Frdata.DocumentName = DocumentName;
            Frdata.DocumentType = DocumentType;
            Frdata.OrgReqId = OrgReqId;
            Frdata.UploadedFrom= Convert.ToString(System.Web.HttpContext.Current.Request.Form["UploadedFrom"]);
            //  Frdata.NewFileName = DocumentName;
            Frdata.FilePath = DateTime.Now.Year + "\\" + DateTime.Now.Month.ToString("00") + "\\" + DateTime.Now.Day.ToString("00") + '\\' + OrgReqId;
            Frdata.IsUploaded = 'n';
            Frdata.EserviceRequestId= Convert.ToString(System.Web.HttpContext.Current.Request.Form["eservicerequestid"]);

            if (!Convert.ToString(System.Web.HttpContext.Current.Request.Form["eservicerequestid"]).All(char.IsDigit))
            {
                Frdata.EserviceRequestId = CommonFunctions.CsUploadDecrypt(Convert.ToString(System.Web.HttpContext.Current.Request.Form["eservicerequestid"]).ToString());
            }

            // CHECK THE FILE COUNT.
            FileInfo F = null;
            for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
            {
                System.Web.HttpPostedFile hpf = hfc[iCnt];

                if (hpf.ContentLength > 0)
                {
                    //if (!Directory.Exists(sPath))
                    //{
                    //    Directory.CreateDirectory(sPath);
                    //}

                    if (!Directory.Exists(Path.Combine(ServerUploadFolder, ShareFolderPath1))) Directory.CreateDirectory(Path.Combine(ServerUploadFolder, ShareFolderPath1));
                    // sPath = ShareFolderPath1 + "\\" + FullfileName;
                   // file.SaveAs(Path.Combine(ServerUploadFolder, sPath));



                    // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
                 //   if (!File.Exists(Path.Combine(sPath, Path.GetFileName(hpf.FileName))) && Regex.IsMatch(hpf.FileName.Trim(), "(\\.(jpg|jpeg|pdf))$", RegexOptions.IgnoreCase))
                    {
                        // SAVE THE FILES IN THE FOLDER.
                        string extension = Path.GetExtension(hpf.FileName);
                        string NewfileName = DocumentName + extension;
                        NewfileName= NewfileName.Replace('/', '-');
                        Frdata.NewFileName = NewfileName;
                        //   hpf.SaveAs(Path.Combine(sPath, Path.GetFileName(hpf.FileName)));
                        hpf.SaveAs(Path.Combine(ServerUploadFolder, sPath, NewfileName));

                        Frdata.Name = Path.Combine(sPath, NewfileName);
                        Frdata.FilePath= Path.Combine(sPath, NewfileName);
                        //    F = new FileInfo(Path.Combine(sPath, Path.GetFileName(hpf.FileName)));
                        F = new FileInfo(Path.Combine(ServerUploadFolder,sPath, NewfileName));

                        Frdata.FileSize = (F.Length / 1024).ToString("0.00");
                        Frdata.IsUploaded = 'y';
                        break;

                    }
                }
            }
            if (Frdata.IsUploaded == 'y')
            {
                rslt.Data = MobileDataBase.UpdateUploadDataDS(rslt.mUserId, Frdata);
              //  if (F != null)
               // {
               //     F.Rename(Frdata.NewFileName);
                //}
            }
            return rslt;
        }

        private string GetSingleOrDefault(IEnumerable<string> lst)
        {
            if (lst == null) return "";
            else return lst.FirstOrDefault();
        }

        //[Route("DocTypes/{lang}")]
        [Route("DocTypes")]
        [HttpPost]        
        public HttpResponseMessage GetDocumentTypes([FromBody] OrgReqResultInfoParams data)
        {            
            return  new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.GetTypes("etrade.Usp_MApp_GetOrgDocTypes", data.lang, data.sOrgReqId), System.Text.Encoding.UTF8, "application/json")
            };
        }

        
        [Route("OpenFile")]
        [HttpPost]
        public HttpResponseMessage OpenFile([FromBody] OpenDocumentParams data)
        {
            string login = ConfigurationManager.AppSettings["UploadAccessUserId"].ToString();
            string domain = ConfigurationManager.AppSettings["UploadAccessDomainName"].ToString();
            string password = Crypt.DecryptData(ConfigurationManager.AppSettings["UploadAccessPassword"].ToString());


            string tokenId = (data == null || data.tokenId == null) ? "" : data.tokenId;
            string mUserid = (data == null || data.mUserid == null) ? "" : data.mUserid;
            string DocumentId = (data == null || data.DocumentId == null) ? "" : data.DocumentId;
            
            //tokenId = Convert.ToString(System.Web.HttpContext.Current.Request.Form["tokenId"]);
            //mUserid = Convert.ToString(System.Web.HttpContext.Current.Request.Form["mUserid"]);
            //DocumentId = Convert.ToString(System.Web.HttpContext.Current.Request.Form["DocumentId"]);
            

            MobileDataBase.Result rslt = MobileDataBase.GetValidUserDetails(tokenId, mUserid);
            rslt.Data = null;

            try
            {
                if (rslt.status == "0")
                {
                    string SuffixPath = MobileDataBase.GetDocPath(DocumentId, mUserid);
                    string sPath = "";
                    sPath = Path.Combine(ServerUploadFolder, SuffixPath);
                    if (ConfigurationManager.AppSettings["UploadAccessImpersinationRequired"].ToString() == "Y")
                    {
                        using (UserImpersonation user = new UserImpersonation(login, domain, password))
                        {
                            if (user.ImpersonateValidUser())
                            {
                                return DownLoadFile(sPath);
                            }
                        }
                    }
                    else
                    {
                        return DownLoadFile(sPath);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return this.Request.CreateResponse(HttpStatusCode.NotFound, "File not found.");                           
        }
        //azhar
        public HttpResponseMessage GetFile(string fileName)
        {
            //Create HTTP Response.
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

            //Set the File Path.
            string filePath = fileName;

            //Check whether File exists.
            if (!File.Exists(filePath))
            {
                //Throw 404 (Not Found) exception if File not found.
                response.StatusCode = HttpStatusCode.NotFound;
                response.ReasonPhrase = string.Format("File not found: {0} .", fileName);
                throw new HttpResponseException(response);
            }

            //Read the File into a Byte Array.
            byte[] bytes = File.ReadAllBytes(filePath);

            //Set the Response Content.
            response.Content = new ByteArrayContent(bytes);

            //Set the Response Content Length.
            response.Content.Headers.ContentLength = bytes.LongLength;

            //Set the Content Disposition Header Value and FileName.
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = fileName;

            //Set the File Content Type.
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(fileName));
            return response;
        }

        private HttpResponseMessage DownLoadFile(string sPath)
        {

            

            using (MemoryStream ms = new MemoryStream())
            {
                using (FileStream file = new FileStream(sPath, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = new byte[file.Length];
                    file.Read(bytes, 0, (int)file.Length);
                    ms.Write(bytes, 0, (int)file.Length);

                    HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
                    httpResponseMessage.Content = new ByteArrayContent(bytes.ToArray());
                    httpResponseMessage.Content.Headers.Add("x-filename", Path.GetFileName(sPath));
                    httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    httpResponseMessage.Content.Headers.ContentDisposition.FileName = Path.GetFileName(sPath);
                    httpResponseMessage.StatusCode = HttpStatusCode.OK;
                    return httpResponseMessage;
                }
            }
            //if (!File.Exists(sPath))
            //    return new HttpResponseMessage(HttpStatusCode.BadRequest);

            //HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            ////response.Content = new StreamContent(new FileStream(localFilePath, FileMode.Open, FileAccess.Read));
            //Byte[] bytes = File.ReadAllBytes(sPath);
            ////String file = Convert.ToBase64String(bytes);
            //response.Content = new ByteArrayContent(bytes);
            //response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            ////response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");// MimeTypeMap.GetMimeType(Path.GetFileName(sPath)));
            //response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeTypeMap.GetMimeType(Path.GetFileName(sPath)));
            //response.Content.Headers.ContentDisposition.FileName = Path.GetFileName(sPath);
            //response.StatusCode = HttpStatusCode.OK;
            //return response;
        }
        

        //[Route("DocTypes/{lang}")]
        [Route("DeleteOrgReqDoc")]
        [HttpPost]
        public HttpResponseMessage DeleteOrgReqDocument([FromBody] OrgReqResultDocInfoParams data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.DeleteOrgReqDocument(data), System.Text.Encoding.UTF8, "application/json")
            };
        }
        //byazhar
        [Route("DeleteOrgReqDocForEservice")]
        [HttpPost]
        public HttpResponseMessage DeleteOrgReqDocForEservice([FromBody] OrgReqResultDocInfoParams data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.DeleteOrgReqDocForEservice(data), System.Text.Encoding.UTF8, "application/json")
            };
        }
        //byazhar
        [Route("OpenFileForEservice")]
        [HttpPost]
        public HttpResponseMessage OpenFileForEservice([FromBody] OpenDocumentParams data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.DownloadOrgReqDocForEservice(data), System.Text.Encoding.UTF8, "application/json")
            };
        }

        //        public HttpResponseMessage OpenFileForEservice([FromBody] OpenDocumentParams data)
        //        {
        //            string login = ConfigurationManager.AppSettings["UploadAccessUserId"].ToString();
        //            string domain = ConfigurationManager.AppSettings["UploadAccessDomainName"].ToString();
        //            string password = Crypt.DecryptData(ConfigurationManager.AppSettings["UploadAccessPassword"].ToString());


        //            string tokenId = (data == null || data.tokenId == null) ? "" : data.tokenId;
        //            string mUserid = (data == null || data.mUserid == null) ? "" : data.mUserid;
        //            string DocumentId = (data == null || data.DocumentId == null) ? "" : data.DocumentId;

        //            //tokenId = Convert.ToString(System.Web.HttpContext.Current.Request.Form["tokenId"]);
        //            //mUserid = Convert.ToString(System.Web.HttpContext.Current.Request.Form["mUserid"]);
        //            //DocumentId = Convert.ToString(System.Web.HttpContext.Current.Request.Form["DocumentId"]);


        //            //MobileDataBase.Result rslt = MobileDataBase.GetValidUserDetails(tokenId, mUserid);
        //            //rslt.Data = null;
        //            MobileDataBase.Result rslt = new MobileDataBase.Result();
        //            rslt.status = "0";
        //            try
        //            {
        //                if (rslt.status == "0")
        //                {
        //                    string SuffixPath = MobileDataBase.GetDocPathForEservice(DocumentId,data.hiderefprofile,data.EserviceRequestid);

        //                    string sPath = "";
        //                    sPath = Path.Combine(ServerUploadFolder, SuffixPath);
        //                    if (ConfigurationManager.AppSettings["UploadAccessImpersinationRequired"].ToString() == "Y")
        //                    {
        //                        using (UserImpersonation user = new UserImpersonation(login, domain, password))
        //                        {
        //                            if (user.ImpersonateValidUser())
        //                            {
        ////                                return DownLoadFile(sPath);
        //                                return GetFile(sPath);

        //                               // GetFile
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        return GetFile(sPath);
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //            return this.Request.CreateResponse(HttpStatusCode.NotFound, "File not found.");
        //        }

    }
    public static class FileInfoExtensions
    {
        /// <summary>
        /// behavior when new filename is exist.
        /// </summary>
        public enum FileExistBehavior
        {
            /// <summary>
            /// None: throw IOException "The destination file already exists."
            /// </summary>
            None = 0,
            /// <summary>
            /// Replace: replace the file in the destination.
            /// </summary>
            Replace = 1,
            /// <summary>
            /// Skip: skip this file.
            /// </summary>
            Skip = 2,
            /// <summary>
            /// Rename: rename the file. (like a window behavior)
            /// </summary>
            Rename = 3
        }
        /// <summary>
        /// Rename the file.
        /// </summary>
        /// <param name="fileInfo">the target file.</param>
        /// <param name="newFileName">new filename with extension.</param>
        /// <param name="fileExistBehavior">behavior when new filename is exist.</param>
        public static void Rename(this System.IO.FileInfo fileInfo, string newFileName, FileExistBehavior fileExistBehavior = FileExistBehavior.None)
        {
            string newFileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(newFileName);
            string newFileNameExtension = System.IO.Path.GetExtension(newFileName);
            string newFilePath = System.IO.Path.Combine(fileInfo.Directory.FullName, newFileName);

            if (System.IO.File.Exists(newFilePath))
            {
                switch (fileExistBehavior)
                {
                    case FileExistBehavior.None:
                        throw new System.IO.IOException("The destination file already exists.");
                    case FileExistBehavior.Replace:
                        System.IO.File.Delete(newFilePath);
                        break;
                    case FileExistBehavior.Rename:
                        int dupplicate_count = 0;
                        string newFileNameWithDupplicateIndex;
                        string newFilePathWithDupplicateIndex;
                        do
                        {
                            dupplicate_count++;
                            newFileNameWithDupplicateIndex = newFileNameWithoutExtension + " (" + dupplicate_count + ")" + newFileNameExtension;
                            newFilePathWithDupplicateIndex = System.IO.Path.Combine(fileInfo.Directory.FullName, newFileNameWithDupplicateIndex);
                        } while (System.IO.File.Exists(newFilePathWithDupplicateIndex));
                        newFilePath = newFilePathWithDupplicateIndex;
                        break;
                    case FileExistBehavior.Skip:
                        return;
                }
            }
            System.IO.File.Move(fileInfo.FullName, newFilePath);
        }
    }
}

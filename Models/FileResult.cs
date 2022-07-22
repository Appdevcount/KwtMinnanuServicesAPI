using System;
using System.Collections.Generic;

using System.Linq;
using System.Web;

namespace ETradeAPI.Models
{
    public class FileResult : AdditionalRequestInfo
    {
       
      //  public string FileName { get; set; }
        public string NewFileName { get; set; }
        public string OrgReqId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentType { get; set; }      
        public string Name { get; set; }
        public string ContentType { get; set; }
        public string FilePath { get; set; }
        public string FileSize { get; set; }
        public char IsUploaded { get; set; }
        public string tokenId { get; set; }
        public string EserviceRequestId { get; set; }
        public string UploadedFrom { get; set; }
    }

    public class FileUploaded : AdditionalRequestInfo
    {
        public string FileName, DocumentType,FileSizeInKB;

    }
    public class DocumentType : AdditionalRequestInfo
    {
        public string Name { set; get; }
        public string Value { set; get; }

    }

}
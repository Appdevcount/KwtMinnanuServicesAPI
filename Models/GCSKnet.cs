using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ETradeAPI.Models
{
    public class VerifyReceiptDetailsforGCSSite
    {
        public bool ReceiptValid { get; set; }
        public bool PaymentTried { get; set; }
        public string PaymentTriedStatus { get; set; }
        public int OPDetailId { get; set; }
        public bool Proceed { get; set; }
        public DateTime TranSttDateTime { get; set; }
        public string Message { get; set; }
        public int MessageCode { get; set; }
        //public string Token { get; set; }
    }
 public class ReceiptDetails
    {
        public int ReferenceId { get; set; } //(int, null)
        public string ReferenceNumber { get; set; } //(varchar(30), null)
        public string ReferenceType { get; set; } //(char(1), null)
        public string Amount { get; set; } //(decimal(18,3), null)
        public Int64? OLPaymentId { get; set; } //(varchar(7), not null)
        public string PortalLoginId { get; set; } //(varchar(7), not null)
        public int? LogInPortId { get; set; } //(varchar(7), not null)
        public int OrganizationId { get; set; } //(int, null)
        public string PaymentFor { get; set; } //(varchar(1), not null)
        public string PaidByType { get; set; } //(varchar(1), not null)
        public string lang { get; set; } //(varchar(3), not null)
        public int? ReceiptId { get; set; } //(int, not null)
        public string KNETAccType { get; set; } //(varchar(9), not null)
        public int BrPaymentTransactionId { get; set; } //(varchar(1), not null)
        public DateTime PostDate { get; set; } //(varchar(7), not null)
        public int? OLTransId { get; set; } //(varchar(7), not null)
        public string UserId { get; set; } //(varchar(30), not null)
        public string ReceiptNumber { get; set; } //(varchar(20), null)
        public string PaidByName { get; set; } //(varchar(30), not null)
        public string PayeeMailId { get; set; } //(varchar(7), not null)
        public string PayeeOrgMailId { get; set; } //(varchar(7), not null)
        public int? TrackId { get; set; } //(varchar(7), not null))
        public DateTime TranStopDateTime { get; set; } //(datetime, null)
        public DateTime ReceiptDate { get; set; } //(smalldatetime, null)
        public decimal? GCSAmount { get; set; } //(decimal(18,3), null)
        public decimal? KGACAmount { get; set; } //(decimal(18,3), null)
        public decimal? Balance { get; set; } //(decimal(18,3), null)
        public string TempReceiptNumber { get; set; } //(varchar(50), null)
        public string TokenId { get; set; } //(varchar(50), null)
        public string PayeeName { get; set; } //(varchar(50), null)
        public string Mobile { get; set; } //(varchar(50), null)
        public string CustEmail { get; set; } //(varchar(50), null)
        public DateTime TokenExpTime { get; set; }
    }
    public class ReceiptAction
    {
        public VerifyReceiptDetailsforGCSSite VerifyReceiptDetailsforGCSSite { get; set; }

        public ReceiptDetailsMinified ReceiptDetailsMinified { get; set; }
    }
    public class ReceiptDetailsMinified
    {
        //public int ReferenceId { get; set; } //(int, null)
        public string ReferenceNumber { get; set; } //(varchar(30), null)
                                                    //public string ReferenceType { get; set; } //(char(1), null)
        public string Amount { get; set; } //(decimal(18,3), null)
                                           //public decimal Amount { get { return val} set { value = Math.Round(value, 3); } } //(decimal(18,3), null)
                                           //public Int64? OLPaymentId { get; set; } //(varchar(7), not null)
                                           //public string PortalLoginId { get; set; } //(varchar(7), not null)
                                           //public int? LogInPortId { get; set; } //(varchar(7), not null)
                                           //public int OrganizationId { get; set; } //(int, null)
                                           //public string PaymentFor { get; set; } //(varchar(1), not null)
                                           //public string PaidByType { get; set; } //(varchar(1), not null)
                                           //public string lang { get; set; } //(varchar(3), not null)
                                           //public int? ReceiptId { get; set; } //(int, not null)
                                           //public string KNETAccType { get; set; } //(varchar(9), not null)
                                           //public int BrPaymentTransactionId { get; set; } //(varchar(1), not null)
                                           //public DateTime PostDate { get; set; } //(varchar(7), not null)
                                           //public int? OLTransId { get; set; } //(varchar(7), not null)
                                           //public string UserId { get; set; } //(varchar(30), not null)
                                           //public string ReceiptNumber { get; set; } //(varchar(20), null)
                                           //public string PaidByName { get; set; } //(varchar(30), not null)
                                           //public string PayeeMailId { get; set; } //(varchar(7), not null)
                                           //public string PayeeOrgMailId { get; set; } //(varchar(7), not null)
                                           //public int? TrackId { get; set; } //(varchar(7), not null))
                                           //public DateTime TranStopDateTime { get; set; } //(datetime, null)
                                           //public DateTime ReceiptDate { get; set; } //(smalldatetime, null)
                                           //public decimal? GCSAmount { get; set; } //(decimal(18,3), null)
                                           //public decimal? KGACAmount { get; set; } //(decimal(18,3), null)
                                           //public decimal? Balance { get; set; } //(decimal(18,3), null)
                                           //public string TempReceiptNumber { get; set; } //(varchar(50), null)
        public string TokenId { get; set; } //(varchar(50), null)
        public string PayeeName { get; set; } //(varchar(50), null)
        public string PaidByName { get; set; } //(varchar(50), null)
        public string UserId { get; set; } //(varchar(50), null)
        public string Mobile { get; set; } //(varchar(50), null)
        public string CustEmail { get; set; } //(varchar(50), null)

    }

    public class GCSReqObj
    {
        public string Action { get; set; }
        public string CommonData { get; set; }
        public string CommonData1 { get; set; }
        public string CommonData2 { get; set; }
        public string CommonData3 { get; set; }
        public string CommonData4 { get; set; }
        public string CommonData5 { get; set; }
        public string UserId { get; set; }
        public string TokenId { get; set; }
        public string LegalEntity { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
    public class GCSResp
    {
        public string Action { get; set; }
        public string CommonData { get; set; }
        public string CommonData1 { get; set; }
        public string CommonData2 { get; set; }
        public string CommonData3 { get; set; }
        public string CommonData4 { get; set; }
        public string CommonData5 { get; set; }
    }

}

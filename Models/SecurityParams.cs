using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ETradeAPI.Models
{
    //azhar test

        public class Verifypassword
    {
        public string email { get; set; }
        public string password { get; set; }

    }


    public class AdditionalRequestInfo
    {
        public string geoLocation { get; set; }
        public string deviceType { get; set; }
        public string browserIp { get; set; }
        public string requestDatetime { get; set; }
    }

    public class SecurityParams : AdditionalRequestInfo
    {
        public string tokenId { get; set; }
        public string mUserid { get; set; }
    }
    public class UserSession 
    {
        public string lang { get; set; }
        public int Userid { get; set; }
    }
    public class DeclarationSearchParams: SecurityParams
    {
        public string tempDeclNumber { get; set; }
        public string lang { get; set; }
    }
    public class DeclarationSearchByIdParams : SecurityParams
    {
        public string OrganizationId { get; set; }
        public bool ApprovedDetail { get; set; }

    }
    public class HBSearchParams : SecurityParams
    {
        public string hbNumber { get; set; }
        public bool HBSearch { get; set; }// OPTION 1 IS TRUE OPTION 2 IS FALSE
        public string DONumber { get; set; }
        public string SecurityCode { get; set; }
        public string lang { get; set; }
    }
    public class HSCodeSearchParams : SecurityParams
    {
        
        public string data { get; set; }
        public string paramType { get; set; }
    }
    public class HSCodeTreeViewParams : SecurityParams
    {

        public string hsCodeId { get; set; }
        
    }
    public class LogOnRequest : AdditionalRequestInfo
    {
        public string email { set; get; }
        public string pwd { set; get; }
        public string Lang { set; get; }
    }
    public class VerifyOTPParams : SecurityParams
    {

        public string Email { get; set; }
        public string Mobile { get; set; }
        public string ReferenceId { get; set; }

        public string Lang { get; set; }
    }
    
    public class ReSendOTPParams : SecurityParams
    {

        public string rType { get; set; }
     
    }
    public class langParams : SecurityParams
    {

        public string lang { get; set; }

    }

    public class ForgotPassWordInput : AdditionalRequestInfo
    {

        public string emailId { get; set; }
        public string mobileNo { get; set; }
        public int CaptchaId { get; set; }
        public string CaptchaValue { get; set; }
    }


    public class ResetPwdByOTPParams : AdditionalRequestInfo
    {
        public string otpId { get; set; }
        public string otpValue { get; set; }
        public string mUserid { get; set; }
        public string newPwd { get; set; }
       
        
    }

    
         public class EPaymentRequestInfoParams : SecurityParams
    {

        public string RequestNo { get; set; }
        public string lang { get; set; }
    }

    public class EPaymentRequestDetailsParams : SecurityParams
    {
        public string lang { get; set; }
        public string pagenumber { get; set; }
        public string searchCriteria { get; set; }
        public string searchDropdown { get; set; }

    }

    public class OrgReqResultInfoParams : langParams
    {
        public string sOrgReqId { get; set; }
        public string sOrgId { get; set; }

    }
    public class GetOrganizationDocuments : SecurityParams
    {
        public string OrganizationId { get; set; }
        public string OrganizationRequestId { get; set; }

    }
    public class OrgReqResultDocInfoParams : SecurityParams
    {

        public string EserviceRequestid { get; set; }
        public string sOrgReqDocId { get; set; }

    }

    public class OpenDocumentParams : SecurityParams
    {

        public string EserviceRequestid { get; set; }
        public string DocumentId { get; set; }
        public string hiderefprofile { get; set; }
        public string tokenId { get; set; }

    }
    public class CallbackRedirectInfo : SecurityParams
    {
        public string OPTokenId { get; set; }
        public string RedirectUrl { get; set; }
        public string EpayReqNo { get; set; }
    }
    public class EPaymentReceipt : SecurityParams
    {
        public string OPTokenId { get; set; }
        public string PaymentStatus { get; set; }
    }

    public class ValidateContacts : SecurityParams
    {
        public string Reference { get; set; }
        public string ReferenceType { get; set; }
        public string ReferenceId { get; set; }
    }

}
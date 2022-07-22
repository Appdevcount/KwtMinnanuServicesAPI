using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ETradeAPI.Models
{
    public class BrokerUpdateModel : PaymentsModel
    {
        public String Userid { get; set; }
        public String BrokerType { get; set; }
        public String requestNo { get; set; }
        public String ParentBrokerName { get; set; }
        public String BrokerArabicName { get; set; }
        public String BrokerEnglishName { get; set; }
        public String CivilIdNo { get; set; }
        public int Organizationid { get; set; }
        public String CivilIdExpirydate { get; set; }
        public String passportNo { get; set; }
        public String PassportExpirydate { get; set; }
        public String TradeLicenseExpiryDate { get; set; }
        public String MobileNumber { get; set; }
        public String MailAddress { get; set; }
        public String officialAddress { get; set; }
        public String stateid { get; set; }
        public String RequestNumber { get; set; }
        public String Eservicerequestid { get; set; }
        public String tokenId { get; set; }
        public String ownerlocid { get; set; }
        public String ownerorgid { get; set; }
        public String lang { get; set; }
        public String Referenceprofile { get; set; }

        // 27-FEB
        public Int64 requestForMobileUserId { get; set; }
        public int mobileUserId { get; set; }

        //   public List<SelectListItem> ddlDocumentTypesitems { get; set; }

        //    public String docsid { get; set; }
        public String sessionid { get; set; }
        public int Serviceid { get; set; }

public string SelectedOrgidForIssuance { get; set; }
        public string Requestorserviceid { get; set; }
        public string Nationality { get; set; }
        public string BrokerLicenseNumber { get; set; }
        public string TypeOfAction { get; set; }


        public string eServiceUserEmailId { get; set; }



        public String fromBusiness { get; set; }

       // public String CommercialLicenseNumber { get; set; }

        public String ChangeJobTitleFrom { get; set; }

        public String ChangeJobTitleTo { get; set; }


       

    }

    public class PaymentsModel
    {
        public String paymentMode { get; set; }
        public String PaymentTypeId { get; set; }

        public String paidfor { get; set; }
        public String paidby { get; set; }

        public String PaidByType { get; set; }

        public String urltoredirectforpayments { get; set; }
        public String redirecturl { get; set; }
    }


    public class BrokerRenewalModel
    {
        public String tokenid { get; set; }

        public String Userid { get; set; }
        public String paidfor { get; set; }
        public String paidby { get; set; }

        public String PaidByType { get; set; }

        public String urltoredirectforpayments { get; set; }
        public String redirecturl { get; set; }
        public String PaymentTypeId { get; set; }
        public string Orgid { get; set; }

        public string Requestorserviceid { get; set; }
    }
    // logging of UserActivities
    public class LogUserActivity
    {
        public string LoginUserid { get; set; }
        public string LoginTime { get; set; }
        public string sessionId { get; set; }
        public string Serviceid { get; set; }
        public string IPAddress { get; set; }
        public string McUserOrgId { get; set; }
        public string McUserName { get; set; }
        public string legalentity { get; set; }
        public bool ClearingAgentservice { get; set; }
        public bool OrganizationService { get; set; }
        public string ActivityPerformed { get; set; }
        public string SignInSignOut { get; set; }
        public string LogOutTime { get; set; }
        public string OtherAdditionalInfo { get; set; }
    }
    #region BrokerEntityServiceRequestModel

    public class BrokerServiceRequestModel
    {
        public String userid { get; set; }
        public String legalentity { get; set; }
        public String SelectedFileId { get; set; }
        public String UnSelectedFileId { get; set; }
        public String requestNo { get; set; }
        public String Status { get; set; }
        public List<EserviceList> ListOfAvailableServices { get; set; }
        public String docsid { get; set; }
        public String SelectedServiceids { get; set; }
        public String UnSelectedServiceids { get; set; }
         public bool? CheckAvailableServicesforRequest { get; set; }
        public int MobileUserid { get; set; }
        public int RequestedForMobileUserid { get; set; }
		 
    }
    public class EserviceList
    {
        public String AvailableEserviceId { get; set; }
        public String AvailableEservicename { get; set; }
        public String StatusOfRequestedServices { get; set; }
    }
    #endregion



}
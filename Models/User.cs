using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ETradeAPI.Models
{
    public class User : SecurityParams
    {
        public string FirstName { set; get; }        
        public string LastName { set; get; }
        public string CivilId  { set; get; }
        public string MobileTelNumber { set; get; }
        public string EmailId  { set; get; }
        public string Pass  { set; get; }
        public string Language { set; get; }
        public int CaptchaId { get; set; }
        public string CaptchaValue { get; set; }


        public string Themes { get; set; }

        public string LegalEntity { set; get; }
        public string Gender { set; get; }
        public List<EService> AvailableEServices { get; set; }
        public List<Organization> AvailableOrganizations { get; set; }

        public string SelectedServices { get; set; }
        public string SelectedOrganizations { get; set; }

        public string LicenceNumber { get; set; }

        public string LegalEntitySubType { set; get; }

        public bool ExistingUser { get; set; }
        public string TradeLicenseNumber { get; set; }
        public string GeneralBrokerLicenseNumber { get; set; }
        public string MCUserID { get; set; }
        public string MCPassword { get; set; }
        public bool IsAdmin { get; set; }


        public bool SubUser { get; set; }
        public int ParentID { get; set; }
        public string Token { get; set; }//added newly for additional user cross check for - security feedback -Siraj


        public bool IsIndustrial { get; set; }//added newly OrgSvcNew Chnages 02-10-2019  -Siraj
        public string IndustrialLicenseNumber { get; set; }//added newly OrgSvcNew Chnages 02-10-2019  -Siraj
        public string CommercialLicenseNumber { get; set; }//added newly OrgSvcNew Chnages 02-10-2019  -Siraj
        public string CommercialLicenseYear { get; set; }//added newly OrgSvcNew Chnages 02-10-2019  -Siraj
        public string Governorate { get; set; }//added newly OrgSvcNew Chnages 02-10-2019  -Siraj
        public string Region { get; set; }//added newly OrgSvcNew Chnages 02-10-2019  -Siraj
        public string Block { get; set; }//added newly OrgSvcNew Chnages 02-10-2019  -Siraj
        public string Street { get; set; }//added newly OrgSvcNew Chnages 02-10-2019  -Siraj
        public string PostalCode { get; set; }//added newly OrgSvcNew Chnages 02-10-2019  -Siraj
        public string Address { get; set; }//added newly OrgSvcNew Chnages 02-10-2019  -Siraj
        public bool AgreeInfoCorrectness { get; set; }//added newly OrgSvcNew Chnages 02-10-2019  -Siraj

        public string CompanyCivilId { set; get; }//added newly OrgSvcNew Chnages 02-10-2019  -Siraj
        public string importerLicenseNumber { get; set; }//added newly OrgSvcNew Chnages 11-07-2019  -Azhar

        
        public string CompanyName { get; set; }

         public string Nationality { set; get; }



        //---------------------


        public string OrganizationNameEng { get; set; }
        public string OrganizationNameAra { get; set; }
        public string OrganizationCode { get; set; }
        public string ClearanceFileNumber { get; set; }
        public bool DROrg { get; set; }
        public string DROrgStatus { get; set; }
        public HttpPostedFileBase File { get; set; }


    }
    public class DROrgUserRequest
    {
        public string Email { get; set; }
        public string ESERVICEREQUESTID { get; set; }
        public string UserId { get; set; }
        public string ESERVICEREQUESTNUMBER { get; set; }
        public string DateCreated { get; set; }
        public string DateModified { get; set; }
        public string OrganizationCode { get; set; }
        public string OrganizationCivilId { get; set; }
        public string OrganizationNameEng { get; set; }
        public string OrganizationNameAra { get; set; }
        public string TradeLicenseNumber { get; set; }
        public string StatusCode { get; set; }
        public string Status { get; set; }
        public string RejectionRemarks { get; set; }
        public string editable { get; set; }
    }

    public class EService
    {
        public int SubscriptionID { get; set; }
        public int ServiceID { get; set; }
        public string ServiceNameEng { get; set; }
        public string ServiceNameAra { get; set; }
    }
    public class Organization
    {
        public int OrganizationId { get; set; }
        public string OrgNameEng { get; set; }
        public string OrgNameAra { get; set; }
    }
    public class ReqObj
    {
        public int ParentID { get; set; }
        public int ChildUser { get; set; }
        public string Action { get; set; }
        public string CommonData { get; set; }
        public string CommonData1 { get; set; }
        public string CommonData2 { get; set; }
        public string OrgID { get; set; }
        public HttpPostedFileBase File { get; set; }
    }

    public class ServicesAndOrgManagementFortheUser
    {
        public int UserID { get; set; }
        public string ServiceID { get; set; }
        public string SubscriptionID { get; set; }
        public bool isLinked { get; set; }
        public int ParentUserID { get; set; }
        public string OrganizationID { get; set; }
        public string ActionType { get; set; }
    }
}
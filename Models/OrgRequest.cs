using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ETradeAPI.Models
{
    public class OrgRequest : SecurityParams
    {
        public int? OrganizationRequestId { get; set; }
        public int? OrganizationId { get; set; }
        public string OrgEngName { get; set; }
        public string OrgAraName { get; set; }
        public string Description { get; set; }
        public string TradeLicNumber { get; set; }
        public string CivilId { get; set; }
        public string AuthPerson { get; set; }
        public string POBoxNo { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        public string CityBiling { get; set; }
        public string StateBiling { get; set; }

        public string PostalCode { get; set; }
        public string CountryId { get; set; }
        public string BusiNo { get; set; }
        public string BusiFaxNo { get; set; }
        public string MobileNo { get; set; }
        public string ResidenceNo { get; set; }
        public string EmailId { get; set; }
        public string WebPageAddress { get; set; }
        public bool? isIndustrial { get; set; }
        public bool IsmainCompany { get; set; }
        public bool IsbranchCompany { get; set; }

        public string CompanyType { get; set; }
        //public string mUserId { get; set; }
        //public string Operation { get; set; }


        public string Block { get; set; }//added newly OrgSvcNew Chnages 02-20-2019  -Siraj
        public string Street { get; set; }//added newly OrgSvcNew Chnages 02-20-2019  -Siraj
    }

    public class OrgRequestInd : SecurityParams
    {
        public int? OrganizationRequestId { get; set; }
        public string IndustrialLicenseNumber { get; set; }
        public string IssueDate { get; set; }
        public string ExpiryDate { get; set; }
        public string IndustrialRegistrationNumber { get; set; }
        public string IssuanceDate { get; set; }

    }
    public class OrgAuthorizedSignatory : SecurityParams
    {

        public string OrgAraName { get; set; }
        public Int64? OrgAuthorizedSignatoryId { get; set; }
        public Int64? OrganizationRequestId { get; set; }
        public Int64? OrganizationId { get; set; }
        public string AuthPerson { get; set; }
         public string CivilId { get; set; }
        public bool NewPerson { get; set; }
    }

    public class OrgRequestComm : SecurityParams
    {
        public int? OrganizationRequestId { get; set; }
        public string CommLicType { get; set; }
        public string CommLicSubType { get; set; }
        public string CommLicNo { get; set; }
        public string IssueDate { get; set; }
        public string ExpiryDate { get; set; }
     

    }

    public class OrgRequestImp : SecurityParams
    {
        public int? OrganizationRequestId { get; set; }
        public string ImpLicType { get; set; }
        public string ImpLicNo { get; set; }
        public string IssueDate { get; set; }
        public string ExpiryDate { get; set; }
        public string ImpLicNo1 { get; set; }
        public string ImpLicNo2 { get; set; }
    }
    
    public class ResponseData
    {
        public string data {get; set; }
    }

    public class OrgRequestWithId : SecurityParams
    {
        public int? OrganizationRequestId { get; set; }
    }

}
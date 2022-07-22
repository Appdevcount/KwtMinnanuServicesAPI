using System;
using System.Collections.Generic;

namespace ETradeAPI.Models
{
    public class EservicesRequests
    {
        public Int64 EserviceRequestId { get; set; }
        public String EserviceRequestNumber { get; set; }
        public Int64 RequesterUserId { get; set; }
        public Int64 ServiceId { get; set; }
        public String StateId { get; set; }
        public DateTime? DateCreated { get; set; }
        public String CreatedBy { get; set; }
        public String ModifiedBy { get; set; }
        public DateTime? DateModified { get; set; }
        public int OwnerOrgId { get; set; }
        public int OwnerLocId { get; set; }


        public List<EServiceRequestsDetails> RequestsDetails { get; set; }

    }


    public class EServiceRequestsDetails
    {
        public Int64 EserviceRequestDetailsId { get; set; }
        public Int64 EserviceRequestId { get; set; }
        public Int64 RequestForUserType { get; set; }
        public String RequestServicesId { get; set; }
        public int OrganizationId { get; set; }

        public int RequesterUserType { get; set; }
        public String RequesterLicenseNumber { get; set; }
        public String RequesterMobileNumber { get; set; }
        public String RequesterArabicName { get; set; }
        public String RequesterEnglishName { get; set; }

        public String ArabicFirstName { get; set; }
        public String ArabicSecondName { get; set; }
        public String ArabicThirdName { get; set; }
        public String ArabicLastName { get; set; }
        public String EnglishFirstName { get; set; }
        public String EnglishSecondName { get; set; }
        public String EnglishThirdName { get; set; }
        public String EnglishLastName { get; set; }
        public int Nationality { get; set; }
        public String CivilID { get; set; }
        public DateTime? CivilIDExpiryDate { get; set; }
        public String MobileNumber { get; set; }
        public String PassportNo { get; set; }
        public DateTime? PassportExpiryDate { get; set; }
        public String Address { get; set; }
        public String Email { get; set; }
        public String LicenseNumber { get; set; }
        public DateTime? LicenseNumberExpiryDate { get; set; }
        public String Remarks { get; set; }
        public String RejectionRemarks { get; set; }
        public Boolean RequestForVisit { get; set; }
        public String RequestForVisitRemarks { get; set; }
        public Int64 ExamAddmissionId { get; set; }
        public Int64 ExamDetailsId { get; set; }

        public String status { get; set; }
        public String StateId { get; set; }
        public DateTime? DateCreated { get; set; }
        public String CreatedBy { get; set; }
        public String ModifiedBy { get; set; }
        public DateTime? DateModified { get; set; }
        public int OwnerOrgId { get; set; }
        public int OwnerLocId { get; set; }

        public String Gender { get; set; }
    }



    public class examRequestViewModel : EservicesRequests
    {

        public String Referenceprofile { get; set; }
        public String lang { get; set; }
        public Boolean EXAMREQ { get; set; }
        public int Serviceid { get; set; }

        public String BrokerType { get; set; }


        //public List<SelectListItem> ddlDocumentTypesitems { get; set; }


        //public List<BrFileResult> ListOfUploadedFiles { get; set; }
        public String SelectedFileId { get; set; }
        public String docsid { get; set; }

        public String UploadDiv { get; set; }
        public String SubmitRequest { get; set; }

        public String organizationid { get; set; }

        // 27-FEB
        public Int64 requestForMobileUserId { get; set; }
        public int mobileUserId { get; set; }

    }



    public class RequestExists
    {
        public String RequestForUserType { get; set; }
        public String RequesterUserId { get; set; }
        public String ServiceId { get; set; }
    }


    public class RequestExistsByCivilId
    {
        
        public String CivilId { get; set; }
        public String ServiceId { get; set; }
        public String StateId { get; set; }
    }


    public class ExamCandidateInfo
    {
        public String EserviceRequestId { get; set; }
        public String stateId { get; set; }

    }

    #region Siraj G
    public class EserviceRequest
    {
        public string ESERVICEREQUESTNUMBER { get; set; }
        public int RequesterUserId { get; set; }
    }
    public class RequestList
    {
        public int EServiceRequestID { get; set; }
        public string EServiceRequestNumber { get; set; }
        public int? RequesterUserID { get; set; }
        public int? RequestForUserID { get; set; }
        public string Status { get; set; }
        public DateTime? DateCreated { get; set; }
    }
    public class RequestDetail
    {
        public int EServiceRequestID { get; set; }
        public int EServiceRequestDetailsID { get; set; }
        public string EServiceRequestNumber { get; set; }
        public string ServiceNameEng { get; set; }
        public string ServiceNameAra { get; set; }
        public int? RequesterUserID { get; set; }
        public int? RequestForUserID { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string Status { get; set; }
    }
    #endregion Siraj G
}
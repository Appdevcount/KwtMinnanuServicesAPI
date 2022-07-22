using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace ETradeAPI.Service_References.MociLicensesServiceControl
{
    public class MOCIOrgDetails
    {
        public bool CivilIdStatus { get; set; }
        public bool IsIndustrialLicenseValidNumber { get; set; }
        public bool IsIndustrialLicPassed { get; set; }
        public bool IsCommercialLicenseValidNumber { get; set; }
        public bool IsCommercialLicPassed { get; set; }
        public bool ValidfromMOCI { get; set; }//Ok to go
        public bool IssueinMOCIService { get; set; }//service exception
        public ETradeAPI.Service_References.CommercialLicensewithCivilId.Envelope OrgDetailswithCompanyCivilId { get; set; }
        public ETradeAPI.PAIProxyService.licenseDetail IndustrialDetailswithIndustrialLicenseNumber { get; set; }
    }

    public class MociLicensesServiceControl
    {
        MociLicensesService.MociLicensesServiceProxyWebServiceSoapClient svc1 = new MociLicensesService.MociLicensesServiceProxyWebServiceSoapClient();

        PAIProxyService.PAIProxyServiceSoapClient svc2 = new PAIProxyService.PAIProxyServiceSoapClient();

     
        public MOCIOrgDetails GetOrgDetailswithCompanyCivilId(string CivilID,string IndustrialLicense,string CommercialLicensenumber)
        {
            MOCIOrgDetails MOCIOrgDetails = new MOCIOrgDetails();
            MOCIOrgDetails.CivilIdStatus = false;
            MOCIOrgDetails.IsIndustrialLicPassed = !string.IsNullOrEmpty(IndustrialLicense);
            MOCIOrgDetails.IsCommercialLicPassed = !string.IsNullOrEmpty(CommercialLicensenumber);
            MOCIOrgDetails.IsIndustrialLicenseValidNumber = false;
            MOCIOrgDetails.IsCommercialLicenseValidNumber = false;
            MOCIOrgDetails.IssueinMOCIService = false;
            MOCIOrgDetails.ValidfromMOCI = false;
            IndustrialLicense = !MOCIOrgDetails.IsIndustrialLicPassed ? "" : IndustrialLicense;
            CommercialLicensenumber = !MOCIOrgDetails.IsCommercialLicPassed ? "" : CommercialLicensenumber;
            try
            {
                string response = svc1.InquireCommLicDetailsFromSvcV2(CivilID);
                ETradeAPI.Service_References.CommercialLicensewithCivilId.Envelope OrgDetailswithCompanyCivilId;
                using (TextReader sr = new StringReader(response))
                {
                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(ETradeAPI.Service_References.CommercialLicensewithCivilId.Envelope));
                    OrgDetailswithCompanyCivilId = (ETradeAPI.Service_References.CommercialLicensewithCivilId.Envelope)serializer.Deserialize(sr);
                }
                if (OrgDetailswithCompanyCivilId.Body.GetLicnByLicnCivilIDResponse.GetLicnByLicnCivilIDResult.Status == "1")
                {
                    MOCIOrgDetails.CivilIdStatus = true;

                    if (MOCIOrgDetails.IsCommercialLicPassed)
                    {
                        if(OrgDetailswithCompanyCivilId.Body.GetLicnByLicnCivilIDResponse.GetLicnByLicnCivilIDResult.licnData.licn_comm_no!= CommercialLicensenumber)
                        {
                            MOCIOrgDetails.IsCommercialLicenseValidNumber = false;
                            MOCIOrgDetails.ValidfromMOCI = false;
                            return MOCIOrgDetails;
                        }
                        else
                        {
                            MOCIOrgDetails.IsCommercialLicenseValidNumber = true;
                        }
                    }

                    if (MOCIOrgDetails.IsIndustrialLicPassed)
                    {
                        ETradeAPI.PAIProxyService.licenseDetail OrgIndustrialDetails = GetOrgIndustrialDetails(IndustrialLicense);
                        if (OrgIndustrialDetails!=null && !string.IsNullOrEmpty(OrgIndustrialDetails.applicantCivilId)){
                            MOCIOrgDetails.IsIndustrialLicenseValidNumber = true;
                            MOCIOrgDetails.ValidfromMOCI = true;
                        }
                        else
                        {
                            MOCIOrgDetails.IsIndustrialLicenseValidNumber = false;
                            MOCIOrgDetails.ValidfromMOCI = false;
                        }
                    }
                    else
                    {
                        MOCIOrgDetails.ValidfromMOCI = true;
                    }
                }

                CommonFunctions.LogUserActivity("MOCISvcSuccess : CompCivilId " + CivilID + " CommLic " + CommercialLicensenumber + " IndustLic " + IndustrialLicense + "||||| Service response CommLic = "+ OrgDetailswithCompanyCivilId.Body.GetLicnByLicnCivilIDResponse.GetLicnByLicnCivilIDResult.licnData.licn_comm_no+" TardeLic = "+ OrgDetailswithCompanyCivilId.Body.GetLicnByLicnCivilIDResponse.GetLicnByLicnCivilIDResult.licnData.comm_book_no , "", "", "", "", "ValidfromMOCI "+MOCIOrgDetails.ValidfromMOCI.ToString()+" CivilId " +MOCIOrgDetails.CivilIdStatus.ToString()+" CommLic "+MOCIOrgDetails.IsCommercialLicenseValidNumber+" Indust "+MOCIOrgDetails.IsIndustrialLicenseValidNumber.ToString());

                return MOCIOrgDetails;
            }
            catch(Exception e)
            {
                CommonFunctions.LogUserActivity("MOCISvcFailure : CompCivilId "+ CivilID+ " CommLic " + CommercialLicensenumber + " IndustLic " + IndustrialLicense, "","","","",  e.Message.ToString());
                MOCIOrgDetails.IssueinMOCIService = true;
                MOCIOrgDetails.ValidfromMOCI = false;
                return MOCIOrgDetails;
            }
        }
        public ETradeAPI.PAIProxyService.licenseDetail GetOrgIndustrialDetails(string LicenseNo)
        {
            ETradeAPI.PAIProxyService.licenseDetail response = svc2.GetPAILicenseDetail(LicenseNo);
            return response;
        }
    }
}


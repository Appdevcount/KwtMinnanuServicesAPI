﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ETradeAPI.Models;
using ETradeAPI.Helpers;
using System.Text.RegularExpressions;


namespace ETradeAPI.Controllers
{
    // [AllowAnonymous]
    // [RequireHttps]
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {


        //[Route("Checkpasword")]
        //[HttpPost]

        //public HttpResponseMessage Checkpasword([FromBody] Verifypassword VP)
        //{
        //    return new HttpResponseMessage()
        //    {

        //        Content=new StringContent(MobileDataBase.LogOnAction(VP.email,Crypt.EncryptData(VP.password),"en"),System.Text.Encoding.UTF8, "application/json")
        //    };

        //}GetRequestDetail
        //[Route("GetRequestDetail")]
        //[HttpPost]
        //public HttpResponseMessage GetRequestDetail([FromBody] BrokerRenewalModel data)
        //{
        //    return new HttpResponseMessage()
        //    {
        //        Content = new StringContent(MobileDataBase.GetRequestDetail(data), System.Text.Encoding.UTF8, "application/json")
        //    };
        //}





        [Route("GetRequestDetaillist")]
        [HttpPost]
        public HttpResponseMessage GetRequestDetaillist([FromBody] BrokerRenewalModel data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.GetRequestDetaillist(data), System.Text.Encoding.UTF8, "application/json")
            };
        }

        [Route("GetBrokerDetailslist")]
        [HttpPost]
        public HttpResponseMessage GetBrokerDetailslist([FromBody] BrokerRenewalModel data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.GetBrokerDetailslist(data), System.Text.Encoding.UTF8, "application/json")
            };
        }


        [Route("GetBrokerDetails")]
        [HttpPost]
        public HttpResponseMessage GetBrokerDetails([FromBody] BrokerUpdateModel data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.GetBrokerDetails(data.Organizationid, data), System.Text.Encoding.UTF8, "application/json")
            };
        }

        [Route("UpdateBrokerUserDetails")]
        [HttpPost]
        public HttpResponseMessage UpdateBrokerUserDetails([FromBody] BrokerUpdateModel BrokerData)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.UpdateBrokerUserDetails(BrokerData), System.Text.Encoding.UTF8, "application/json")
            };
        }

        [Route("GetEntityServiceList")]
        [HttpPost]
        public HttpResponseMessage GetEntityServiceList([FromBody] BrokerServiceRequestModel data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.GetEntityServiceList(data), System.Text.Encoding.UTF8, "application/json")
            };
        }

        [Route("PostEntityServiceList")]
        [HttpPost]
        public HttpResponseMessage PostEntityServiceList([FromBody] BrokerServiceRequestModel data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.PostEntityServiceList(data), System.Text.Encoding.UTF8, "application/json")
            };
        }
        //[Route("GetFNCheckAutheticationForBrokerRenwal")]
        ////[HttpPost]
        ////public HttpResponseMessage GetFNCheckAutheticationForBrokerRenwal([FromBody] BrokerRenewalModel BrokerData)
        ////{
        ////    return new HttpResponseMessage()
        ////    {
        ////        Content = new StringContent(MobileDataBase.GetFNCheckAutheticationForBrokerRenwal(BrokerData), System.Text.Encoding.UTF8, "application/json")
        ////    };
        ////}

 [Route("GetBrokerTypesList")]
        [HttpPost]
        public HttpResponseMessage GetBrokerTypesList([FromBody] BrokerUpdateModel data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.GetBrokerTypesList(data), System.Text.Encoding.UTF8, "application/json")
            };
        }


        [Route("LogOnAction")]
        [HttpPost]
        public HttpResponseMessage LogOnAction([FromBody]LogOnRequest data)
        {

            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.LogOnAction(data.email, Crypt.EncryptData(data.pwd), data.Lang), System.Text.Encoding.UTF8, "application/json")
            };
        }

        [Route("LogOutAction")]

        [HttpPost]
        public HttpResponseMessage LogOutAction([FromBody] SecurityParams data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.LogOutAction(data.tokenId, data.mUserid), System.Text.Encoding.UTF8, "application/json")
            };
        }

        [Route("UserDetails")]
        [HttpPost]
        public HttpResponseMessage UserDetails([FromBody] SecurityParams data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.UserDetails(data.tokenId, data.mUserid), System.Text.Encoding.UTF8, "application/json")
            };
        }

        [Route("UpdateUserDetails")]
        [HttpPost]
        public HttpResponseMessage UpdateUserDetails([FromBody] User UserData)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.UpdateUserDetails(UserData), System.Text.Encoding.UTF8, "application/json")
            };
        }

        [Route("SignUp")]
        [HttpPost]
        public HttpResponseMessage SignUp([FromBody] User UserData)
        {

            if (!ValidatePwd(UserData.Pass))
            {
                return new HttpResponseMessage()
                {
                    Content = new StringContent("{\"SignupResult\":[{ \"FirstName\":\"\", \"LastName\":\"\", \"TokenId\":\"\", \"UserId\":\"\", \"Status\":\"-9\", \"EmailKeyValTemporary\":\"\", \"TableName\":\"SignupResult\" }]}", System.Text.Encoding.UTF8, "application/json")
                };
            }
            //if (!MobileDataBase.IsCapthaValid(UserData.CaptchaId, UserData.CaptchaValue))
            //{
            //    return new HttpResponseMessage()
            //    {
            //        Content = new StringContent("{\"SignupResult\":[{ \"FirstName\":\"\", \"LastName\":\"\", \"TokenId\":\"\", \"UserId\":\"\", \"Status\":\"-8\", \"EmailKeyValTemporary\":\"\", \"TableName\":\"SignupResult\" }]}", System.Text.Encoding.UTF8, "application/json")
            //    };
            //}
            //else
            //{
            UserData.Pass = Crypt.EncryptData(UserData.Pass);
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.SignUp(UserData), System.Text.Encoding.UTF8, "application/json")
            };
            //}
        }
        [Route("GETParentUserActiveServices")]
        [HttpPost]
        public HttpResponseMessage GETParentUserActiveServices([FromBody] ReqObj R)//[FromUri]int ParentID)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.GETParentUserActiveServices(R.ParentID, R.Action,R.CommonData), System.Text.Encoding.UTF8, "application/json")
            };
        }
        //[Route("GETUserDetail")]
        //[HttpPost]
        //public HttpResponseMessage GETUserDetail([FromBody] ReqObj R)//[FromUri]int ParentID)
        //{
        //    return new HttpResponseMessage()
        //    {
        //        Content = new StringContent(MobileDataBase.GETUserDetail(R.ParentID), System.Text.Encoding.UTF8, "application/json")
        //    };
        //}
        [Route("CanCreateOrg")]
        [HttpPost]
        public HttpResponseMessage CanCreateOrg([FromBody] ReqObj R)//[FromUri]int ParentID)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.CanCreateOrg(R.ParentID), System.Text.Encoding.UTF8, "application/json")
            };
        }
        [Route("GETChildUsers")]
        [HttpPost]
        public HttpResponseMessage GETChildUsers([FromBody] ReqObj R)//[FromUri]int ParentID)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.GETChildUsers(R.ParentID), System.Text.Encoding.UTF8, "application/json")
            };
        }
        [Route("GETUserDetail")]
        [HttpPost]
        public HttpResponseMessage GETUserDetail([FromBody] ReqObj R)//[FromUri]int ParentID)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.GETUserDetail(R.ParentID), System.Text.Encoding.UTF8, "application/json")
            };
        }
        [Route("DeActivateChildUser")]
        [HttpPost]
        public HttpResponseMessage DeActivateChildUser([FromBody] ReqObj R)//[FromUri]int ParentID, [FromUri] int ChildUser)
        {
            return new HttpResponseMessage()
            {
                //Content = new StringContent(MobileDataBase.DeActivateChildUser(R.ParentID, R.ChildUser), System.Text.Encoding.UTF8, "application/json")
                Content = new StringContent(MobileDataBase.DeActivateChildUser( R.ChildUser,R.Action), System.Text.Encoding.UTF8, "application/json")
            };
        }
        [Route("ServicesAndOrgManagementFortheUser")]
        [HttpPost]
        public HttpResponseMessage ServicesAndOrgManagementFortheUser([FromBody] ServicesAndOrgManagementFortheUser R)//[FromUri]int ParentID, [FromUri] int ChildUser)
        {
            return new HttpResponseMessage()
            {
                //Content = new StringContent(MobileDataBase.DeActivateChildUser(R.ParentID, R.ChildUser), System.Text.Encoding.UTF8, "application/json")
                Content = new StringContent(MobileDataBase.ServicesAndOrgManagementFortheUser(R.UserID, R.ServiceID, R.SubscriptionID, R.isLinked,
            R.ParentUserID, R.OrganizationID, R.ActionType), System.Text.Encoding.UTF8, "application/json")
            };
        }
        [Route("GETAdminUserOrganization")]
        [HttpPost]
        public HttpResponseMessage GETAdminUserOrganization([FromBody] ReqObj R)//[FromUri]int ParentID, [FromUri] int ChildUser)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.GETAdminUserOrganization(R.ParentID), System.Text.Encoding.UTF8, "application/json")
            };
        }
        private bool ValidatePwd(string pass)
        {
            if (Regex.IsMatch(pass, "\\d")// Numeric
                && Regex.IsMatch(pass, "[a-z]")// Small Letter
                && Regex.IsMatch(pass, "[A-Z]")// Capital Letter
                && Regex.IsMatch(pass, @"[!@#$%^&*()_+=\[{\]};:<>|./?,-]")   //special character @"[!@#$%^&*()_+=\[{\]};:<>|./?,-]"
                && pass.Trim().ToString().Length > 7) // 8 Chars Minimum (excluding trailing space)
            {
                return true;
            }
            return false;
        }

        //[Route("VerifyOTP/{tokenId}/{mUserid}")]
        [Route("VerifyOTP")]

        [HttpPost]
        public HttpResponseMessage VerifyOTP([FromBody] VerifyOTPParams data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.VerifyOTP(data.Email, data.Mobile, data.tokenId, data.mUserid), System.Text.Encoding.UTF8, "application/json")
            };
        }

        //[Route("ReSendOTP/{tokenId}/{mUserid}/{rType}")]
        [Route("ReSendOTP")]

        [HttpPost]
        public HttpResponseMessage ReSendOTP([FromBody] ReSendOTPParams data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.ReSendOTP(data.rType, data.tokenId, data.mUserid), System.Text.Encoding.UTF8, "application/json")
            };
        }

        [Route("ValidateContactsWithOTP")]
        [HttpPost]
        public HttpResponseMessage ValidateContactsWithOTP([FromBody] ValidateContacts data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.ValidateContactsWithOTP(data), System.Text.Encoding.UTF8, "application/json")
            };
        }

        [Route("ForgotPwdOTP")]

        [HttpPost]
        public HttpResponseMessage ForgotPwdOTP([FromBody]ForgotPassWordInput data)
        {
            //if (!MobileDataBase.IsCapthaValid(data.CaptchaId, data.CaptchaValue))
            //{
            //    return new HttpResponseMessage()
            //    {
            //        Content = new StringContent("{\"ForgotPwdOTPResult\":[{ \"Status\":\"-8\",\"mUserId\":\"\", \"OTPKeyId\":\"\", \"OTPKeyVal\":\"\", \"Firstname\":\"\", \"Lastname\":\"\", \"Email\":\"\", \"TableName\":\"ForgotPwdOTPResult\" }]}", System.Text.Encoding.UTF8, "application/json")
            //    };
            //}
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.ForgotPwdOTP(data.emailId, data.mobileNo), System.Text.Encoding.UTF8, "application/json")
            };
        }

        [Route("ResetPwdByOTP")]
        [HttpPost]
        public HttpResponseMessage ResetPwdByOTP([FromBody] ResetPwdByOTPParams data)
        {
            if (!ValidatePwd(data.newPwd))
            {
                return new HttpResponseMessage()
                {
                    Content = new StringContent("{\"ResetPwdResult\":[{\"UserId\":\"\", \"Status\":\"-9\", \"TableName\":\"ResetPwdResult\" }]}", System.Text.Encoding.UTF8, "application/json")
                };
            }

            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.ResetPwdByOTP(data.otpId, data.otpValue, data.mUserid, Crypt.EncryptData(data.newPwd)), System.Text.Encoding.UTF8, "application/json")
            };

        }

        [Route("GetCaptcha")]
        [HttpPost]
        public HttpResponseMessage GetCaptcha([FromBody] Captcha data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.GetCaptcha(data.CaptchaId), System.Text.Encoding.UTF8, "application/json")
            };
        }

        [Route("ValidateUserToken")]

        [HttpPost]
        public HttpResponseMessage ValidateUserToken([FromBody] SecurityParams data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.ValidateUserToken(data.tokenId, data.mUserid), System.Text.Encoding.UTF8, "application/json")
            };
        }
        [Route("ChangeUserSession")]

        [HttpPost]
        public HttpResponseMessage UpdateUserSession([FromBody] UserSession data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.UpdateUserSession(data), System.Text.Encoding.UTF8, "application/json")
            };
        }
    }
}

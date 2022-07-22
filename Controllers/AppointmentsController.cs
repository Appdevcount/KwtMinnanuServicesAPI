using ETradeAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ETradeAPI.Controllers
{
    [RoutePrefix("api/Appointments")]
    public class AppointmentsController : ApiController
    {
      
        [Route("CreateInspectionAppointment")]
        [HttpPost]
        public HttpResponseMessage CreateInspectionAppointment([FromBody] InspectionAppointment data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.CreateInspectionAppointment(data), System.Text.Encoding.UTF8, "application/json")
            };
        }
        [Route("UpdateInspectionAppointment")]
        [HttpPost]
        public HttpResponseMessage UpdateInspectionAppointment([FromBody] InspectionAppointment data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.UpdateInspectionAppointment(data), System.Text.Encoding.UTF8, "application/json")
            };
        }
          [Route("GetInspectionAppointmentDetails")]
        [HttpPost]
        public HttpResponseMessage GetInspectionAppointmentDetails([FromBody] ReqObj data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.GetInspectionAppointmentDetails(data), System.Text.Encoding.UTF8, "application/json")
            };
        }
        [Route("CancelInspectionAppointment")]
        [HttpPost]
        public HttpResponseMessage CancelInspectionAppointment([FromBody] ReqObj data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.CancelInspectionAppointment(data), System.Text.Encoding.UTF8, "application/json")
            };
        }
        [Route("GetInspectionRounds")]
        [HttpPost]
        public HttpResponseMessage GetInspectionRounds([FromBody] ReqObj data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.GetInspectionRounds(data), System.Text.Encoding.UTF8, "application/json")
            };
        }
        
           [Route("GetInspectionAppointments")]
        [HttpPost]
        public HttpResponseMessage GetInspectionAppointments([FromBody] ReqObj data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.GetInspectionAppointments(data), System.Text.Encoding.UTF8, "application/json")
            };
        }
    [Route("getVehicleListFromDO")]
        [HttpPost]
        public HttpResponseMessage getVehicleListFromDO([FromBody] ReqObj data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.getVehicleListFromDO(data), System.Text.Encoding.UTF8, "application/json")
            };
        }
       

    }
}

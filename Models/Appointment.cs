using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ETradeAPI.Models
{

    public class InspectionAppointment
    {

        public string AppointmentId { get; set; }
        //[Required(ErrorMessage ="DO Number is required")]
        public string DONumber { get; set; }
        //[Required(ErrorMessage = "Security Code is required")]
        public string SecurityCode { get; set; }
        //public string PortID { get; set; }
        public string PortName { get; set; }

        //[Required(ErrorMessage = "Atleast One vehicle should be while creating inspection schedule")]
        public string SelectedVehicleList { get; set; }

        public List<VehicleList> VehicleFullList { get; set; }
        public List<VehicleList> SelectedVehicleDetails { get; set; }

        //[Required(ErrorMessage = "Inspection Date is required")]
        public string InspectionDate { get; set; }
        //[Required]
        public List<InspectionRounds> InspectionRounds { get; set; }
        //[Required(ErrorMessage = "Inspection Date is required")]
        //[Range(1, Int64.MaxValue, ErrorMessage = "Inspection Date is required")]
        public string InspectionRound { get; set; }


        //==============
        //public string RequestId { get; set; }
        public string RequestNumber { get; set; }
        //public string Requestertype { get; set; }
        public string Requester { get; set; }
        public string DeclarationId { get; set; }
        //public string DONumber { get; set; }
        //public List<VehicleList> SelectedVehicleDetails { get; set; }
        //public string InspectionDate { get; set; }
        //public string RoundId { get; set; }
        public string RoundName { get; set; }//
        public string Status { get; set; }
        public bool UpdateRequest { get; set; }
        public bool EditableRequest { get; set; }

        //===================
        public string UserId { get; set; }
        public string OrgId { get; set; }

        //[Required(ErrorMessage = "Port Id is required")]
        //[Range(1, Int64.MaxValue, ErrorMessage = "Port Id is required")]
        public string PortId { get; set; }

        public string RequesterType { get; set; }
        public string RequestSubmissionDateTime { get; set; }
        public string DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public string OwnerOrgId { get; set; }
        public string OwnerLocId { get; set; }

        //==========

        public string DeclarationType { get; set; }
        //[Required(ErrorMessage = "Inspection Zone is required")]
        //[Range(1, Int64.MaxValue, ErrorMessage = "Inspection Zone is required")]
        public string InspectionZone { get; set; }
        //[Required(ErrorMessage = "Inspection Terminal is required")]
        //[Range(1, Int64.MaxValue, ErrorMessage = "Inspection Terminal is required")]
        public string InspectionTerminal { get; set; }


        public string TempDeclarationId { get; set; }


        public string StatusNumber { get; set; }

        public List<InspectionZone> InsZone { get; set; }
        public List<InspectionTerminal> InsTerminal { get; set; }
        public List<InspectionRounds> InsRound { get; set; }

    }

    public class InspectionRounds
    {
        public string Port { get; set; }
        public string PortName { get; set; }
        public string RoundId { get; set; }
        public string RoundName { get; set; }
        public string Timing { get; set; }
        public int Capacity { get; set; }
        public bool Active { get; set; }
        public bool Availability { get; set; }

        public string DeclType { get; set; }
        public string RecieverDeliver { get; set; }
        public string TimeFrom { get; set; }
        public string TimeTo { get; set; }
        public string Selected { get; set; }
    }
    public class InspectionZone
    {
        public string ZoneId { get; set; }
        public string ZoneName { get; set; }
    }
    public class InspectionTerminal
    {
        public string TerminalId { get; set; }
        public string TerminalName { get; set; }
    }

    public class VehicleList
    {
        public string VehicleID { get; set; }
        public string VehiclePlateNumber { get; set; }
        public string VehicleType { get; set; }
        public string ContainerNumber { get; set; }
        public string DriverName { get; set; }
        public string PortId { get; set; }
        public string PortName { get; set; }
        public string DONumber { get; set; }
        public string DeclarationId { get; set; }



        public string DeclarationType { get; set; }
    }
}
using System;
using System.Configuration;

namespace KnetPayment
{
    public class KnetVariables //: e24PaymentPipeLib.e24PaymentPipeCtlClass
    {
        public String TranStatus { get; set; }
        public Int64 ReferenceId { get; set; }
        public DateTime TranSttDateTime { get; set; }
        public DateTime TranStpDateTime { get; set; }
        public String ClientIPAddress { get; set; }
        public String SessionId { get; set; }
        public Int32 LogInPortId { get; set; }
        public String PortalLoginId { get; set; }
        public Decimal Amount { get; set; }
        public String OrganizationId { get; set; }
        public String PaymentFor { get; set; }
        public Char PaidByType { get; set; }
        public String ReferenceNumber { get; set; }
        public String ReferenceType { get; set; }
        public Int32 CheckId { get; set; }
        public String response { get; set; }
        public String PostDate { get; set; }
        public String RefByBank { get; set; }
        public String TrackId { get; set; }
        public String ErrorMsg { get; set; }
        public String Result { get; set; }
        public String Auth { get; set; }
        public Int64 TransId { get; set; }

        private String Action { get; set; }
        private String Currency { get; set; }
        private String Language { get; set; }
        private String ResponseUrl { get; set; }
        private String ErrorUrl { get; set; }
        private String ResourcePath { get; set; }
        private String Alias { get; set; }
        public String PaymentId { get; set; }
        public String ReceiptId { get; set; }
        public String OLTransId { get; set; }

       

        public KnetVariables()
        {

            Action = "1";
            Currency = "414";
            Language = "USA"; // ARA
            //ResponseUrl = "https://paymentstest.gcskw.com/NewResponse.aspx";
            //ErrorUrl = "https://paymentstest.gcskw.com/internalError.aspx";
            ResponseUrl = ConfigurationManager.AppSettings["responseUrl"].ToString();
            ErrorUrl = ConfigurationManager.AppSettings["errorUrl"].ToString();


            ResourcePath = @"C:\GCSKnetDLL\";
            Alias = "gcs"; // Alias of the plug-in
            //Udf1 = "User Defined Field 1";
            //Udf2 = "User Defined Field 2";
            //Udf3 = "User Defined Field 3";
            //Udf4 = "User Defined Field 4";
            //Udf5 = "User Defined Field 5";
            //TransId = (new Random().Next(10000000) + 1).ToString();
            //ErrorMsg Read Only

        }
    }

   
}





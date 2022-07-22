using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace ETradeAPI.Service_References.CommercialLicensewithCivilId
{
    //using System;
    //using System.Web.Services;
    //using System.Diagnostics;
    //using System.Web.Services.Protocols;
    //using System.Xml.Serialization;
    //using System.ComponentModel;


    /// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3062.0")]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    ////[System.Web.Services.WebServiceBindingAttribute(Name = "ServiceSoap", Namespace = "http://tempuri.org/")]
    //public partial class Service //: System.Web.Services.Protocols.SoapHttpClientProtocol
    //{

    //    private System.Threading.SendOrPostCallback GetLicnByLicnCivilIDOperationCompleted;

    //    private System.Threading.SendOrPostCallback GetImportLicnOperationCompleted;

    //    private bool useDefaultCredentialsSetExplicitly;

    //    /// <remarks/>
    //    public Service()
    //    {
    //        this.Url = global::ConsoleApplication1.Properties.Settings.Default.ConsoleApplication1_WebReference_Service;
    //        if ((this.IsLocalFileSystemWebService(this.Url) == true))
    //        {
    //            this.UseDefaultCredentials = true;
    //            this.useDefaultCredentialsSetExplicitly = false;
    //        }
    //        else
    //        {
    //            this.useDefaultCredentialsSetExplicitly = true;
    //        }
    //    }

    //    public new string Url
    //    {
    //        get
    //        {
    //            return base.Url;
    //        }
    //        set
    //        {
    //            if ((((this.IsLocalFileSystemWebService(base.Url) == true)
    //                        && (this.useDefaultCredentialsSetExplicitly == false))
    //                        && (this.IsLocalFileSystemWebService(value) == false)))
    //            {
    //                base.UseDefaultCredentials = false;
    //            }
    //            base.Url = value;
    //        }
    //    }

    //    public new bool UseDefaultCredentials
    //    {
    //        get
    //        {
    //            return base.UseDefaultCredentials;
    //        }
    //        set
    //        {
    //            base.UseDefaultCredentials = value;
    //            this.useDefaultCredentialsSetExplicitly = true;
    //        }
    //    }

    //    /// <remarks/>
    //    public event GetLicnByLicnCivilIDCompletedEventHandler GetLicnByLicnCivilIDCompleted;

    //    /// <remarks/>
    //    public event GetImportLicnCompletedEventHandler GetImportLicnCompleted;

    //    /// <remarks/>
    //    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetLicnByLicnCivilID", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    //    public Licn GetLicnByLicnCivilID(string LicnCivilID, string UserName, string Password)
    //    {
    //        object[] results = this.Invoke("GetLicnByLicnCivilID", new object[] {
    //                    LicnCivilID,
    //                    UserName,
    //                    Password});
    //        return ((Licn)(results[0]));
    //    }

    //    /// <remarks/>
    //    public void GetLicnByLicnCivilIDAsync(string LicnCivilID, string UserName, string Password)
    //    {
    //        this.GetLicnByLicnCivilIDAsync(LicnCivilID, UserName, Password, null);
    //    }

    //    /// <remarks/>
    //    public void GetLicnByLicnCivilIDAsync(string LicnCivilID, string UserName, string Password, object userState)
    //    {
    //        if ((this.GetLicnByLicnCivilIDOperationCompleted == null))
    //        {
    //            this.GetLicnByLicnCivilIDOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetLicnByLicnCivilIDOperationCompleted);
    //        }
    //        this.InvokeAsync("GetLicnByLicnCivilID", new object[] {
    //                    LicnCivilID,
    //                    UserName,
    //                    Password}, this.GetLicnByLicnCivilIDOperationCompleted, userState);
    //    }

    //    private void OnGetLicnByLicnCivilIDOperationCompleted(object arg)
    //    {
    //        if ((this.GetLicnByLicnCivilIDCompleted != null))
    //        {
    //            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
    //            this.GetLicnByLicnCivilIDCompleted(this, new GetLicnByLicnCivilIDCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetImportLicn", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    //    public ImportLicn GetImportLicn(string ImportNumber, string UserName, string Password)
    //    {
    //        object[] results = this.Invoke("GetImportLicn", new object[] {
    //                    ImportNumber,
    //                    UserName,
    //                    Password});
    //        return ((ImportLicn)(results[0]));
    //    }

    //    /// <remarks/>
    //    public void GetImportLicnAsync(string ImportNumber, string UserName, string Password)
    //    {
    //        this.GetImportLicnAsync(ImportNumber, UserName, Password, null);
    //    }

    //    /// <remarks/>
    //    public void GetImportLicnAsync(string ImportNumber, string UserName, string Password, object userState)
    //    {
    //        if ((this.GetImportLicnOperationCompleted == null))
    //        {
    //            this.GetImportLicnOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetImportLicnOperationCompleted);
    //        }
    //        this.InvokeAsync("GetImportLicn", new object[] {
    //                    ImportNumber,
    //                    UserName,
    //                    Password}, this.GetImportLicnOperationCompleted, userState);
    //    }

    //    private void OnGetImportLicnOperationCompleted(object arg)
    //    {
    //        if ((this.GetImportLicnCompleted != null))
    //        {
    //            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
    //            this.GetImportLicnCompleted(this, new GetImportLicnCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
    //        }
    //    }

    //    /// <remarks/>
    //    public new void CancelAsync(object userState)
    //    {
    //        base.CancelAsync(userState);
    //    }

    //    private bool IsLocalFileSystemWebService(string url)
    //    {
    //        if (((url == null)
    //                    || (url == string.Empty)))
    //        {
    //            return false;
    //        }
    //        System.Uri wsUri = new System.Uri(url);
    //        if (((wsUri.Port >= 1024)
    //                    && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0)))
    //        {
    //            return true;
    //        }
    //        return false;
    //    }
    //}

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3062.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://tempuri.org/")]
    public partial class Licn
    {

        private LicnData licnDataField;

        private LicnOwners[] licnOwnersField;

        private LicnActivity[] licnActivityField;

        private LicnExtraAddress[] licnExtraAddressField;

        private ImportLicn[] importLicnField;

        private string statusField;

        private string statusDescField;

        /// <remarks/>
        public LicnData licnData
        {
            get
            {
                return this.licnDataField;
            }
            set
            {
                this.licnDataField = value;
            }
        }

        /// <remarks/>
        public LicnOwners[] licnOwners
        {
            get
            {
                return this.licnOwnersField;
            }
            set
            {
                this.licnOwnersField = value;
            }
        }

        /// <remarks/>
        public LicnActivity[] licnActivity
        {
            get
            {
                return this.licnActivityField;
            }
            set
            {
                this.licnActivityField = value;
            }
        }

        /// <remarks/>
        public LicnExtraAddress[] LicnExtraAddress
        {
            get
            {
                return this.licnExtraAddressField;
            }
            set
            {
                this.licnExtraAddressField = value;
            }
        }

        /// <remarks/>
        public ImportLicn[] importLicn
        {
            get
            {
                return this.importLicnField;
            }
            set
            {
                this.importLicnField = value;
            }
        }

        /// <remarks/>
        public string Status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }

        /// <remarks/>
        public string StatusDesc
        {
            get
            {
                return this.statusDescField;
            }
            set
            {
                this.statusDescField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3062.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://tempuri.org/")]
    public partial class LicnData
    {

        private string licn_comm_noField;

        private string company_nameField;

        private string licn_civil_idField;

        private string active_purposeField;

        private string active_purpose_codeField;

        private string licn_file_enter_noField;

        private string address_auto_noField;

        private string comm_book_noField;

        private string civil_idField;

        private string emp_nameField;

        private string company_typeField;

        private string licn_targetField;

        private string licn_statusField;

        private string trade_nameField;

        private string licn_comm_sdateField;

        private string licn_comm_edateField;

        private string main_companyField;

        private string total_share_noField;

        private string total_capitalField;

        private string linc_typeField;

        private string rEG_COMM_DATEField;

        private string cONT_APRV_DATEField;

        private string cONT_NOField;

        private string new_licn_flagField;

        private string is_main_licnField;

        private string notesField;

        /// <remarks/>
        public string licn_comm_no
        {
            get
            {
                return this.licn_comm_noField;
            }
            set
            {
                this.licn_comm_noField = value;
            }
        }

        /// <remarks/>
        public string company_name
        {
            get
            {
                return this.company_nameField;
            }
            set
            {
                this.company_nameField = value;
            }
        }

        /// <remarks/>
        public string licn_civil_id
        {
            get
            {
                return this.licn_civil_idField;
            }
            set
            {
                this.licn_civil_idField = value;
            }
        }

        /// <remarks/>
        public string active_purpose
        {
            get
            {
                return this.active_purposeField;
            }
            set
            {
                this.active_purposeField = value;
            }
        }

        /// <remarks/>
        public string active_purpose_code
        {
            get
            {
                return this.active_purpose_codeField;
            }
            set
            {
                this.active_purpose_codeField = value;
            }
        }

        /// <remarks/>
        public string licn_file_enter_no
        {
            get
            {
                return this.licn_file_enter_noField;
            }
            set
            {
                this.licn_file_enter_noField = value;
            }
        }

        /// <remarks/>
        public string address_auto_no
        {
            get
            {
                return this.address_auto_noField;
            }
            set
            {
                this.address_auto_noField = value;
            }
        }

        /// <remarks/>
        public string comm_book_no
        {
            get
            {
                return this.comm_book_noField;
            }
            set
            {
                this.comm_book_noField = value;
            }
        }

        /// <remarks/>
        public string civil_id
        {
            get
            {
                return this.civil_idField;
            }
            set
            {
                this.civil_idField = value;
            }
        }

        /// <remarks/>
        public string emp_name
        {
            get
            {
                return this.emp_nameField;
            }
            set
            {
                this.emp_nameField = value;
            }
        }

        /// <remarks/>
        public string company_type
        {
            get
            {
                return this.company_typeField;
            }
            set
            {
                this.company_typeField = value;
            }
        }

        /// <remarks/>
        public string licn_target
        {
            get
            {
                return this.licn_targetField;
            }
            set
            {
                this.licn_targetField = value;
            }
        }

        /// <remarks/>
        public string licn_status
        {
            get
            {
                return this.licn_statusField;
            }
            set
            {
                this.licn_statusField = value;
            }
        }

        /// <remarks/>
        public string trade_name
        {
            get
            {
                return this.trade_nameField;
            }
            set
            {
                this.trade_nameField = value;
            }
        }

        /// <remarks/>
        public string licn_comm_sdate
        {
            get
            {
                return this.licn_comm_sdateField;
            }
            set
            {
                this.licn_comm_sdateField = value;
            }
        }

        /// <remarks/>
        public string licn_comm_edate
        {
            get
            {
                return this.licn_comm_edateField;
            }
            set
            {
                this.licn_comm_edateField = value;
            }
        }

        /// <remarks/>
        public string main_company
        {
            get
            {
                return this.main_companyField;
            }
            set
            {
                this.main_companyField = value;
            }
        }

        /// <remarks/>
        public string total_share_no
        {
            get
            {
                return this.total_share_noField;
            }
            set
            {
                this.total_share_noField = value;
            }
        }

        /// <remarks/>
        public string total_capital
        {
            get
            {
                return this.total_capitalField;
            }
            set
            {
                this.total_capitalField = value;
            }
        }

        /// <remarks/>
        public string linc_type
        {
            get
            {
                return this.linc_typeField;
            }
            set
            {
                this.linc_typeField = value;
            }
        }

        /// <remarks/>
        public string REG_COMM_DATE
        {
            get
            {
                return this.rEG_COMM_DATEField;
            }
            set
            {
                this.rEG_COMM_DATEField = value;
            }
        }

        /// <remarks/>
        public string CONT_APRV_DATE
        {
            get
            {
                return this.cONT_APRV_DATEField;
            }
            set
            {
                this.cONT_APRV_DATEField = value;
            }
        }

        /// <remarks/>
        public string CONT_NO
        {
            get
            {
                return this.cONT_NOField;
            }
            set
            {
                this.cONT_NOField = value;
            }
        }

        /// <remarks/>
        public string new_licn_flag
        {
            get
            {
                return this.new_licn_flagField;
            }
            set
            {
                this.new_licn_flagField = value;
            }
        }

        /// <remarks/>
        public string is_main_licn
        {
            get
            {
                return this.is_main_licnField;
            }
            set
            {
                this.is_main_licnField = value;
            }
        }

        /// <remarks/>
        public string notes
        {
            get
            {
                return this.notesField;
            }
            set
            {
                this.notesField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3062.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://tempuri.org/")]
    public partial class ImportLicn
    {

        private string iMPORT_NUMBERField;

        private string iMPORT_SDATEField;

        private string iMPORT_EDATEField;

        private string lICN_CIVIL_IDField;

        private string iMPORT_TYPE_CODEField;

        private string iMPORT_TYPE_DESCField;

        private string statusField;

        private string statusDescField;

        /// <remarks/>
        public string IMPORT_NUMBER
        {
            get
            {
                return this.iMPORT_NUMBERField;
            }
            set
            {
                this.iMPORT_NUMBERField = value;
            }
        }

        /// <remarks/>
        public string IMPORT_SDATE
        {
            get
            {
                return this.iMPORT_SDATEField;
            }
            set
            {
                this.iMPORT_SDATEField = value;
            }
        }

        /// <remarks/>
        public string IMPORT_EDATE
        {
            get
            {
                return this.iMPORT_EDATEField;
            }
            set
            {
                this.iMPORT_EDATEField = value;
            }
        }

        /// <remarks/>
        public string LICN_CIVIL_ID
        {
            get
            {
                return this.lICN_CIVIL_IDField;
            }
            set
            {
                this.lICN_CIVIL_IDField = value;
            }
        }

        /// <remarks/>
        public string IMPORT_TYPE_CODE
        {
            get
            {
                return this.iMPORT_TYPE_CODEField;
            }
            set
            {
                this.iMPORT_TYPE_CODEField = value;
            }
        }

        /// <remarks/>
        public string IMPORT_TYPE_DESC
        {
            get
            {
                return this.iMPORT_TYPE_DESCField;
            }
            set
            {
                this.iMPORT_TYPE_DESCField = value;
            }
        }

        /// <remarks/>
        public string Status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }

        /// <remarks/>
        public string StatusDesc
        {
            get
            {
                return this.statusDescField;
            }
            set
            {
                this.statusDescField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3062.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://tempuri.org/")]
    public partial class LicnExtraAddress
    {

        private string address_auto_noField;

        private string uNIT_TYPEField;

        private string dISTRICTField;

        private string wV_DISTRICT_TEXTField;

        private string fLOOR_NOField;

        private string wV_BLOCKField;

        private string wV_PLOT_NOField;

        private string wV_STREETField;

        private string wV_UNIT_NOField;

        private string wV_UNIT_TYPE_TEXTField;

        private string wV_BLDG_NAMEField;

        private string wV_GOV_CODEField;

        /// <remarks/>
        public string address_auto_no
        {
            get
            {
                return this.address_auto_noField;
            }
            set
            {
                this.address_auto_noField = value;
            }
        }

        /// <remarks/>
        public string UNIT_TYPE
        {
            get
            {
                return this.uNIT_TYPEField;
            }
            set
            {
                this.uNIT_TYPEField = value;
            }
        }

        /// <remarks/>
        public string DISTRICT
        {
            get
            {
                return this.dISTRICTField;
            }
            set
            {
                this.dISTRICTField = value;
            }
        }

        /// <remarks/>
        public string WV_DISTRICT_TEXT
        {
            get
            {
                return this.wV_DISTRICT_TEXTField;
            }
            set
            {
                this.wV_DISTRICT_TEXTField = value;
            }
        }

        /// <remarks/>
        public string FLOOR_NO
        {
            get
            {
                return this.fLOOR_NOField;
            }
            set
            {
                this.fLOOR_NOField = value;
            }
        }

        /// <remarks/>
        public string WV_BLOCK
        {
            get
            {
                return this.wV_BLOCKField;
            }
            set
            {
                this.wV_BLOCKField = value;
            }
        }

        /// <remarks/>
        public string WV_PLOT_NO
        {
            get
            {
                return this.wV_PLOT_NOField;
            }
            set
            {
                this.wV_PLOT_NOField = value;
            }
        }

        /// <remarks/>
        public string WV_STREET
        {
            get
            {
                return this.wV_STREETField;
            }
            set
            {
                this.wV_STREETField = value;
            }
        }

        /// <remarks/>
        public string WV_UNIT_NO
        {
            get
            {
                return this.wV_UNIT_NOField;
            }
            set
            {
                this.wV_UNIT_NOField = value;
            }
        }

        /// <remarks/>
        public string WV_UNIT_TYPE_TEXT
        {
            get
            {
                return this.wV_UNIT_TYPE_TEXTField;
            }
            set
            {
                this.wV_UNIT_TYPE_TEXTField = value;
            }
        }

        /// <remarks/>
        public string WV_BLDG_NAME
        {
            get
            {
                return this.wV_BLDG_NAMEField;
            }
            set
            {
                this.wV_BLDG_NAMEField = value;
            }
        }

        /// <remarks/>
        public string WV_GOV_CODE
        {
            get
            {
                return this.wV_GOV_CODEField;
            }
            set
            {
                this.wV_GOV_CODEField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3062.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://tempuri.org/")]
    public partial class LicnActivity
    {

        private string active_purposeField;

        private string active_purpose_codeField;

        /// <remarks/>
        public string active_purpose
        {
            get
            {
                return this.active_purposeField;
            }
            set
            {
                this.active_purposeField = value;
            }
        }

        /// <remarks/>
        public string active_purpose_code
        {
            get
            {
                return this.active_purpose_codeField;
            }
            set
            {
                this.active_purpose_codeField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3062.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://tempuri.org/")]
    public partial class LicnOwners
    {

        private string owner_civil_idField;

        private string nameField;

        private string partener_shareField;

        private string partener_percentField;

        private string owner_statusField;

        /// <remarks/>
        public string owner_civil_id
        {
            get
            {
                return this.owner_civil_idField;
            }
            set
            {
                this.owner_civil_idField = value;
            }
        }

        /// <remarks/>
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public string partener_share
        {
            get
            {
                return this.partener_shareField;
            }
            set
            {
                this.partener_shareField = value;
            }
        }

        /// <remarks/>
        public string partener_percent
        {
            get
            {
                return this.partener_percentField;
            }
            set
            {
                this.partener_percentField = value;
            }
        }

        /// <remarks/>
        public string owner_status
        {
            get
            {
                return this.owner_statusField;
            }
            set
            {
                this.owner_statusField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3062.0")]
    public delegate void GetLicnByLicnCivilIDCompletedEventHandler(object sender, GetLicnByLicnCivilIDCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3062.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetLicnByLicnCivilIDCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal GetLicnByLicnCivilIDCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public Licn Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((Licn)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3062.0")]
    public delegate void GetImportLicnCompletedEventHandler(object sender, GetImportLicnCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3062.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetImportLicnCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal GetImportLicnCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public ImportLicn Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((ImportLicn)(this.results[0]));
            }
        }
    }


    //================ ADDED SEPARATELY

    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
    public partial class Envelope
    {

        private EnvelopeBody bodyField;

        /// <remarks/>
        public EnvelopeBody Body
        {
            get
            {
                return this.bodyField;
            }
            set
            {
                this.bodyField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public partial class EnvelopeBody
    {

        private GetLicnByLicnCivilIDResponse getLicnByLicnCivilIDResponseField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://tempuri.org/")]
        public GetLicnByLicnCivilIDResponse GetLicnByLicnCivilIDResponse
        {
            get
            {
                return this.getLicnByLicnCivilIDResponseField;
            }
            set
            {
                this.getLicnByLicnCivilIDResponseField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://tempuri.org/", IsNullable = false)]
    public partial class GetLicnByLicnCivilIDResponse
    {

        private Licn getLicnByLicnCivilIDResultField;

        /// <remarks/>
        public Licn GetLicnByLicnCivilIDResult
        {
            get
            {
                return this.getLicnByLicnCivilIDResultField;
            }
            set
            {
                this.getLicnByLicnCivilIDResultField = value;
            }
        }
    }


}



//namespace ETradeAPI.Service_References.CommercialLicensewithCivilId
//{
//    //=======================Commerciallicensce with civilid==========================

//    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
//    /// <remarks/>
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
//    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
//    public partial class Envelope
//    {

//        private EnvelopeBody bodyField;

//        /// <remarks/>
//        public EnvelopeBody Body
//        {
//            get
//            {
//                return this.bodyField;
//            }
//            set
//            {
//                this.bodyField = value;
//            }
//        }
//    }

//    /// <remarks/>
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
//    public partial class EnvelopeBody
//    {

//        private GetLicnByLicnCivilIDResponse getLicnByLicnCivilIDResponseField;

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://tempuri.org/")]
//        public GetLicnByLicnCivilIDResponse GetLicnByLicnCivilIDResponse
//        {
//            get
//            {
//                return this.getLicnByLicnCivilIDResponseField;
//            }
//            set
//            {
//                this.getLicnByLicnCivilIDResponseField = value;
//            }
//        }
//    }

//    /// <remarks/>
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
//    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://tempuri.org/", IsNullable = false)]
//    public partial class GetLicnByLicnCivilIDResponse
//    {

//        private GetLicnByLicnCivilIDResponseGetLicnByLicnCivilIDResult getLicnByLicnCivilIDResultField;

//        /// <remarks/>
//        public GetLicnByLicnCivilIDResponseGetLicnByLicnCivilIDResult GetLicnByLicnCivilIDResult
//        {
//            get
//            {
//                return this.getLicnByLicnCivilIDResultField;
//            }
//            set
//            {
//                this.getLicnByLicnCivilIDResultField = value;
//            }
//        }
//    }

//    /// <remarks/>
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
//    public partial class GetLicnByLicnCivilIDResponseGetLicnByLicnCivilIDResult
//    {

//        private GetLicnByLicnCivilIDResponseGetLicnByLicnCivilIDResultLicnData licnDataField;

//        private GetLicnByLicnCivilIDResponseGetLicnByLicnCivilIDResultLicnActivity licnActivityField;

//        private GetLicnByLicnCivilIDResponseGetLicnByLicnCivilIDResultLicnExtraAddress licnExtraAddressField;

//        private GetLicnByLicnCivilIDResponseGetLicnByLicnCivilIDResultImportLicn importLicnField;

//        //private byte statusField;
//        private string statusField;

//        private string statusDescField;

//        /// <remarks/>
//        public GetLicnByLicnCivilIDResponseGetLicnByLicnCivilIDResultLicnData licnData
//        {
//            get
//            {
//                return this.licnDataField;
//            }
//            set
//            {
//                this.licnDataField = value;
//            }
//        }

//        /// <remarks/>
//        public GetLicnByLicnCivilIDResponseGetLicnByLicnCivilIDResultLicnActivity licnActivity
//        {
//            get
//            {
//                return this.licnActivityField;
//            }
//            set
//            {
//                this.licnActivityField = value;
//            }
//        }

//        /// <remarks/>
//        public GetLicnByLicnCivilIDResponseGetLicnByLicnCivilIDResultLicnExtraAddress LicnExtraAddress
//        {
//            get
//            {
//                return this.licnExtraAddressField;
//            }
//            set
//            {
//                this.licnExtraAddressField = value;
//            }
//        }

//        /// <remarks/>
//        public GetLicnByLicnCivilIDResponseGetLicnByLicnCivilIDResultImportLicn importLicn
//        {
//            get
//            {
//                return this.importLicnField;
//            }
//            set
//            {
//                this.importLicnField = value;
//            }
//        }

//        /// <remarks/>
//        //public byte Status
//        public string Status
//        {
//            get
//            {
//                return this.statusField;
//            }
//            set
//            {
//                this.statusField = value;
//            }
//        }

//        /// <remarks/>
//        public string StatusDesc
//        {
//            get
//            {
//                return this.statusDescField;
//            }
//            set
//            {
//                this.statusDescField = value;
//            }
//        }
//    }

//    /// <remarks/>
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
//    public partial class GetLicnByLicnCivilIDResponseGetLicnByLicnCivilIDResultLicnData
//    {

//        private string licn_comm_noField;

//        private string company_nameField;

//        private uint licn_civil_idField;

//        private string active_purposeField;

//        private ushort active_purpose_codeField;

//        private ushort licn_file_enter_noField;

//        private uint address_auto_noField;

//        private ushort comm_book_noField;

//        private ulong civil_idField;

//        private string emp_nameField;

//        private object company_typeField;

//        private object licn_targetField;

//        private string licn_statusField;

//        private object trade_nameField;

//        private string licn_comm_sdateField;

//        private string licn_comm_edateField;

//        private object main_companyField;

//        private object total_share_noField;

//        private object total_capitalField;

//        private string linc_typeField;

//        private object rEG_COMM_DATEField;

//        private object cONT_APRV_DATEField;

//        private object cONT_NOField;

//        private byte new_licn_flagField;

//        private object is_main_licnField;

//        private object notesField;

//        /// <remarks/>
//        public string licn_comm_no
//        {
//            get
//            {
//                return this.licn_comm_noField;
//            }
//            set
//            {
//                this.licn_comm_noField = value;
//            }
//        }

//        /// <remarks/>
//        public string company_name
//        {
//            get
//            {
//                return this.company_nameField;
//            }
//            set
//            {
//                this.company_nameField = value;
//            }
//        }

//        /// <remarks/>
//        public uint licn_civil_id
//        {
//            get
//            {
//                return this.licn_civil_idField;
//            }
//            set
//            {
//                this.licn_civil_idField = value;
//            }
//        }

//        /// <remarks/>
//        public string active_purpose
//        {
//            get
//            {
//                return this.active_purposeField;
//            }
//            set
//            {
//                this.active_purposeField = value;
//            }
//        }

//        /// <remarks/>
//        public ushort active_purpose_code
//        {
//            get
//            {
//                return this.active_purpose_codeField;
//            }
//            set
//            {
//                this.active_purpose_codeField = value;
//            }
//        }

//        /// <remarks/>
//        public ushort licn_file_enter_no
//        {
//            get
//            {
//                return this.licn_file_enter_noField;
//            }
//            set
//            {
//                this.licn_file_enter_noField = value;
//            }
//        }

//        /// <remarks/>
//        public uint address_auto_no
//        {
//            get
//            {
//                return this.address_auto_noField;
//            }
//            set
//            {
//                this.address_auto_noField = value;
//            }
//        }

//        /// <remarks/>
//        public ushort comm_book_no
//        {
//            get
//            {
//                return this.comm_book_noField;
//            }
//            set
//            {
//                this.comm_book_noField = value;
//            }
//        }

//        /// <remarks/>
//        public ulong civil_id
//        {
//            get
//            {
//                return this.civil_idField;
//            }
//            set
//            {
//                this.civil_idField = value;
//            }
//        }

//        /// <remarks/>
//        public string emp_name
//        {
//            get
//            {
//                return this.emp_nameField;
//            }
//            set
//            {
//                this.emp_nameField = value;
//            }
//        }

//        /// <remarks/>
//        public object company_type
//        {
//            get
//            {
//                return this.company_typeField;
//            }
//            set
//            {
//                this.company_typeField = value;
//            }
//        }

//        /// <remarks/>
//        public object licn_target
//        {
//            get
//            {
//                return this.licn_targetField;
//            }
//            set
//            {
//                this.licn_targetField = value;
//            }
//        }

//        /// <remarks/>
//        public string licn_status
//        {
//            get
//            {
//                return this.licn_statusField;
//            }
//            set
//            {
//                this.licn_statusField = value;
//            }
//        }

//        /// <remarks/>
//        public object trade_name
//        {
//            get
//            {
//                return this.trade_nameField;
//            }
//            set
//            {
//                this.trade_nameField = value;
//            }
//        }

//        /// <remarks/>
//        public string licn_comm_sdate
//        {
//            get
//            {
//                return this.licn_comm_sdateField;
//            }
//            set
//            {
//                this.licn_comm_sdateField = value;
//            }
//        }

//        /// <remarks/>
//        public string licn_comm_edate
//        {
//            get
//            {
//                return this.licn_comm_edateField;
//            }
//            set
//            {
//                this.licn_comm_edateField = value;
//            }
//        }

//        /// <remarks/>
//        public object main_company
//        {
//            get
//            {
//                return this.main_companyField;
//            }
//            set
//            {
//                this.main_companyField = value;
//            }
//        }

//        /// <remarks/>
//        public object total_share_no
//        {
//            get
//            {
//                return this.total_share_noField;
//            }
//            set
//            {
//                this.total_share_noField = value;
//            }
//        }

//        /// <remarks/>
//        public object total_capital
//        {
//            get
//            {
//                return this.total_capitalField;
//            }
//            set
//            {
//                this.total_capitalField = value;
//            }
//        }

//        /// <remarks/>
//        public string linc_type
//        {
//            get
//            {
//                return this.linc_typeField;
//            }
//            set
//            {
//                this.linc_typeField = value;
//            }
//        }

//        /// <remarks/>
//        public object REG_COMM_DATE
//        {
//            get
//            {
//                return this.rEG_COMM_DATEField;
//            }
//            set
//            {
//                this.rEG_COMM_DATEField = value;
//            }
//        }

//        /// <remarks/>
//        public object CONT_APRV_DATE
//        {
//            get
//            {
//                return this.cONT_APRV_DATEField;
//            }
//            set
//            {
//                this.cONT_APRV_DATEField = value;
//            }
//        }

//        /// <remarks/>
//        public object CONT_NO
//        {
//            get
//            {
//                return this.cONT_NOField;
//            }
//            set
//            {
//                this.cONT_NOField = value;
//            }
//        }

//        /// <remarks/>
//        public byte new_licn_flag
//        {
//            get
//            {
//                return this.new_licn_flagField;
//            }
//            set
//            {
//                this.new_licn_flagField = value;
//            }
//        }

//        /// <remarks/>
//        public object is_main_licn
//        {
//            get
//            {
//                return this.is_main_licnField;
//            }
//            set
//            {
//                this.is_main_licnField = value;
//            }
//        }

//        /// <remarks/>
//        public object notes
//        {
//            get
//            {
//                return this.notesField;
//            }
//            set
//            {
//                this.notesField = value;
//            }
//        }
//    }

//    /// <remarks/>
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
//    public partial class GetLicnByLicnCivilIDResponseGetLicnByLicnCivilIDResultLicnActivity
//    {

//        private GetLicnByLicnCivilIDResponseGetLicnByLicnCivilIDResultLicnActivityLicnActivity licnActivityField;

//        /// <remarks/>
//        public GetLicnByLicnCivilIDResponseGetLicnByLicnCivilIDResultLicnActivityLicnActivity LicnActivity
//        {
//            get
//            {
//                return this.licnActivityField;
//            }
//            set
//            {
//                this.licnActivityField = value;
//            }
//        }
//    }

//    /// <remarks/>
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
//    public partial class GetLicnByLicnCivilIDResponseGetLicnByLicnCivilIDResultLicnActivityLicnActivity
//    {

//        private string active_purposeField;

//        private ushort active_purpose_codeField;

//        /// <remarks/>
//        public string active_purpose
//        {
//            get
//            {
//                return this.active_purposeField;
//            }
//            set
//            {
//                this.active_purposeField = value;
//            }
//        }

//        /// <remarks/>
//        public ushort active_purpose_code
//        {
//            get
//            {
//                return this.active_purpose_codeField;
//            }
//            set
//            {
//                this.active_purpose_codeField = value;
//            }
//        }
//    }

//    /// <remarks/>
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
//    public partial class GetLicnByLicnCivilIDResponseGetLicnByLicnCivilIDResultLicnExtraAddress
//    {

//        private GetLicnByLicnCivilIDResponseGetLicnByLicnCivilIDResultLicnExtraAddressLicnExtraAddress licnExtraAddressField;

//        /// <remarks/>
//        public GetLicnByLicnCivilIDResponseGetLicnByLicnCivilIDResultLicnExtraAddressLicnExtraAddress LicnExtraAddress
//        {
//            get
//            {
//                return this.licnExtraAddressField;
//            }
//            set
//            {
//                this.licnExtraAddressField = value;
//            }
//        }
//    }

//    /// <remarks/>
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
//    public partial class GetLicnByLicnCivilIDResponseGetLicnByLicnCivilIDResultLicnExtraAddressLicnExtraAddress
//    {

//        private uint address_auto_noField;

//        private byte uNIT_TYPEField;

//        private ushort dISTRICTField;

//        private string wV_DISTRICT_TEXTField;

//        private object fLOOR_NOField;

//        private byte wV_BLOCKField;

//        private ushort wV_PLOT_NOField;

//        private string wV_STREETField;

//        private byte wV_UNIT_NOField;

//        private string wV_UNIT_TYPE_TEXTField;

//        private string wV_BLDG_NAMEField;

//        private string wV_GOV_CODEField;

//        /// <remarks/>
//        public uint address_auto_no
//        {
//            get
//            {
//                return this.address_auto_noField;
//            }
//            set
//            {
//                this.address_auto_noField = value;
//            }
//        }

//        /// <remarks/>
//        public byte UNIT_TYPE
//        {
//            get
//            {
//                return this.uNIT_TYPEField;
//            }
//            set
//            {
//                this.uNIT_TYPEField = value;
//            }
//        }

//        /// <remarks/>
//        public ushort DISTRICT
//        {
//            get
//            {
//                return this.dISTRICTField;
//            }
//            set
//            {
//                this.dISTRICTField = value;
//            }
//        }

//        /// <remarks/>
//        public string WV_DISTRICT_TEXT
//        {
//            get
//            {
//                return this.wV_DISTRICT_TEXTField;
//            }
//            set
//            {
//                this.wV_DISTRICT_TEXTField = value;
//            }
//        }

//        /// <remarks/>
//        public object FLOOR_NO
//        {
//            get
//            {
//                return this.fLOOR_NOField;
//            }
//            set
//            {
//                this.fLOOR_NOField = value;
//            }
//        }

//        /// <remarks/>
//        public byte WV_BLOCK
//        {
//            get
//            {
//                return this.wV_BLOCKField;
//            }
//            set
//            {
//                this.wV_BLOCKField = value;
//            }
//        }

//        /// <remarks/>
//        public ushort WV_PLOT_NO
//        {
//            get
//            {
//                return this.wV_PLOT_NOField;
//            }
//            set
//            {
//                this.wV_PLOT_NOField = value;
//            }
//        }

//        /// <remarks/>
//        public string WV_STREET
//        {
//            get
//            {
//                return this.wV_STREETField;
//            }
//            set
//            {
//                this.wV_STREETField = value;
//            }
//        }

//        /// <remarks/>
//        public byte WV_UNIT_NO
//        {
//            get
//            {
//                return this.wV_UNIT_NOField;
//            }
//            set
//            {
//                this.wV_UNIT_NOField = value;
//            }
//        }

//        /// <remarks/>
//        public string WV_UNIT_TYPE_TEXT
//        {
//            get
//            {
//                return this.wV_UNIT_TYPE_TEXTField;
//            }
//            set
//            {
//                this.wV_UNIT_TYPE_TEXTField = value;
//            }
//        }

//        /// <remarks/>
//        public string WV_BLDG_NAME
//        {
//            get
//            {
//                return this.wV_BLDG_NAMEField;
//            }
//            set
//            {
//                this.wV_BLDG_NAMEField = value;
//            }
//        }

//        /// <remarks/>
//        public string WV_GOV_CODE
//        {
//            get
//            {
//                return this.wV_GOV_CODEField;
//            }
//            set
//            {
//                this.wV_GOV_CODEField = value;
//            }
//        }
//    }

//    /// <remarks/>
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
//    public partial class GetLicnByLicnCivilIDResponseGetLicnByLicnCivilIDResultImportLicn
//    {

//        private GetLicnByLicnCivilIDResponseGetLicnByLicnCivilIDResultImportLicnImportLicn importLicnField;

//        /// <remarks/>
//        public GetLicnByLicnCivilIDResponseGetLicnByLicnCivilIDResultImportLicnImportLicn ImportLicn
//        {
//            get
//            {
//                return this.importLicnField;
//            }
//            set
//            {
//                this.importLicnField = value;
//            }
//        }
//    }

//    /// <remarks/>
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
//    public partial class GetLicnByLicnCivilIDResponseGetLicnByLicnCivilIDResultImportLicnImportLicn
//    {

//        private uint iMPORT_NUMBERField;

//        private string iMPORT_SDATEField;

//        private string iMPORT_EDATEField;

//        private uint lICN_CIVIL_IDField;

//        private byte iMPORT_TYPE_CODEField;

//        private string iMPORT_TYPE_DESCField;

//        /// <remarks/>
//        public uint IMPORT_NUMBER
//        {
//            get
//            {
//                return this.iMPORT_NUMBERField;
//            }
//            set
//            {
//                this.iMPORT_NUMBERField = value;
//            }
//        }

//        /// <remarks/>
//        public string IMPORT_SDATE
//        {
//            get
//            {
//                return this.iMPORT_SDATEField;
//            }
//            set
//            {
//                this.iMPORT_SDATEField = value;
//            }
//        }

//        /// <remarks/>
//        public string IMPORT_EDATE
//        {
//            get
//            {
//                return this.iMPORT_EDATEField;
//            }
//            set
//            {
//                this.iMPORT_EDATEField = value;
//            }
//        }

//        /// <remarks/>
//        public uint LICN_CIVIL_ID
//        {
//            get
//            {
//                return this.lICN_CIVIL_IDField;
//            }
//            set
//            {
//                this.lICN_CIVIL_IDField = value;
//            }
//        }

//        /// <remarks/>
//        public byte IMPORT_TYPE_CODE
//        {
//            get
//            {
//                return this.iMPORT_TYPE_CODEField;
//            }
//            set
//            {
//                this.iMPORT_TYPE_CODEField = value;
//            }
//        }

//        /// <remarks/>
//        public string IMPORT_TYPE_DESC
//        {
//            get
//            {
//                return this.iMPORT_TYPE_DESCField;
//            }
//            set
//            {
//                this.iMPORT_TYPE_DESCField = value;
//            }
//        }
//    }




//}

////==========================Industriallicensewithcivilid=============================

//namespace ETradeAPI.Service_References.Industriallicensewithcivilid
//{

//    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
//    /// <remarks/>
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2003/05/soap-envelope")]
//    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.w3.org/2003/05/soap-envelope", IsNullable = false)]
//    public partial class Envelope
//    {

//        private EnvelopeBody bodyField;

//        /// <remarks/>
//        public EnvelopeBody Body
//        {
//            get
//            {
//                return this.bodyField;
//            }
//            set
//            {
//                this.bodyField = value;
//            }
//        }
//    }

//    /// <remarks/>
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2003/05/soap-envelope")]
//    public partial class EnvelopeBody
//    {

//        private GetPAILicenseDetailResponse getPAILicenseDetailResponseField;

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://tempuri.org/")]
//        public GetPAILicenseDetailResponse GetPAILicenseDetailResponse
//        {
//            get
//            {
//                return this.getPAILicenseDetailResponseField;
//            }
//            set
//            {
//                this.getPAILicenseDetailResponseField = value;
//            }
//        }
//    }

//    /// <remarks/>
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
//    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://tempuri.org/", IsNullable = false)]
//    public partial class GetPAILicenseDetailResponse
//    {

//        private GetPAILicenseDetailResponseGetPAILicenseDetailResult getPAILicenseDetailResultField;

//        /// <remarks/>
//        public GetPAILicenseDetailResponseGetPAILicenseDetailResult GetPAILicenseDetailResult
//        {
//            get
//            {
//                return this.getPAILicenseDetailResultField;
//            }
//            set
//            {
//                this.getPAILicenseDetailResultField = value;
//            }
//        }
//    }

//    /// <remarks/>
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
//    public partial class GetPAILicenseDetailResponseGetPAILicenseDetailResult
//    {

//        private string applicantCivilIdField;

//        private string firstLicenseIssueDateField;

//        private string licenseCancelDateField;

//        private string industryTypeField;

//        private string productionKIndField;

//        private string companyNameField;

//        private string poBoxNrField;

//        private string telephoneNrField;

//        private string faxNrField;

//        private string emailField;

//        private string licenseActiveField;

//        private string commercialNameField;

//        private string licenseStatusField;

//        private string licenseStartDateField;

//        private string licenseEndDateField;

//        private string statutsRemarkField;

//        private string applicantNameField;

//        private string commercialRegistrationNoField;

//        private string industrialRegisttionNoField;

//        private string industrialRegistrationDateField;

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.dpskw.com/licenseDetal")]
//        public string applicantCivilId
//        {
//            get
//            {
//                return this.applicantCivilIdField;
//            }
//            set
//            {
//                this.applicantCivilIdField = value;
//            }
//        }

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.dpskw.com/licenseDetal")]
//        public string firstLicenseIssueDate
//        {
//            get
//            {
//                return this.firstLicenseIssueDateField;
//            }
//            set
//            {
//                this.firstLicenseIssueDateField = value;
//            }
//        }

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.dpskw.com/licenseDetal")]
//        public string licenseCancelDate
//        {
//            get
//            {
//                return this.licenseCancelDateField;
//            }
//            set
//            {
//                this.licenseCancelDateField = value;
//            }
//        }

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.dpskw.com/licenseDetal")]
//        public string industryType
//        {
//            get
//            {
//                return this.industryTypeField;
//            }
//            set
//            {
//                this.industryTypeField = value;
//            }
//        }

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.dpskw.com/licenseDetal")]
//        public string productionKInd
//        {
//            get
//            {
//                return this.productionKIndField;
//            }
//            set
//            {
//                this.productionKIndField = value;
//            }
//        }

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.dpskw.com/licenseDetal")]
//        public string companyName
//        {
//            get
//            {
//                return this.companyNameField;
//            }
//            set
//            {
//                this.companyNameField = value;
//            }
//        }

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.dpskw.com/licenseDetal")]
//        public string poBoxNr
//        {
//            get
//            {
//                return this.poBoxNrField;
//            }
//            set
//            {
//                this.poBoxNrField = value;
//            }
//        }

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.dpskw.com/licenseDetal")]
//        public string telephoneNr
//        {
//            get
//            {
//                return this.telephoneNrField;
//            }
//            set
//            {
//                this.telephoneNrField = value;
//            }
//        }

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.dpskw.com/licenseDetal")]
//        public string faxNr
//        {
//            get
//            {
//                return this.faxNrField;
//            }
//            set
//            {
//                this.faxNrField = value;
//            }
//        }

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.dpskw.com/licenseDetal")]
//        public string email
//        {
//            get
//            {
//                return this.emailField;
//            }
//            set
//            {
//                this.emailField = value;
//            }
//        }

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.dpskw.com/licenseDetal")]
//        public string licenseActive
//        {
//            get
//            {
//                return this.licenseActiveField;
//            }
//            set
//            {
//                this.licenseActiveField = value;
//            }
//        }

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.dpskw.com/licenseDetal")]
//        public string commercialName
//        {
//            get
//            {
//                return this.commercialNameField;
//            }
//            set
//            {
//                this.commercialNameField = value;
//            }
//        }

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.dpskw.com/licenseDetal")]
//        public string licenseStatus
//        {
//            get
//            {
//                return this.licenseStatusField;
//            }
//            set
//            {
//                this.licenseStatusField = value;
//            }
//        }

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.dpskw.com/licenseDetal")]
//        public string licenseStartDate
//        {
//            get
//            {
//                return this.licenseStartDateField;
//            }
//            set
//            {
//                this.licenseStartDateField = value;
//            }
//        }

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.dpskw.com/licenseDetal")]
//        public string licenseEndDate
//        {
//            get
//            {
//                return this.licenseEndDateField;
//            }
//            set
//            {
//                this.licenseEndDateField = value;
//            }
//        }

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.dpskw.com/licenseDetal")]
//        public string statutsRemark
//        {
//            get
//            {
//                return this.statutsRemarkField;
//            }
//            set
//            {
//                this.statutsRemarkField = value;
//            }
//        }

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.dpskw.com/licenseDetal")]
//        public string applicantName
//        {
//            get
//            {
//                return this.applicantNameField;
//            }
//            set
//            {
//                this.applicantNameField = value;
//            }
//        }

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.dpskw.com/licenseDetal")]
//        public string commercialRegistrationNo
//        {
//            get
//            {
//                return this.commercialRegistrationNoField;
//            }
//            set
//            {
//                this.commercialRegistrationNoField = value;
//            }
//        }

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.dpskw.com/licenseDetal")]
//        public string industrialRegisttionNo
//        {
//            get
//            {
//                return this.industrialRegisttionNoField;
//            }
//            set
//            {
//                this.industrialRegisttionNoField = value;
//            }
//        }

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.dpskw.com/licenseDetal")]
//        public string industrialRegistrationDate
//        {
//            get
//            {
//                return this.industrialRegistrationDateField;
//            }
//            set
//            {
//                this.industrialRegistrationDateField = value;
//            }
//        }
//    }


//}


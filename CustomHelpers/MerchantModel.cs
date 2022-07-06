using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;


namespace Nop.Plugin.Payments.LipanaMpesa.CustomHelpers
{
    public class MerchantModel :BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        //use this to set testing phase or switch to production
        [NopResourceDisplayName("Plugins.Payments.LipanaMpesa.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }
        public bool UseSandbox_override { get; set; }

        /// <summary>
        /// Gets or sets a business paybill number
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.LipanaMpesa.Fields.PartyB")]
        public string PartyB { get; set; }
        public bool PartyB_override { get; set; }

        /// <summary>
        /// Gets or sets access  token.
        /// base64 encoded string of an app's consumer key and consumer secret 
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.LipanaMpesa.Fields.Authorization")]
        public string Authorization { get; set; }
        public bool Authorization_override { get; set; }

        //the passkey used alongside timestamp+businessshortcode to generate password for
        //for payment processing
        [NopResourceDisplayName("Plugins.Payments.LipanaMpesa.Fields.Passkey")]
        public string PassKey { get; set; }
        public bool PassKey_override { get; set; }

        //your call back url
        [NopResourceDisplayName("Plugins.Payments.LipanaMpesa.Fields.CallBackURL")]
        public string CallBackURL { get; set; }
        public bool CallBackURL_override { get; set; }

        //account name. usually paybill send=>to name
        [NopResourceDisplayName("Plugins.Payments.LipanaMpesa.Fields.AccountReference")]
        public string AccountReference { get; set; }
        public bool AccountReference_override { get; set; }

        //reason for payment.
        //eg,payment for online purchase
        [NopResourceDisplayName("Plugins.Payments.LipanaMpesa.Fields.TransactionDesc")]
        public string TransactionDesc { get; set; }
        public bool TransactionDesc_override { get; set; }

    }

}

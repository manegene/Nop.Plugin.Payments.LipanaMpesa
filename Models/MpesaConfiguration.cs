using Nop.Core.Configuration;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.LipanaMpesa.Models
{
    public class MpesaConfiguration:ISettings
    {
        // public int ActiveStoreScopeConfiguration { get; set; }
        public long PartyA { get; set; }


        //use this to set testing phase or switch to production
        //[NopResourceDisplayName("Plugins.Payments.LipanaMpesa.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }

        /// <summary>
        /// Gets or sets a business paybill number
        /// </summary>
        public string PartyB { get; set; }

        /// <summary>
        /// Gets or sets access  token.
        /// base64 encoded string of an app's consumer key and consumer secret 
        /// used to  get the access token for payment transaction
        /// Basis Authorization value
        /// </summary>
        public string Authorization { get; set; }

        //This is the password used for encrypting the request sent:
        //A base64 encoded string.
        //(The base64 string is a combination of Shortcode+Passkey+Timestamp) 
        public string PassKey { get; set; }

        //your call back url
        public string CallBackURL { get; set; }

        //account name. usually paybill send to name
        public string AccountReference { get; set; }

        //reason for payment.
        //eg,payment for online purchase
        public string TransactionDesc { get; set; }


    }
}

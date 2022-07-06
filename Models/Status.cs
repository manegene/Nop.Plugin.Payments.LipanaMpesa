using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.LipanaMpesa.Models
{
   public class Status
    {
        //mpesa gateway responses
        public string Access_token { get; set; }
        public string MerchantRequestID { get; set; }
        public string CheckoutRequestID { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseDescription { get; set; }
        public string CustomerMessage { get; set; }
        public string ErrorMessage { get; set; }
    }
}

using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Nop.Plugin.Payments.LipanaMpesa.Models
{
    public class PaymentParams: BaseNopModel
    {
    public int BusinessShortCode { get; set; }
    public string Password { get; set; }
    public string Timestamp { get; set; }
    public string TransactionType { get; set; }
    public decimal Amount { get; set; }

    [Display(Name="Phone number",Prompt= "number with country code.eg 254720000000")]
    [Phone]
    public long? PartyA { get; set; }

    public int PartyB { get; set; }
    public long PhoneNumber { get; set; }
    public string CallBackURL { get; set; }
    public string AccountReference { get; set; }
    public string TransactionDesc { get; set; }
    }
}

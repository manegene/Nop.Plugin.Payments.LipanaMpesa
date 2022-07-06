using Nop.Plugin.Payments.LipanaMpesa.Models;
using Nop.Services.Payments;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.LipanaMpesa.Services
{
    public interface IPayment
    {
       Task<Status> PayStatus(PaymentParams paymentParams);
    }
}

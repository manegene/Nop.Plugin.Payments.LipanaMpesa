using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.LipanaMpesa.Models;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Order;
using System;
using System.Collections.Generic;
using System.Net;

namespace Nop.Plugin.Payments.LipanaMpesa.Components
{
    [ViewComponent(Name = "LipanaMpesa")]
    public class LipanaMpesaViewComponent : NopViewComponent
    {
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
   


        public LipanaMpesaViewComponent(ISettingService settingService,
            IStoreContext storeContext)
        {
            _settingService = settingService;
            _storeContext = storeContext;

        }

        public IViewComponentResult Invoke()
        {
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var LipanaMpesaSettings = _settingService.LoadSetting<MpesaConfiguration>(storeScope);

            //load saved paybill and account number for manual payment
            var Userdetails = new PaymentParams
            {
                PartyB =Convert.ToInt32( LipanaMpesaSettings.PartyB),
                AccountReference = LipanaMpesaSettings.AccountReference,

            };


            return View("~/Plugins/Payments.LipanaMpesa/Views/PaymentInfo.cshtml",Userdetails );
        }
    }
}

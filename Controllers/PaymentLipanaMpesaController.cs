using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.LipanaMpesa.CustomHelpers;
using Nop.Plugin.Payments.LipanaMpesa.Models;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.LipanaMpesa.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class PaymentLipanaMpesa : BasePaymentController
    {
        #region Fields

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly INotificationService _notificationService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        #endregion

        #region Ctor

        public PaymentLipanaMpesa(IGenericAttributeService genericAttributeService,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IPaymentPluginManager paymentPluginManager,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            ILogger logger,
            INotificationService notificationService,
            ISettingService settingService,
            IStoreContext storeContext,
            IWebHelper webHelper,
            IWorkContext workContext,
            ShoppingCartSettings shoppingCartSettings)
        {
            _genericAttributeService = genericAttributeService;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _paymentPluginManager = paymentPluginManager;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _logger = logger;
            _notificationService = notificationService;
            _settingService = settingService;
            _storeContext = storeContext;
            _webHelper = webHelper;
            _workContext = workContext;
            _shoppingCartSettings = shoppingCartSettings;
        }

        #endregion

        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var LipanaMpesaSettings = _settingService.LoadSetting<MpesaConfiguration>(storeScope);

            var model = new MerchantModel
            {
                UseSandbox = LipanaMpesaSettings.UseSandbox,
                PartyB = LipanaMpesaSettings.PartyB,
                Authorization=LipanaMpesaSettings.Authorization,
                PassKey = LipanaMpesaSettings.PassKey,
                CallBackURL = LipanaMpesaSettings.CallBackURL,
                AccountReference=LipanaMpesaSettings.AccountReference,
                TransactionDesc=LipanaMpesaSettings.TransactionDesc,
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope <= 0)
                return View("~/Plugins/Payments.LipanaMpesa/Views/Configure.cshtml", model);

            model.UseSandbox_override = _settingService.SettingExists(LipanaMpesaSettings, x => x.UseSandbox, storeScope);
            model.PartyB_override = _settingService.SettingExists(LipanaMpesaSettings, x => x.PartyB, storeScope);
            model.Authorization_override = _settingService.SettingExists(LipanaMpesaSettings, x => x.Authorization, storeScope);
            model.PassKey_override = _settingService.SettingExists(LipanaMpesaSettings, x => x.PassKey, storeScope);
            model.CallBackURL_override = _settingService.SettingExists(LipanaMpesaSettings, x => x.CallBackURL, storeScope);
            model.AccountReference_override = _settingService.SettingExists(LipanaMpesaSettings, x => x.AccountReference, storeScope);
            model.TransactionDesc_override = _settingService.SettingExists(LipanaMpesaSettings, x => x.TransactionDesc, storeScope);


            return View("~/Plugins/Payments.LipanaMpesa/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(MerchantModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var MerchantSettings = _settingService.LoadSetting<MpesaConfiguration>(storeScope);

            //save settings
            MerchantSettings.UseSandbox = model.UseSandbox;
            MerchantSettings.PartyB = model.PartyB;
            MerchantSettings.Authorization = model.Authorization;
            MerchantSettings.PassKey = model.PassKey;
            MerchantSettings.CallBackURL = model.CallBackURL;
            MerchantSettings.AccountReference = model.AccountReference;
            MerchantSettings.TransactionDesc = model.TransactionDesc;


            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(MerchantSettings, x => x.UseSandbox, model.UseSandbox_override, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(MerchantSettings, x => x.PartyB, model.PartyB_override, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(MerchantSettings, x => x.Authorization, model.Authorization_override, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(MerchantSettings, x => x.PassKey, model.PassKey_override, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(MerchantSettings, x => x.CallBackURL, model.CallBackURL_override, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(MerchantSettings, x => x.AccountReference, model.AccountReference_override, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(MerchantSettings, x => x.TransactionDesc, model.TransactionDesc_override, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        //action displaying notification (warning) to a store owner
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult RoundingWarning(bool passProductNamesAndTotals)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //prices and total aren't rounded, so display warning
            if (passProductNamesAndTotals && !_shoppingCartSettings.RoundPricesDuringCalculation)
                return Json(new { Result = _localizationService.GetResource("Plugins.Payments.LipanaMpesa.RoundingWarning") });

            return Json(new { Result = string.Empty });
        }

        #endregion
    }
}
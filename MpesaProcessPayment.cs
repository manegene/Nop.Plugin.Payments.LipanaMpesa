using Microsoft.AspNetCore.Http;
using Nito.AsyncEx;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.LipanaMpesa.Components;
using Nop.Plugin.Payments.LipanaMpesa.CustomHelpers;
using Nop.Plugin.Payments.LipanaMpesa.Models;
using Nop.Plugin.Payments.LipanaMpesa.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.LipanaMpesa
{
    public class MpesaProcessPayment : BasePlugin, IPaymentMethod
    {
        #region fields
        private readonly IPayment _ipayment;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly IStoreContext _storeContext;

        #endregion
        #region constructor
        public MpesaProcessPayment
            (IPayment ipayment,
            ISettingService settingService,
            ILocalizationService localizationService,
            IWebHelper webHelper,
            IStoreContext storeContext
            )
        {
            _webHelper = webHelper;
            _ipayment = ipayment;
            _settingService = settingService;
            _localizationService = localizationService;
            _storeContext = storeContext;
        }
        #endregion
        #region Properties

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture => false;

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund => false;

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund => false;

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid => false;

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType => PaymentMethodType.Standard;

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo => false;

        /// <summary>
        /// Gets a payment method description that will be displayed on checkout pages in the public store
        /// </summary>
        public string PaymentMethodDescription => _localizationService.GetResource("Plugins.Payments.LipanaMpesa.PaymentMethodDescription");

        #endregion
        #region Methods

        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public  ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var orderT = Math.Round(processPaymentRequest.OrderTotal, 2);
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var LipanaMpesaSettings = _settingService.LoadSetting<MpesaConfiguration>(storeScope);
            var  mobilenumber = processPaymentRequest.CustomValues["Mpesano"].ToString();
            if (mobilenumber != null)
            {
                var paymentParams = new PaymentParams
                {
                    BusinessShortCode = Convert.ToInt32(LipanaMpesaSettings.PartyB),
                    Timestamp = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    Password = Convert.ToBase64String(Encoding.UTF8.GetBytes(LipanaMpesaSettings.PartyB + LipanaMpesaSettings.PassKey + DateTime.Now.ToString("yyyyMMddHHmmss"))),
                    TransactionType = "CustomerPayBillOnline",
                    Amount = orderT,
                    PartyA = Int64.Parse(mobilenumber),//Convert.ToInt64( _lipanaMpesaViewComponent.Request.Form["PartyA"]),
                    PartyB = Convert.ToInt32(LipanaMpesaSettings.PartyB),
                    PhoneNumber = Int64.Parse(mobilenumber),//Convert.ToInt64(_lipanaMpesaViewComponent.Request.Form["PartyA"]),
                    CallBackURL = LipanaMpesaSettings.CallBackURL,
                    AccountReference = LipanaMpesaSettings.AccountReference,
                    TransactionDesc = LipanaMpesaSettings.TransactionDesc
                   
                };
                var transaction = _ipayment.PayStatus(paymentParams).GetAwaiter().GetResult();

                if (transaction.ResponseCode == "0")
                {
                    return new ProcessPaymentResult();
                }
                return new ProcessPaymentResult();
            }

            return new ProcessPaymentResult();
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            return;
        }

        /// <summary>
        /// Returns a value indicating whether payment method should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>true - hide; false - display.</returns>
        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country
            return false;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return 0;
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            return new CapturePaymentResult { Errors = new[] { "Capture method not supported" } };
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            return new RefundPaymentResult { Errors = new[] { "Refund method not supported" } };
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            return new VoidPaymentResult { Errors = new[] { "Void method not supported" } };
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            return new ProcessPaymentResult { Errors = new[] { "Recurring payment not supported" } };
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            return new CancelRecurringPaymentResult { Errors = new[] { "Recurring payment not supported" } };
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            return true;
        }

        /// <summary>
        /// Validate payment form
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>List of validating errors</returns>
        public IList<string> ValidatePaymentForm(IFormCollection form)
        {
            var warnings= new List<string>();
            var model = new PaymentParams
            {
                PartyA=Int64.Parse(form["PartyA"])
            };
            if (model.PartyA == 0 )
                warnings.AddRange(warnings);

            return warnings;
        }

        /// <summary>
        /// Get payment information
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>Payment info holder</returns>
        public ProcessPaymentRequest GetPaymentInfo(IFormCollection form)
        {
            var paymentnumber = new ProcessPaymentRequest
           {
                CustomValues= new Dictionary<string, object>()
                {
                    {"Mpesano",form["PartyA"].ToString() }
                }
            };
            return paymentnumber;
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/PaymentLipanaMpesa/Configure";
        }

        /// <summary>
        /// Gets a name of a view component for displaying plugin in public store ("payment info" checkout step)
        /// </summary>
        /// <returns>View component name</returns>
        public string GetPublicViewComponentName()
        {
            return "LipanaMpesa";
        }

        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new MpesaConfiguration
            {
                UseSandbox = true
            });

            //locales
            _localizationService.AddPluginLocaleResource(new Dictionary<string, string>
            {
                ["Plugins.Payments.LipanaMpesa.Fields.Authorization"] = "Authorization key.Used to get access token",
                ["Plugins.Payments.LipanaMpesa.Fields.Authorization.Hint"] = "Base64 encoded string of an app's consumer key and consumer secret",
                ["Plugins.Payments.LipanaMpesa.Fields.PassKey"] = "Pass key",
                ["Plugins.Payments.LipanaMpesa.Fields.PassKey.Hint"] = "The key used in combination of Shortcode+Timestamp to get transaction password",
                ["Plugins.Payments.LipanaMpesa.Fields.PartyB"] = "Paybill number",
                ["Plugins.Payments.LipanaMpesa.Fields.PartyB.Hint"] = "Your business M-pesa paybill number.",
                ["Plugins.Payments.LipanaMpesa.Fields.CallBackURL"] = "Callback URL",
                ["Plugins.Payments.LipanaMpesa.Fields.CallBackURL.Hint"] = "URL called when transaction completes",
                ["Plugins.Payments.LipanaMpesa.Fields.AccountReference"] = "Your business name",
                ["Plugins.Payments.LipanaMpesa.Fields.AccountReference.Hint"] = "Name used in paybill registration.will be visible to users alongside amount.e.g pay Robert KShs 1000",
                ["Plugins.Payments.LipanaMpesa.Fields.TransactionDesc"] = "Transaction description",
                ["Plugins.Payments.LipanaMpesa.Fields.TransactionDesc.Hint"] = "eg, online checkout. Max 13 characters",
                ["Plugins.Payments.LipanaMpesa.Fields.UseSandbox"] = "Use Sandbox",
                ["Plugins.Payments.LipanaMpesa.Fields.UseSandbox.Hint"] = "Check to enable Sandbox (testing environment).",
                ["Plugins.Payments.LipanaMpesa.Fields.PaymentInfo"] = "Confirm your mpesa number.\"" +
                "Will send Mpesa payment payment notification to this number on order confirmation.\"" +
                " Keep your phone close to accept payment and enter M-pesa pin",
                ["Plugins.Payments.LipanaMpesa.Fields.PayManual"] = "If you choose to pay manually, follow below guide:",
                ["Plugins.Payments.LipanaMpesa.Instructions"] = @"
                    <p>
	                    <b>If you're using this gateway ensure you support Kenya Shillings currency.</br>
                       The Sandbox flag is just a warnign to store owner if they would like to test. </br>
                     Otherwise only the payment details matter between production and testing. </b>
	                    <br />
	                    <br />You must have a  valid paybill from Safaricom Paybill.This plugin is M-Pesa Express api integration.<br />
                         To register for the paybill, use below guide :<br />
	                    <br />1. Log in to your Safaricom deleloper account (click <a href=""https://developer.safaricom.co.ke"" target=""_blank"">here</a> to manage your account).
	                    <br />2. Click on the Login /Sign Up button to login/ setup a new account.
	                    <br />3. Once logged in, Click on Create new APP. Give it a name, select Lipa na Mpesa integration on Mpesa products mapping. You could include the others but dont miss the Mpesa express(option 1).
	                    <br />4. Once created, select APIs from Menu.You will be able to get configuration information from this area for use with this plugin.
	                    <br />5. Select on authorisation. Select your app on the new page, copy the Basic authorization key and paste as is in authorisation plugin field
	                    <br />6. Select on Mpesa express. Business Shortcode is your paybill number.
                        <br />7. Passkey is your app passkey available in Mpesa express api, select your app.Passkey will be displayed
	                    <br />8. Business name is the name mapped to your paybill number
                        <br />9. Transaction description is a descriptive statement. Should not exceed 13 characters.
	                    <br />10. Click Save.
	                    <br />
                    </p>",
                ["Plugins.Payments.LipanaMpesa.PaymentMethodDescription"] = "You will receive the mpesa request on your phone.\"ensure you have sufficient cash in your Mpeasa account",

            });

            base.Install();
        }
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<MpesaConfiguration>();

            //locales
            _localizationService.DeletePluginLocaleResources("Plugins.Payments.LipanaMpesa");

            base.Uninstall();
        }

        #endregion
    }

}

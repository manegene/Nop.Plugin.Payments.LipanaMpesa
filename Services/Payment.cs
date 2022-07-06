using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Payments.LipanaMpesa.Models;
using Nop.Services.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.LipanaMpesa.Services
{

    class Payment : IPayment
    {
        #region fields
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly HttpClient httpClient;
        #endregion
        #region constructor
        public Payment
            (ISettingService settingService,
            IStoreContext storeContext
            )
        {
            httpClient = new HttpClient();
            _settingService = settingService;
            _storeContext = storeContext;
        }
        #endregion
        public async Task<Status> PayStatus(PaymentParams paymentParams)
        {
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var _merchantSettings = _settingService.LoadSetting<MpesaConfiguration>(storeScope); ;
            var status = new Status();
            try
            {
                //get authorization token first
                var Tokenuri = new UriBuilder(_merchantSettings.UseSandbox ?
                    "https://sandbox.safaricom.co.ke/oauth/v1/generate?grant_type=client_credentials":
                    "https://api.safaricom.co.ke/oauth/v1/generate?grant_type=client_credentials");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _merchantSettings.Authorization);
                var Tokenresponse = await httpClient.GetAsync(Tokenuri.ToString());
                if (Tokenresponse != null && Tokenresponse.IsSuccessStatusCode)
                {
                    var Tokenresult = Tokenresponse.Content.ReadAsStringAsync().Result;
                    if (Tokenresult != null)
                    {
                        //deserialize the reponse and extract access_token
                        var token = JsonConvert.DeserializeObject<Status>(Tokenresult);


                        //get the access token for use in payment request process
                        var bearertoken = token.Access_token;

                        //proceed with payment now that we have a valida access_token
                        //serialize the payment parameters
                        var payparameters = JsonConvert.SerializeObject(paymentParams);
                       var httpContent= new StringContent(payparameters, Encoding.UTF8, "application/json");

                        var uri = new UriBuilder(_merchantSettings.UseSandbox ?
                            "https://sandbox.safaricom.co.ke/mpesa/stkpush/v1/processrequest" :
                            "https://api.safaricom.co.ke/mpesa/stkpush/v1/processrequest");

                        httpClient.DefaultRequestHeaders
                            .Accept
                            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearertoken);

                        var response = await httpClient.PostAsync(uri.ToString(), httpContent);

                        //read the response data
                        if (response != null && response.IsSuccessStatusCode)
                        {
                            var result = response.Content.ReadAsStringAsync().Result;
                            if (result != null)

                                status = JsonConvert.DeserializeObject<Status>(result);
                        }
                        else
                        {
                            var result = response.Content.ReadAsStringAsync().Result;
                            if (result != null)

                                status = JsonConvert.DeserializeObject<Status>(result);
                        }
                         Console.WriteLine(response.Content);
                    }
                }
            }
            catch (Exception ex )
            {
                throw new ArgumentException ("The following error occured: " +ex);
            }
            return status;

        }

    }
}

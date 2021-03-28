using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Office365HealthPage.Models;

namespace Office365HealthPage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {

        private static List<MSService> serviceList = new List<MSService>();

        // GET: api/<ServicesController>
        [HttpGet]
        public string Get()
        
        {
            string resultList;
            
            try
            {
                RunAsync().GetAwaiter().GetResult();

                resultList = "{\"value\" :" + JsonConvert.SerializeObject(serviceList) + "}";
            }
            catch (Exception ex)
            {
                return ex.InnerException.ToString();
            }
            
            return resultList;

        }

                 
        private static async Task RunAsync()
        {
            AuthenticationConfig config = AuthenticationConfig.ReadFromJsonFile("appsettings.json");

            // You can run this sample using ClientSecret or Certificate. The code will differ only when instantiating the IConfidentialClientApplication
            bool isUsingClientSecret = AppUsesClientSecret(config);

            // Even if this is a console application here, a daemon application is a confidential client application
            IConfidentialClientApplication app;


            app = ConfidentialClientApplicationBuilder.Create(config.ClientId)
                .WithClientSecret(config.ClientSecret)
                .WithAuthority(new Uri(config.Authority))
                .Build();


            // With client credentials flows the scopes is ALWAYS of the shape "resource/.default", as the 
            // application permissions need to be set statically (in the portal or by PowerShell), and then granted by
            // a tenant administrator. 
            string[] scopes = new string[] { $"{config.ApiUrl}.default" };

            AuthenticationResult result = null;
            try
            {
                result = await app.AcquireTokenForClient(scopes)
                    .ExecuteAsync();
      
            }
            catch (MsalServiceException ex) when (ex.Message.Contains("AADSTS70011"))
            {
                // Invalid scope. The scope has to be of the form "https://resourceurl/.default"
                // Mitigation: change the scope to be as expected
                
                throw new Exception("Scope provided is not supported");
            }

            if (result != null)
            {
                var httpClient = new HttpClient();
                var apiCaller = new ProtectedApiCallHelper(httpClient);
                //await apiCaller.CallWebApiAndProcessResultASync($"{config.ApiUrl}v1.0/users", result.AccessToken, Display);
                await apiCaller.CallWebApiAndProcessResultASync($"{config.ApiUrl}/api/v1.0/{config.Tenant}/ServiceComms/CurrentStatus", result.AccessToken, BuildList);

            }
        }

        /// <summary>
        /// Display the result of the Web API call
        /// </summary>
        /// <param name="rawList">Object to display</param>
        private static void BuildList(JObject rawList)
        {
            serviceList.Clear();

            var svcList = JsonConvert.DeserializeObject<Example>(rawList.ToString());

            foreach (var svc in svcList.Value)
            {
                serviceList.Add(
                    new MSService
                    {
                        ServiceName = svc.Id,
                        Status = svc.Status
                    }
                    );
            }
        }

        private static bool AppUsesClientSecret(AuthenticationConfig config)
        {
            string clientSecretPlaceholderValue = "[Enter here a client secret for your application]";
            string certificatePlaceholderValue = "[Or instead of client secret: Enter here the name of a certificate (from the user cert store) as registered with your application]";

            if (!String.IsNullOrWhiteSpace(config.ClientSecret) && config.ClientSecret != clientSecretPlaceholderValue)
            {
                return true;
            }

            else if (!String.IsNullOrWhiteSpace(config.CertificateName) && config.CertificateName != certificatePlaceholderValue)
            {
                return false;
            }

            else
                throw new Exception("You must choose between using client secret or certificate. Please update appsettings.json file.");
        }

    }

    
}
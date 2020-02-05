using IMS.Entities;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Core.services
{
    public class NotificationProvider : INotificationProvider
    {
        private IConfiguration _configuration;
        private ILogManager _logger;
        public NotificationProvider(IConfiguration configuration, ILogManager logger)
        {
            _configuration = configuration;
            _logger = logger;

        }
        public async Task<bool> SendEmail(Email email)
        {
            try
            {
                HttpClient client = new HttpClient();
                JObject emailJson = JsonMaker(email);
                var content = new StringContent(emailJson.ToString()+"hello"+ _configuration["docker"]+"hello", Encoding.UTF8, "application/json");
                var result = client.PostAsync(_configuration["baseURL"]+"/api/Notification", content).Result;
                if (result.IsSuccessStatusCode)
                {
                    string responseString = result.Content.ReadAsStringAsync().Result;
                    Response response = result.Content.ReadAsAsync<Response>().Result;
                    if (response.Status == Status.Success)
                        return true;
                }
                return false;
            }
            catch(Exception exception)
            {
                Error error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.UnableToPlaceOrder);
                new Task(() => { _logger.LogException(exception, "SendEmail", IMS.Entities.Severity.Medium, email, false); }).Start();
                throw exception;
            }
        }
        private JObject JsonMaker(Email email)
        {
            string jsonString = JsonConvert.SerializeObject(email);
            JObject Json = JObject.Parse(jsonString);
            return Json;
        }
    }
}

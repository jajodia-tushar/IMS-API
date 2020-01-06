using IMS.Entities;
using IMS.Entities.Interfaces;
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
        public async Task<bool> SendEmail(Email email)
        {
            try
            {
                HttpClient client = new HttpClient();
                JObject emailJson = JsonMaker(email);
                var content = new StringContent(emailJson.ToString(), Encoding.UTF8, "application/json");
                var result = client.PostAsync("http://localhost:52950/api/Notification", content).Result;
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

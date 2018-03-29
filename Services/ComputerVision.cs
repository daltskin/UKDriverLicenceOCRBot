namespace UKDriverLicenceOCRBot
{
    using Newtonsoft.Json;
    using System;
    using System.Configuration;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    public static class ComputerVision
    {
        private static string UrlEndpoint = ConfigurationManager.AppSettings["ComputerVisionEndpoint"];
        private static string ApiKey = ConfigurationManager.AppSettings["ComputerVisionKey"];

        public static async Task<OCRResponse> GetOCRResult(Uri requestUri)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ApiKey);
                var payload = JsonConvert.SerializeObject(new {Url = requestUri});
                var stringContent = new StringContent(payload, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(UrlEndpoint, stringContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<OCRResponse>(responseString);
                    return result;
                }
            }
            return null;
        }
    }
}
  
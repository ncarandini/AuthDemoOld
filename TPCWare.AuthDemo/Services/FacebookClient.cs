// FacebookClient.cs /Users/biagioparuolo/Projects/AuthDemo/TPCWare.AuthDemo/Services
// Biagio Paruolo
// 20205272152
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Flurl;
using Flurl.Http;

using Newtonsoft.Json;

namespace TPCWare.AuthDemo.Services
{
    public class FacebookClient : IFacebookClient
    {
        private readonly HttpClient _httpClient;
        string _apiUrl = "https://graph.facebook.com/v5.0/";

        public FacebookClient()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://graph.facebook.com/v5.0/")
            };

            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<T> GetAsync<T>(string accessToken, string endpoint, string args = null)
        {
            var url = new Url(_apiUrl + $"{endpoint}?access_token={accessToken}&{args}");

            try
            {
                var response = await url.GetAsync();
                if (!response.IsSuccessStatusCode)
                    return default(T);

                var result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //var response = await _httpClient.GetAsync($"{endpoint}?access_token={accessToken}&{args}");

            return default(T);



        }

        public async Task PostAsync(string accessToken, string endpoint, object data, string args = null)
        {
            var payload = GetPayload(data);
            await _httpClient.PostAsync($"{endpoint}?access_token={accessToken}&{args}", payload);
        }

        private static StringContent GetPayload(object data)
        {
            var json = JsonConvert.SerializeObject(data);

            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}

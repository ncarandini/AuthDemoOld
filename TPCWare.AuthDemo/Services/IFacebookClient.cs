// IFacebookClient.cs /Users/biagioparuolo/Projects/AuthDemo/TPCWare.AuthDemo/Services
// Biagio Paruolo
// 20205272151
using System;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace TPCWare.AuthDemo.Services
{
    public interface IFacebookClient
    {
        Task<T> GetAsync<T>(string accessToken, string endpoint, string args = null);
        Task PostAsync(string accessToken, string endpoint, object data, string args = null);
    }
}

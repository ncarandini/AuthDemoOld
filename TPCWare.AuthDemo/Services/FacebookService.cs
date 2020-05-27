// FacebookService.cs /Users/biagioparuolo/Projects/AuthDemo/TPCWare.AuthDemo/Services
// Biagio Paruolo
// 20205272149
using System;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

using TPCWare.AuthDemo.Models;

namespace TPCWare.AuthDemo.Services
{
    public class FacebookService : IFacebookService
    {
        private readonly IFacebookClient _facebookClient;

        public FacebookService(IFacebookClient facebookClient)
        {
            _facebookClient = facebookClient;
        }

        public async Task<FBAccount> GetAccountAsync(string accessToken)
        {
            var account = new FBAccount();

            try
            {

                //var result = await _facebookClient.GetAsync<dynamic>(accessToken, "me", "fields=id,name,email,first_name,last_name,age_range,birthday,gender,locale,picture");


                var result = await _facebookClient.GetAsync<FBAccount>(
                              accessToken, "me", "fields=id,name,email,first_name,last_name,age_range,birthday,gender,locale,picture");

                if (result == null)
                {
                    return new FBAccount();
                }

                account = result;

                //account = new FBAccount
                //{
                //    Id = result.id,
                //    Email = result.email,
                //    Name = result.name,
                //    UserName = result.username,
                //    FirstName = result.first_name,
                //    LastName = result.last_name,
                //    Locale = result.locale,
                //    //Picture = result.picture
                //};

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            return account;
        }

        public async Task PostOnWallAsync(string accessToken, string message)
            => await _facebookClient.PostAsync(accessToken, "me/feed", new { message });
    }
}

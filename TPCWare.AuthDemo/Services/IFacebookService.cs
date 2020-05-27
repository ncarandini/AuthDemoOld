// IFacebookService.cs /Users/biagioparuolo/Projects/AuthDemo/TPCWare.AuthDemo/Services
// Biagio Paruolo
// 20205272148
using System;
using System.Threading;
using System.Threading.Tasks;

using TPCWare.AuthDemo.Models;

namespace TPCWare.AuthDemo.Services
{

    public interface IFacebookService
    {
        Task<FBAccount> GetAccountAsync(string accessToken);
        Task PostOnWallAsync(string accessToken, string message);
    }

}

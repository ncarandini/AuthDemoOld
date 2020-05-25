using System;
using System.Threading.Tasks;
using TPCWare.AuthDemo.Models;

namespace TPCWare.AuthDemo.Services
{
    public interface IAuthenticationService
    {
        event EventHandler AuthenticationStateChanged;

        UserContext UserContext { get; }

        Task<bool> SignInAsync();
        Task<bool> ResetPasswordAsync();
        Task<bool> EditProfileAsync();
        Task<bool> SignOutAsync();
    }
}

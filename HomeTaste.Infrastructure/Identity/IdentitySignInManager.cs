using HomeTaste.Application.DTOs.Auth;
using HomeTaste.Application.Interfaces.Auth;
using HomeTaste.Domain.Entities;
using HomeTaste.Infrastructure.Identity.Entity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace HomeTaste.Infrastructure.Identity
{
    public class IdentitySignInManager : ISignInManager
    {
        private readonly SignInManager<IdentityApplicationUser> _signInManager;

        public IdentitySignInManager(SignInManager<IdentityApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<bool> CheckPasswordSignInAsync(ApplicationUser user, string password)
        {
            var identityUser = await _signInManager.UserManager.FindByIdAsync(user.Id!.ToString());
            if (identityUser == null) return false;

            var result = await _signInManager.CheckPasswordSignInAsync(identityUser, password, false);
            return result.Succeeded;
        }

    }
}

using HomeTaste.Domain.Entities;

namespace HomeTaste.Application.Interfaces.Auth
{
    public interface ISignInManager
    {
        Task<bool> CheckPasswordSignInAsync(ApplicationUser user, string password);
    }
}

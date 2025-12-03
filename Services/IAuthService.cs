using DocumentManagerApi.Models;
using DocumentManagerApi.Models.Dtos;

namespace DocumentManagerApi.Services
{
    public interface IAuthService
    {
        Task<User?> ValidateUserAsync(string username, string password);
        Task<LoginResponse?> LoginAsync(LoginRequest request);
    }
}

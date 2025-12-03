using DocumentManagerApi.Models;

namespace DocumentManagerApi.Helpers
{
    public interface IJwtHelper
    {
        string GenerateToken(User user);
    }
}

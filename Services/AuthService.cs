using DocumentManagerApi.Data;
using DocumentManagerApi.Helpers;
using DocumentManagerApi.Models;
using DocumentManagerApi.Models.Dtos;
using System.Data;
using System.Data.SqlClient;

namespace DocumentManagerApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IDatabaseHelper _db;
        private readonly IConfiguration _config;

        public AuthService(IDatabaseHelper db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task<User?> ValidateUserAsync(string username, string password)
        {
            string sql = @"SELECT Id, Username, PasswordHash, Role 
                           FROM Users 
                           WHERE Username = @u";

            var dt = await _db.ExecuteDataTableAsync(sql,
                new SqlParameter("@u", username));

            if (dt.Rows.Count == 0)
                return null;

            var row = dt.Rows[0];

            var user = new User
            {
                Id = Convert.ToInt32(row["Id"]),
                Username = row["Username"].ToString()!,
                PasswordHash = row["PasswordHash"].ToString()!,
                Role = row["Role"].ToString()!
            };

            // For interview: plain-text match (simple)
            if (user.PasswordHash != password)
                return null;

            return user;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            var user = await ValidateUserAsync(request.Username, request.Password);
            if (user == null) return null;

            var key = _config["Jwt:Key"];
            var token = JwtHelper.GenerateToken(user, key);

            return new LoginResponse
            {
                Token = token,
                Expires = DateTime.UtcNow.AddHours(3)
            };
        }
    }
}

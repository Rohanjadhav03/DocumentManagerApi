namespace DocumentManagerApi.Models.Dtos
{
    public class LoginResponse
    {
        public string Token { get; set; } = null!;
        public DateTime Expires { get; set; }
    }
}

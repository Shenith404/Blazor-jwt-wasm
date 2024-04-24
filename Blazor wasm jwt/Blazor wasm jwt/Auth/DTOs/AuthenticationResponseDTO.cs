namespace Blazor_wasm_jwt.Auth.DTOs
{
    public class AuthenticationResponseDTO
    {
        public string UserName { get; set; }
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
        public bool? IsLocked { get; set; }
        public bool? EmailConfirmed { get; set; }
        public string Message { get; set; }

    }
}

using Blazor_wasm_jwt.Auth.DTOs;

namespace Blazor_wasm_jwt.Auth.Services
{
    public interface IJwtAuthenticationService
    {
        ValueTask<string> GetJwtAsync();
        Task<string> LoginAsync(AuthenticationRequestDTO request);
        Task LogoutAcync();

        Task<UserModelResponseDTO?> GetUserDetails();

        public Task<bool> Refresh();
    }
}
using Blazor_wasm_jwt.Auth.DTOs;
using Blazored.SessionStorage;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;

namespace Blazor_wasm_jwt.Auth.Services
{
    public class JwtAuthenticationService : IJwtAuthenticationService
    {
        private readonly IHttpClientFactory _factory;
        private readonly ISessionStorageService _sessionStorageService;

        private string? _jwtCache;
        private const string JWT_KEY = nameof(JWT_KEY);
        private const string RERESH_KEY = nameof(RERESH_KEY);

        public JwtAuthenticationService(IHttpClientFactory factory, ISessionStorageService sessionStorageService)
        {
            _factory = factory;
            _sessionStorageService = sessionStorageService;

        }

        //Get Jwt Token
         public async ValueTask<string> GetJwtAsync()
        {
            if (string.IsNullOrEmpty(_jwtCache))
                _jwtCache = await _sessionStorageService.GetItemAsync<string>(JWT_KEY);

            return await _sessionStorageService.GetItemAsync<string>(JWT_KEY); 
        }

        // Logout User
        public async Task LogoutAcync()
        {
            await _sessionStorageService.RemoveItemAsync(JWT_KEY);
            await _sessionStorageService.RemoveItemAsync(RERESH_KEY);

            _jwtCache = null;
            await Console.Out.WriteAsync("logout");
        }

        //Login User
        public async Task<string> LoginAsync(AuthenticationRequestDTO request)
        {
            var result = await _factory.CreateClient("ServerApi").PostAsJsonAsync("api/Account/Login", request);
            var content = await result.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<AuthenticationResponseDTO>(content);
            if (result.IsSuccessStatusCode)
            {
                
                if (response!.JwtToken != null)
                {
                    //store the token in session storage
                    await _sessionStorageService.SetItemAsync(JWT_KEY, response.JwtToken);
                    await _sessionStorageService.SetItemAsync(RERESH_KEY, response.RefreshToken);
                }

                return response.Message;
            }

            return response!.Message;

        }

        public async Task<UserModelResponseDTO?> GetUserDetails()
        {
            
            var result = await _factory.CreateClient("ServerApi").GetAsync("api/Account/Get-User-Details");
           
            if (result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserModelResponseDTO>(content);
            }

            return null;

        }

        public async Task<bool> Refresh()
        {
            var request = new TokenInfoDTO
            {
                JwtToken = await _sessionStorageService.GetItemAsync<string>(JWT_KEY),
                RefreshToken = await _sessionStorageService.GetItemAsync<string>(RERESH_KEY)
            };

            var result = await _factory.CreateClient("ServerApi").PostAsJsonAsync("api/Account/Request-RefreshToken", request);

            if(result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<AuthenticationResponseDTO>(content);

                await _sessionStorageService.SetItemAsync(JWT_KEY, response!.JwtToken);
                await _sessionStorageService.SetItemAsync(RERESH_KEY, response.RefreshToken);

                _jwtCache = response.JwtToken;

               return true;

            }

            //await LogoutAcync();
            await Console.Out.WriteAsync("refresh fail");

            return false;
        }
    }
}

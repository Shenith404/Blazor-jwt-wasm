using Blazor_wasm_jwt.Auth.Services;

namespace Blazor_wasm_jwt.Auth.Handler
{
    public class JwtAuthenticationHandler : DelegatingHandler
    {
        private readonly IJwtAuthenticationService _jwtAuthenticationService;
        private readonly IConfiguration _configuration;
        private bool _refreshing;

        public JwtAuthenticationHandler(IJwtAuthenticationService jwtAuthenticationService,
            IConfiguration configuration)
        {
            _jwtAuthenticationService = jwtAuthenticationService;
            _configuration = configuration;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Retrieve JWT token
            var jwt = await _jwtAuthenticationService.GetJwtAsync();
            Console.WriteLine(jwt);

            await Console.Out.WriteAsync("tst");

            // Set Authorization header if JWT token is not empty or null
            if (!string.IsNullOrEmpty(jwt))
            {
                 
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
            }

            // Call the base SendAsync method to continue processing the request pipeline
            var response = await base.SendAsync(request, cancellationToken);

            if(!string.IsNullOrEmpty(jwt) && response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                try
                {
                    _refreshing = true;
                    if(await _jwtAuthenticationService.Refresh())
                    {
                        jwt = await _jwtAuthenticationService.GetJwtAsync();
                        Console.WriteLine(jwt);

                        // Set Authorization header if JWT token is not empty or null
                        if (!string.IsNullOrEmpty(jwt))
                        {

                            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
                        }

                        // Call the base SendAsync method to continue processing the request pipeline
                         response = await base.SendAsync(request, cancellationToken);

                    }
                }
                finally
                {
                    _refreshing=false;
                }
            }

            return response;

        }
    }
}

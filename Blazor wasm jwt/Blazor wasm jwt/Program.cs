using Blazor_wasm_jwt;
using Blazor_wasm_jwt.Auth.Handler;
using Blazor_wasm_jwt.Auth.Services;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddTransient<JwtAuthenticationHandler>();

builder.Services.AddHttpClient("ServerApi", client => client.BaseAddress = new Uri("http://localhost:5261"))
                .AddHttpMessageHandler<JwtAuthenticationHandler>();

builder.Services.AddSingleton<IJwtAuthenticationService, JwtAuthenticationService>();
builder.Services.AddBlazoredSessionStorageAsSingleton();
await builder.Build().RunAsync();

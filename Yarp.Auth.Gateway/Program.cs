using Microsoft.AspNetCore.Authentication.BearerToken;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services
    .AddAuthentication(BearerTokenDefaults.AuthenticationScheme)
    .AddBearerToken();

builder.Services.
    AddAuthorization(o =>
    {
        o.AddPolicy("first-api-access",
            policy => policy.RequireAuthenticatedUser().RequireClaim("first-api-access", true.ToString()));

        o.AddPolicy("second-api-access",
            policy => policy.RequireAuthenticatedUser().RequireClaim("second-api-access", true.ToString()));
    });

var app = builder.Build();

app.MapGet("login", (bool firstApi = false, bool secondApi = false) =>
    Results.SignIn(
        new ClaimsPrincipal(
            new ClaimsIdentity(
                [
                    new Claim("sub", Guid.NewGuid().ToString()),
                    new Claim("first-api-access", firstApi.ToString()),
                    new Claim("second-api-access", secondApi.ToString())
                ],
                BearerTokenDefaults.AuthenticationScheme)),
        authenticationScheme: BearerTokenDefaults.AuthenticationScheme
        ));

app.UseAuthentication();

app.UseAuthorization();

app.MapReverseProxy();

app.Run();


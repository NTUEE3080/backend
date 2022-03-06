using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace PitaPairing.Auth;

public class AuthPolicy
{
  public const string Admin = "Admin";
}

public static class AuthServiceExt
{
  private static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IAuth auth)
  {
    services
      .AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      })
      .AddJwtBearer(options =>
      {
        options.Authority = auth.Authority;
        options.Audience = auth.Audience;
        options.TokenValidationParameters = new TokenValidationParameters
        {
          NameClaimType = ClaimTypes.NameIdentifier,
          ValidateLifetime = true,
          ValidateIssuer = true,
          ValidateAudience = true,
          ClockSkew = TimeSpan.Zero,
          ValidateIssuerSigningKey = true,
        };
      });

    return services;
  }

  private static IServiceCollection AddAuthorizationServices(this IServiceCollection services, IAuth auth)
  {
    
    services.AddSingleton<IAuthorizationHandler, LambdaScopeHandler>();
    services.AddAuthorization(
      o => o.AddScopePolicy(AuthPolicy.Admin, auth.Authority,
        scopes =>
        {
          var allowed = scopes.Any(scope => scope == "admin");
          Log.Information("Allowed: {@Allowed}", allowed);
          return allowed;
        })
    );
    return services;
  }
  
  public static IApplicationBuilder UseAtomiAuthService(this IApplicationBuilder app)
  {
    return app
      .UseAuthentication()
      .UseAuthorization();
  }

  public static IServiceCollection AddAtomiAuthServices(this IServiceCollection services, IAuth auth)
  {
    return services
      .AddAuthenticationServices(auth)
      .AddAuthorizationServices(auth);
  }
}
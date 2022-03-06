using Microsoft.AspNetCore.Authorization;
using Serilog;

namespace PitaPairing.Auth;

public class ScopeFilterRequirement : IAuthorizationRequirement
{
  public ScopeFilterRequirement(string issuer, Func<IEnumerable<string>, bool> filter)
  {
    Issuer = issuer;
    Filter = enumerable => Task.FromResult(filter(enumerable));
  }

  public ScopeFilterRequirement(string issuer, Func<IEnumerable<string>, Task<bool>> filter)
  {
    Issuer = issuer;
    Filter = filter;
  }

  public string Issuer { get; }
  public Func<IEnumerable<string>, Task<bool>> Filter { get; }
}

public class LambdaScopeHandler : AuthorizationHandler<ScopeFilterRequirement>
{
  protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
    ScopeFilterRequirement requirement)
  {
    // Split the scopes string into an array
    var scopes = context.User.FindFirst(c => c.Type == "scope" && c.Issuer == requirement.Issuer)?.Value.Split(' ');
    Log.Debug("Scopes: {@Scopes}", scopes);
    // If no such "scope" claim 
    if (scopes == null) return;

    // Evaluate lambda filter
    var result = await requirement.Filter(scopes);
    Log.Debug("Scope Result: {@Result}", result);
    // Succeed if the the filters decide that the scopes are satisfactory
    if (result) context.Succeed(requirement);
  }
}
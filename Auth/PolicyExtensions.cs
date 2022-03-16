using Microsoft.AspNetCore.Authorization;

namespace PitaPairing.Auth;

public static class PolicyExtensions
{
    public static void AddScopePolicy(this AuthorizationOptions o, string policyName, string authority,
      Func<IEnumerable<string>, bool> filter)
    {
        o.AddPolicy(policyName, p =>
          p.Requirements.Add(
            new ScopeFilterRequirement(authority, filter)
          )
        );
    }
}

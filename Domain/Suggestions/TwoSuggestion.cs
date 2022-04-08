using PitaPairing.Domain.Post;
using PitaPairing.User;

namespace PitaPairing.Domain.Suggestions;

public record TwoWaySuggestion(Guid Id, long Counter, UserPrincipal User, PostPrincipal Post1, PostPrincipal Post2);

public record TwoWaySuggestionResp(Guid Id, long Counter, string Module, UserPrincipalResp User, PostPrincipalResp Post1,
    PostPrincipalResp Post2);

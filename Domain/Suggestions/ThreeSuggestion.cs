using PitaPairing.Domain.Post;
using PitaPairing.User;

namespace PitaPairing.Domain.Suggestions;

public record ThreeWaySuggestion(Guid Id, long Counter, UserPrincipal User,
    PostPrincipal Post1, PostPrincipal Post2, PostPrincipal Post3);

public record ThreeWaySuggestionResp(Guid Id, long Counter, UserPrincipalResp User, PostPrincipalResp Post1,
    PostPrincipalResp Post2, PostPrincipalResp Post3);

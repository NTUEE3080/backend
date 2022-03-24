using PitaPairing.Domain.Application;
using PitaPairing.User;
using PitaPairing.Domain.Index;
using PitaPairing.Domain.Module;

namespace PitaPairing.Domain.Post;

public record PostProps(
    UserPrincipal Owner,
    ModulePrincipal Module,
    IndexPrincipal Index,
    IEnumerable<IndexPrincipal> LookingFor,
    bool Completed);

public record PostPrincipal(
    Guid Id,
    PostProps Props
);

public record TradePrincipal(
    PostPrincipal Principal,
    ApplicationStatus Status
    );

public record Post(PostPrincipal Principal, IEnumerable<TradePrincipal> Offers, IEnumerable<TradePrincipal> Applied);

public record CompletedPost(PostPrincipal Principal, PostPrincipal Traded);

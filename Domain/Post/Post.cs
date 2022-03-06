using PitaPairing.Domain.Application;
using PitaPairing.User;
using PitaPairing.Domain.Index;
using PitaPairing.Domain.Module;

namespace PitaPairing.Domain.Post;

public record PostProps(
  ModulePrincipal Module,
  IndexPrincipal Index,
  IEnumerable<IndexPrincipal> LookingFor,
  bool Completed);

public record PostPrincipal(
  Guid Id,
  PostProps Props
);

public record Post(UserPrincipal User, PostPrincipal Principal, IEnumerable<ApplicationPrincipal> Applications);


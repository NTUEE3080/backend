using PitaPairing.Domain.Post;

namespace PitaPairing.Domain.Application;

public record Application(Guid Id, PostPrincipal Post, PostPrincipal Applied, ApplicationStatus Status);

public record ApplicationRes(Guid Id, PostPrincipalResp Post, PostPrincipalResp Applied, string Status);

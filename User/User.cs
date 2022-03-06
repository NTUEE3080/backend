namespace PitaPairing.User;

public record UserProps(string Name, string Email, string Sub);
public record UserPrincipal(Guid Id, UserProps Props);
public record CreateUserReq(string Name, string Email);
public record UpdateUserReq(string Name, string Email);
public record UserPrincipalResp(Guid Id, string Name, string Email, string Sub);
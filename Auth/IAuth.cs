namespace PitaPairing.Auth;

public interface IAuth
{
  public string Audience { get; }
  public string Authority { get; }
  
}

public class Auth0: IAuth
{
  private readonly IConfiguration _config;
  public Auth0(IConfiguration config)
  {
    _config = config;
  }
  public string Audience => _config["AUTH0_AUDIENCE"] ?? throw new ApplicationException("Auth0 Audience not set");
  public string Authority => _config["AUTH0_AUTHORITY"] ?? throw new ApplicationException("Auth0 Authority not set");
  
}
namespace PitaPairing.StartUp;

public interface IGlobal
{
    /// <summary>
    /// Whether to automatically run migration
    /// </summary>
    public bool AutoMigration { get; }

    /// <summary>
    /// Number of attempts to connect to database
    /// </summary>
    public int DatabaseReconnectionAttempt { get; }

    /// <summary>
    /// Whether the environment is local
    /// </summary>
    public bool LocalEnv { get; }

    /// <summary>
    /// Connection string for database
    /// </summary>
    public string ConnectionString { get; }

    public string FCMKey { get; }
}

public class Global : IGlobal
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<Global> _l;

    public Global(IConfiguration configuration, IWebHostEnvironment env, ILogger<Global> l)
    {
        _configuration = configuration;
        _env = env;
        _l = l;
    }

    public bool AutoMigration => _configuration["MIGRATE"]?.ToLower() == "auto";

    public int DatabaseReconnectionAttempt => int.Parse(_configuration["DB_CONNECT_TRIES"] ?? "50");
    public bool LocalEnv => _configuration["DEV_ENV"]?.ToLower() == "local";

    public string ConnectionString
    {
        get
        {
            _l.LogInformation("Database Environment Check, Is Development: {@IsDevelopment}", _env.IsDevelopment());
            if (!_env.IsDevelopment())
                return _configuration["CONNECTION_STRING"] ??
                       throw new ApplicationException("No connection string specified");
            var key = LocalEnv ? "Local" : "Postgres";
            _l.LogInformation("Development Env Type {@DevEnvType}", key);
            return _configuration.GetConnectionString(key);
        }
    }

    public string FCMKey => _configuration["FCM_KEY"] ?? throw new ApplicationException("No FCM Key specified");
}

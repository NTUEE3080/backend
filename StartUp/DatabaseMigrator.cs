using Microsoft.EntityFrameworkCore;
using PitaPairing.Database;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PitaPairing.StartUp;

public class DbMigratorHostedService : IHostedService
{
    private readonly IGlobal _global;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ILogger<DbMigratorHostedService> _log;

    public DbMigratorHostedService(IServiceProvider serviceProvider, IGlobal global,
        IHostApplicationLifetime lifetime, ILogger<DbMigratorHostedService> log)
    {
        _serviceProvider = serviceProvider;
        _global = global;
        _lifetime = lifetime;
        _log = log;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_global.AutoMigration)
        {
            _log.LogInformation("Automatic programmatic migration is not enabled");
            return;
        }
        using var scope = _serviceProvider.CreateScope();
        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
            var timeout = _global.DatabaseReconnectionAttempt;
            var tries = 0;
            var canConnect = false;
            while (!canConnect)
            {
                if (tries > timeout)
                {
                    _log.LogCritical("Failed to contact DB, exiting application");
                    Environment.ExitCode = 1;
                    _lifetime.StopApplication();
                    break;
                }

                _log.LogInformation("Attempting to contact database: Attempt #{@int}", tries);
                try
                {
                    canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);
                }
                catch (Exception e)
                {
                    _log.LogInformation("Cannot Connect: {@String}", e.Message);
                    _log.LogInformation("Waiting for new attempt...");
                    Thread.Sleep(1000);
                }

                if (!canConnect)
                {
                    _log.LogInformation("Cannot Connect: {@String}", "Unknown");
                    _log.LogInformation("Waiting for new attempt...");
                    Thread.Sleep(1000);
                }

                tries++;
            }

            _log.LogInformation("Contacted Database Successfully!");
            await dbContext.Database.MigrateAsync(cancellationToken);
            _log.LogInformation("Database migrated successfully");
        }
        catch (Exception e)
        {
            _log.LogCritical("Database migration failed: {msg}", e.Message);
            _log.LogCritical("{StackTrace}", e?.StackTrace);
            Environment.ExitCode = 1;
            _lifetime.StopApplication();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

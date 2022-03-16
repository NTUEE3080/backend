using Serilog;
using Serilog.Events;

namespace PitaPairing.StartUp;
public static class Logger
{
    public static ILoggerFactory Initialize()
    {
        var logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        Log.Logger = logger;
        var loggerFactory = new LoggerFactory().AddSerilog(logger);
        return loggerFactory;
    }
}

using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using PitaPairing.Database;
using PitaPairing.Domain.Index;

namespace PitaPairing.StartUp;

public static class ServicesExt
{
    public static void AddAppServices(this IServiceCollection services, IGlobal global)
    {
        // System-level 
        services.AddSingleton<IGlobal>(global);

        // Database services
        services.AddDbContext<CoreDbContext>(options =>
            options.UseNpgsql(global.ConnectionString));
        services.AddHostedService<DbMigratorHostedService>();

        // Validation Services
        // services.AddTransient<IValidator<CreateIndexReq>, IndexValidator>();

        // Other Services
    }
}
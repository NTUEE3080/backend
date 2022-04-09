using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hellang.Middleware.ProblemDetails;
using PitaPairing.Auth;
using PitaPairing.Domain.Index;
using PitaPairing.Errors;
using PitaPairing.StartUp;
using Serilog;

var loggerFactory = Logger.Initialize();
Log.Information("Starting Building web host..");

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
// Configuration
builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddEnvironmentVariables();
    config.AddCommandLine(args);
});

var globalLogger = loggerFactory.CreateLogger<Global>();
var global = new Global(builder.Configuration, builder.Environment, globalLogger);
var auth = new Auth0(builder.Configuration);




// Add framework services to the container.
var detailMiddleware = ErrorMiddleware.ConfigureProblemDetails(builder.Environment);
builder.Services.AddProblemDetails(detailMiddleware);
builder.Services.AddControllers();
builder.Services.AddFluentValidation(fv =>
{
    fv.RegisterValidatorsFromAssembly(Assembly.GetEntryAssembly());
    fv.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    fv.ImplicitlyValidateChildProperties = true;
});
builder.Services.AddScoped<IValidator<CreateIndexReq>, IndexValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add us-defined services
builder.Services.AddAppServices(global);
builder.Services.AddAtomiAuthServices(auth);
builder.Services.AddHttpClients(global);


var app = builder.Build();

Log.Information("Done!");

app.UseProblemDetails();
app.UseSerilogRequestLogging();
app.UseAtomiAuthService();

app.MapControllers();
app.UseSwagger().UseSwaggerUI();

Log.Information("Starting server...");
app.Run();

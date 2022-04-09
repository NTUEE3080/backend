using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PitaPairing.Database;
using PitaPairing.Errors;

namespace PitaPairing.User;

[ApiController]
[Authorize]
[Route("[controller]")]
public class DeviceController : ControllerBase
{
    private readonly ILogger<DeviceController> _logger;
    private readonly CoreDbContext _db;

    public DeviceController(ILogger<DeviceController> logger, CoreDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    [HttpPost("{token}")]
    public async Task<ActionResult> Register(string token)
    {
        try
        {
            var (guid, _) = await GetUser();

            await _db.Devices.Upsert(
                    new DeviceData
                    {
                        Id = Guid.NewGuid(),
                        UserId = guid,
                        DeviceToken = token,
                        TimeStamp = DateTime.Now.Ticks,
                    }
                )
                .On(x => new { x.DeviceToken })
                .WhenMatched(x => new
                    DeviceData
                {
                    UserId = x.UserId,
                    DeviceToken = x.DeviceToken,
                    TimeStamp = x.TimeStamp
                })
                .RunAsync();


            return NoContent();
        }
        catch (Exception e)
        {
            if (e is not BaseErrorException) _logger.LogCritical(e, "Fail to retrieve list of users");
            throw;
        }
    }

    [HttpDelete("{token}")]
    public async Task<ActionResult> Deregister(string token)
    {
        try
        {
            var (guid, _) = await GetUser();

            var f = await _db.Devices.FirstOrDefaultAsync(x => x.UserId == guid && x.DeviceToken == token);
            if (f == null) return NoContent();

            _db.Remove(f);
            await _db.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception e)
        {
            if (e is not BaseErrorException) _logger.LogCritical(e, "Fail to retrieve list of users");
            throw;
        }
    }


    [NonAction]
    private async Task<UserPrincipal> GetUser()
    {
        var sub = Sub();
        if (sub == null)
            throw new UnauthorizedError("Unauthorized",
                "Attempt to access protected API without valid Bearer containing 'sub'");
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Sub == sub);
        if (user == null)
            throw new UserNotRegisteredError("User Not Registered",
                "User may have been registered on Auth server but not in backend server");
        return user.ToPrincipal();
    }

    [NonAction]
    private bool IsAdmin()
    {
        var scopes = HttpContext.User.FindFirst(c => c.Type == "scope")?.Value.Split(' ');
        return scopes != null && scopes.Any(s => s == "admin");
    }

    [NonAction]
    private string? Sub()
    {
        return HttpContext.User?.Identity?.Name;
    }
}

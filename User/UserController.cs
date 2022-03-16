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
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly CoreDbContext _db;

    public UserController(ILogger<UserController> logger, CoreDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    [HttpGet]
    public ActionResult<IEnumerable<UserPrincipalResp>> GetAll()
    {
        try
        {
            var allUsers = _db.Users
                .Select(x => x.ToPrincipal())
                .Select(x => x.ToResp());
            return Ok(allUsers);
        }
        catch (Exception e)
        {
            if (e is not BaseErrorException) _logger.LogCritical(e, "Fail to retrieve list of users");
            throw;
        }
    }

    [HttpGet("self")]
    public async Task<ActionResult<UserPrincipalResp>> Self()
    {
        try
        {
            var sub = Sub();
            _logger.LogInformation("Sub: {@Sub}", sub);
            if (sub == null)
                throw new UnauthorizedError("Unauthorized",
                    "Attempt to access protected API without valid Bearer containing 'sub'");
            var user = await _db.Users
                .FirstOrDefaultAsync(x => x.Sub == sub);
            if (user == null) throw new NotFoundError("User not found", $"User with Sub '{sub}' does not exist");
            var resp = user.ToPrincipal()
                .ToResp();
            return Ok(resp);
        }
        catch (Exception e)
        {
            if (e is not BaseErrorException) _logger.LogCritical(e, "Fail to retrieve list of users");
            throw;
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserPrincipalResp>> Get(Guid id)
    {
        try
        {
            var sub = Sub();
            _logger.LogInformation("Sub: {@Sub}", sub);
            if (sub == null)
                throw new UnauthorizedError("Unauthorized",
                    "Attempt to access protected API without valid Bearer containing 'sub'");
            var user = await _db.Users
                .FirstOrDefaultAsync(x => x.Id == id);
            if (user == null) throw new NotFoundError("User not found", $"User with Id '{id}' does not exist");
            var resp = user.ToPrincipal()
                .ToResp();
            return Ok(resp);
        }
        catch (Exception e)
        {
            if (e is not BaseErrorException) _logger.LogCritical(e, "Fail to retrieve list of users");
            throw;
        }
    }

    [HttpPost]
    public async Task<ActionResult<UserPrincipalResp>> Create([FromBody] CreateUserReq req)
    {
        var sub = Sub();
        _logger.LogInformation("Sub: {@Sub}", sub);
        try
        {
            if (sub == null)
                throw new UnauthorizedError("Unauthorized",
                    "Attempt to access protected API without valid Bearer containing 'sub'");

            var d = req.ToDomain(sub).ToData();
            var r = await _db.Users.AddAsync(d);
            await _db.SaveChangesAsync();
            return Ok(r.Entity.ToPrincipal().ToResp());
        }
        catch (Exception ex)
        {
            var r = ex switch
            {
                DbUpdateException
                {
                    InnerException: PostgresException { SqlState: "23505", ConstraintName: "IX_Users_Sub" }
                } =>
                    new UniqueConflictError("User already exist",
                            "A user with this 'sub' field already exist")
                        .AddField("sub", $"User with sub '{sub}' already exist"),
                DbUpdateException
                {
                    InnerException: PostgresException { SqlState: "23505", ConstraintName: "IX_Users_Email" }
                } =>
                    new UniqueConflictError("User already exist",
                            "A user with this 'email' field already exist")
                        .AddField("sub", $"User with email '{req.Email}' already exist"),
                _ => null
            };
            if (r != null) throw r;
            if (ex is not BaseErrorException)
                _logger.LogCritical(ex, "Exception occurred when creating user with {@Req}", req);
            throw;
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserPrincipalResp>> Update(Guid id, [FromBody] UpdateUserReq req)
    {
        var sub = Sub();
        _logger.LogInformation("Sub: {@Sub}", sub);
        try
        {
            if (sub == null)
                throw new UnauthorizedError("Unauthorized",
                    "Attempt to access protected API without valid Bearer containing 'sub'");

            var d = req.ToDomain(sub);
            var user = await _db.Users
                .FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                throw new NotFoundError("User not found", $"Cannot update user with Id '{id}' as it does not exist");

            if (user.Sub != sub && !IsAdmin())
                throw new NoPermissionError("No permission", "Do not have permission to update the user");

            if (user.Email != req.Email)
                throw new UnsupportedOperationError("Not allowed to change email",
                    "Email field is only used for validation");
            user.FromDomain(d);
            await _db.SaveChangesAsync();
            return Ok(user.ToPrincipal().ToResp());
        }
        catch (Exception ex)
        {
            if (ex is not BaseErrorException)
                _logger.LogCritical(ex, "Fail to update user with id {@Id} with req {@Req}", id, req);
            throw;
        }
    }


    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var sub = Sub();
        _logger.LogInformation("Sub: {@Sub}", sub);
        try
        {
            if (sub == null)
                throw new UnauthorizedError("Unauthorized",
                    "Attempt to access protected API without valid Bearer containing 'sub'");

            var user = await _db.Users
                .FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                throw new NotFoundError("User not found", $"Cannot delete user with Id '{id}' as it does not exist");

            if (user.Sub != sub && !IsAdmin())
                throw new NoPermissionError("No permission", "Do not have permission to delete the user");

            _db.Remove(user);
            await _db.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            if (ex is not BaseErrorException) _logger.LogCritical(ex, "Fail to delete user of id {@Id}", id);
            throw;
        }
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

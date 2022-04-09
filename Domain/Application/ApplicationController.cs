using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PitaPairing.Database;
using PitaPairing.Errors;
using PitaPairing.User;

namespace PitaPairing.Domain.Application;

[ApiController]
[Authorize]
[Route("[controller]")]
public class ApplicationController : ControllerBase
{
    private readonly ILogger<ApplicationController> _l;
    private readonly CoreDbContext _db;
    private readonly INotificationService _n;

    public ApplicationController(CoreDbContext db, ILogger<ApplicationController> l, INotificationService n)
    {
        _db = db;
        _l = l;
        _n = n;
    }

    [HttpGet]
    public ActionResult<IEnumerable<ApplicationRes>> GetAll(Guid? posterId, Guid? applierId, string? status)
    {
        try
        {
            var appls = _db.Applications

                // post
                .Include(x => x.Post).ThenInclude(p => p.Index).ThenInclude(i => i.Info)
                .Include(x => x.Post).ThenInclude(p => p.Module)
                .Include(x => x.Post).ThenInclude(p => p.User)
                .Include(x => x.Post).ThenInclude(p => p.LookingFor).ThenInclude(i => i.Info)

                // applied
                .Include(x => x.ApplierPost).ThenInclude(p => p.Index).ThenInclude(i => i.Info)
                .Include(x => x.ApplierPost).ThenInclude(p => p.Module)
                .Include(x => x.ApplierPost).ThenInclude(p => p.User)
                .Include(x => x.ApplierPost).ThenInclude(p => p.LookingFor).ThenInclude(i => i.Info)
                .AsQueryable();

            var dmStatus = status?.ToDomain();
            var dStatus = dmStatus?.ToData();

            if (posterId != null) appls = appls.Where(x => x.Post.UserId == posterId);
            if (applierId != null) appls = appls.Where(x => x.ApplierPost.UserId == applierId);
            if (dStatus != null) appls = appls.Where(x => x.Status == dStatus);

            return Ok(appls.Select(x => x.ToDomain().ToResp()));
        }
        catch (Exception e)
        {
            if (e is not BaseErrorException)
                _l.LogCritical(e,
                    "Failed to list applications. Query:  PosterId: {@PosterId}, ModuleId: {@ModuleId}, IndexId: {@IndexId}",
                    posterId, applierId, status);
            throw;
        }
    }

    [HttpPost("{postId:guid}/{appId:guid}")]
    public async Task<ActionResult> Apply(Guid postId, Guid appId)
    {
        try
        {
            var user = await GetUser();
            var post = await _db.Posts.FirstOrDefaultAsync(x => x.Id == appId);
            if (post == null)
                throw new NotFoundError("Post Not Found", $"Applying post with id {appId} does not exist");
            if (post.UserId != user.Id)
                throw new NoPermissionError("No permission", "Not owner of applying post");

            // create new application
            var appl = new ApplicationData
            {
                PostId = postId,
                ApplierPostId = appId,
                Status = ApplicationStatus.Pending.ToData(),
            };
            await _db.Applications.AddAsync(appl);
            await _db.SaveChangesAsync();

            var p = await _db.Posts
                .Include(x => x.Module)
                .FirstOrDefaultAsync(x => x.Id == postId);
            var notif = new PushNotification(
                "Someone made an offer!",
                $"Someone made an offer for your index in {p!.Module.CourseCode}",
                "offer",
                "info"
            );
            await _n.Send(p.UserId, notif);


            return Ok();
        }
        catch (Exception e)
        {
            BaseErrorException? r = e switch
            {
                DbUpdateException
                {
                    InnerException: PostgresException
                    {
                        SqlState: "23503", ConstraintName: "FK_Applications_Posts_PostId"
                    }
                } =>
                    new NotFoundError("Post Not Found",
                        $"Post with id {postId} does not exist"),
                DbUpdateException
                {
                    InnerException: PostgresException
                    {
                        SqlState: "23505", ConstraintName: "IX_Applications_PostId_ApplierPostId"
                    }
                } =>
                    new UniqueConflictError("Application already exist",
                        $"A trade of '{postId}' => '{appId}' already exist"),
                _ => null,
            };
            if (r != null) throw r;
            if (e is not BaseErrorException)
                _l.LogCritical(e, "Failed to apply PostId: {@PostId}, AppId: {@AppId}", postId, appId);
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
    private string? Sub()
    {
        return HttpContext.User?.Identity?.Name;
    }
}

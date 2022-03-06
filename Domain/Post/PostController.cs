using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PitaPairing.Database;
using PitaPairing.Domain.Application;
using PitaPairing.Errors;
using PitaPairing.User;

namespace PitaPairing.Domain.Post;

[ApiController]
[Authorize]
[Route("[controller]")]
public class PostController : ControllerBase
{
    private readonly ILogger<PostController> _l;
    private readonly CoreDbContext _db;

    public PostController(ILogger<PostController> l, CoreDbContext db)
    {
        _l = l;
        _db = db;
    }


    [HttpGet]
    public ActionResult<IEnumerable<PostPrincipalResp>>
        GetAll(string? semester, Guid? posterId, Guid? moduleId, Guid? indexId, Guid? lookId)
    {
        try
        {
            var posts = _db.Posts
                .Include(x => x.Index)
                .ThenInclude(x => x.Info)
                .Include(x => x.LookingFor)
                .ThenInclude(x => x.Info)
                .Include(x => x.Module)
                .AsQueryable();

            if (semester != null) posts = posts.Where(x => x.Module.Semester == semester);
            if (posterId != null) posts = posts.Where(x => x.UserId == posterId);
            if (moduleId != null) posts = posts.Where(x => x.ModuleId == moduleId);
            if (indexId != null) posts = posts.Where(x => x.IndexId == indexId);
            if (lookId != null) posts = posts.Where(x => x.LookingFor.Any(index => index.Id == lookId));

            return Ok(posts.Select(x => x.ToPrincipal().ToResp()));
        }
        catch (Exception e)
        {
            if (e is not BaseErrorException)
                _l.LogCritical(e,
                    "Failed to list posts. Query:  PosterId: {@PosterId}, ModuleId: {@ModuleId}, IndexId: {@IndexId}, LookForIndexId: {@LookForId}",
                    posterId, moduleId, indexId, lookId);
            throw;
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostResp>> Get(Guid id)
    {
        try
        {
            var post = await _db.Posts
                // user principal
                .Include(x => x.User)

                // post principal
                .Include(x => x.Index) // index principal
                .ThenInclude(x => x.Info)
                .Include(x => x.Module) // module principal
                .Include(x => x.LookingFor) // index list
                .ThenInclude(x => x.Info)

                // application
                .Include(x => x.Applications)
                .ThenInclude(x => x.User)
                .Include(x => x.Applications)
                .ThenInclude(x => x.Offers)
                .ThenInclude(x => x.Info)
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Id == id);
            if (post == null) throw new NotFoundError("Post Not Found", $"Post of id '{id}' does not exist");
            return Ok(post.ToPrincipal().ToResp());
        }
        catch (Exception e)
        {
            if (e is not BaseErrorException) _l.LogCritical(e, "Failed to get posts. PostId: {@PostId}", id);
            throw;
        }
    }

    [HttpPost("reject/{postId:guid}/{appId:guid}")]
    public async Task<ActionResult> Reject(Guid postId, Guid appId)
    {
        try
        {
            var (guid, _) = await GetUser();
            var post = await _db.Posts
                .Include(x => x.Applications)
                .FirstOrDefaultAsync(x => x.Id == postId);

            if (post == null) throw new NotFoundError("Post Not Found", $"Post with id '{postId}' does not exist");
            if (guid != post.UserId && !IsAdmin())
                throw new NoPermissionError("No permission", "Do not have permission to update the Post");
            if (post.Completed)
                throw new UnsupportedOperationError("Cannot Accept Completed Post",
                    "The post is already completed by accepting an application. You can't accept it again");
            var exist = post.Applications.Any(x => x.Id == appId);
            if (!exist)
                throw new NotFoundError("Application Not Found", $"Application with id '{appId}' does not exist");

            foreach (var application in post.Applications)
            {
                if (application.Id == appId)
                    application.Status = ApplicationStatus.Rejected.ToData();
            }

            post.Completed = false;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        catch (Exception e)
        {
            if (e is not BaseErrorException)
                _l.LogCritical(e, "Failed to reject application. PostId: {@PostId} ApplicationId: {@ApplicationId}",
                    postId,
                    appId);
            throw;
        }
    }

    [HttpPost("accept/{postId:guid}/{appId:guid}")]
    public async Task<ActionResult> Accept(Guid postId, Guid appId)
    {
        try
        {
            var (guid, _) = await GetUser();
            var post = await _db.Posts
                .Include(x => x.Applications)
                .FirstOrDefaultAsync(x => x.Id == postId);

            if (post == null) throw new NotFoundError("Post Not Found", $"Post with id '{postId}' does not exist");
            if (guid != post.UserId && !IsAdmin())
                throw new NoPermissionError("No permission", "Do not have permission to update the Post");

            if (post.Completed)
                throw new UnsupportedOperationError("Cannot Accept Completed Post",
                    "The post is already completed by accepting an application. You can't accept it again");
            var exist = post.Applications.Any(x => x.Id == appId);
            if (!exist)
                throw new NotFoundError("Application Not Found", $"Application with id '{appId}' does not exist");

            foreach (var application in post.Applications)
            {
                if (application.Id == appId)
                    application.Status = ApplicationStatus.Accepted.ToData();
                else
                    application.Status = ApplicationStatus.Rejected.ToData();
            }

            post.Completed = true;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        catch (Exception e)
        {
            if (e is not BaseErrorException)
                _l.LogCritical(e, "Failed to accept application. PostId: {@PostId} ApplicationId: {@ApplicationId}",
                    postId, appId);
            throw;
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            var user = await GetUser();
            var post = await _db.Posts
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (post == null) throw new NotFoundError("Post Not Found", $"Post with id '{id}' does not exist");

            if (user.Id != post.UserId && !IsAdmin())
                throw new NoPermissionError("No permission", "Do not have permission to update the Post");

            _db.Posts.Remove(post);
            await _db.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception e)
        {
            if (e is not BaseErrorException) _l.LogCritical(e, "Failed to delete post. PostId: {@PostId}", id);
            throw;
        }
    }

    [HttpPost]
    public async Task<ActionResult<PostResp>> Create([FromBody] CreatePostReq req)
    {
        try
        {
            var user = await GetUser();

            var data = req.ToData(user.Id);
            // Obtain looking for index
            var indexes = await _db.Indexes
                .Where(x => req.LookingForId.Any(search => search == x.Id))
                .ToArrayAsync();

            // Validate indexes
            if (indexes.Length != req.LookingForId.Count())
            {
                var notFound = req.LookingForId.Where(x => indexes.All(id => id.Id != x));
                throw new NotFoundError("Indexes Not Found", "Id of indexes within 'LookingFor' not found")
                    .AddInfo("indexes", notFound.Select(x => x.ToString()).ToArray());
            }

            var moduleExist = _db.Modules.Any(x => x.Id == req.ModulesId);
            if (!moduleExist)
                throw new NotFoundError("Module Not Found", $"Module with id '{req.ModulesId}' does not exist");
            var index = await _db.Indexes.FirstOrDefaultAsync(x => x.Id == req.IndexId);
            if (index == null)
                throw new NotFoundError("Index Not Found", $"Index with id '{req.IndexId}' does not exist");
            if (index.ModuleId != req.ModulesId)
                throw new UnsupportedOperationError("Module Index MisMatch",
                    $"Index '{req.IndexId}' does not belong to Module '{req.ModulesId}'");
            foreach (var i in indexes)
            {
                if (i.ModuleId != req.ModulesId)
                    throw new UnsupportedOperationError("Module Index MisMatch",
                        $"Index '{i.Id}' does not belong to Module '{req.ModulesId}'");
            }

            data.LookingFor = indexes;

            var r = await _db.Posts.AddAsync(data);
            await _db.SaveChangesAsync();

            // var added = r.Entity.ToDomain().ToResp();
            return Ok(null);
        }
        catch (Exception e)
        {
            var r = e switch
            {
                DbUpdateException
                    {
                        InnerException: PostgresException {SqlState: "23505", ConstraintName: "IX_Posts_UserId_IndexId"}
                    } =>
                    new UniqueConflictError("Post already exist",
                        "A post by this user of this index already exist"),
                _ => null,
            };
            if (r != null) throw r;
            if (e is not BaseErrorException)
                _l.LogCritical(e, "Failed to create post. Post: {@Req}", req);
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
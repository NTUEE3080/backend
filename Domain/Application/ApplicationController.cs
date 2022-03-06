using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PitaPairing.Database;
using PitaPairing.Errors;
using PitaPairing.User;

namespace PitaPairing.Domain.Application
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class ApplicationController : ControllerBase
    {
        private readonly ILogger<ApplicationController> _l;
        private readonly CoreDbContext _db;

        public ApplicationController(ILogger<ApplicationController> l, CoreDbContext db)
        {
            _l = l;
            _db = db;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ApplicationPrincipalRes>> GetAll(Guid? moduleId, Guid? indexId, Guid? postId,
            Guid? applierId, Guid? accepterId)
        {
            try
            {
                var applications = _db.Applications
                    .Include(x => x.Post)
                    .Include(x => x.User)
                    .Include(x => x.Offers)
                    .ThenInclude(x => x.Info)
                    .AsQueryable();

                if (moduleId != null) applications = applications.Where(x => x.Post.ModuleId == moduleId);
                if (indexId != null) applications = applications.Where(x => x.Post.IndexId == indexId);
                if (postId != null) applications = applications.Where(x => x.Post.Id == postId);
                if (applierId != null) applications = applications.Where(x => x.UserId == applierId);
                if (accepterId != null) applications = applications.Where(x => x.Post.UserId == accepterId);

                var ret = applications.Select(x => x.ToPrincipal().ToResp()).ToArray();
                return Ok(ret);
            }
            catch (Exception e)
            {
                if (e is not BaseErrorException)
                    _l.LogCritical(e,
                        "Failed to list posts. Query:  PostId: {@PostId}, ModuleId: {@ModuleId}, IndexId: {@IndexId}, AccepterId: {@AccepterId}, ApplierId: {@ApplierId}",
                        postId, moduleId, indexId, accepterId, applierId);
                throw;
            }
        }


        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApplicationRes>> Get(Guid id)
        {
            try
            {
                var application = await _db.Applications
                    .Include(x => x.Post)
                    .Include(x => x.User)
                    .Include(x => x.Offers)
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (application == null)
                    throw new NotFoundError("Application Not Found",
                        $"Application with id '{id}' does not exist");
                return Ok(application.ToDomain().ToResp());
            }
            catch (Exception e)
            {
                if (e is not BaseErrorException)
                    _l.LogCritical(e, "Failed to get application. ApplicationId: {@ApplicationId}", id);
                throw;
            }
        }


        [HttpPost("{postId:Guid}")]
        public async Task<ActionResult<ApplicationRes>> Apply(Guid postId, [FromBody] CreateApplicationReq req)
        {
            try
            {
                var user = await GetUser();
                var post = await _db.Posts.FirstOrDefaultAsync(x => x.Id == postId);
                if (post == null) throw new NotFoundError("Post Not Found", $"Post with id '{postId}' does not exist");

                var indexes = await _db.Indexes.Where(x => req.OfferId.Any(o => o == x.Id))
                    .ToArrayAsync();
                if (indexes.Length != req.OfferId.Count())
                {
                    var notFound = req.OfferId.Where(x => indexes.All(pd => pd.Id != x));
                    throw new NotFoundError("Indexes Not Found", "Id of indexes within 'LookingFor' not found")
                        .AddInfo("indexes", notFound.Select(x => x.ToString()).ToArray());
                }

                var m = indexes.Select(x => x.ModuleId).Distinct();
                if (m.Count() != 1)
                    throw new UnsupportedOperationError("Non-uniform Indexes",
                        "The offered indexes have different Modules");
                if (m.First() != post.ModuleId)
                    throw new UnsupportedOperationError("Post Offer Mismatch",
                        "Indexes offered in the application do not have the same module as the post");

                var data = req.ToData(user.Id, postId);
                data.Offers = indexes;
                var r = await _db.Applications.AddAsync(data);
                await _db.SaveChangesAsync();
                return Ok(null);
            }
            catch (Exception e)
            {
                var r = e switch
                {
                    DbUpdateException
                        {
                            InnerException: PostgresException {SqlState: "23505", ConstraintName: "IX_Applications_PostId_UserId"}
                        } =>
                        new UniqueConflictError("Application already exist",
                            "A application by this user of to this post already exist"),
                    _ => null,
                };
                if (r != null) throw r;
                if (e is not BaseErrorException)
                    _l.LogCritical(e, "Failed to apply to post: {@PostId} with {@Req}", postId, req);
                throw;
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var user = await GetUser();

                var application = await _db.Applications
                    .Include(x => x.Post)
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (application == null)
                    throw new NotFoundError("Application Not Found",
                        $"Application with id '{id}' does not exist");
                if (user.Id != application.UserId && !IsAdmin())
                    throw new NoPermissionError("No permission", "Do not have permission to update the Post");
                if (application.Post.Completed)
                    throw new UnsupportedOperationError("Cannot Remove Accepted Post",
                        "Cannot delete application that belongs to a post that has been completed");
                _db.Remove(application);
                await _db.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception e)
            {
                if (e is not BaseErrorException)
                    _l.LogCritical(e, "Failed to delete application. ApplicationId: {@ApplicationId}", id);
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
}
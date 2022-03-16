using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PitaPairing.Auth;
using PitaPairing.Database;
using PitaPairing.Domain.Module;
using PitaPairing.Errors;

namespace PitaPairing.Domain.Index;


[Authorize]
[ApiController]
[Route("[controller]")]
public class IndexController : ControllerBase
{
    private readonly ILogger<IndexController> _l;
    private readonly CoreDbContext _db;

    public IndexController(ILogger<IndexController> l, CoreDbContext db)
    {
        _l = l;
        _db = db;
    }

    [HttpGet]
    public ActionResult<IEnumerable<IndexRes>> GetAll(string semester, Guid? course, string? day, string? venue)
    {
        try
        {
            var query = _db.Indexes
                .Include(x => x.Info)
                .Include(x => x.Module)
                .Where(x => x.Module.Semester == semester);
            if (course != null) query = query.Where(x => x.Module.Id == course);
            if (day != null) query = query.Where(x => x.Info.Any(p => p.Day == day));
            if (venue != null) query = query.Where(x => x.Info.Any(p => p.Venue == venue));

            var indexed = query
                .Select(x => x.ToIndex())
                .Select(x => x.ToResp())
                .ToArray();

            return Ok(indexed);
        }
        catch (Exception e)
        {
            if (e is not BaseErrorException) _l.LogCritical(e, "Failed to list indexes");
            throw;
        }
    }


    [HttpGet("{id:guid}")]
    public async Task<ActionResult<IndexRes>> Get(Guid id)
    {
        try
        {
            var index = await _db.Indexes
                .Include(x => x.Info)
                .Include(x => x.Module)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (index == null) throw new NotFoundError("Index Not Found", $"Index with {id} does not exist");
            var resp = index.ToIndex().ToResp();
            return Ok(resp);
        }
        catch (Exception e)
        {
            if (e is not BaseErrorException) _l.LogCritical(e, "Failed to get index {@Id}", id);
            throw;
        }
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicy.Admin)]
    public async Task<ActionResult> Create(
        [FromBody] IEnumerable<CreateIndexReq> modules)
    {
        try
        {
            _l.LogInformation("ModelState Valid: {@ModelStateValid}", ModelState.IsValid);

            var indexDatas = modules.Select(x =>
            {
                var (moduleId, prop) = x.ToDomain();
                var data = prop.ToData();
                data.ModuleId = moduleId;
                return data;
            });

            await _db.Indexes.AddRangeAsync(indexDatas);
            await _db.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception e)
        {
            var r = e switch
            {
                DbUpdateException
                {
                    InnerException: PostgresException
                    {
                        SqlState: "23505", ConstraintName: "IX_Indexes_ModuleId_Code"
                    }
                } => new UniqueConflictError("Index already exist",
                    "One of the index that is being created already exist"),
                _ => null
            };
            if (r != null) throw r;
            if (e is not BaseErrorException) _l.LogCritical(e, "Failed to create index");
            throw;
        }
    }
}

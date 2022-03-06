using Kirinnee.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PitaPairing.Auth;
using PitaPairing.Database;
using PitaPairing.Errors;

namespace PitaPairing.Domain.Module;

public record ModuleComparer(string Sem, string CourseCode);

[ApiController]
[Authorize]
[Route("[controller]")]
public class ModuleController : ControllerBase
{
    private readonly ILogger<ModuleController> _l;
    private readonly CoreDbContext _db;


    public ModuleController(ILogger<ModuleController> l, CoreDbContext db)
    {
        _l = l;
        _db = db;
    }

    [HttpGet]
    public ActionResult<IEnumerable<ModulePrincipalRes>> GetAll(string semester)
    {
        try
        {
            var allModules = _db.Modules
                .Select(x => x.ToPrincipal())
                .Select(x => x.ToResp()).ToList();
            return Ok(allModules);
        }
        catch (Exception e)
        {
            if (e is not BaseErrorException) _l.LogCritical(e, "Failed to list modules");
            throw;
        }
    }


    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ModuleRes>> Get(Guid id)
    {
        try
        {
            var module = await _db.Modules
                .Include(x => x.Indexes)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (module == null) throw new NotFoundError("Module not found", $"Module with id '{id}' does not exist");
            var resp = module.ToDomain().ToResp();
            return Ok(resp);
        }
        catch (Exception e)
        {
            if (e is not BaseErrorException) _l.LogCritical(e, "Failed to get module {@Id}", id);
            throw;
        }
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicy.Admin)]
    public async Task<ActionResult> Create(
        [FromBody] IEnumerable<CreateModuleReq> modules)
    {
        try
        {
            var createModuleReqs = modules as CreateModuleReq[] ?? modules.ToArray();
            var m = createModuleReqs.Select(x => new ModuleComparer(x.Semester, x.CourseCode)).ToArray();
            if (m.Distinct().Count() != m.Length)
            {
                var dups = m.Occurrence()
                    .Where(kvp => kvp.Value > 1)
                    .Select(x => x.Key)
                    .ToArray();
                var e = new UniqueConflictError("Duplicate Modules",
                        $"The request includes duplicated modules. Modules are required to be unique")
                    .AddInfo("duplicates", dups.Select(x => $"{x.Sem} {x.CourseCode}").ToArray());
                throw e;
            }

            var moduleDatas = createModuleReqs.Select(x => x.ToDomain().ToData());
            await _db.Modules.AddRangeAsync(moduleDatas);
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
                        SqlState: "23505", ConstraintName: "IX_Modules_Semester_CourseCode"
                    }
                } => new UniqueConflictError("Module already exist",
                    "One of the module that is being created already exist"),
                _ => null
            };
            if (r != null) throw r;
            if (e is not BaseErrorException) _l.LogCritical(e, "Failed to create modules: {@Modules}", modules);
            throw;
        }
    }
}
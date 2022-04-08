using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PitaPairing.Auth;
using PitaPairing.Database;
using PitaPairing.Errors;
using PitaPairing.User;
using Serilog;

namespace PitaPairing.Domain.Suggestions;

[ApiController]
[Authorize]
[Route("[controller]")]
public class SuggestionController : ControllerBase
{
    private readonly ILogger<SuggestionController> _l;

    private readonly CoreDbContext _db;
    private readonly IMatchSearcher _match;
    private readonly ISuggestionService _suggestionService;

    // GET
    public SuggestionController(ILogger<SuggestionController> l, CoreDbContext db, IMatchSearcher match,
        ISuggestionService suggestionService)
    {
        _l = l;
        _db = db;
        _match = match;
        _suggestionService = suggestionService;
    }

    [Authorize(Policy = AuthPolicy.Admin)]
    [HttpGet("admin/two/{userId:guid}")]
    public async Task<ActionResult<IEnumerable<TwoWaySuggestionResp>>> AdminTwo(Guid userId, Guid? connectorId, int? take)
    {
        try
        {
            var suggestions = await _suggestionService.GetTwo(userId, connectorId, take);
            return Ok(suggestions.Select(x => x.ToResp()));
        }
        catch (Exception e)
        {
            if (e is not BaseErrorException)
                _l.LogCritical(e,
                    "Failed to get two-way suggestions for user {@User}. Query Params: connecter: {@Connector}, take: {@Take}",
                    null, connectorId, take);
            throw;
        }
    }

    [Authorize(Policy = AuthPolicy.Admin)]
    [HttpGet("admin/three/{userId:guid}")]
    public async Task<ActionResult<IEnumerable<ThreeWaySuggestionResp>>> AdminThree(Guid userId, Guid? connectorId, int? take)
    {
        try
        {
            var suggestions = await _suggestionService.GetThree(userId, connectorId, take);
            return Ok(suggestions.Select(x => x.ToResp()));
        }
        catch (Exception e)
        {
            if (e is not BaseErrorException)
                _l.LogCritical(e,
                    "Failed to get two-way suggestions for user {@User}. Query Params: connecter: {@Connector}, take: {@Take}",
                    null, connectorId, take);
            throw;
        }
    }

    [HttpGet("two")]
    public async Task<ActionResult<IEnumerable<TwoWaySuggestionResp>>> GetTwo(Guid? connectorId, int? take)
    {
        try
        {
            var (guid, _) = await GetUser();
            var suggestions = await _suggestionService.GetTwo(guid, connectorId, take);
            return Ok(suggestions.Select(x => x.ToResp()));
        }
        catch (Exception e)
        {
            if (e is not BaseErrorException)
                _l.LogCritical(e,
                    "Failed to get two-way suggestions for user {@User}. Query Params: connecter: {@Connector}, take: {@Take}",
                    null, connectorId, take);
            throw;
        }
    }


    [HttpGet("three")]
    public async Task<ActionResult<IEnumerable<ThreeWaySuggestionResp>>> GetThree(Guid? connectorId, int? take)
    {
        try
        {
            var (guid, _) = await GetUser();
            var suggestions = await _suggestionService.GetThree(guid, connectorId, take);
            return Ok(suggestions.Select(x => x.ToResp()));
        }
        catch (Exception e)
        {
            if (e is not BaseErrorException)
                _l.LogCritical(e,
                    "Failed to get two-way suggestions for user {@User}. Query Params: connecter: {@Connector}, take: {@Take}",
                    null, connectorId, take);
            throw;
        }
    }



    [HttpGet("add/{source:guid}")]
    public async Task<ActionResult> Add(Guid source)
    {
        try
        {
            await _suggestionService.Add(source);
            return NoContent();
        }
        catch (Exception e)
        {
            _l.LogError(e, "error occured while searching");
            throw;
        }
    }

    [HttpGet("search/{source:guid}")]
    public async Task<ActionResult> Search(Guid source)
    {
        try
        {
            var a = await _match.Search(source);
            var s = a.ToArray();
            var three = await _match.SearchThree(source);
            three = three.Where(x => !s.Contains(x.Two) && !s.Contains(x.One));

            _l.LogInformation("Two Way {@TwoWay}", s);
            _l.LogInformation("Three Way {@ThreeWay}", three);
            return NoContent();
        }
        catch (Exception e)
        {
            _l.LogError(e, "error occured while searching");
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
    private string? Sub()
    {
        return HttpContext.User?.Identity?.Name;
    }
}

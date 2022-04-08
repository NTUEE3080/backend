using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PitaPairing.Database;
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
    public SuggestionController(ILogger<SuggestionController> l, CoreDbContext db, IMatchSearcher match, ISuggestionService suggestionService)
    {
        _l = l;
        _db = db;
        _match = match;
        _suggestionService = suggestionService;
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
}

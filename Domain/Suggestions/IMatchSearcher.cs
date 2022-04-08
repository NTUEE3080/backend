using Microsoft.EntityFrameworkCore;
using PitaPairing.Database;
using PitaPairing.Domain.Post;

namespace PitaPairing.Domain.Suggestions;

public record TwoGuid(Guid One, Guid Two);

public interface IMatchSearcher
{
    Task<IEnumerable<Guid>> Search(Guid s);
    Task<IEnumerable<TwoGuid>> SearchThree(Guid s);
}

public class MatchSearcher : IMatchSearcher
{
    private readonly CoreDbContext _db;
    private readonly ILogger<SuggestionService> _l;


    public MatchSearcher(CoreDbContext db, ILogger<SuggestionService> l)
    {
        _db = db;
        _l = l;
    }

    public async Task<IEnumerable<Guid>> Search(Guid s)
    {
        var source = await _db.Posts.FirstOrDefaultAsync(x => x.Id == s);
        if (source == null) return Array.Empty<Guid>();

        var r = _db.Posts
            .Include(x => x.LookingFor)
            .ThenInclude(x => x.PrincipalPosts)
            .ThenInclude(x => x.LookingFor)
            .Where(x => x.Id == source.Id)
            .SelectMany(x => x.LookingFor.SelectMany(lf => lf.PrincipalPosts)) // 2nd layer posts
            .Where(x => x.Id != source.Id) //remove Id from 2nd layer post
            .Where(x => x.LookingFor.Select(lf => lf.Id).Contains(source.IndexId));

        return r.Select(x => x.Id);
    }

    public async Task<IEnumerable<TwoGuid>> SearchThree(Guid s)
    {
        var source = await _db.Posts.FirstOrDefaultAsync(x => x.Id == s);
        if (source == null) return Array.Empty<TwoGuid>();
        _l.LogInformation("Starting Search");
        var r = await _db.Posts
            .Include(x => x.LookingFor)
            .ThenInclude(x => x.PrincipalPosts) // possible targets
            .ThenInclude(x => x.LookingFor)
            .ThenInclude(x => x.PrincipalPosts) // possible target's possible target
            .ThenInclude(x => x.LookingFor) //possible target' possible target's looking for
            .Where(x => x.Id == source.Id)
            .SelectMany(x => x.LookingFor.SelectMany(lf => lf.PrincipalPosts))
            .Where(p => p.Id != source.Id)
            .SelectMany(
                p => p.LookingFor.SelectMany(
                    lf => lf.PrincipalPosts.SelectMany(
                        post => post.LookingFor
                            .Select(f => new { Second = p, Third = post, SearchingFor = f }
                            )
                    )
                )
            )
            .Where(x => x.SearchingFor.Id == source.IndexId)
            .Select(x => new { Second = x.Second.Id, Third = x.Third.Id })
            .Distinct()
            .ToArrayAsync();
        _l.LogInformation("Search finish");
        _l.LogInformation("r: @{R}", r);
        return r.Select(x => new TwoGuid(x.Second, x.Third)).Distinct();
    }
}

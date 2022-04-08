using Kirinnee.Helper;
using Microsoft.EntityFrameworkCore;
using PitaPairing.Database;
using PitaPairing.Domain.Post;

namespace PitaPairing.Domain.Suggestions;

public interface ISuggestionService
{
    Task Add(Guid id);
}

public class SuggestionService : ISuggestionService
{
    private readonly CoreDbContext _db;
    private readonly ILogger<SuggestionService> _l;
    private readonly IMatchSearcher _ms;

    public SuggestionService(CoreDbContext db, ILogger<SuggestionService> l, IMatchSearcher ms)
    {
        _db = db;
        _l = l;
        _ms = ms;
    }

    private string generateIdentifier(params PostData[] posts)
    {
        return posts.OrderBy(x => x.Id).Select(x => x.Id.ToString()).JoinBy();
    }

    public async Task Add(Guid id)
    {
        var a = await _ms.Search(id);
        var two = a.ToArray();
        var three = await _ms.SearchThree(id);
        three = three.Where(x => !two.Contains(x.Two) && !two.Contains(x.One));

        _l.LogInformation("Two Way {@TwoWay}", two);
        _l.LogInformation("Three Way {@ThreeWay}", three);

        var query = two.Concat(three.SelectMany(x => new[] { x.One, x.Two })).Concat(new[] { id }).Distinct();

        var posts = await _db.Posts
            .Include(x => x.User)
            .Include(x => x.Index).ThenInclude(x => x.Info)
            .Include(x => x.LookingFor).ThenInclude(x => x.Info)
            .Include(x => x.Module)
            .Include(x => x.Offers).ThenInclude(x => x.ApplierPost)
            .Where(x => query.Contains(x.Id))
            .ToDictionaryAsync(p => p.Id, p => p);

        var twoSuggestions = two.Select(x => new
        {
            One = posts[id],
            Two = posts[x],
            Unique = generateIdentifier(posts[id], posts[x]),
        })
            .Where(x => x.One.UserId != x.Two.UserId)
            .Where(x => !x.One.Completed && !x.Two.Completed)
            .ToArray();

        var threeSuggestions = three.Select(x => new
        {
            One = posts[id],
            Two = posts[x.One],
            Three = posts[x.Two],
            Unique = generateIdentifier(posts[id], posts[x.One], posts[x.Two]),
        })
            .Where(x => x.One.UserId != x.Two.UserId && x.Two.UserId != x.Three.UserId &&
                        x.One.UserId != x.Three.UserId)
            .Where(x => !x.One.Completed && !x.Two.Completed && !x.Three.Completed)
            .ToArray();

        // Storing two way
        var twoData = twoSuggestions.SelectMany(x => new TwoWaySuggestionData[]
        {
            new()
            {
                Counter = 0,
                TimeStamp = DateTime.Now.Ticks,
                Id = Guid.NewGuid(),
                Post1Id = x.One.Id,
                Post2Id = x.Two.Id,
                UniqueChecker = x.Unique,
                UserId = x.One.UserId,
            },
            new()
            {
                Counter = 0,
                TimeStamp = DateTime.Now.Ticks,
                Id = Guid.NewGuid(),
                Post1Id = x.One.Id,
                Post2Id = x.Two.Id,
                UniqueChecker = x.Unique,
                UserId = x.Two.UserId,
            }
        }).ToArray();

        var threeData = threeSuggestions.SelectMany(x => new ThreeWaySuggestionData[]
        {
            new()
            {
                Counter = 0,
                TimeStamp = DateTime.Now.Ticks,
                Id = Guid.NewGuid(),
                Post1Id = x.One.Id,
                Post2Id = x.Two.Id,
                Post3Id = x.Three.Id,
                UniqueChecker = x.Unique,
                UserId = x.One.UserId,
            },
            new()
            {
                Counter = 0,
                TimeStamp = DateTime.Now.Ticks,
                Id = Guid.NewGuid(),
                Post1Id = x.One.Id,
                Post2Id = x.Two.Id,
                Post3Id = x.Three.Id,
                UniqueChecker = x.Unique,
                UserId = x.Two.UserId,
            },
            new()
            {
                Counter = 0,
                TimeStamp = DateTime.Now.Ticks,
                Id = Guid.NewGuid(),
                Post1Id = x.One.Id,
                Post2Id = x.Two.Id,
                Post3Id = x.Three.Id,
                UniqueChecker = x.Unique,
                UserId = x.Three.UserId,
            }
        }).ToArray();

        _l.LogInformation("TwoWay Data: {@TwoWay}", twoData);

        await _db.TwoWaySuggestions
            .UpsertRange(twoData)
            .On(t => new { t.UniqueChecker, t.UserId })
            .WhenMatched(x => new TwoWaySuggestionData()
            {
                Counter = x.Counter + 1,
            })
            .RunAsync();

        await _db.ThreeWaySuggestions
            .UpsertRange(threeData)
            .On(t => new { t.UniqueChecker, t.UserId })
            .WhenMatched(x => new ThreeWaySuggestionData()
            {
                Counter = x.Counter + 1,
            })
            .RunAsync();

    }
}

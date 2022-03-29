using Microsoft.EntityFrameworkCore;
using PitaPairing.Database;
using PitaPairing.Domain.Post;

namespace PitaPairing.Domain.Suggestions;

public interface ITwoSuggestionService
{

}

public class TwoSuggestionService : ITwoSuggestionService
{
    private readonly CoreDbContext _db;
    private readonly ILogger<TwoSuggestionService> _l;

    public TwoSuggestionService(CoreDbContext db, ILogger<TwoSuggestionService> l)
    {
        _db = db;
        _l = l;
    }

    public Task Add(Guid id)
    {
        var q = _db.Posts.Include(x => x.Index).ThenInclude(x => x.Info)
            .Include(x => x.LookingFor).ThenInclude(x => x.Info)
            .Include(x => x.User).AsQueryable();
        return Task.CompletedTask;
    }
}

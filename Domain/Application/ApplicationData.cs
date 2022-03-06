using PitaPairing.Domain.Index;
using PitaPairing.Domain.Post;
using PitaPairing.User;

namespace PitaPairing.Domain.Application;

#pragma warning disable CS8618
public record ApplicationData
{
    public ApplicationData(Guid id, Guid userId)
    {
        Id = id;
        UserId = userId;
    }

    public Guid Id { get; set; }

    public byte Status { get; set; }

    public Guid UserId { get; set; }
    public UserData User { get; set; }

    public Guid PostId { get; set; }
    public PostData Post { get; set; }

    public IEnumerable<IndexData> Offers { get; set; }
}
#pragma warning restore CS8618
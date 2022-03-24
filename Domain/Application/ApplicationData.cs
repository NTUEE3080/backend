using PitaPairing.Domain.Index;
using PitaPairing.Domain.Post;
using PitaPairing.User;

namespace PitaPairing.Domain.Application;

#pragma warning disable CS8618
public record ApplicationData
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public PostData Post { get; set; }
    public Guid ApplierPostId { get; set; }
    public PostData ApplierPost { get; set; }
    public byte Status { get; set; }
}
#pragma warning restore CS8618

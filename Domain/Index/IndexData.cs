using System.ComponentModel.DataAnnotations.Schema;
using PitaPairing.Domain.Application;
using PitaPairing.Domain.Module;
using PitaPairing.Domain.Post;

namespace PitaPairing.Domain.Index;

#pragma warning disable CS8618
public record IndexPropsData
{
    public void Deconstruct(out string @group, out string type, out string day, out int startTime, out int endTime,
        out string venue)
    {
        @group = Group;
        type = Type;
        day = Day;
        startTime = StartTime;
        endTime = EndTime;
        venue = Venue;
    }

    public int Id { get; set; }
    public string Group { get; set; }
    public string Type { get; set; }
    public string Day { get; set; }
    public int StartTime { get; set; }
    public int EndTime { get; set; }
    public string Venue { get; set; }
}

public record IndexData
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public Guid ModuleId { get; set; }
    public ModuleData Module { get; set; }

    public IEnumerable<PostData> PrincipalPosts { get; set; }
    public IEnumerable<IndexPropsData> Info { get; set; }
    public IEnumerable<PostData> RelatedPosts { get; set; }

    public IEnumerable<ApplicationData> RelatedApplications { get; set; }
}
#pragma warning restore CS8618

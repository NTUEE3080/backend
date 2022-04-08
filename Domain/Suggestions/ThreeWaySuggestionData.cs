using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PitaPairing.Domain.Post;
using PitaPairing.User;

namespace PitaPairing.Domain.Suggestions;

#pragma warning disable CS8601
public record ThreeWaySuggestionData
{
    public void Deconstruct(out Guid id, out long counter, out string? uniqueChecker, out Guid userId, out UserData? user, out Guid post1Id, out PostData? post1Data, out Guid post2Id, out PostData? post2Data, out Guid post3Id, out PostData? post3Data, out long timeStamp)
    {
        id = Id;
        counter = Counter;
        uniqueChecker = UniqueChecker;
        userId = UserId;
        user = User;
        post1Id = Post1Id;
        post1Data = Post1;
        post2Id = Post2Id;
        post2Data = Post2;
        post3Id = Post3Id;
        post3Data = Post3;
        timeStamp = TimeStamp;
    }

    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    public long Counter { get; set; } = long.MinValue;
    public string? UniqueChecker { get; set; }

    public Guid UserId { get; set; }
    public UserData? User { get; set; }

    public Guid Post1Id { get; set; }
    public PostData? Post1 { get; set; }

    public Guid Post2Id { get; set; }
    public PostData? Post2 { get; set; }

    public Guid Post3Id { get; set; }
    public PostData? Post3 { get; set; }

    public long TimeStamp { get; set; }
}
#pragma warning restore CS8601

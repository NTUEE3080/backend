using PitaPairing.Domain.Post;
using PitaPairing.User;

namespace PitaPairing.Domain.Suggestions;

public record TwoWaySuggestionData
{
    public void Deconstruct(out Guid id, out long counter, out string uniqueChecker, out Guid userId, out UserData user, out Guid post1Id, out PostData post1Data, out Guid post2Id, out PostData post2Data, out long timeStamp)
    {
        id = Id;
        counter = Counter;
        uniqueChecker = UniqueChecker;
        userId = UserId;
        user = User;
        post1Id = Post1Id;
        post1Data = Post1Data;
        post2Id = Post2Id;
        post2Data = Post2Data;
        timeStamp = TimeStamp;
    }

    public Guid Id { get; set; }

    public long Counter { get; set; } = long.MinValue;
    public string? UniqueChecker { get; set; }

    public Guid UserId { get; set; }
    public UserData? User { get; set; }

    public Guid Post1Id { get; set; }
    public PostData? Post1Data { get; set; }

    public Guid Post2Id { get; set; }
    public PostData? Post2Data { get; set; }

    public long TimeStamp { get; set; }
}

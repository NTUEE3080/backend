using PitaPairing.Domain.Post;
using PitaPairing.User;

namespace PitaPairing.Domain.Suggestions;

public static class TwoWaySuggestionMapper
{
    public static TwoWaySuggestion ToDomain(this TwoWaySuggestionData data)
    {
        var (id, counter, uniqueChecker, userId, user, post1Id, post1Data, post2Id, post2Data, timeStamp) = data;
        return new TwoWaySuggestion(id, counter, user.ToPrincipal(), post1Data.ToPrincipal(), post2Data.ToPrincipal());
    }

    public static TwoWaySuggestionResp ToResp(this TwoWaySuggestion data)
    {
        var (guid, counter, userPrincipal, post1, post2) = data;
        return new TwoWaySuggestionResp(guid, counter, post1.Props.Module.Props.CourseCode, userPrincipal.ToResp(), post1.ToResp(), post2.ToResp());
    }
}

using PitaPairing.Domain.Post;
using PitaPairing.User;

namespace PitaPairing.Domain.Suggestions;


public static class ThreeWaySuggestionMapper
{
    public static ThreeWaySuggestion ToDomain(this ThreeWaySuggestionData data)
    {
        var (id, counter, uniqueChecker, userId, user, post1Id, post1Data, post2Id, post2Data, post3Id, post3Data,
            timeStamp) = data;
        return new ThreeWaySuggestion(id, counter, user.ToPrincipal(), post1Data.ToPrincipal(), post2Data.ToPrincipal(),
            post3Data.ToPrincipal());
    }

    public static ThreeWaySuggestionResp ToResp(this ThreeWaySuggestion data)
    {
        var (guid, counter, userPrincipal, post1, post2, post3) = data;
        return new ThreeWaySuggestionResp(guid, counter, userPrincipal.ToResp(), post1.ToResp(), post2.ToResp(),
            post3.ToResp());
    }
}

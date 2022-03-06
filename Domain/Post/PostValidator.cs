using FluentValidation;
using PitaPairing.Domain.Index;

namespace PitaPairing.Domain.Post;

public class PostValidator : AbstractValidator<CreatePostReq>
{
    public PostValidator()
    {
        RuleFor(x => x.IndexId).NotNull().NonEmptyGuid();
        RuleFor(x => x.ModulesId).NotNull().NonEmptyGuid();
        RuleFor(x => x.LookingForId).MinCount(1);
        RuleForEach(x => x.LookingForId).ChildRules(c => { c.RuleFor(x => x).NonEmptyGuid(); });
    }
}
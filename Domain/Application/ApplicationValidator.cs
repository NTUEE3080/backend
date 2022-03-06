using FluentValidation;
using PitaPairing.Domain.Post;

namespace PitaPairing.Domain.Application;

public class ApplicationValidator : AbstractValidator<CreateApplicationReq>
{
    public ApplicationValidator()
    {
        RuleFor(x => x.OfferId).MinCount(1);
        RuleForEach(x => x.OfferId).ChildRules(c =>
        {
            c.RuleFor(x => x).NonEmptyGuid(); 
            
        });
    }
}
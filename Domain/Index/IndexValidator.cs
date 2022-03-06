using FluentValidation;
using Serilog;

namespace PitaPairing.Domain.Index;

public class IndexValidator : AbstractValidator<CreateIndexReq>
{
    public IndexValidator()
    {
        RuleFor(i => i.Code).Length(5, 8);
        RuleFor(i => i.Props).NotNull().MinCount(1);
        RuleFor(i => i.ModuleId).NotNull().NonEmptyGuid();
        RuleForEach(i => i.Props).ChildRules(s =>
        {
            s.RuleFor(p => p.Day).IsDayEnum();
            s.RuleFor(p => p.Start).IsTime();
            s.RuleFor(p => p.Stop).IsTime();
        });
    }
}
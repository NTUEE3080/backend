using System.Text.RegularExpressions;
using FluentValidation;
using Kirinnee.Helper;
using Microsoft.VisualBasic;

namespace PitaPairing;

public static class CustomValidators
{
    public static IRuleBuilderOptions<T, Guid> NonEmptyGuid<T>(
        this IRuleBuilder<T, Guid> ruleBuilder)
    {
        return ruleBuilder
            .NotEqual(Guid.Empty)
            .WithMessage("Invalid Guid. Guid cannot be empty");
    }

    public static IRuleBuilderOptions<T, string> IsTime<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .Must(t =>
            {
                var l = t.SplitBy(":").ToArray();
                if (l.Length != 2) return false;
                try
                {
                    var start = l[0].ToInt();
                    var end = l[1].ToInt();
                    return start <= 23 && start >= 0 && end >= 0 && end <= 59;
                }
                catch
                {
                    return false;
                }
            })
            .WithMessage("Invalid Time format. Must be in 24h format of HH:MM");
    }

    public static IRuleBuilderOptions<T, string> IsDayEnum<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .Must(d => d is "Monday" or "Tuesday" or "Wednesday" or "Thursday" or "Friday")
            .WithMessage("Must be a 'day': 'Monday', 'Tuesday', 'Wednesday', 'Thursday' or 'Friday'");
    }


    public static IRuleBuilderOptions<T, IEnumerable<TEle>> MinCount<T, TEle>(
        this IRuleBuilder<T, IEnumerable<TEle>> ruleBuilder, int count)
    {
        return ruleBuilder
            .Must((o, d, context) =>
            {
                context.MessageFormatter.AppendArgument("MinCount", count);
                return d.Count() >= count;
            })
            .WithMessage("{PropertyName} must contain more than {MinCount} items.");
    }

    public static IRuleBuilderOptions<T, IEnumerable<TEle>> MaxCount<T, TEle>(
        this IRuleBuilder<T, IEnumerable<TEle>> ruleBuilder, int count)
    {
        return ruleBuilder
            .Must((o, d, context) =>
            {
                context.MessageFormatter.AppendArgument("MaxCount", count);
                return d.Count() <= count;
            })
            .WithMessage("{PropertyName} must contain less than {MaxCount} items.");
    }

    public static IRuleBuilderOptions<T, IEnumerable<TEle>> Count<T, TEle>(
        this IRuleBuilder<T, IEnumerable<TEle>> ruleBuilder, int minCount, int maxCount)
    {
        return ruleBuilder
            .Must((o, d, context) =>
            {
                var min = Math.Min(minCount, maxCount);
                var max = Math.Min(minCount, maxCount);
                context.MessageFormatter.AppendArgument("MaxCount", max);
                context.MessageFormatter.AppendArgument("MinCount", min);
                var arr = d as TEle[] ?? d.ToArray();
                return arr.Length <= max && arr.Length >= min;
            })
            .WithMessage("{PropertyName} must contain between {MinCount} and {MaxCount} items.");
    }
}
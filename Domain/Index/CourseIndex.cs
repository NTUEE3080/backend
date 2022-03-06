using PitaPairing.Domain.Module;

namespace PitaPairing.Domain.Index;

public enum Day
{
    Monday,
    Tuesday,
    Wednesday,
    Thursday,
    Friday
}

public record IndexSingleProp(string Group, string Type, Day Day, TimeOnly Start, TimeOnly Stop, string Venue);

public record IndexProps(string Code, IEnumerable<IndexSingleProp> Prop);

public record IndexPrincipal(Guid Id, IndexProps Props);

public record CourseIndex(IndexPrincipal Principal, ModulePrincipal Module);


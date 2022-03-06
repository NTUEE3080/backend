using PitaPairing.Domain.Module;

namespace PitaPairing.Domain.Index;

public record CreateIndexSingleReq(
    string Group,
    string Type,
    string Day,
    string Start,
    string Stop,
    string Venue
);

public record CreateIndexReq(Guid ModuleId, string Code, IEnumerable<CreateIndexSingleReq> Props);

public record IndexPropsRes(string Group, string Type, string Day, string Start, string Stop,
    string Venue);

public record IndexPrincipalRes(Guid Id, string Code, IEnumerable<IndexPropsRes> Props);

public record IndexRes(IndexPrincipalRes Principal, ModulePrincipalRes Module);
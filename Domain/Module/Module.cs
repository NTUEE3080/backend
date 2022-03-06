using PitaPairing.Domain.Index;

namespace PitaPairing.Domain.Module;

public record ModuleProps(string Semester, string CourseCode, string Name, string Description, int AcademicUnit);

public record ModulePrincipal(Guid Id, ModuleProps Props);

public record Module(ModulePrincipal Principal, IEnumerable<IndexPrincipal> Indexes);

public record CreateModuleReq(string Semester, string CourseCode, string Name, string Description, int AcademicUnit);

public record ModulePrincipalRes(Guid Id, string Semester, string CourseCode, string Name, string Description, int AcademicUnit);

public record ModuleRes(ModulePrincipalRes Principal, IEnumerable<IndexPrincipalRes> Indexes);
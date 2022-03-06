using PitaPairing.Domain.Index;

namespace PitaPairing.Domain.Module;

public static class ModuleDataMapper
{
  #region DomainToData

  public static ModuleData ToData(this ModuleProps props)
  {
    return new ModuleData(Guid.Empty, props.Semester, props.CourseCode, props.Name, props.Description,
      props.AcademicUnit);
  }

  #endregion

  #region DataToDomain

  public static ModuleProps ToProps(this ModuleData d)
  {
    return new ModuleProps(d.Semester, d.CourseCode, d.Name, d.Description, d.AcademicUnit);
  }

  public static ModulePrincipal ToPrincipal(this ModuleData d)
  {
    return new ModulePrincipal(d.Id, d.ToProps());
  }

  public static Module ToDomain(this ModuleData d)
  {
    return new Module(d.ToPrincipal(), d.Indexes?.Select(x => x.ToPrincipal()) ?? Array.Empty<IndexPrincipal>());
  }

  #endregion
}

public static class ModuleWebMapper
{
  #region WebToDomain

  public static ModuleProps ToDomain(this CreateModuleReq req)
  {
    var (semester, courseCode, name, description, academicUnit) = req;
    return new ModuleProps(semester, courseCode, name, description, academicUnit);
  }

  #endregion

  #region DomainToWeb

  public static ModulePrincipalRes ToResp(this ModulePrincipal p)
  {
    var (guid, (semester, courseCode, name, description, academicUnit)) = p;
    return new ModulePrincipalRes(guid, semester, courseCode, name, description, academicUnit);
  }

  public static ModuleRes ToResp(this Module m)
  {
    var (modulePrincipal, indexPrincipals) = m;
    return new ModuleRes(modulePrincipal.ToResp(),
      indexPrincipals?.Select(x => x.ToResp()) ?? new List<IndexPrincipalRes>());
  }

  #endregion
}
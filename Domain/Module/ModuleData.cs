using PitaPairing.Domain.Index;

namespace PitaPairing.Domain.Module;

#pragma warning disable CS8618
public record ModuleData
{
  public ModuleData(Guid id, string semester, string courseCode, string name, string description, int academicUnit)
  {
    Id = id;
    Semester = semester;
    CourseCode = courseCode;
    Name = name;
    Description = description;
    AcademicUnit = academicUnit;
  }

  public Guid Id { get; set; }
  public string Semester { get; set; }
  public string CourseCode { get; set; }
  public string Name { get; set; }
  public string Description { get; set; }
  public int AcademicUnit { get; set; }

  public List<IndexData> Indexes { get; set; }
}
#pragma warning restore CS8618
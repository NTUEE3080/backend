namespace PitaPairing.Domain.Semester;

#pragma warning disable CS8618
public record SemesterData
{
    public string Id { get; set; }
    public bool Current { get; set; }
}

public record SemesterRes(string Semester, bool Current);

public record CreateSemesterReq(string Semester);



#pragma warning restore CS8618
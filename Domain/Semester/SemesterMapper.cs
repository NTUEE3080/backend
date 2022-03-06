namespace PitaPairing.Domain.Semester;

public static class SemesterMapper
{
    public static SemesterRes ToResp(this SemesterData d)
    {
        return new SemesterRes(d.Id, d.Current);
    }
}
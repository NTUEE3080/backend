using PitaPairing.Domain.Module;

namespace PitaPairing.Domain.Index;

public static class IndexDataMapper
{
    #region DomainToData

    public static int Serialize(this TimeOnly time)
    {
        return time.Hour * 10000 + time.Minute * 100 + time.Second;
    }

    public static TimeOnly DeserializeTime(this int time)
    {
        var hour = time / 10000;
        var t1 = time - hour * 10000;
        var min = t1 / 100;
        var ss = t1 - min * 100;
        return new TimeOnly(hour, min, ss);
    }

    public static string DayToData(this Day day)
    {
        return day switch
        {
            Day.Monday => "Monday",
            Day.Tuesday => "Tuesday",
            Day.Wednesday => "Wednesday",
            Day.Thursday => "Thursday",
            Day.Friday => "Friday",
            Day.Saturday => "Saturday",
            Day.Sunday => "Sunday",
            Day.Free => "Free",
            _ => throw new ArgumentOutOfRangeException(nameof(day), day, null)
        };
    }

    public static IndexData ToData(this IndexProps props)
    {
        return new IndexData()
        {
            Code = props.Code,
            Info =
                props.Prop.Select(x => new IndexPropsData()
                {
                    Group = x.Group,
                    Type = x.Type,
                    Day = x.Day.DayToData(),
                    StartTime = x.Start.Serialize(),
                    EndTime = x.Stop.Serialize(),
                    Venue = x.Venue,
                }).ToArray()
        };
    }

    #endregion

    #region DataToDomain

    public static Day DayFromData(this string day)
    {
        return day switch
        {
            "Monday" => Day.Monday,
            "Tuesday" => Day.Tuesday,
            "Wednesday" => Day.Wednesday,
            "Thursday" => Day.Thursday,
            "Friday" => Day.Friday,
            "Saturday" => Day.Saturday,
            "Sunday" => Day.Sunday,
            "Free" => Day.Free,
            _ => throw new ArgumentOutOfRangeException(nameof(day), day, null)
        };
    }

    public static IndexSingleProp ToSingle(this IndexPropsData d)
    {
        var (@group, type, day, startTime, endTime, venue) = d;
        return new IndexSingleProp(@group, type, day.DayFromData(), startTime.DeserializeTime(),
            endTime.DeserializeTime(), venue);
    }

    public static IndexProps ToProps(this IndexData d)
    {
        return new IndexProps(d.Code, d.Info.Select(x => x.ToSingle()).ToArray());
    }

    public static IndexPrincipal ToPrincipal(this IndexData d)
    {
        return new IndexPrincipal(d.Id, d.ToProps());
    }

    public static CourseIndex ToIndex(this IndexData d)
    {
        return new CourseIndex(d.ToPrincipal(), d.Module.ToPrincipal());
    }

    #endregion
}

public static class IndexWebMapper
{
    #region WebToDomain

    public static IndexSingleProp ToDomain(this CreateIndexSingleReq r)
    {
        var (group, type, day, start, stop, venue) = r;
        return new IndexSingleProp(group, type, day.DayFromData(), TimeOnly.Parse(start), TimeOnly.Parse(stop), venue);
    }

    public static (Guid, IndexProps) ToDomain(this CreateIndexReq r)
    {
        var (moduleId, code, single) = r;
        var props = new IndexProps(code, single.Select(x => x.ToDomain()).ToArray());
        return (moduleId, props);
    }

    #endregion

    #region DomainToWeb

    public static IndexPropsRes ToResp(this IndexSingleProp p)
    {
        var (@group, type, day, start, stop, venue) = p;
        return new IndexPropsRes(@group, type, day.DayToData(), start.ToShortTimeString(), stop.ToShortTimeString(),
            venue);
    }

    public static IndexPrincipalRes ToResp(this IndexPrincipal p)
    {
        var (guid, (code, props)) = p;
        return new IndexPrincipalRes(guid, code, props.Select(x => x.ToResp()));
    }

    public static IndexRes ToResp(this CourseIndex i)
    {
        var (indexPrincipal, modulePrincipal) = i;

        return new IndexRes(indexPrincipal.ToResp(), modulePrincipal.ToResp());
    }

    #endregion
}

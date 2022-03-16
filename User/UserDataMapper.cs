namespace PitaPairing.User;

public static class UserDataMapper
{
    #region DomainToData

    public static UserData ToData(this UserProps props)
    {
        return new UserData(Guid.Empty, props.Name, props.Email, props.Sub);
    }

    public static UserData FromDomain(this UserData data, UserProps props)
    {
        var (name, email, sub) = props;
        data.Email = email;
        data.Name = name;
        data.Sub = sub;
        return data;
    }

    #endregion

    #region DataToDomain

    public static UserProps ToProps(this UserData userData)
    {
        return new UserProps(userData.Name, userData.Email, userData.Sub);
    }

    public static UserPrincipal ToPrincipal(this UserData principal)
    {
        return new UserPrincipal(principal.Id, principal.ToProps());
    }

    #endregion
}

public static class UserWebMapper
{
    #region WebToDomain

    public static UserProps ToDomain(this CreateUserReq req, string sub)
    {
        var (name, email) = req;
        return new UserProps(name, email, sub);
    }

    public static UserProps ToDomain(this UpdateUserReq req, string sub)
    {
        var (name, email) = req;
        return new UserProps(name, email, sub);
    }

    #endregion

    #region DomainToWeb

    public static UserPrincipalResp ToResp(this UserPrincipal principal)
    {
        var (id, (name, email, sub)) = principal;
        return new UserPrincipalResp(id, name, email, sub);
    }

    #endregion
}

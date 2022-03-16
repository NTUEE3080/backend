namespace PitaPairing.User;

public record UserData
{
    public UserData(Guid id, string name, string email, string sub)
    {
        this.Id = id;
        this.Name = name;
        this.Email = email;
        Sub = sub;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }

    public string Sub { get; set; }
    public string Email { get; set; }

    public void Deconstruct(out Guid id, out string name, out string email, out string sub)
    {
        id = this.Id;
        name = this.Name;
        email = this.Email;
        sub = this.Sub;
    }
}

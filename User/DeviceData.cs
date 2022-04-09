using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PitaPairing.User;

public record DeviceData
{

    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    public string DeviceToken { get; set; }

    public long TimeStamp { get; set; }

    public Guid UserId { get; set; }

    public UserData User { get; set; }
}

using PitaPairing.Domain.Application;
using PitaPairing.Domain.Index;
using PitaPairing.Domain.Module;
using PitaPairing.User;

namespace PitaPairing.Domain.Post;

#pragma warning disable CS8618
public record PostData
{
  public Guid Id { get; set; }
  
  public Guid UserId { get; set; }
  public UserData User { get; set; }
  
  public Guid ModuleId { get; set; }
  public ModuleData Module { get; set; }
  
  public Guid IndexId { get; set; }
  public IndexData Index { get; set; }
  public IEnumerable<IndexData> LookingFor { get; set; }
  
  public bool Completed { get; set; }
  
  public IEnumerable<ApplicationData> Applications { get; set; }
}
#pragma warning restore CS8618
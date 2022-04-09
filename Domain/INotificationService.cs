using FCM.Net;
using Microsoft.EntityFrameworkCore;
using PitaPairing.Database;
using PitaPairing.StartUp;

namespace PitaPairing.Domain;

public record PushNotification(string Title, string Body, string Type, string Icon);

public interface INotificationService
{
    Task Send(Guid user, PushNotification notif);
}

public class NotificationService : INotificationService
{
    private readonly CoreDbContext _db;
    private readonly IGlobal _global;


    public NotificationService(CoreDbContext db, IGlobal @global)
    {
        _db = db;
        _global = global;
    }

    public async Task Send(Guid user, PushNotification notif)
    {
        var deviceTokens = await _db.Users.Include(x => x.Devices)
            .Where(x => x.Id == user)
            .SelectMany(x => x.Devices.Select(y => y.DeviceToken))
            .ToListAsync();
        using (var sender = new Sender(_global.FCMKey))
        {
            var message = new Message
            {
                RegistrationIds = deviceTokens,
                Notification = new Notification
                {
                    Title = notif.Title,
                    Body = notif.Body,
                },
                Data = new
                {
                    click_action = "FLUTTER_CLICK_NOTIFICATION",
                    title = notif.Title,
                    body = notif.Body,
                    icon = notif.Icon,
                    type = notif.Type,
                }
            };
            var result = await sender.SendAsync(message);
            Console.WriteLine($"Success: {result.MessageResponse.Success}");
        }
    }
}

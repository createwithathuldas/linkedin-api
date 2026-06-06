using Microsoft.AspNetCore.SignalR;

namespace linkedin_api.Hubs;

public class NotificationHub : Hub
{
    public Task JoinUser(int userId) => Groups.AddToGroupAsync(Context.ConnectionId, $"notifications:{userId}");
    public Task Publish(int userId, object notification) => Clients.Group($"notifications:{userId}").SendAsync("notification", notification);
}

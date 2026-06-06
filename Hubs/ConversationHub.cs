using Microsoft.AspNetCore.SignalR;

namespace linkedin_api.Hubs;

public class ConversationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var conversationId = Context.GetHttpContext()?.Request.RouteValues["conversationId"]?.ToString();
        if (!string.IsNullOrWhiteSpace(conversationId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation:{conversationId}");
        }
        await base.OnConnectedAsync();
    }

    public Task Typing(int conversationId, int userId) =>
        Clients.Group($"conversation:{conversationId}").SendAsync("typing", new { conversationId, userId, at = DateTime.UtcNow });

    public Task SendRealtimeMessage(int conversationId, object message) =>
        Clients.Group($"conversation:{conversationId}").SendAsync("message", message);
}

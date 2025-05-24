using Microsoft.AspNetCore.SignalR;

public class OrderHub : Hub
{
    // عند اتصال العميل، نضيفه إلى مجموعة تحتوي على معرف الصيدلية
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var pharmacyId = httpContext?.Request.Query["pharmacyId"];

        if (!string.IsNullOrEmpty(pharmacyId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"pharmacy-{pharmacyId}");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var httpContext = Context.GetHttpContext();
        var pharmacyId = httpContext?.Request.Query["pharmacyId"];

        if (!string.IsNullOrEmpty(pharmacyId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"pharmacy-{pharmacyId}");
        }

        await base.OnDisconnectedAsync(exception);
    }
}


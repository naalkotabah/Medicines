using Microsoft.AspNetCore.SignalR;

public class NotificationHub : Hub
{
   
    public async Task SendNotificationToPharmacy(int pharmacyId, string message)
    {
    
        await Clients.Group(pharmacyId.ToString()).SendAsync("ReceiveNotification", message);
    }

  
    public async Task JoinPharmacyGroup(int pharmacyId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, pharmacyId.ToString());
    }

   
    public async Task LeavePharmacyGroup(int pharmacyId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, pharmacyId.ToString());
    }
}

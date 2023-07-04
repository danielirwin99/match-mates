using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    // Only Authorised users to access the hub
    [Authorize]
    // Hub --> Derived from SignalR
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _tracker;
        public PresenceHub(PresenceTracker tracker)
        {
            _tracker = tracker;

        }

        // Connecting to the hub
        public override async Task OnConnectedAsync()
        {
            // Tracker that tracks the user's connection depending on their username and connectionId
            await _tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);

            // When this user connects it will notify the other users
            await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());

            // Gets who is online
            var currentUsersOnline = await _tracker.GetOnlineUsers();

            // List of who is current online / Listens to who is online
            await Clients.All.SendAsync("GetOnlineUsers", currentUsersOnline);
        }



        // Disconnecting to the hub
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Tracking our Users that disconnected
            await _tracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);

            await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());

            // Gets a list of online users MINUS the user we just disconnected/removed
            var currentUsersOnline = await _tracker.GetOnlineUsers();


            // List of who is current online --> Passing back currents users online
            await Clients.All.SendAsync("GetOnlineUsers", currentUsersOnline);

            // Since we are passing through an exception then we need to call this
            await base.OnDisconnectedAsync(exception);
        }
    }
}
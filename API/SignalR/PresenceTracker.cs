namespace API.SignalR
{
    public class PresenceTracker
    {
        // Takes in keys --> Username, List of Connection Id's from the username
        private static readonly Dictionary<string, List<string>> OnlineUsers = new Dictionary<string, List<string>>();

        public Task UserConnected(string username, string connectionId)
        {
            // Lock --> Stops multiple users accessing the dictionary at one time
            lock (OnlineUsers)
            {
                // If we do have a valid user
                if (OnlineUsers.ContainsKey(username))
                {
                    // Add it to the connection
                    OnlineUsers[username].Add(connectionId);
                }
                else
                // If there is no key add them to the list
                {
                    OnlineUsers.Add(username, new List<string> { connectionId });
                }
            }
            return Task.CompletedTask;
        }

        public Task UserDisconnected(string username, string connectionId)
        {
            lock (OnlineUsers)
            {
                // If the username is not there in our dictionary key
                if (!OnlineUsers.ContainsKey(username)) return Task.CompletedTask;

                // If it makes it passed the line above (safety check)
                OnlineUsers[username].Remove(connectionId);

                // Another safety precaution
                if (OnlineUsers[username].Count == 0)
                {
                    OnlineUsers.Remove(username);
                }
            }

            return Task.CompletedTask;
        }

        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUsers;
            lock (OnlineUsers)
            {
                // Alphabetical list of users that are online
                // We are only interested in the key so we are storing it in an array
                onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
            }

            // Returns the list of onlineUsers
            return Task.FromResult(onlineUsers);
        }

    }
}


namespace API.Entities
{
    public class Message
    {
        // Id of the message
        public int Id { get; set; }

        // Id of the Sender Id
        public int SenderId { get; set; }

        // Username of the Sender
        public string SenderUsername { get; set; }

        // Related Entity
        public AppUser Sender { get; set; }

        // ID of the receiver
        public int RecipientId { get; set; }

        // Receiver Username 
        public string RecipientUsername { get; set; }

        public AppUser Recipient { get; set; }

        // Content of the message
        public string Content { get; set; }

        // Time the receiver read the message (Optional)
        public DateTime? DateRead { get; set; }

        // When the message was sent
        public DateTime MessageSent { get; set; } = DateTime.UtcNow;

        // Checking if the Sender deleted the message 
        // --> Must have both sender and receiver delete message to remove it from the DB <--
        public bool SenderDeleted { get; set; }

        // Checking if the Receiver deleted the message
        public bool RecipientDeleted { get; set; }
    }
}
namespace API.DTOs
{
    public class MessageDTO
    {
        // Id of the message
        public int Id { get; set; }

        // Id of the Sender Id
        public int SenderId { get; set; }

        // Username of the Sender
        public string SenderUsername { get; set; }

        // Returns the Photo when they send the message
        public string SenderPhotoUrl { get; set; }


        // ID of the receiver
        public int RecipientId { get; set; }

        // Receiver Username 
        public string RecipientUsername { get; set; }

        // Returns the Photo of the Recipient
        public string RecipientPhotoUrl { get; set; }

        // Content of the message
        public string Content { get; set; }

        // Time the receiver read the message (Optional)
        public DateTime? DateRead { get; set; }

        // When the message was sent
        public DateTime MessageSent { get; set; }
    }
}
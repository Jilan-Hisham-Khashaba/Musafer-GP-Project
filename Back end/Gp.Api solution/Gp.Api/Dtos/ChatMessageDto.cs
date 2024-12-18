namespace Gp.Api.Dtos
{
    public class ChatMessageDto
    {
            public int Id { get; set; }
            public string Content { get; set; }
            public string? Sender { get; set; }
            public string Reciever { get; set; }
            public bool Seen { get; set; }
            public DateTime Timestamp { get; set; }
         public int? ReplyToMessageId { get; set; }
    }
}

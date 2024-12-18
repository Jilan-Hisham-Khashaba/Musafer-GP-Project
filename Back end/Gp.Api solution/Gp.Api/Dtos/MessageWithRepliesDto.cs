namespace Gp.Api.Dtos
{
    public class MessageWithRepliesDto
    {
        public ChatMessageDto Message { get; set; }
        public List<ChatMessageDto> Replies { get; set; }
    }
}

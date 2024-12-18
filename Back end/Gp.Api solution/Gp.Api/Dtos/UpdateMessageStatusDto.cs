namespace Gp.Api.Dtos
{
    public class UpdateMessageStatusDto
    {
        public int MessageId { get; set; }
        public bool IsRead { get; set; }
    }
}

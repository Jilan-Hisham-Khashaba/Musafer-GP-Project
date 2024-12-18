using GP.core.Entities.identity;

namespace Gp.Api.Dtos
{
    public class CommentDto
    {

        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }= DateTime.Now;

        public decimal Rate { get; set; }
        public string? UserId { get; set; }
        
        public string? userName { get; set;}

        public string? UserIdSender { get; set; }
        public string? UserSender{ get; set;}
    }
}

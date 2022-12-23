using System.ComponentModel.DataAnnotations;

namespace VlogAPI.Data.DTOs
{
    public class LikesDTO
    {
        public record CreateLikeDTO([Required] bool IsPositive);

        public record UpdateLikeDTO([Required] bool IsPositive);
        public record LikeDTO(int Id, bool IsPositive, DateTime CreationDate, int CommentId, int PostId, string NormalizedUsername, string UserId);
    }
}

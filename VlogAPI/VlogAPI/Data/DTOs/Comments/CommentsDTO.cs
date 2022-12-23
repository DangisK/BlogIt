using System.ComponentModel.DataAnnotations;

namespace VlogAPI.Data.DTOs
{
    public class CommentsDTO
    {
        public record CreateCommentDTO([Required] string Content);

        public record UpdateCommentDTO([Required] string Content);
        public record CommentDTO(int Id, string Content, DateTime CreationDate, int PostId, string Username, string UserId);
    }
}

using System.ComponentModel.DataAnnotations;

namespace VlogAPI.Data.DTOs
{
    public class CommentsDTO
    {
        public record CreateCommentDTO([Required] string content);

        public record UpdateCommentDTO([Required] string content);
        public record CommentDTO(int id, string content, DateTime creationDate);
    }
}

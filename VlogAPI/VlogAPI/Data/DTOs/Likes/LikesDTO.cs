using System.ComponentModel.DataAnnotations;

namespace VlogAPI.Data.DTOs
{
    public class LikesDTO
    {
        public record CreateLikeDTO([Required] bool isPositive);

        public record UpdateLikeDTO([Required] bool isPositive);
        public record LikeDTO(int id, bool isPositive, DateTime creationDate);
    }
}

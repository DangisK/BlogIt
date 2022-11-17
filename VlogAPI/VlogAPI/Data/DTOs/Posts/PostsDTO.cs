using System.ComponentModel.DataAnnotations;

namespace VlogAPI.Data.DTOs
{
    public record CreatePostDTO(string name, [Required] string body);

    public record UpdatePostDTO([Required] string body);
    public record PostDTO(int id, string name, string body, DateTime creationDate);
}

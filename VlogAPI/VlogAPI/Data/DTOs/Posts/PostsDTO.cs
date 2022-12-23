using System.ComponentModel.DataAnnotations;

namespace VlogAPI.Data.DTOs
{
    public record CreatePostDTO(string Name, [Required] string Body);

    public record UpdatePostDTO(string Name, [Required] string Body);
    public record PostDTO(int Id, string Name, string Body, DateTime CreationDate, string Username, string UserId);


}

using System.ComponentModel.DataAnnotations;

namespace VlogAPI.Auth.DTOs
{
    public record RegisterUserDTO([Required] string UserName, [EmailAddress][Required] string Email, [Required] string Password);

    public record LoginDTO(string UserName, string Password);

    public record UserDTO(string Id, string UserName, string Email);

    public record SuccessfulLoginDTO(string AccessToken);
}

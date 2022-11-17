using VlogAPI.Auth.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VlogAPI.Data.Entities
{
    public class Comment : IUserOwnedResource
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreationDate { get; set; }
        public Post Post { get; set; }
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public VlogUser? User { get; set; }
    }
}

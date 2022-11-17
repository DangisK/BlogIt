using VlogAPI.Auth.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VlogAPI.Data.Entities
{
    public class Like : IUserOwnedResource
    {
        public int Id { get; set; }
        public bool IsPositive { get; set; }
        public DateTime CreationDate { get; set; }
        public Comment Comment { get; set; }
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public VlogUser? User { get; set; }
    }
}

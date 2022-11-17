using Microsoft.AspNetCore.Identity;

namespace VlogAPI.Auth.Model
{
    public class VlogUser : IdentityUser
    {
        [PersonalData]
        public string? AdditionalInfo { get; set; }
    }
}

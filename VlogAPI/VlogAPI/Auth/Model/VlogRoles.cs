namespace VlogAPI.Auth.Model
{
    public class VlogRoles
    {
        public const string Admin = nameof(Admin);
        public const string VlogUser = nameof(VlogUser);

        public static readonly IReadOnlyCollection<string> All = new[] { Admin, VlogUser };
    }
}

using Microsoft.AspNetCore.Identity;


namespace Entities
{
    public class User : IdentityUser
    {
        public ICollection<UserLibary> userlibary { get; set; }
    }
}

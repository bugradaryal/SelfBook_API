using Microsoft.AspNetCore.Identity;


namespace Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<UserLibary> userlibary { get; set; }
    }
}

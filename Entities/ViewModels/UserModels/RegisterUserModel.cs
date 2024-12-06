using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.UserModels
{
    public class RegisterUserModel
    {
        [Required]
        [MaxLength(120)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(120)]
        public string LastName { get; set; }
        [Required]
        [MaxLength(60)]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MaxLength(16)]
        [MinLength(6)]
        public string Password { get; set; }
        [Required]
        [AllowedValues("Male", "Female", "Undefined")]
        public string Gender { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.UserModels
{
    public class UpdateUserModel
    {
        [MaxLength(60)]
        public string UserName { get; set; }
        [Phone]
        public string phoneNumber { get; set; }
        [MaxLength(120)]
        public string firstName { get; set; }
        [MaxLength(120)]
        public string lastName { get; set; }
        [AllowedValues("Male", "Female", "Undefined")]
        public string Gender { get; set; }
    }
}

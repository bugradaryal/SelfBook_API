using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.TokenModels
{
    public class TokenValidation
    {
        public User? user { get; set; }
        public string? errorMessage { get; set; }

        public List<string>? Role { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class UpdateUser
    {
        public string? Id { get; set; }
        public string UserName { get; set; }
        public string phoneNumber { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
    }
}

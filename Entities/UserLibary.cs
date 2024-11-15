using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class UserLibary
    {
        public int id { get; set; }
        public string user_id { get; set; }  // because of identity
        public int book_id { get; set; }
        public int current_page { get; set; }
        public DateOnly add_date { get; set; }

        public User user { get; set; }
        public Book book { get; set; }

    }
}

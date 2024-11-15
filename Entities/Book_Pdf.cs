using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Book_Pdf
    {
        public int id { get; set; }
        public int book_id { get; set; }
        public byte pdf { get; set; }

        public Book book { get; set; }
    }
}
 
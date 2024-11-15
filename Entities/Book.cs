using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Book
    {
        public int id { get; set; }
        public string name { get; set; }
        public string author { get; set; }
        public DateOnly release_date { get; set; }
        public int category_id { get; set; }
        public int page { get; set; }
        public byte image {  get; set; }
        public UserLibary userlibary { get; set; }
        public Category b_category { get; set; }
        public Book_Pdf book_pdf { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.BookModels
{
    public class UpdateBookModel
    {
        [MaxLength(160)]
        public string name { get; set; }
        [MaxLength(160)]
        public string author { get; set; }
        public DateOnly release_date { get; set; }
        public int category_id { get; set; }
        public int page { get; set; }
        public byte image { get; set; }
        public byte pdf { get; set; }
    }
}

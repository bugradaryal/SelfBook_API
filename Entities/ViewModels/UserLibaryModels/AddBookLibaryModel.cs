using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.UserLibaryModels
{
    public class AddBookLibaryModel
    {
        [Required]
        public int book_id { get; set; }
        [Required]
        public int current_page { get; set; }
        public DateOnly add_date { get; set; }
    }
}

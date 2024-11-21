using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace DataAccess.SeedData
{
    public class InsertCategories : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> entity)
        {
            entity.HasData(
                new Category { id = 1, category = "Health & Fitness" },
                new Category { id = 2, category = "Biographies" },
                new Category { id = 3, category = "Business" },
                new Category { id = 4, category = "Comics" },
                new Category { id = 5, category = "Cooking" },
                new Category { id = 6, category = "Education" },
                new Category { id = 7, category = "History" },
                new Category { id = 8, category = "Home & Garden & Crafts" },
                new Category { id = 9, category = "Romance" },
                new Category { id = 10, category = "Sci-Fi & Fantasy" },
                new Category { id = 11, category = "Sports & Travel" }
            );
        }
    }
}

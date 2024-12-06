using DataAccess.Abstract;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete
{
    public class CategoryRepository : ICategoryRepository
    {
        public async Task AddCategory(Category category)
        {
            using (var _DBContext = new DBContext())
            {
                _DBContext.Categories.Add(category);
                await _DBContext.SaveChangesAsync();
            }
        }
        public async Task DeleteCategory(int categoryId)
        {
            using (var _DBContext = new DBContext())
            {
                _DBContext.Categories.Remove(new Category { id = categoryId });
                await _DBContext.SaveChangesAsync();
            }
        }
    }
}

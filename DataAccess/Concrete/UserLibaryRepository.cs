using Azure;
using DataAccess.Abstract;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete
{
    public class UserLibaryRepository : IUserLibaryRepository
    {
        public async Task AddBookToLibary(UserLibary userLibary)
        {
            using (var _DBContext = new DBContext())
            {
                _DBContext.UserLibaries.Add(userLibary);
                await _DBContext.SaveChangesAsync();
            }
        }
        public async Task DeleteBookInLibary(string userId, int bookId)
        {
            using (var _DBContext = new DBContext())
            {
                _DBContext.UserLibaries.Remove(new UserLibary { book_id = bookId, user_id = userId });
                await _DBContext.SaveChangesAsync();
            }
        }
        public async Task<UserLibary> GetBookInLibary(int bookId)
        {
            using (var _DBContext = new DBContext())
            {
                var result = await _DBContext.UserLibaries.FirstOrDefaultAsync(x => x.id == bookId);
                return result;
            }
        }
        public async Task<List<UserLibary>> GetAllBookInLibary(int page, string orderBy)
        {
            using (var _DBContext = new DBContext())
            {
                IQueryable<UserLibary> query = _DBContext.UserLibaries;
                var parameter = Expression.Parameter(typeof(Book), "x");
                var property = Expression.Property(Expression.Parameter(typeof(UserLibary), "x"), orderBy);

                var lambda = Expression.Lambda<Func<UserLibary, object>>(Expression.Convert(property, typeof(object)), parameter);
                query = query.OrderBy(lambda);

                var result = await query.Skip((page - 1) * 5).Take(5).ToListAsync();
                return result;
            }
        }

    }
}

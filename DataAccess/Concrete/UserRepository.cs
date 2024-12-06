using DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccess.Concrete
{
    public class UserRepository : IUserRepository
    {
        public async Task<List<User>> GetAllUser(int page, string orderBy)
        {
            using (var _DBContext = new DBContext())
            {
                IQueryable<User> query = _DBContext.Users;
                var parameter = Expression.Parameter(typeof(User), "x");
                var property = Expression.Property(Expression.Parameter(typeof(User), "x"), orderBy);

                var lambda = Expression.Lambda<Func<User, object>>(Expression.Convert(property, typeof(object)), parameter);
                query = query.OrderBy(lambda);

                var result = await query.Skip((page - 1) * 10).Take(10).ToListAsync();
                return result;
            }
        }
    }
}

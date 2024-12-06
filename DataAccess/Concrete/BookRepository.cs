using DataAccess.Abstract;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete
{
    public class BookRepository : IBookRepository
    {
        public async Task AddBook(Book book)
        {
            using (var _DBContext = new DBContext())
            {
                var result = _DBContext.Books.Add(book);
                await _DBContext.SaveChangesAsync();
            }
        }

        public async Task DeleteBook(int bookId)
        {
            using (var _DBContext = new DBContext())
            {
                _DBContext.Books.Remove(new Book { id = bookId });
                await _DBContext.SaveChangesAsync();
            }
        }
        public async Task UpdateBook(Book book)
        {
            using (var _DBContext = new DBContext())
            {
                _DBContext.Books.Update(book);
                await _DBContext.SaveChangesAsync();
            }
        }
        public Book GetBook(int bookId)
        {
            using (var _DBContext = new DBContext())
            {
                var result = _DBContext.Books.FirstOrDefault(x => x.id == bookId);
                return result;
            }
        }
        public List<Book> GetAllBook(int page, string orderBy)
        {
            using (var _DBContext = new DBContext())
            {
                IQueryable<Book> query = _DBContext.Books;
                var parameter = Expression.Parameter(typeof(Book), "x"); 
                var property = Expression.Property(Expression.Parameter(typeof(Book), "x"), orderBy);

                var lambda = Expression.Lambda<Func<Book, object>>(Expression.Convert(property, typeof(object)), parameter);
                query = query.OrderBy(lambda);

                var result = query.Skip((page - 1) * 5).Take(5).ToList();
                return result;
            }
        }
    }
}

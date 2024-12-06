using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstract
{
    public interface IBookRepository
    {
        Task AddBook(Book book);
        Task DeleteBook(int bookId);
        Task UpdateBook(Book book);
        Task<Book> GetBook(int bookId);
        Task<List<Book>> GetAllBook(int page, string orderBy);
    }
}

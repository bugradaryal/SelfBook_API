using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IBookService
    {
        Task AddBook(Book book);
        Task DeleteBook(int bookId);
        Task UpdateBook(Book book);
        Book GetBook(int bookId);
        List<Book> GetAllBook(int page, string orderBy);
    }
}

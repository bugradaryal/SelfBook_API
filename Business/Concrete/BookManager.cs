using Business.Abstract;
using DataAccess.Abstract;
using DataAccess.Concrete;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class BookManager : IBookService
    {
        private IBookRepository _bookRepository;

        public BookManager()
        {
            _bookRepository = new BookRepository();
        }


        public async Task AddBook(Book book)
        {
            await _bookRepository.AddBook(book);
        }
        public async Task DeleteBook(int bookId)
        {
            await _bookRepository.DeleteBook(bookId);
        }
        public async Task UpdateBook(Book book)
        {
            await _bookRepository.UpdateBook(book);
        }
        public Book GetBook(int bookId)
        {
            var book = _bookRepository.GetBook(bookId);
            return book;
        }
        public List<Book> GetAllBook(int page, string orderBy)
        {
            if (orderBy != "name" || orderBy != "author" || orderBy != "release_date" || orderBy != "page")
                throw new Exception("Wrong order type!!");
            var books = _bookRepository.GetAllBook(page, orderBy);
            return books;
        }
    }
}

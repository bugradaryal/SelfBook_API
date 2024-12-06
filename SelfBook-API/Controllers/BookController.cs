using Business.Abstract;
using Business.Concrete;
using Entities;
using Entities.DTOs;
using Entities.ViewModels.BookModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SelfBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private IBookService _bookService;

        public BookController()
        {
            _bookService = new BookManager();
        }


        [HttpPost("AddBook")]
        [Authorize(Roles = "Moderator,Administrator")]
        public async Task<IActionResult> AddBook(AddBookModel bookModel)
        {
            try
            {
                await _bookService.AddBook(new Book
                {
                    name = bookModel.name,
                    author = bookModel.author,
                    release_date = bookModel.release_date,
                    category_id = bookModel.category_id,
                    page = bookModel.page,
                    image = bookModel.image,
                    pdf = bookModel.pdf,
                });
                return Ok(new {message = "Book added!"});
            }
            catch (Exception ex) 
            {
                return BadRequest(new {message = ex.Message });
            }
        }

        [HttpPost("DeleteBook")]
        [Authorize(Roles = "Moderator,Administrator")]
        public async Task<IActionResult> DeleteBook(int bookId)
        {
            try
            {
                await _bookService.DeleteBook(bookId);
                return Ok(new { message = "Book deleted!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("UpdateBook")]
        [Authorize(Roles = "Moderator,Administrator")]
        public async Task<IActionResult> UpdateBook(UpdateBookModel updateBookModel)
        {
            try
            {
                var book = new Book();
                if (!string.IsNullOrEmpty(updateBookModel.name))
                    book.name = updateBookModel.name;

                if (!string.IsNullOrEmpty(updateBookModel.author))
                    book.author = updateBookModel.author;

                if (updateBookModel.release_date != null)
                    book.release_date = updateBookModel.release_date;

                if (updateBookModel.category_id != null)
                    book.category_id = updateBookModel.category_id;

                if (updateBookModel.page != null)
                    book.page = updateBookModel.page;

                if (updateBookModel.image != null)
                    book.image = updateBookModel.image;

                if (updateBookModel.pdf != null)
                    book.pdf = updateBookModel.pdf;

                await _bookService.UpdateBook(book);
                return Ok(new { message = "Book uptaded!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("GetBook")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBook(int bookId)
        {
            try
            {
                var book = await _bookService.GetBook(bookId);
                return Ok(book);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetAllBook")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllBook(int page = 1, string orderBy = "name")
        {
            try
            {
                if (orderBy != "name" || orderBy != "author" || orderBy != "release_date" || orderBy != "page")
                    return BadRequest(new { message = "Wrong order type!!"});
                var bookList = await _bookService.GetAllBook(page, orderBy);
                return Ok(bookList);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

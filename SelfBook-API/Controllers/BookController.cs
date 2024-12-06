using Business.Abstract;
using Business.Concrete;
using Entities;
using Entities.DTOs;
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


        [HttpPost]
        [Authorize(Roles = "Moderator,Administrator")]
        public async Task<IActionResult> AddBook(Book book)
        {
            try
            {
                await _bookService.AddBook(book);
                return Ok();
            }
            catch (Exception ex) 
            {
                return BadRequest(ex);
            }
        }
    }
}

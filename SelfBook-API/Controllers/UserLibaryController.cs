using Business.Abstract;
using Business.Concrete;
using Entities;
using Entities.ViewModels.ConfigurationModels;
using Entities.ViewModels.UserLibaryModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;

namespace SelfBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserLibaryController : ControllerBase
    {
        private ITokenServices _tokenServices;
        private IUserLibaryService _userLibaryService;
        public UserLibaryController(IOptions<JWT> jwt,UserManager<User> userManager) 
        {
            _tokenServices = new TokenManager(jwt, userManager);
            _userLibaryService = new UserLibaryManager();
        }

        [HttpPost("AddBookToLibary")]
        [Authorize]
        public async Task<IActionResult> AddBookToLibary(AddBookLibaryModel addBookLibaryModel)
        {
            try
            {
                var response = await _tokenServices.ValidateToken(HttpContext);
                if (response.errorMessage != null)
                    return BadRequest(new { message = response.errorMessage });
                await _userLibaryService.AddBookToLibary(new UserLibary
                {
                    user_id = response.user.Id,
                    book_id = addBookLibaryModel.book_id,
                    current_page = addBookLibaryModel.current_page,
                    add_date = addBookLibaryModel.add_date,
                });
                return Ok(new { message = "Book added to libary!!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("DeleteBookInLibary")]
        [Authorize]
        public async Task<IActionResult> DeleteBookInLibary(int bookId)
        {
            try
            {
                var response = await _tokenServices.ValidateToken(HttpContext);
                if (response.errorMessage != null)
                    return BadRequest(new { message = response.errorMessage });

                await _userLibaryService.DeleteBookInLibary(response.user.Id,bookId);
                return Ok(new { message = "Book deleted in libary!!" });
            }
            catch (Exception ex) 
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("GetBookInLibary")]
        [Authorize]
        public async Task<IActionResult> GetBookInLibary(int bookId)
        {
            try
            {
                var book = await _userLibaryService.GetBookInLibary(bookId);
                return Ok(book);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("GetAllBookInLibary")]
        [Authorize]
        public async Task<IActionResult> GetAllBookInLibary(int page, string orderBy)
        {
            try
            {
                var bookList = await _userLibaryService.GetAllBookInLibary(page, orderBy);
                return Ok(bookList);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}

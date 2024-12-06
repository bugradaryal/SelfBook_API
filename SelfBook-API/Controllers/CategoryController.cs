using Business.Abstract;
using Business.Concrete;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SelfBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private ICategoryService _categoryService;
        public CategoryController() 
        {
            _categoryService = new CategoryManager();
        }

        [HttpPost("AddCategory")]
        [Authorize(Roles = "Moderator,Administrator")]
        public async Task<IActionResult> AddCategory(string newCategory)
        {
            try
            {
                await _categoryService.AddCategory(new Category { category = newCategory });
                return Ok(new { message = "New Category Added!" });
            }
            catch (Exception ex) 
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("DeleteCategory")]
        [Authorize(Roles = "Moderator,Administrator")]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            try
            {
                await _categoryService.DeleteCategory(categoryId);
                return Ok(new { message = "Category Deleted!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

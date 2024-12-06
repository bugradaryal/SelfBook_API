using Business.Abstract;
using DataAccess.Abstract;
using DataAccess.Concrete;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class UserLibaryManager : IUserLibaryService
    {
        private IUserLibaryRepository _libaryRepository;
        public UserLibaryManager()
        {
           _libaryRepository = new UserLibaryRepository();
        }
        public async Task AddBookToLibary(UserLibary userLibary)
        {
            await _libaryRepository.AddBookToLibary(userLibary);
        }
        public async Task DeleteBookInLibary(string userId, int bookId)
        {
            await _libaryRepository.DeleteBookInLibary(userId, bookId);
        }
        public async Task<UserLibary> GetBookInLibary(int bookId)
        {
            var result = await _libaryRepository.GetBookInLibary(bookId);
            return result;
        }
        public async Task<List<UserLibary>> GetAllBookInLibary(int page, string orderBy)
        {
            var result = await _libaryRepository.GetAllBookInLibary(page, orderBy);
            return result;
        }

    }
}

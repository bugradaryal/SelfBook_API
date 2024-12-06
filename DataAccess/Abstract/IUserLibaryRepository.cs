﻿using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstract
{
    public interface IUserLibaryRepository
    {
        Task AddBookToLibary(UserLibary userLibary);
        Task DeleteBookInLibary(string userId, int bookId);
        Task<UserLibary> GetBookInLibary(int bookId);
        Task<List<UserLibary>> GetAllBookInLibary(int page, string orderBy);
    }
}

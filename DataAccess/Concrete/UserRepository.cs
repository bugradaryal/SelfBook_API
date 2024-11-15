using DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Microsoft.AspNetCore.Identity;

namespace DataAccess.Concrete
{
    public class UserRepository : IUserRepository
    {
        /*
        public List<Hotel> GetAllHotels()
        {
            using (var _DBContext = new DBContext())
            {
                return _DBContext.Hotels.Where(x => x.MyProperty == 1).ToList();
            }
        }
        */
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class UsersRepository : Repository<User>
    {

        public UsersRepository(UnitOfWork uow)
            : base(uow)
        { }

        public User GetUserByEmail(string email)
        {
            return CurrentUOW.UserManager.FindByEmailSync(email);
        }
    }
}

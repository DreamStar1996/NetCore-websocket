using System;
using System.Collections.Generic;
using System.Text;
using Model;
namespace DAL.Impl
{

    public class UserDAL : BaseDAL<User>, IUserDAL
    {
        public UserDAL(MyDbContext db) : base(db)
        {
        }
    }
}

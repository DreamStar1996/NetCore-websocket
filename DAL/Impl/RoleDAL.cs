using System;
using System.Collections.Generic;
using System.Text;
using Model;
namespace DAL.Impl
{

    public class RoleDAL : BaseDAL<Role>, IRoleDAL
    {
        public RoleDAL(MyDbContext db) : base(db)
        {
        }
    }
}

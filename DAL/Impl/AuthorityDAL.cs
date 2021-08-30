using System;
using System.Collections.Generic;
using System.Text;
using Model;
namespace DAL.Impl
{

    public class AuthorityDAL : BaseDAL<Authority>, IAuthorityDAL
    {
        public AuthorityDAL(MyDbContext db) : base(db)
        {
        }
    }
}

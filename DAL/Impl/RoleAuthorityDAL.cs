using System;
using System.Collections.Generic;
using System.Text;
using Model;
namespace DAL.Impl
{

    public class RoleAuthorityDAL : BaseDAL<RoleAuthority>, IRoleAuthorityDAL
    {
        public RoleAuthorityDAL(MyDbContext db) : base(db)
        {
        }
    }
}

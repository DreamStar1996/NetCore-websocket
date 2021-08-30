using System;
using System.Collections.Generic;
using System.Text;
using Model;
using DAL;
namespace Bll.Impl
{
    public class RoleAuthorityBll : BaseBll<RoleAuthority>, IRoleAuthorityBll
    {
        public RoleAuthorityBll(IRoleAuthorityDAL dal):base(dal)
        {
        }
    }
}

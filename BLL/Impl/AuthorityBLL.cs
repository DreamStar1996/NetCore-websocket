using System;
using System.Collections.Generic;
using System.Text;
using Model;
using DAL;
namespace Bll.Impl
{
    public class AuthorityBll : BaseBll<Authority>, IAuthorityBll
    {
        public AuthorityBll(IAuthorityDAL dal):base(dal)
        {
        }
    }
}

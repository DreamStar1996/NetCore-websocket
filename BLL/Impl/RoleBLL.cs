using System;
using System.Collections.Generic;
using System.Text;
using Model;
using DAL;
namespace Bll.Impl
{
    public class RoleBll : BaseBll<Role>, IRoleBll
    {
        public RoleBll(IRoleDAL dal):base(dal)
        {
        }
    }
}

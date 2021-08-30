using System;
using System.Collections.Generic;
using System.Text;
using Model;
using DAL;
namespace Bll.Impl
{
    public class AreaBll : BaseBll<Area>, IAreaBll
    {
        public AreaBll(IAreaDAL dal):base(dal)
        {
        }
    }
}

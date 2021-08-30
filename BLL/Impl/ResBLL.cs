using System;
using System.Collections.Generic;
using System.Text;
using Model;
using DAL;
namespace Bll.Impl
{
    public class ResBll : BaseBll<Res>, IResBll
    {
        public ResBll(IResDAL dal):base(dal)
        {
        }
    }
}

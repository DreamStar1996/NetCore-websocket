using System;
using System.Collections.Generic;
using System.Text;
using Model;
namespace DAL.Impl
{

    public class ResDAL : BaseDAL<Res>, IResDAL
    {
        public ResDAL(MyDbContext db) : base(db)
        {
        }
    }
}

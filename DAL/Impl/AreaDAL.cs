using System;
using System.Collections.Generic;
using System.Text;
using Model;
namespace DAL.Impl
{

    public class AreaDAL : BaseDAL<Area>, IAreaDAL
    {
        public AreaDAL(MyDbContext db) : base(db)
        {
        }
    }
}

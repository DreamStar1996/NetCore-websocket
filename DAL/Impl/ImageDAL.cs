using System;
using System.Collections.Generic;
using System.Text;
using Model;
namespace DAL.Impl
{

    public class ImageDAL : BaseDAL<Image>, IImageDAL
    {
        public ImageDAL(MyDbContext db) : base(db)
        {
        }
    }
}

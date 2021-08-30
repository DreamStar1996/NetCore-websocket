using System;
using System.Collections.Generic;
using System.Text;
using Model;
using DAL;
namespace Bll.Impl
{
    public class ImageBll : BaseBll<Image>, IImageBll
    {
        public ImageBll(IImageDAL dal):base(dal)
        {
        }
    }
}

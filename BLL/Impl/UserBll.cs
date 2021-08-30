using System;
using System.Collections.Generic;
using System.Text;
using Model;
using DAL;
using Common.Util;
namespace Bll.Impl
{
    public class UserBll : BaseBll<User>, IUserBll
    {
        public UserBll(IUserDAL dal) : base(dal)
        {
        }
        public new bool Add(User o)
        {
            o.Pswd = Security.Md5(o.Pswd);
            return dal.Add(o);
        }
        public User Login(string userame, string pswd)
        {
            pswd = Security.Md5(pswd);
            return dal.SelectOne(o => o.Username == userame && o.Pswd == pswd);
        }
       
    }
}

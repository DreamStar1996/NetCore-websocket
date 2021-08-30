using System;
using System.Collections.Generic;
using System.Text;
using Model;
namespace Bll
{
   
    public interface IUserBll:IBaseBll<User>
    {
        public  User Login(string nuserame, string pswd);
       
    }
}

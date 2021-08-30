using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Model;

namespace Web.DB
{
    /// <summary>
    /// 初始化数据
    /// </summary>
    public static class InitDatabase
    {
        public static bool Initialize(MyDbContext context)
        {
            // Look for any Users.
            if (context.Users.Any())
            {
                return false;   // DB has been seeded
            }

            var Users = new User[]
            {
            new User{Username="admin",Pswd=Common.Util.Security.Md5("123456"),Role="admin"}
            };
            foreach (User s in Users)
            {
                context.Users.Add(s);
            }
            context.SaveChanges();
            return true;
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore;
using Common.Util;

namespace Model
{
    public class MyDbContext : DbContext
    {
        //public MyDbContext()
        //{
        //}
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //modelBuilder.Entity<User>().HasData(
            //    new User {Id=1, Username = "admin", Pswd = Security.Md5("123456") });
            //    modelBuilder
            //.Entity<Goods>()
            //.Property(e => e.Photos)
            //.HasConversion(
            //    v => string.Join("|", v), 
            //    v => v.Split(new char[] { '|' })
            //    );
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //如果启用了数据池services.AddDbContextPool，不能在OnConfiguring配置
            //开启EF Core的延迟加载功能  使用延迟加载的最简单方式是安装 Microsoft.EntityFrameworkCore.Proxies 包，并通过调用 UseLazyLoadingProxies 来启用。
            //optionsBuilder.UseLazyLoadingProxies();
        }

        public virtual DbSet<Area> Areas { get; set; }       
        public virtual DbSet<Image> Images { get; set; }
       
        
        public virtual DbSet<User> Users { get; set; }
       
        public virtual DbSet<Authority> Authoritys { get; set; }
        public virtual DbSet<AuthorityRes> AuthorityRess { get; set; }      
       
        public virtual DbSet<Res> Ress { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<RoleAuthority> RoleAuthoritys { get; set; }      
    }
}

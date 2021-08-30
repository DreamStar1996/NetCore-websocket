using System;
using System.Collections.Generic;
using System.Text;
using DAL;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Common;
using DAL.Extension;
namespace DAL.Impl
{

    public class BaseDAL<T> : IBaseDAL<T> where T : ID, new()
    {
        private MyDbContext db;
        public BaseDAL(MyDbContext db)
        {
            this.db = db;
        }
        public MyDbContext DbContext()
        {
            return this.db;
        }
        public bool Add(T entity)
        {
            db.Set<T>().Add(entity);
            return db.SaveChanges() > 0;
        }
        public bool Add(IEnumerable<T> entities)
        {
            db.Set<T>().AddRangeAsync(entities);
            return db.SaveChanges() > 0;
        }

        public bool Delete(T entity)
        {
            db.Set<T>().Remove(entity);
            return db.SaveChanges() > 0;
        }
        public bool Delete(int id)
        {
            var obj = db.Set<T>().SingleOrDefault(o => o.Id == id);
            db.Set<T>().Remove(obj);
            return db.SaveChanges() > 0;
        }
        public bool Delete(Expression<Func<T, bool>> where)
        {
            IQueryable<T> qlist = db.Set<T>().Where(where);
            if (qlist != null)
            {
                db.Set<T>().RemoveRange(qlist);
                return db.SaveChanges() > 0;
            }
            return false;
        }
        public bool Delete(Dictionary<string, string> where)
        {
            IQueryable<T> qlist = db.Set<T>().Where(where);
            if (qlist != null)
            {
                db.Set<T>().RemoveRange(qlist);
                return db.SaveChanges() > 0;
            }
            return false;
        }
        public bool Update(T entity, params string[] propertyNames)
        {
            EntityEntry entry = db.Entry<T>(entity);
            entry.State = EntityState.Unchanged;
            foreach (var item in propertyNames)
            {
                entry.Property(item).IsModified = true;
            }
            return db.SaveChanges() > 0;
        }
        public bool Update(T o)
        {
             //db.Entry(o).State = EntityState.Deleted;//先删除掉原来的，为什么？  这样写会在一个操作方法  同时调用Add和Update方法时，导致前面调用的 db.SaveChanges()方法无效，即使加了事务。
            //将上面 的改成如下 可解决 同时调用两个savechange问题。初步测试可行
            //if (db.Entry(o).State != EntityState.Unchanged)
            //{
            //   // db.Entry(o).State = EntityState.Deleted;//先删除掉原来的
            //}
            db.Entry(o).State = EntityState.Modified;
            //db.Set<TValue>().Update(o);
            return db.SaveChanges() > 0;
        }
        public T SelectOne(int id)
        {
            return db.Set<T>().FirstOrDefault(o => o.Id == id);
        }
        public T SelectOne(Expression<Func<T, bool>> where)
        {
            return db.Set<T>().Where(where)?.FirstOrDefault();
        }
        public T SelectOne(Dictionary<string, string> where, string excludes = null)
        {
            return db.Set<T>().Where(where, excludes)?.FirstOrDefault();
        }
        public Pagination<T> Query(Dictionary<string, string> where, string excludes = null)
        {
            return db.Set<T>().Conditions<T>(where, excludes);
        }
        public DbSet<T> Query()
        {
            return db.Set<T>();
        }
        public IEnumerable<T> SelectAll()
        {
            return db.Set<T>().ToList();
        }
        public IEnumerable<T> SelectAll(Expression<Func<T, bool>> where)
        {
            return db.Set<T>().Where(where).ToList();
        }
        public IEnumerable<T> SelectAll(Dictionary<string, string> where, string excludes = null)
        {
            return db.Set<T>().Where(where, excludes).ToList();
        }
        public Pagination<T> SelectAll(Expression<Func<T, bool>> where, int pageNo, int pageSize)
        {
            return SelectAll<object>(where, o => o.Id, true, pageNo, pageSize);
        }
        public Pagination<T> SelectAll<OrderKey>(Expression<Func<T, bool>> whereLambda, Func<T, OrderKey> orderbyLambda, bool asc, int pageNo, int pageSize)
        {
            var objs = db.Set<T>();
            int total = objs.Count();
            int pageCount = (int)(Math.Ceiling(total * 1.0 / pageSize));
            if (asc)
            {
                return Pagination<T>.Init(pageCount, total, objs.Where(whereLambda).OrderBy<T, OrderKey>(orderbyLambda).Skip((pageNo - 1) * pageSize).Take(pageSize));
            }
            else
            {
                return Pagination<T>.Init(pageCount, total, objs.Where(whereLambda).OrderByDescending<T, OrderKey>(orderbyLambda).Skip((pageNo - 1) * pageSize).Take(pageSize));
            }
        }      
    }
}

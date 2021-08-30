using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Model;
using Common;
using Microsoft.EntityFrameworkCore;

namespace Bll
{
    public interface IBaseBll<T> where T : ID, new()
    {
        MyDbContext DbContext();
        bool Add(T entity);
        bool Add(IEnumerable<T> entities);
        bool Delete(T entity);
        bool Delete(int id);
        bool Delete(Expression<Func<T, bool>> where);
        bool Delete(Dictionary<string, string> where);
        bool Update(T entity, params string[] propertyNames);
        bool Update(T entity);
        T SelectOne(int id);
        T SelectOne(Dictionary<string, string> where, string excludes = null);
        Pagination<T> Query(Dictionary<string, string> where, string excludes = null);
        DbSet<T> Query();
        T SelectOne(Expression<Func<T, bool>> whereLambda);
        IEnumerable<T> SelectAll();
        IEnumerable<T> SelectAll(Expression<Func<T, bool>> where);
        IEnumerable<T> SelectAll(Dictionary<string, string> where, string excludes = null);
        Pagination<T> SelectAll(Expression<Func<T, bool>> whereLambda, int pageNo, int pageSize);
        Pagination<T> SelectAll<OrderKey>(Expression<Func<T, bool>> whereLambda, Func<T, OrderKey> orderbyLambda, bool asc, int pageNo, int pageSize);
    }
}

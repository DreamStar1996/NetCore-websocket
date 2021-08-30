using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Linq;
using Model;
using Common;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public interface IBaseDAL<T> where T : ID, new()
    {
        /// <summary>
        /// 获取数据上下文对象
        /// </summary>
        /// <returns></returns>
        MyDbContext DbContext();
        /// <summary>
        /// 添加实体对象
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Add(T entity);
        /// <summary>
        /// 批量添加实体对象
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        bool Add(IEnumerable<T> entities);      
        /// <summary>
        /// 删除指定实体对象
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Delete(T entity);
        /// <summary>
        /// 根据id删除实体uix
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Delete(int id);
        /// <summary>
        /// 根据拉姆达条件参数删除实体对象
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        bool Delete(Expression<Func<T, bool>> where);
        /// <summary>
        /// 根据查询条件删除。参考DAL.Extension.QueryableExtensions#Conditions<T>(this IQueryable<T> query, Dictionary<string, string> where, string excludes = null)方法。
        /// </summary>
        /// <param name="where">
        /// 添加过滤条件 如key="parent.id__eq" value = 1 如果添加时不加操作符 默认是等于eq 即如key=parent
        /// 实际key是parent_eq
        /// key 如 name__like、name__eq
        /// value 如果是in查询 多个值之间","分隔
        /// 如果是排序 值的格式为：+|-字段名,+|-字段名</param>
        /// <returns></returns>
        bool Delete(Dictionary<string, string> where);
        /// <summary>
        /// 修改实体对象属性
        /// </summary>
        /// <param name="entity">实体对象，即属性值变化后的实体对象</param>
        /// <param name="propertyNames">指定那几个字段是要修改的</param>
        /// <returns></returns>
        bool Update(T entity, params string[] propertyNames);
        /// <summary>
        /// 修改实体对象顺序
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Update(T entity);
        /// <summary>
        /// 根据id查询单个实体对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T SelectOne(int id);
        /// <summary>
        /// 根据拉姆达表达式查询单个实体对象
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        T SelectOne(Expression<Func<T, bool>> whereLambda);
        /// <summary>
        /// 根据查询字符串查询单个实体对象
        /// </summary>
        /// <param name="where"> 
        /// 添加过滤条件 如key="parent.id__eq" value = 1 如果添加时不加操作符 默认是等于eq 即如key=parent
        /// 实际key是parent_eq
        /// key 如 name__like、name__eq
        /// value 如果是in查询 多个值之间","分隔
        /// 如果是排序 值的格式为：+|-字段名,+|-字段名
        /// 
        /// 参考DAL.Extension.QueryableExtensions#Conditions<T>(this IQueryable<T> query, Dictionary<string, string> where, string excludes = null)方法。
        /// </param>
        /// <param name="excludes">排除那些字段，多个字段用,隔开</param>
        /// <returns></returns>
        T SelectOne(Dictionary<string, string> where, string excludes = null);
        /// <summary>
        /// 条件查询，并附带分页
        /// </summary>
        /// <param name="where"></param>
        /// <param name="excludes">排除那些字段，多个字段用,隔开</param>
        /// <returns></returns>
        Pagination<T> Query(Dictionary<string, string> where,string excludes= null);
        /// <summary>
        /// 获取查询集合
        /// </summary>
        /// <returns></returns>
        DbSet<T> Query();
        IEnumerable<T> SelectAll();
        /// <summary>
        /// 根据拉姆达条件表达式查询所有实体集合
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        IEnumerable<T> SelectAll(Expression<Func<T, bool>> where);
        /// <summary>
        /// 查询所有
        /// </summary>
        /// <param name="where">
        /// 添加过滤条件 如key="parent.id__eq" value = 1 如果添加时不加操作符 默认是等于eq 即如key=parent
        /// 实际key是parent_eq
        /// key 如 name__like、name__eq
        /// value 如果是in查询 多个值之间","分隔
        /// 如果是排序 值的格式为：+|-字段名,+|-字段名
        /// 
        /// 参考DAL.Extension.QueryableExtensions#Conditions<T>(this IQueryable<T> query, Dictionary<string, string> where, string excludes = null)方法。
        /// </param>
        /// <param name="excludes">排除那些字段，多个字段用,隔开</param>
        /// <returns></returns>
        IEnumerable<T> SelectAll(Dictionary<string, string> where, string excludes = null);
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="whereLambda">条件表达式</param>
        /// <param name="pageNo">页码</param>
        /// <param name="pageSize">每页显示数量</param>
        /// <returns></returns>
        Pagination<T> SelectAll(Expression<Func<T, bool>> whereLambda, int pageNo, int pageSize);
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="OrderKey"></typeparam>
        /// <param name="whereLambda">条件表达式</param>
        /// <param name="orderbyLambda">排序字段</param>
        /// <param name="asc">是否升序</param>
        /// <param name="pageNo">页码</param>
        /// <param name="pageSize">每页显示记录数</param>
        /// <returns></returns>
        Pagination<T>  SelectAll<OrderKey>(Expression<Func<T, bool>> whereLambda, Func<T, OrderKey> orderbyLambda, bool asc, int pageNo, int pageSize);

    }
}

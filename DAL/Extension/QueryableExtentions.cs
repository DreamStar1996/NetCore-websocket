using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;
using static System.Linq.Expressions.Expression;

namespace DAL.Extension
{
    public static class QueryableExtentions
    {
        public static IQueryable<TTarget> Select<TTarget>(this IQueryable<object> query)
        {
            return Queryable.Select(query, GetLamda<object, TTarget>(query.GetType().GetGenericArguments()[0]));
        }
        public static IQueryable<TTarget> Select<TTarget>(this IQueryable<object> query,string excludes)
        {
            return Queryable.Select(query, GetLamda<object, TTarget>(query.GetType().GetGenericArguments()[0], excludes));
        }

        public static IQueryable<TTarget> Select<TSource, TTarget>(this IQueryable<TSource> query, string excludes)
        {
            return Queryable.Select(query, GetLamda<TSource, TTarget>(null, excludes));
        }        
        public static Expression<Func<TSource, TTarget>> GetLamda<TSource, TTarget>(Type type = null, string excludes=null)
        {
            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);
            var parameter = Parameter(sourceType);
            Expression propertyParameter;
            if (type != null)
            {
                propertyParameter = Convert(parameter, type);
                sourceType = type;
            }
            else
                propertyParameter = parameter;

            return Lambda<Func<TSource, TTarget>>(GetExpression(propertyParameter, sourceType, targetType, excludes), parameter);
        }

        public static MemberInitExpression GetExpression(Expression parameter, Type sourceType, Type targetType, string excludes = null)
        {
            var memberBindings = new List<MemberBinding>();
            foreach (var targetItem in targetType.GetProperties().Where(x => x.CanWrite))
            {
                //先排除
                if (excludes != null&& excludes.ToLower().Contains(targetItem.Name.ToLower())) continue;
                //关联
                var fromEntityAttr = targetItem.GetCustomAttribute<FromModelAttribute>();
                if (fromEntityAttr != null)
                {
                    var property = GetFromEntityExpression(parameter, sourceType, fromEntityAttr);
                    if (property != null)
                        memberBindings.Add(Bind(targetItem, property));
                    continue;
                }

                var sourceItem = sourceType.GetProperty(targetItem.Name);
                if (sourceItem == null)//当没有对应的属性时，查找 实体名+属性
                {
                    var complexSourceItemProperty = GetCombinationExpression(parameter, sourceType, targetItem);
                    if (complexSourceItemProperty != null)
                        memberBindings.Add(Bind(targetItem, complexSourceItemProperty));
                    continue;
                }

                //判断实体的读写权限
                if (sourceItem == null || !sourceItem.CanRead)
                    continue;

                //标注NotMapped特性的属性忽略转换
                if (sourceItem.GetCustomAttribute<NotMappedAttribute>() != null)
                    continue;

                var sourceProperty = Property(parameter, sourceItem);

                //当非值类型且类型不相同时
                if (!sourceItem.PropertyType.IsValueType && sourceItem.PropertyType != targetItem.PropertyType && targetItem.PropertyType != targetType)
                {
                    //判断都是(非泛型、非数组)class
                    if (sourceItem.PropertyType.IsClass && targetItem.PropertyType.IsClass
                        && !sourceItem.PropertyType.IsArray && !targetItem.PropertyType.IsArray
                        && !sourceItem.PropertyType.IsGenericType && !targetItem.PropertyType.IsGenericType)
                    {
                        var expression = GetExpression(sourceProperty, sourceItem.PropertyType, targetItem.PropertyType);
                        memberBindings.Add(Bind(targetItem, expression));
                    }
                    continue;
                }

                if (targetItem.PropertyType != sourceItem.PropertyType)
                    continue;

                memberBindings.Add(Bind(targetItem, sourceProperty));
            }

            return MemberInit(New(targetType), memberBindings);
        }

        /// <summary>
        /// 根据FromModelAttribute 的值获取属性对应的路径
        /// </summary>
        /// <param name="sourceProperty"></param>
        /// <param name="sourceType"></param>
        /// <param name="FromModelAttribute"></param>
        /// <returns></returns>
        private static Expression GetFromEntityExpression(Expression sourceProperty, Type sourceType, FromModelAttribute FromModelAttribute)
        {
            var findType = sourceType;
            var resultProperty = sourceProperty;
            var tableNames = FromModelAttribute.ModelNames;
            if (tableNames == null)
            {
                var columnProperty = findType.GetProperty(FromModelAttribute.ModelColumn);
                if (columnProperty == null)
                    return null;
                else
                    return Property(resultProperty, columnProperty);
            }

            for (int i = 0; i < tableNames.Length; i++)
            {
                var tableProperty = findType.GetProperty(tableNames[i]);
                if (tableProperty == null)
                    return null;

                findType = tableProperty.PropertyType;
                resultProperty = Property(resultProperty, tableProperty);
            }

            var property = findType.GetProperty(FromModelAttribute.ModelColumn);
            if (property == null)
                return null;
            else
                return Property(resultProperty, property);
        }

        /// <summary>
        /// 根据组合字段获取其属性路径
        /// </summary>
        /// <param name="sourceProperty"></param>
        /// <param name="sourcePropertys"></param>
        /// <param name="targetItem"></param>
        /// <returns></returns>
        private static Expression GetCombinationExpression(Expression sourceProperty, Type sourceType, PropertyInfo targetItem)
        {
            foreach (var item in sourceType.GetProperties().Where(x => x.CanRead))
            {
                if (targetItem.Name.StartsWith(item.Name))
                {
                    if (item != null && item.CanRead && item.PropertyType.IsClass && !item.PropertyType.IsGenericType)
                    {
                        var rightName = targetItem.Name.Substring(item.Name.Length);

                        var complexSourceItem = item.PropertyType.GetProperty(rightName);
                        if (complexSourceItem != null && complexSourceItem.CanRead)
                            return Property(Property(sourceProperty, item), complexSourceItem);
                    }
                }
            }

            return null;
        }
    }

    /// <summary>
    /// 用于标注字段 来自哪个表的的哪一列(仅限于有关联的表中)
    /// </summary>
    public class FromModelAttribute : Attribute
    {
        /// <summary>
        /// 类名(表名)
        /// </summary>
        public string[] ModelNames { get; }

        /// <summary>
        /// 字段(列名)
        /// </summary>
        public string ModelColumn { get; }

        /// <summary>
        /// 示例：表1.表2.表3....列
        /// </summary>
        public FromModelAttribute(string entityName)
        {
            if (string.IsNullOrWhiteSpace(entityName))
            {
                throw new ArgumentException();
            }
            var names = entityName.Split('.');
            ModelColumn = names[names.Length - 1];
            if (names.Length > 1)
            {
                ModelNames = new string[names.Length - 1];
                for (int i = 0; i < names.Length - 1; i++)
                {
                    ModelNames[i] = names[i];
                }
            }
        }
    }
}

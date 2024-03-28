using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LightPoint.IdentityServer.DtoModels.Tools.Mappers
{
    /// <summary>
    /// 仅同名映射
    /// 仅针对String类型的同名属性做转换处理
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TEntityVM"></typeparam>
    public class Mapper<TEntity, TEntityVM>
        where TEntity : class, new()
        where TEntityVM : class, new()
    {
        /// <summary>
        /// 转视图模型的lambda表达式缓存
        /// </summary>
        private static Action<TEntity, TEntityVM> _mapToActionCache;
        private static Func<TEntity, TEntityVM> _mapToFuncCache;

        static Mapper()
        {
            _mapToActionCache = GetMapToAction();
            _mapToFuncCache = GetMapToFunc();
        }

        /// <summary>
        /// 生成映射的Lambda表达式
        /// </summary>
        /// <returns></returns>
        private static Action<TEntity, TEntityVM> GetMapToAction()
        {
            ParameterExpression sourceParameter = Expression.Parameter(typeof(TEntity), "source");
            ParameterExpression targetParameter = Expression.Parameter(typeof(TEntityVM), "target");

            List<Expression> expressions = new List<Expression>();
            foreach (var entityProperty in typeof(TEntity).GetProperties())
            {
                var item = typeof(TEntityVM).GetProperty(entityProperty.Name);
                if (item == null)
                {
                    continue;
                }
                else if (item.PropertyType == entityProperty.PropertyType)
                {
                    // 如果是不能写的属性
                    if (!item.CanWrite)
                        continue;
                    MemberExpression sourceProperty = Expression.Property(sourceParameter, entityProperty.Name);
                    MemberExpression targetProperty = Expression.Property(targetParameter, item);
                    BinaryExpression assignExpression = Expression.Assign(targetProperty, sourceProperty);
                    expressions.Add(assignExpression);

                    if (entityProperty.PropertyType.IsEnum)
                    {
                        var enumNameProp = typeof(TEntityVM).GetProperty(entityProperty.Name + "Name");
                        // 搜索一下是否有后缀加Name的字符串值
                        if (enumNameProp != null)
                        {
                            MethodInfo methodInfo = entityProperty.PropertyType.GetMethod("ToString", new Type[] { })!;
                            var resultExpression = Expression.Call(sourceProperty, methodInfo);
                            MemberExpression targetEnumNameProperty = Expression.Property(targetParameter, enumNameProp);
                            BinaryExpression enumNameAssignExpression = Expression.Assign(targetEnumNameProperty, resultExpression);
                            expressions.Add(enumNameAssignExpression);
                        }
                    }
                }
                // 其他类型转string
                else if (item.PropertyType != entityProperty.PropertyType && item.PropertyType == typeof(string))
                {
                    // 如果是不能写的属性
                    if (!item.CanWrite)
                        continue;
                    MemberExpression sourceProperty = Expression.Property(sourceParameter, entityProperty.Name);
                    // 获取ToString方法，一般来说，都应该是String类型
                    MethodInfo methodInfo = sourceProperty.Type.GetMethod("ToString", new Type[] { })!;
                    var resultExpression = Expression.Call(sourceProperty, methodInfo);
                    MemberExpression targetProperty = Expression.Property(targetParameter, item);
                    BinaryExpression assignExpression = Expression.Assign(targetProperty, resultExpression);
                    expressions.Add(assignExpression);
                }
                // string转其他类型
                else if (item.PropertyType != entityProperty.PropertyType && entityProperty.PropertyType == typeof(string))
                {
                    // 如果是不能写的属性
                    if (!item.CanWrite)
                        continue;
                    MemberExpression sourceProperty = Expression.Property(sourceParameter, typeof(TEntity).GetProperty(entityProperty.Name)!);
                    MethodInfo parseMethod = item.PropertyType.GetMethod("Parse", new Type[] { typeof(string) })!;
                    MethodCallExpression resultExpression = Expression.Call(parseMethod, new List<Expression>() { sourceProperty });
                    MemberExpression targetProperty = Expression.Property(targetParameter, item);
                    BinaryExpression assignExpression = Expression.Assign(targetProperty, resultExpression);
                    expressions.Add(assignExpression);
                }
            }

            BlockExpression blockExpression = Expression.Block(expressions);
            Expression<Action<TEntity, TEntityVM>> lambda = Expression.Lambda<Action<TEntity, TEntityVM>>(blockExpression, new ParameterExpression[] { sourceParameter, targetParameter });

            return lambda.Compile();
        }


        public static void MapTo(TEntity tIn, TEntityVM tout, Action<TEntity, TEntityVM>? mapperExtensionAction = null)
        {
            if (tIn == null)
            {
                return;
            }
            if (tout == null)
            {
                tout = new TEntityVM();
            }
            _mapToActionCache(tIn, tout);
            if (mapperExtensionAction != null)
            {
                mapperExtensionAction(tIn, tout);
            }
        }


        private static Func<TEntity, TEntityVM> GetMapToFunc()
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TEntity), "x");
            // 一组用于映射的模型绑定表达式
            List<MemberBinding> memberBindingList = new List<MemberBinding>();
            foreach (var entityProperty in typeof(TEntity).GetProperties())
            {
                var item = typeof(TEntityVM).GetProperty(entityProperty.Name);
                if (item == null)
                {
                    continue;
                }
                else if (item.PropertyType == entityProperty.PropertyType)
                {
                    // 如果是不能写的属性
                    if (!item.CanWrite)
                        continue;
                    MemberExpression eProperty = Expression.Property(parameterExpression, entityProperty.Name);
                    MemberBinding memberBindingResult = Expression.Bind(item, eProperty);
                    memberBindingList.Add(memberBindingResult);

                    if (entityProperty.PropertyType.IsEnum)
                    {
                        var enumNameProp = typeof(TEntityVM).GetProperty(entityProperty.Name + "Name");
                        // 搜索一下是否有后缀加Name的字符串值
                        if (enumNameProp != null)
                        {
                            MethodInfo methodInfo = entityProperty.PropertyType.GetMethod("ToString", new Type[] { })!;
                            var resultExpression = Expression.Call(eProperty, methodInfo);
                            MemberBinding enumNameMemberBindingResult = Expression.Bind(enumNameProp, resultExpression);
                            memberBindingList.Add(enumNameMemberBindingResult);
                        }
                    }
                }
                // 其他类型转string
                else if (item.PropertyType != entityProperty.PropertyType && item.PropertyType == typeof(string))
                {
                    // 如果是不能写的属性
                    if (!item.CanWrite)
                        continue;
                    MemberExpression memberExp = Expression.Property(parameterExpression, entityProperty.Name);
                    // 获取ToString方法，一般来说，都应该是String类型
                    MethodInfo methodInfo = memberExp.Type.GetMethod("ToString", new Type[] { })!;
                    var resultExpression = Expression.Call(memberExp, methodInfo);
                    // ToString()之后再传输过去
                    MemberBinding memberBindingResult = Expression.Bind(item, resultExpression);
                    memberBindingList.Add(memberBindingResult);
                }
                // string转其他类型
                else if (item.PropertyType != entityProperty.PropertyType && entityProperty.PropertyType == typeof(string))
                {
                    // 如果是不能写的属性
                    if (!item.CanWrite)
                        continue;
                    MemberExpression property = Expression.Property(parameterExpression, typeof(TEntity).GetProperty(entityProperty.Name)!);
                    MethodInfo parseMethod = item.PropertyType.GetMethod("Parse", new Type[] { typeof(string) })!;
                    MethodCallExpression resultExpression = Expression.Call(parseMethod, new List<Expression>() { property });
                    // p=> p.item.Name = p.item.Name
                    MemberBinding memberBindingResult = Expression.Bind(item, resultExpression);
                    memberBindingList.Add(memberBindingResult);
                }
            }
            // p => new TOut(){item.Name = p.item.Name}
            MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TEntityVM)), memberBindingList.ToArray());
            Expression<Func<TEntity, TEntityVM>> lambda = Expression.Lambda<Func<TEntity, TEntityVM>>(memberInitExpression, new ParameterExpression[] { parameterExpression });

            return lambda.Compile();
        }


        public static TEntityVM? MapToNewObj(TEntity tIn, Action<TEntity, TEntityVM>? mapperExtensionAction = null)
        {
            if (tIn == null)
            {
                return null;
            }
            if (mapperExtensionAction == null)
            {
                return _mapToFuncCache(tIn);
            }
            else
            {
                var res = _mapToFuncCache(tIn);
                mapperExtensionAction(tIn, res);
                return res;
            }

        }

        public static List<TEntityVM> MapToNewObj(IEnumerable<TEntity> tIns, Action<TEntity, TEntityVM>? mapperExtensionAction = null)
        {
            if (tIns == null)
            {
                return new List<TEntityVM>();
            }
            if (mapperExtensionAction == null)
            {
                return tIns.ToList().Select(x => _mapToFuncCache(x)).ToList();
            }
            else
            {
                return tIns.ToList().Select(x =>
                {
                    var temp = _mapToFuncCache(x);
                    mapperExtensionAction(x, temp);
                    return temp;
                }).ToList();
            }
        }
    }
}

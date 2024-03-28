using LightPoint.IdentityServer.Shared;
using LightPoint.IdentityServer.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LightPoint.IdentityServer.Shared.Helpers
{
    public static class LambdaCreater
    {
        /// <summary>
        /// 根据类型创建对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object CreateObject(Type type)
        {
            var newExpression = Expression.Lambda(Expression.New(type));
            var propertyDelegate = newExpression.Compile();//编译成委托
            var result = propertyDelegate.DynamicInvoke();
            return result!;
        }

        /// <summary>
        /// 使用lambda直接获取元素的属性值(转为string类型)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bo"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static string GetPropertyValue<T>(T bo, string propName)
        {
            Type type = bo!.GetType();
            var parameter = Expression.Parameter(type, "m");
            PropertyInfo property = type.GetProperty(propName)!;
            if (property == null) return "";
            try
            {
                Expression expProperty = Expression.Property(parameter, property.Name);//取参数的属性m.propName
                var propertyDelegateExpression = Expression.Lambda(expProperty, parameter);//变成表达式 m => m.propName
                var propertyDelegate = propertyDelegateExpression.Compile();//编译成委托
                var obj = propertyDelegate.DynamicInvoke(bo);
                if (obj != null) return obj.ToString()!;
                var value = propertyDelegate.DynamicInvoke(bo) as string;
                if (value == null) return "";
                return value;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static Expression<Func<T, object>>[] GetIncludeExpressions<T>() where T : class
        {
            return typeof(T).GetProperties()
                .Where(x => x.PropertyType.FullName!.Contains("LightPoint") && x.PropertyType.IsClass)
                .Select(x => GetBoPropertyObj<T>(x.Name))
                .ToArray();
        }

        /// <summary>
        /// 使用lambda直接获取元素的属性值(转为string类型)
        /// </summary>
        /// <param name="bo"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static string GetPropertyValue(object bo, string propName)
        {
            Type type = bo.GetType();
            var parameter = Expression.Parameter(type, "m");
            PropertyInfo property = type.GetProperty(propName)!;
            Expression expProperty = Expression.Property(parameter, property!.Name);//取参数的属性m.propName
            var propertyDelegateExpression = Expression.Lambda(expProperty, parameter);//变成表达式 m => m.propName
            var propertyDelegate = propertyDelegateExpression.Compile();//编译成委托
            var value = propertyDelegate.DynamicInvoke(bo) as string;
            return value!;
        }

        /// <summary>
        /// 获得对象属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bo"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static object? GetProperty<T>(T bo, string propName)
        {
            try
            {
                Type type = bo!.GetType();
                var parameter = Expression.Parameter(type, "m");
                PropertyInfo property = type.GetProperty(propName)!;
                Expression expProperty = Expression.Property(parameter, property.Name);//取参数的属性m.propName
                var propertyDelegateExpression = Expression.Lambda(expProperty, parameter);//变成表达式 m => m.propName
                var propertyDelegate = propertyDelegateExpression.Compile();//编译成委托
                var value = propertyDelegate.DynamicInvoke(bo);
                return value!;
            }
            catch (Exception)
            {
                return null;
            }

        }

        // 最终形成的结果应该是x=> (decimal)x.Property1 * (decimal)x.Property2
        public static Expression<Func<T, decimal>> GetBoPropertyMultiply<T>(string propertyNames)
        {
            var propertyNameList = propertyNames.Split(',');
            if (propertyNameList.Length < 2)
            {
                return null!;
            }
            ParameterExpression x = Expression.Parameter(typeof(T), "x");
            // 全部转为decimal
            var baseLambda = Expression.Convert(Expression.Property(x, propertyNameList[0]), typeof(decimal));
            var next = Expression.Convert(Expression.Property(x, propertyNameList[1]), typeof(decimal));
            BinaryExpression binary = Expression.Multiply(baseLambda, next);
            for (var i = 2; i < propertyNameList.Length; i++)
            {
                var then = Expression.Convert(Expression.Property(x, propertyNameList[i]), typeof(decimal));
                binary = Expression.Multiply(binary, then);
            }
            var result = Expression.Lambda<Func<T, decimal>>(binary, x);
            return result;
        }

        public static TConvert GetConvertProperty<Tid, T, TConvert>(T bo, string propName) where TConvert : class, IData<Tid>, new()
        {
            Type type = bo!.GetType();
            var parameter = Expression.Parameter(type, "m");
            PropertyInfo property = type.GetProperty(propName)!;
            Expression expProperty = Expression.Property(parameter, property.Name);//取参数的属性m.propName
            var propertyDelegateExpression = Expression.Lambda(Expression.Convert(expProperty, typeof(TConvert)), parameter);//变成表达式 m => m.propName
            var propertyDelegate = propertyDelegateExpression.Compile();//编译成委托
            var value = propertyDelegate.DynamicInvoke(bo) as TConvert;
            return value!;
        }

        /// <summary>
        /// 获取二级属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bo"></param>
        /// <param name="propName"></param>
        /// <param name="thenPropName"></param>
        /// <returns></returns>
        public static string? GetThenPropertyValue<T>(T bo, string propName, string thenPropName)
        {
            try
            {
                Type type = bo!.GetType();
                var parameter = Expression.Parameter(type, "m");
                PropertyInfo property = type.GetProperty(propName)!;
                if (property == null) return "";
                if (property.GetValue(bo) == null) return "";
                Expression expProperty = Expression.Property(Expression.Property(parameter, property.Name), thenPropName);//取参数的属性m.propName
                var propertyDelegateExpression = Expression.Lambda(expProperty, parameter);//变成表达式 m => m.propName
                var propertyDelegate = propertyDelegateExpression.Compile();//编译成委托
                var tempValue = propertyDelegate.DynamicInvoke(bo);
                return tempValue as string;
            }
            catch (Exception)
            {
                return "";
                throw;
            }
        }

        /// <summary>
        /// 返回x=>x.propertyName的表达式,但不经过转换，这对于数据库的linq会有用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static Expression<Func<T, object>> GetBoPropertyObj<T>(string propertyName)
        {
            ParameterExpression x = Expression.Parameter(typeof(T), "x");
            return Expression.Lambda<Func<T, object>>(Expression.Property(x, propertyName), x);
        }


        public static Expression<Func<T, int>> GetIntBoPropertyObj<T>(string propertyName)
        {
            ParameterExpression x = Expression.Parameter(typeof(T), "x");
            return Expression.Lambda<Func<T, int>>(Expression.Property(x, propertyName), x);
        }
        public static Expression<Func<T, double>> GetDoubleBoPropertyObj<T>(string propertyName)
        {
            ParameterExpression x = Expression.Parameter(typeof(T), "x");
            return Expression.Lambda<Func<T, double>>(Expression.Property(x, propertyName), x);
        }
        public static Expression<Func<T, float>> GetFloatBoPropertyObj<T>(string propertyName)
        {
            ParameterExpression x = Expression.Parameter(typeof(T), "x");
            return Expression.Lambda<Func<T, float>>(Expression.Property(x, propertyName), x);
        }
        public static Expression<Func<T, decimal>> GetDecimalBoPropertyObj<T>(string propertyName)
        {
            ParameterExpression x = Expression.Parameter(typeof(T), "x");
            return Expression.Lambda<Func<T, decimal>>(Expression.Property(x, propertyName), x);
        }
        public static Expression<Func<T, long>> GetLongBoPropertyObj<T>(string propertyName)
        {
            ParameterExpression x = Expression.Parameter(typeof(T), "x");
            return Expression.Lambda<Func<T, long>>(Expression.Property(x, propertyName), x);
        }

        /// <summary>
        /// 返回x=>x.propertyName的表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static Expression<Func<T, object>> GetBoProperty<T>(string propertyName)
        {
            ParameterExpression x = Expression.Parameter(typeof(T), "x");
            return Expression.Lambda<Func<T, object>>(Expression.Convert(Expression.Property(x, propertyName), typeof(object)), x);
        }

        public static MemberExpression GetMemberExpression<T>(string propertyName)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            MemberExpression member = Expression.PropertyOrField(parameter, propertyName);
            return member;
        }

        /// <summary>
        /// 获得x=>x.PropertyName.ThenName的表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Key"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="nextProperty"></param>
        /// <returns></returns>
        public static Expression<Func<T, object>> GetThenBoProperty<T, Key>(string propertyName, string nextProperty)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            try
            {
                return Expression.Lambda<Func<T, object>>(Expression.Convert(Expression.Property(Expression.Convert(Expression.Property(parameter, propertyName), typeof(Key)), nextProperty), typeof(object)), parameter);
            }
            catch (Exception)
            {
                return null!;
            }

        }
        /// <summary>
        /// 获得x=>x.PropertyName.ThenName.Contains(ValueString)的表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="nextProperty"></param>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetThenBoPropertyContains<T>(string propertyName, Type propertyType, string nextProperty, string propertyValue)
        {
            //x
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
            ConstantExpression constant = Expression.Constant(propertyValue, typeof(string));
            try
            {
                return Expression.Lambda<Func<T, bool>>(Expression.Call
                (Expression.Convert(Expression.Property(Expression.Convert(Expression.Property(parameter, propertyName),
                propertyType), nextProperty), typeof(string)),
                method, constant),
                parameter);
            }
            catch (ArgumentException)
            {
                return null!;
            }

        }

        /// <summary>
        /// 获得x=>x.PropertyName.ThenName.Equals(ValueString)的表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="nextProperty"></param>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetThenBoPropertyEquals<Type, T, Key>(string propertyName,
            string nextProperty,
            string propertyValue)
        {
            propertyName = propertyName.FirstToUpper();
            nextProperty = nextProperty.FirstToUpper();
            //x
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            // 获得Equals函数
            MethodInfo method = typeof(Type).GetMethod("Equals", new[] { typeof(Type) })!;
            // 设置用于比较的常量值
            ConstantExpression constant = Expression.Constant(propertyValue, typeof(string));
            // 生成lambda表达式
            return Expression.Lambda<Func<T, bool>>(Expression.Call
                (Expression.Convert(Expression.Property(Expression.Convert(Expression.Property(parameter, propertyName),
                typeof(Key)), nextProperty), typeof(Type)),
                method, constant),
                parameter);
        }

        /// <summary>
        /// 获取某个属性等于某个值的表达式，用于对比的类型，一定要有Equals的实现
        /// </summary>
        /// <typeparam name="T">用于比较的类型</typeparam>
        /// <typeparam name="TValue">要比较的值的类型</typeparam>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetPropertyEquals<T, TValue>(string propertyName,
            TValue propertyValue)
        {
            propertyName = propertyName.FirstToUpper();
            //x
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            // 获得Equals函数
            MethodInfo method = typeof(TValue).GetMethod("Equals", new[] { typeof(TValue) })!;
            // 设置用于比较的常量值
            ConstantExpression constant = Expression.Constant(propertyValue, typeof(TValue));
            // 生成lambda表达式
            return Expression.Lambda<Func<T, bool>>(Expression.Call
                (Expression.Convert(Expression.PropertyOrField(parameter, propertyName), typeof(TValue)),
                method, constant),
                parameter);
        }

        public static Expression<Func<T, bool>> GetStringPropertyEquals<T>(string propertyName,
            string propertyValue)
        {
            propertyName = propertyName.FirstToUpper();
            //x
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            // 获得Equals函数
            MethodInfo method = typeof(string).GetMethod("Equals", new[] { typeof(string) })!;
            // 设置用于比较的常量值
            ConstantExpression constant = Expression.Constant(propertyValue, typeof(string));
            // 生成lambda表达式
            return Expression.Lambda<Func<T, bool>>(Expression.Call
                (Expression.Convert(Expression.PropertyOrField(parameter, propertyName), typeof(string)),
                method, constant),
                parameter);
        }

        /// <summary>
        /// 获取日期相等表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetEqualDay<T>(string propertyName, DateTime date)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            //日期类型.Day
            MemberExpression member = Expression.Property(Expression.PropertyOrField(parameter, propertyName), "Day");
            MethodInfo equals = typeof(int).GetMethod("Equals", new Type[] { typeof(int) })!;
            ConstantExpression constant = Expression.Constant(date.Day, typeof(int));
            var dayLambda = Expression.Lambda<Func<T, bool>>(Expression.Call(member, equals, constant), parameter);

            //日期类型.Day
            MemberExpression monthMember = Expression.Property(Expression.PropertyOrField(parameter, propertyName), "Month");
            ConstantExpression constantMonth = Expression.Constant(date.Month, typeof(int));
            var monthLambda = Expression.Lambda<Func<T, bool>>(Expression.Call(monthMember, equals, constantMonth), parameter);

            //日期类型.Day
            MemberExpression yearMember = Expression.Property(Expression.PropertyOrField(parameter, propertyName), "Year");
            ConstantExpression constantYear = Expression.Constant(date.Year, typeof(int));
            var yearLambda = Expression.Lambda<Func<T, bool>>(Expression.Call(yearMember, equals, constantYear), parameter);

            return dayLambda.ExpressionAnd(monthLambda).ExpressionAnd(yearLambda);
        }

        /// <summary>
        /// x=>x.Property<value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="ValueType"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetLessThanValue<T, ValueType>(string propertyName, ValueType value)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            MemberExpression member = Expression.PropertyOrField(parameter, propertyName);
            ConstantExpression constant = Expression.Constant(value, typeof(ValueType));

            return Expression.Lambda<Func<T, bool>>(Expression.LessThan(member, constant), parameter);
        }

        public static Expression<Func<T, bool>> GetThenLessThanValue<T, ValueType>(string propertyName, string thenPropertyName, ValueType value)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            MemberExpression member = Expression.PropertyOrField(Expression.PropertyOrField(parameter, propertyName), thenPropertyName);
            ConstantExpression constant = Expression.Constant(value, typeof(ValueType));

            return Expression.Lambda<Func<T, bool>>(Expression.LessThan(member, constant), parameter);
        }

        /// <summary>
        /// x=>x.Property>value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="ValueType"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetBigThenValue<T, ValueType>(string propertyName, ValueType value)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            MemberExpression member = Expression.PropertyOrField(parameter, propertyName);
            ConstantExpression constant = Expression.Constant(value, typeof(ValueType));

            return Expression.Lambda<Func<T, bool>>(Expression.LessThan(constant, member), parameter);
        }

        public static Expression<Func<T, bool>> GetThenBigThenValue<T, ValueType>(string propertyName, string thenPropertyName, ValueType value)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            MemberExpression member = Expression.PropertyOrField(Expression.PropertyOrField(parameter, propertyName), thenPropertyName);
            ConstantExpression constant = Expression.Constant(value, typeof(ValueType));

            return Expression.Lambda<Func<T, bool>>(Expression.LessThan(constant, member), parameter);
        }

        /// <summary>
        /// x=>minValue<x.Property<maxValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="ValueType"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetBetweenValue<T, ValueType>(string propertyName, ValueType minValue, ValueType maxValue)
        {
            var resultLambda = GetLessThanValue<T, ValueType>(propertyName, maxValue).ExpressionAnd(GetBigThenValue<T, ValueType>(propertyName, minValue));
            return resultLambda;
        }

        public static Expression<Func<T, bool>> GetThenBetweenValue<T, ValueType>(string propertyName, string thenPropertyName, ValueType minValue, ValueType maxValue)
        {
            var resultLambda = GetThenLessThanValue<T, ValueType>(propertyName, thenPropertyName, maxValue).ExpressionAnd(GetThenBigThenValue<T, ValueType>(propertyName, thenPropertyName, minValue));
            return resultLambda;
        }

        /// <summary>
        /// 根据需要的指定属性名生成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyNames"></param>
        /// <returns></returns>
        public static Expression<Func<T, T>> NewSelfClassWithProperies<T>(params string[] propertyNames)
        {
            var source = typeof(T);

            var parameterExpression = Expression.Parameter(source, "x");
            List<MemberBinding> memberBindingList = new List<MemberBinding>();

            foreach (var propertyName in propertyNames)
            {
                var property = typeof(T).GetProperty(propertyName);
                if (property == null) continue;
                // p=>p.item.Name
                MemberExpression propertyExpression = Expression.Property(parameterExpression, typeof(T).GetProperty(propertyName)!);
                // p=> p.item.Name = p.item.Name
                MemberBinding memberBinding = Expression.Bind(property, propertyExpression);
                memberBindingList.Add(memberBinding);
            }
            MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(T)), memberBindingList.ToArray());
            Expression<Func<T, T>> lambda = Expression.Lambda<Func<T, T>>(memberInitExpression, new ParameterExpression[] { parameterExpression });
            return lambda;
        }

        /// <summary>
        /// 获得x=>x.PropertyName.Contains(ValueString)的表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetContains<T>(string propertyName, string propertyValue)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            MemberExpression member = Expression.PropertyOrField(parameter, propertyName);
            MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
            ConstantExpression constant = Expression.Constant(propertyValue, typeof(string));

            return Expression.Lambda<Func<T, bool>>(Expression.Call(member, method, constant), parameter);
        }

        /// <summary>
        /// 用于枚举数据与string数据的比较
        /// 但是EF的对象不能使用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="euqalsStr"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetEnumEquals<T, TEnum>(string propertyName, TEnum targetEnum)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            MethodInfo tostring = typeof(TEnum).GetMethod("ToString", new Type[] { })!;
            MethodInfo equals = typeof(Enum).GetMethod("Equals", new Type[] { typeof(Enum) })!;
            var member = Expression.PropertyOrField(parameter, propertyName);
            ConstantExpression constant = Expression.Constant(targetEnum, typeof(string));
            return Expression.Lambda<Func<T, bool>>(Expression.Call(Expression.Call(member, tostring), equals, constant), parameter);
        }

        /// <summary>
        /// 获得x=>x.PropertyName.ToString().Contains(ValueString)的表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetToStringContains<T>(string propertyName, string propertyValue)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            MemberExpression member = Expression.PropertyOrField(parameter, propertyName);
            MethodInfo method = typeof(T).GetMethod("ToString")!;
            MethodInfo method2 = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
            ConstantExpression constant = Expression.Constant(propertyValue, typeof(string));

            return Expression.Lambda<Func<T, bool>>(Expression.Call(Expression.Call(member, method), method2, constant), parameter);
        }

        /// <summary>
        /// 表达式拼接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="merge"></param>
        /// <returns></returns>
        private static Expression<T> Combine<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            MyExpressionVisitor visitor = new MyExpressionVisitor(first.Parameters[0]);
            Expression bodyone = visitor.Visit(first.Body);
            Expression bodytwo = visitor.Visit(second.Body);
            return Expression.Lambda<T>(merge(bodyone, bodytwo), first.Parameters[0]);
        }

        //返回|| 
        public static Expression<Func<T, bool>> ExpressionOr<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Combine(second, Expression.Or);
        }

        //返回&&
        public static Expression<Func<T, bool>> ExpressionAnd<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Combine(second, Expression.And);
        }

        /// <summary>
        /// 获得一组成员的表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyNames"></param>
        /// <returns></returns>
        public static List<MemberExpression> GetMemberExpressions<T>(params string[] propertyNames)
        {
            if (propertyNames != null)
            {
                List<MemberExpression> result = new List<MemberExpression>();
                foreach (var propertyName in propertyNames)
                {
                    var tempExpression = GetMemberExpression<T>(propertyName);
                    result.Add(tempExpression);
                }
                return result;
            }
            return null!;
        }


        public class MyExpressionVisitor : ExpressionVisitor
        {
            public ParameterExpression _Parameter { get; set; }

            public MyExpressionVisitor(ParameterExpression Parameter)
            {
                _Parameter = Parameter;
            }
            protected override Expression VisitParameter(ParameterExpression p)
            {
                return _Parameter;
            }

            public override Expression Visit(Expression? node)
            {
                return base.Visit(node)!;//Visit会根据VisitParameter()方法返回的Expression修改这里的node变量
            }
        }
    }
}

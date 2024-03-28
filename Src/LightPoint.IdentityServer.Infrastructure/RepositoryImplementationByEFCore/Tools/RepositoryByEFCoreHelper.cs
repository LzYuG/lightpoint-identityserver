using LightPoint.IdentityServer.Shared;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LightPoint.IdentityServer.Shared.Helpers;

namespace LightPoint.IdentityServer.Infrastructure.RepositoryImplementationByEFCore.Tools
{
    public static class RepositoryByEFCoreHelper
    {
        /// <summary>
        /// 提取包含属性的表达式
        /// </summary>
        /// <typeparam name="TModelData"></typeparam>
        /// <returns></returns>
        public static List<Expression<Func<TModelData, object>>> GetIncludeExpression<Tid, TModelData>() where TModelData : class, IData<Tid>, new()
        {
            var result = new List<Expression<Func<TModelData, object>>>();

            PropertyInfo[] ddoPropertyCollection = typeof(TModelData).GetProperties();
            foreach (var ddoProperty in ddoPropertyCollection)
            {
                var ddoPropertyTypeFullName = ddoProperty.PropertyType.FullName;
                if (ddoPropertyTypeFullName!.Contains(GlobalConstants.GlobalNamespace))
                {
                    ParameterExpression parameter = Expression.Parameter(typeof(TModelData), "i");
                    var property = Expression.Property(parameter, ddoProperty.Name);
                    var lambda = Expression.Lambda<Func<TModelData, object>>(property, parameter);
                    result.Add(lambda);
                }

            }
            return result;
        }
    }
}

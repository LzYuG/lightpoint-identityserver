using LightPoint.IdentityServer.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Shared.Helpers
{
    /// <summary>
    /// 通用的比较器
    /// </summary>
    public class CommonEqualityComparer<Tid, T> : IEqualityComparer<T>
        where T : class, IData<Tid>, new()
    {
        private readonly string _propertyName;

        public CommonEqualityComparer(Expression<Func<T, object>> expression)
        {
            _propertyName = expression.ToString().Split(".")[^1];
        }
        public bool Equals(T? x, T? y)
        {
            if (x == null || y == null)
                return false;

            if (LambdaCreater.GetProperty(x, _propertyName) == LambdaCreater.GetProperty(y, _propertyName))
            {
                return true;
            }

            return false;
        }

        public int GetHashCode([DisallowNull] T obj)
        {
            if (obj == null)
                return 0;
            else
                return obj.Id!.GetHashCode();
        }
    }
}

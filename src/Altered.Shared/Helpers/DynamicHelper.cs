using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Altered.Shared.Helpers
{
    public static class DynamicHelper
    {
        public static bool HasMember(object o, string member)
        {
            return GetAllMembers(o).Contains(member);
        }

        public static IEnumerable<string> GetAllMembers(object o)
        {
            if (!(o is IDynamicMetaObjectProvider metaObjectProvider))
            {
                throw new ArgumentException("The provided value was not a dynamic object.");
            }

            DynamicMetaObject metaObjects = metaObjectProvider.GetMetaObject(Expression.Constant(metaObjectProvider));
            return metaObjects.GetDynamicMemberNames();

        }
    }
}

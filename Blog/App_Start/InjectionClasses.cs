using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Omu.ValueInjecter;

namespace Blog.App_Start
{
    public class CustomInjection : FlatLoopValueInjection
    {
        protected override bool TypesMatch(Type sourceType, Type targetType)
        {
            var snt = Nullable.GetUnderlyingType(sourceType);
            var tnt = Nullable.GetUnderlyingType(targetType);

            return sourceType == targetType
                   || sourceType == tnt
                   || targetType == snt
                   || snt == tnt;
        }
    }
}
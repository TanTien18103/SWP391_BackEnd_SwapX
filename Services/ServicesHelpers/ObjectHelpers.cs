using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Services.ServicesHelpers
{
    public static class ObjectHelpers
    {
        public static TDestination Map<TDestination>(this object source)
            where TDestination : class, new()
        {
            if (source == null)
                return null;

            var destType = typeof(TDestination);

            if (destType.IsGenericType && destType.GetGenericTypeDefinition() == typeof(List<>))
            {
                var itemType = destType.GetGenericArguments()[0];
                var list = (IList)Activator.CreateInstance(destType);

                if (source is IEnumerable enumerable && !(source is string))
                {
                    foreach (var item in enumerable)
                    {
                        var mappedItem = MapSingle(item, itemType);
                        list.Add(mappedItem);
                    }
                }

                return (TDestination)list;
            }

            var single = MapSingle(source, destType);
            return (TDestination)single;
        }

        private static object MapSingle(object source, Type destinationType)
        {
            if (source == null)
                return null;

            var destination = Activator.CreateInstance(destinationType);

            var sourceProps = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var destProps = destinationType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var destProp in destProps)
            {
                if (!IsSimpleType(destProp.PropertyType))
                    continue;

                var sourceProp = sourceProps.FirstOrDefault(p =>
                    p.Name.Equals(destProp.Name, StringComparison.OrdinalIgnoreCase) &&
                    IsSimpleType(p.PropertyType) &&
                    destProp.CanWrite
                );

                if (sourceProp == null) continue;

                var value = sourceProp.GetValue(source);
                destProp.SetValue(destination, value);
            }

            return destination;
        }

        private static bool IsSimpleType(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

            return underlyingType.IsPrimitive
                   || underlyingType.IsEnum
                   || underlyingType == typeof(string)
                   || underlyingType == typeof(decimal)
                   || underlyingType == typeof(DateTime)
                   || underlyingType == typeof(Guid);
        }
    }
}

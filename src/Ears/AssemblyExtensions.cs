using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ears
{
    internal static class AssemblyExtensions
    {
        internal static IEnumerable<(Type listener, Type ofEvent)> GetDispatchersAndListeners(
            this IEnumerable<Assembly> assemblies)
        {
            bool FilterInterfaces(Type type) =>
                type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEventListener<>);

            foreach (var assembly in assemblies)
            {
                var listeners = assembly.GetTypes()
                    .Where(x => !x.IsAbstract && !x.IsInterface)
                    .Where(x => x.GetInterfaces().Any(FilterInterfaces));

                foreach (var listener in listeners)
                {
                    var ofEventTypes = listener.GetInterfaces()
                        .Where(FilterInterfaces)
                        .SelectMany(i => i.GenericTypeArguments);

                    foreach (var e in ofEventTypes)
                        yield return (listener, e);
                }
            }
        }
    }
}
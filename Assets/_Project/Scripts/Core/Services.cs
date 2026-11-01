using System;
using System.Collections.Generic;

namespace Heartbeat.Core
{
    /// <summary>
    /// The only global in the project (CLAUDE.md: no singletons outside Bootstrap).
    /// Bootstrap registers the persistent services; scene-level services (e.g. AlertDirector)
    /// register themselves on enable and unregister on disable.
    /// </summary>
    public static class Services
    {
        private static readonly Dictionary<Type, object> Registry = new Dictionary<Type, object>();

        public static void Register<T>(T service) where T : class
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            Registry[typeof(T)] = service;
        }

        public static void Unregister<T>(T service) where T : class
        {
            if (Registry.TryGetValue(typeof(T), out var current) && ReferenceEquals(current, service))
                Registry.Remove(typeof(T));
        }

        public static T Get<T>() where T : class
        {
            if (Registry.TryGetValue(typeof(T), out var s)) return (T)s;
            throw new InvalidOperationException($"[Heartbeat][Core] Service {typeof(T).Name} is not registered. Is Bootstrap loaded?");
        }

        public static bool TryGet<T>(out T service) where T : class
        {
            if (Registry.TryGetValue(typeof(T), out var s)) { service = (T)s; return true; }
            service = null;
            return false;
        }

        public static void Clear() => Registry.Clear();
    }
}

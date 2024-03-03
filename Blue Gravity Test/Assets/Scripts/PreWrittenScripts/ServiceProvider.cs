using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Jega.BlueGravity.PreWrittenCode
{
    public static class ServiceProvider
    {
        private static readonly Dictionary<Type, IService> Services = new Dictionary<Type, IService>();
        private static readonly ServicePriorityComparer ServiceSorter = new ServicePriorityComparer();

        /// <summary>
        /// Gets the first service that is T.
        /// </summary>
        /// <typeparam name="T">Any IService</typeparam>
        /// <returns></returns>
        public static T GetAbstract<T>() where T : class, IService
        {
            foreach (KeyValuePair<Type, IService> pair in Services)
            {
                if (pair.Value is T value)
                    return value;
            }
            Debug.LogError("[ServiceProvider]: No concrete service initialized for this abstraction.");
            return default;
        }

        /// <summary>
        /// Get or Create T service. must be a concrete class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetService<T>() where T : class, IService, new()
        {
            if (Services.TryGetValue(typeof(T), out IService service))
                return service as T;

            T nService = new T();
            if (nService == null)
            {
                Debug.LogError("[ServiceProvider]: Error while creating " + typeof(T).Name + " service.");
                return null;
            }
            Services.Add(typeof(T), nService);
            nService.Preprocess();
            return nService;
        }

        /// <summary>
        /// Removes a  service, by calling PostProcess() then removing it from the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool RemoveService<T>() where T : IService
        {
            Type type = typeof(T);
            if (!Services.ContainsKey(type)) return false;

            IService service = Services[type];
            service.Postprocess();
            Services.Remove(type);
            return true;
        }

        /// <summary>
        /// Destroy and clear all services.
        /// </summary>
        public static void Shutdown()
        {
            List<IService> services = Services.Values.ToList();
            services.Sort(ServiceSorter);
            foreach (IService service in services)
            {
                service.Postprocess();
            }
            Services.Clear();
        }

        public class ServicePriorityComparer : IComparer<IService>
        {
            public int Compare(IService x, IService y)
            {
                if (x == null || y == null) return 0;
                return x.Priority.CompareTo(y.Priority);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RegisterServiceEvents()
        {
            Application.quitting -= Shutdown;
            Application.quitting += Shutdown;
        }
    }
}
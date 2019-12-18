using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using DependencyInjectionContainer.Attribute;
using DependencyInjectionContainer.Exception;

namespace DependencyInjectionContainer
{
    public class DependencyProvider
    {
        private readonly DependenciesConfiguration _dependencyConfiguration;

        private readonly ConcurrentDictionary<Dependency, object> _instances = new ConcurrentDictionary<Dependency, object>();

        public DependencyProvider(DependenciesConfiguration dependencyConfiguration)
        {
            _dependencyConfiguration = dependencyConfiguration;
        }

        internal object Resolve(ParameterInfo parameter)
        {
            var name = parameter.GetCustomAttribute<DependencyKeyAttribute>()?.Key;
            return Resolve(parameter.ParameterType, name);
        }

        public TInterface Resolve<TInterface>()
            where TInterface : class
        {
            return (TInterface) Resolve(typeof(TInterface));
        }

        public TInterface Resolve<TInterface>(object name)
        {
            return (TInterface) Resolve(typeof(TInterface), name);
        }

        public IEnumerable<T> ResolveAll<T>()
            where T : class
        {
            return (IEnumerable<T>) ResolveAll(typeof(T));
        }

        public IEnumerable<object> ResolveAll(Type @interface)
        {
            if (_dependencyConfiguration.TryGetAll(@interface, out var dependencies))
            {
                var collection = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(@interface));

                foreach (var dependency in dependencies)
                {
                    collection.Add(ResolveDependency(dependency));
                }

                return (IEnumerable<object>) collection;
            }

            return null;
        }

        private object ResolveDependency(Dependency dependency)
        {
            if (dependency.LifeType == LifeType.InstancePerDependency)
            {
                return Creator.GetInstance(dependency.Type, _dependencyConfiguration);
            }
            if (dependency.LifeType == LifeType.Singleton)
            {
                if (_instances.TryGetValue(dependency, out var instance))
                {
                    return instance;
                }
                instance = Creator.GetInstance(dependency.Type, _dependencyConfiguration);
                while (!_instances.TryAdd(dependency, instance))
                {
                    Thread.Sleep(1);
                }

                return instance;
            }

            return null;
        }

        private Dependency GetNamedDependency(Type @interface, object key)
        {
            if (_dependencyConfiguration.TryGetAll(@interface, out var namedDependencies))
            {
                foreach (var dependency in namedDependencies)
                {
                    if (key.Equals(dependency.Key)) return dependency;
                }
            }

            throw new DependencyException($"Dependency with [{key}] key for type {@interface} is not registered");
        }

        private Dependency GetDependency(Type @interface, object key = null)
        {
            if (key != null) return GetNamedDependency(@interface, key);
            if (@interface.IsGenericType &&
                _dependencyConfiguration.TryGet(@interface.GetGenericTypeDefinition(), out var genericDependency))
            {
                var genericType = genericDependency.Type.MakeGenericType(@interface.GenericTypeArguments);
                return new Dependency(genericType, genericDependency.LifeType, genericDependency.Key);
            }

            if (_dependencyConfiguration.TryGet(@interface, out var dependency))
            {
                return dependency;
            }

            throw new DependencyException($"Dependency for type {@interface} is not registered");
        }

        public object Resolve(Type @interface, object key = null)
        {
            if (typeof(IEnumerable).IsAssignableFrom(@interface))
            {
                return ResolveAll(@interface.GetGenericArguments()[0]);
            }
            var dependency = GetDependency(@interface, key);

            return ResolveDependency(dependency); 
        }

    }
}

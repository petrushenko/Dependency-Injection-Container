using System;
using System.Collections.Generic;
using System.Linq;
using DependencyInjectionContainer.Attribute;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DependencyInjectionContainer.Tests
{
    public interface IRepository{}
    public class RepositoryImpl : IRepository
    {
        public RepositoryImpl(){} // может иметь свои зависимости, опустим для простоты
    }

    interface IService<TRepository> where TRepository : IRepository
    {
    }

    class ServiceImpl<TRepository> : IService<TRepository> 
        where TRepository : IRepository
    {
        public TRepository rep;
        public ServiceImpl(TRepository repository)
        {
            rep = repository;
        }
    }

    class MySqlRepository : IRepository
    {

    }

    public class SomeAnotherService
    {
        public IRepository Repository;

        public SomeAnotherService([DependencyKey("lol")] IRepository repository)
        {
            Repository = repository;
        }
    }

    [TestClass]
    public class ProviderTests
    {
        [TestMethod]
        public void RecursionTest()
        {
            // конфигурация и использование контейнера
            var dependencies = new DependenciesConfiguration();
            dependencies.Register<IRepository, RepositoryImpl>();
            dependencies.Register(typeof(IService<>), typeof(ServiceImpl<>));
            //dependencies.Register<SomeAnotherService, SomeAnotherService>();
            //var obj = new object();

            //dependencies.Register(typeof(IRepository), typeof(MySqlRepository), LifeType.InstancePerDependency, obj);
            //dependencies.Register<IRepository, MySqlRepository>();
 
            var provider = new DependencyProvider(dependencies);
            var impl = provider.Resolve<IService<IRepository>>();
            // должен быть создан ServiceImpl (реализация IService), в конструктор которому передана
            // RepositoryImpl (реализация IRepository)
            //var service1 = provider.ResolveAll<IService>().ToList(); 
        }
    }
}

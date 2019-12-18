namespace DependencyInjectionContainer.Tests.TestableClasses
{
    public interface IService<T> : IBaseService where T: IRepository
    {

    }
}
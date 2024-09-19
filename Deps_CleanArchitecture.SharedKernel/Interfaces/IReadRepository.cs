using Ardalis.Specification;

namespace Deps_CleanArchitecture.SharedKernel.Interfaces
{
    public interface IReadRepository<T> : IReadRepositoryBase<T> where T : class, IAggregateRoot
    {
    }
}
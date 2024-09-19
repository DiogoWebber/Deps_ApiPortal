using Ardalis.Specification.EntityFrameworkCore;
using Deps_CleanArchitecture.SharedKernel.Interfaces;

namespace Deps_CleanArchitecture.Infrastructure.Data
{
    // inherit from Ardalis.Specification type
    public class EfRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T> where T : class, IAggregateRoot
    {
        public EfRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}

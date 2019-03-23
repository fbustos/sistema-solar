using Contracts;
using Entities;
using Entities.Models;

namespace Repository
{
    public class JobRepository : RepositoryBase<Job>, IJobRepository
    {
        public JobRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }
}

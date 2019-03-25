using Contracts;
using Entities;
using Entities.Models;

namespace Repository
{
    public class PlanetaRepository : RepositoryBase<Planeta>, IPlanetaRepository
    {
        public PlanetaRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }
}

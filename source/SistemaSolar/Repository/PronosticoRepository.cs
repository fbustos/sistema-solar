using Contracts;
using Entities;
using Entities.Models;

namespace Repository
{
    public class PronosticoRepository : RepositoryBase<Pronostico>, IPronosticoRepository
    {
        public PronosticoRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }
}

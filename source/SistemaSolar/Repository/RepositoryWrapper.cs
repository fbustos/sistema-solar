using Contracts;
using Entities;

namespace Repository
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private RepositoryContext _repoContext;
        private IPronosticoRepository _pronostico;
        private IPlanetaRepository _planeta;

        public IPronosticoRepository Pronostico
        {
            get
            {
                if (_pronostico == null)
                {
                    _pronostico = new PronosticoRepository(_repoContext);
                }

                return _pronostico;
            }
        }

        public IPlanetaRepository Planeta
        {
            get
            {
                if (_planeta == null)
                {
                    _planeta = new PlanetaRepository(_repoContext);
                }

                return _planeta;
            }
        }

        public RepositoryWrapper(RepositoryContext repositoryContext)
        {
            _repoContext = repositoryContext;
        }
    }
}

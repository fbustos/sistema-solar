using Contracts;
using Entities;

namespace Repository
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private RepositoryContext _repoContext;
        private IPronosticoRepository _pronostico;
        private IPlanetaRepository _planeta;
        private IJobRepository _job;

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

        public IJobRepository Job
        {
            get
            {
                if (_job == null)
                {
                    _job = new JobRepository(_repoContext);
                }

                return _job;
            }
        }

        public RepositoryWrapper(RepositoryContext repositoryContext)
        {
            _repoContext = repositoryContext;
        }
    }
}

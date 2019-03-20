using Contracts;
using Entities;

namespace Repository
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private RepositoryContext _repoContext;
        private IPronosticoRepository _pronostico;

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

        public RepositoryWrapper(RepositoryContext repositoryContext)
        {
            _repoContext = repositoryContext;
        }
    }
}

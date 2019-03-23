using Contracts;
using Entities.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services
{
    public class PlanetaService : IPlanetaService
    {
        private readonly ILogger<PlanetaService> _logger;
        private readonly IRepositoryWrapper _repository;

        public PlanetaService(IRepositoryWrapper repository, ILogger<PlanetaService> logger)
        {
            _logger = logger;
            _repository = repository;
        }

        public IEnumerable<Planeta> GetAll()
        {
            return _repository.Planeta.FindAll();
        }

        public void CreatePlanetas(IEnumerable<Planeta> planetas)
        {
            _repository.Planeta.CreateMultiple(planetas);
            _repository.Planeta.Save();
        }

        public void DeleteAll()
        {
            var dbPlanets = _repository.Planeta.FindAll();
            dbPlanets?.ToList().ForEach(p => _repository.Planeta.Delete(p));
            _repository.Planeta.Save();
        }
    }
}

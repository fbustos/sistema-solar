using Contracts;
using Entities.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Services
{
    public class JobService : IJobService
    {
        private readonly ILogger<JobService> _logger;
        private readonly IRepositoryWrapper _repository;
        private readonly IPlanetaService _planetaService;
        private readonly IPronosticoService _pronosticoService;

        public JobService(ILogger<JobService> logger, IRepositoryWrapper repository, IPlanetaService planetaService, IPronosticoService pronosticoService)
        {
            _logger = logger;
            _repository = repository;
            _planetaService = planetaService;
            _pronosticoService = pronosticoService;
        }

        public void Run(IEnumerable<Planeta> planetas, int anios = 10, string fechaInicio = null)
        {
            try
            {
                Initialize(planetas);
                var fecha = string.IsNullOrEmpty(fechaInicio) ? DateTime.Today : Convert.ToDateTime(fechaInicio);
                var job = this.CreateJob(anios, fecha);
                _pronosticoService.PronosticarClima(planetas, anios, fecha, job.JobId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ha ocurrido un error mientras se ejecutaba el job: ", ex.Message);
            }
        }

        private void Initialize(IEnumerable<Planeta> planetas)
        {
            if (planetas == null)
            {
                planetas = _planetaService.GetAll();
            }
            else
            {
                _planetaService.DeleteAll();
                _planetaService.CreatePlanetas(planetas);
            }

            _pronosticoService.DeleteAll();
        }

        private Job CreateJob(int anios, DateTime fechaInicio)
        {
            var job = new Job
            {
                Anios = anios,
                FechaInicio = fechaInicio
            };

            _repository.Job.Create(job);
            _repository.Job.Save();

            return job;
        }
    }
}

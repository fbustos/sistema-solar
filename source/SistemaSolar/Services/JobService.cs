using Contracts;
using Entities.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public Job Run(IEnumerable<Planeta> planetas, int anios = 10, string fechaInicio = null)
        {
            try
            {
                if (anios <= 0)
                {
                    return null;
                }

                // Se inicializan los planetas y se borran los pronosticos existentes
                planetas = Initialize(planetas).ToList();
                // Calculo de la fecha de inicio
                var fecha = string.IsNullOrEmpty(fechaInicio) ? DateTime.Today : Convert.ToDateTime(fechaInicio);
                // Se crea el job
                var job = this.CreateJob(anios, fecha);
                // Se corre el pronosticador
                _pronosticoService.PronosticarClima(planetas, anios, fecha, job.JobId);

                return job;
            }
            catch (Exception ex)
            {
                _logger.LogError("Ha ocurrido un error mientras se ejecutaba el job: ", ex.Message);
                return null;
            }
        }

        private IEnumerable<Planeta> Initialize(IEnumerable<Planeta> planetas)
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

            planetas.ToList().ForEach(p => p.Posicion = 0);
            _pronosticoService.DeleteAll();

            return planetas;
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

using Contracts;
using Entities;
using Entities.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public class PronosticoService : IPronosticoService
    {
        private readonly ILogger<PronosticoService> _logger;
        private readonly IRepositoryWrapper _repository;

        public PronosticoService(ILogger<PronosticoService> logger, IRepositoryWrapper repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public IEnumerable<Pronostico> GetAll()
        {
            return _repository.Pronostico.FindAll();
        }

        public Pronostico GetByDia(int dia)
        {
            return _repository.Pronostico.FindByCondition(x => x.Dia == dia).SingleOrDefault();
        }

        public IEnumerable<Pronostico> GetByClimas(string[] climas)
        {
            return _repository.Pronostico.FindByCondition(x => climas.Contains(x.Clima));
        }

        public void DeleteAll()
        {
            _repository.Pronostico.FindAll()?.ToList().ForEach(p => _repository.Pronostico.Delete(p));
            _repository.Pronostico.Save();
        }

        public void PronosticarClima(IEnumerable<Planeta> planetas, int anios, DateTime fecha, int? jobId = null)
        {
            var pronosticos = new List<Pronostico>();
            var totalDays = this.GetTotalDays(anios, fecha);
            var perimetroMax = double.MinValue;

            for (var i = 1; i <= totalDays; i++)
            {
                var clima = PronosticarClimaPorDia(i, planetas);

                var p = clima == ClimaConstants.Lluvia ? this.CalcularPerimetro(planetas) : 0.0;
                perimetroMax = Math.Max(perimetroMax, p);

                pronosticos.Add(new Pronostico
                {
                    Dia = i,
                    Fecha = DateTime.Today.AddDays(i),
                    Clima = clima,
                    NivelDeLluvia = p,
                    JobId = jobId
                });
            }

            foreach (var pronostico in pronosticos.Where(p => p.Clima == ClimaConstants.Lluvia && p.NivelDeLluvia == perimetroMax))
            {
                pronostico.Clima = ClimaConstants.LluviaIntensa;
            }

            _repository.Pronostico.CreateMultiple(pronosticos);
            _repository.Pronostico.Save();
        }

        private double GetTotalDays(int anios, DateTime fechaInicio)
        {
            return (fechaInicio.AddYears(anios) - fechaInicio).TotalDays;
        }

        private string PronosticarClimaPorDia(int dia, IEnumerable<Planeta> planetas)
        {
            var sol = new Tuple<double, double>(0.0, 0.0);
            planetas.ToList().ForEach(p => p.ActualizarPosicionUnDia());

            if (this.EstanAlineados(planetas, out double? pendiente))
            {
                if (this.AlineadosAlSol(pendiente, planetas.First(), sol))
                    return ClimaConstants.Sequia;
                else
                    return ClimaConstants.Optimo;
            }

            if (this.PlanetasContienenAlSol(planetas, sol))
            {
                return ClimaConstants.Lluvia;
            }

            return ClimaConstants.Indeterminado;
        }

        private double CalcularPerimetro(IEnumerable<Planeta> planetas)
        {
            var p1 = planetas.ElementAt(0);
            var p2 = planetas.ElementAt(1);
            var p3 = planetas.ElementAt(2);

            var d1 = this.DistanciaEntre(p1.PosicionRectangular, p2.PosicionRectangular);
            var d2 = this.DistanciaEntre(p1.PosicionRectangular, p3.PosicionRectangular);
            var d3 = this.DistanciaEntre(p2.PosicionRectangular, p3.PosicionRectangular);

            return d1 + d2 + d3;
        }

        private bool PlanetasContienenAlSol(IEnumerable<Planeta> planetas, Tuple<double, double> sol)
        {
            // Solo implementado para 3 planetas (forman un triangulo)
            if (planetas.Count() == 3)
            {
                var p1 = planetas.ElementAt(0);
                var p2 = planetas.ElementAt(1);
                var p3 = planetas.ElementAt(2);

                var areaTotal = this.AreaTriangulo(p1.PosicionRectangular, p2.PosicionRectangular, p3.PosicionRectangular);
                var area1 = this.AreaTriangulo(p1.PosicionRectangular, p2.PosicionRectangular, sol);
                var area2 = this.AreaTriangulo(p1.PosicionRectangular, sol, p3.PosicionRectangular);
                var area3 = this.AreaTriangulo(sol, p2.PosicionRectangular, p3.PosicionRectangular);

                return areaTotal == (area1 + area2 + area3);
            }

            return false;
        }

        private double AreaTriangulo(Tuple<double, double> pos1, Tuple<double, double> pos2, Tuple<double, double> pos3)
        {
            var d1 = this.DistanciaEntre(pos1, pos2);
            var d2 = this.DistanciaEntre(pos1, pos3);
            var d3 = this.DistanciaEntre(pos2, pos3);
            // calculo del semiperimetro
            var s = (d1 + d2 + d3) / 2;

            return Math.Sqrt(s * (s - d1) * (s - d2) * (s - d3));
        }

        private double DistanciaEntre(Tuple<double, double> pos1, Tuple<double, double> pos2)
        {
            return Math.Sqrt(Math.Pow(pos1.Item1 - pos2.Item1, 2) + Math.Pow(pos1.Item2 - pos2.Item2, 2));
        }

        private bool EstanAlineados(IEnumerable<Planeta> planetas, out double? pendiente)
        {
            pendiente = null;

            for (int i = 0; i < planetas.Count() - 1; i++)
            {
                var p1 = planetas.ElementAt(i);
                var p2 = planetas.ElementAt(i + 1);
                var p = CalcularPendiente(p1.PosicionRectangular, p2.PosicionRectangular);

                if (i == 0)
                    pendiente = p;
                else
                {
                    if (p != pendiente)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool AlineadosAlSol(double? pendiente, Planeta planeta, Tuple<double, double> sol)
        {
            var p = CalcularPendiente(planeta.PosicionRectangular, sol);

            return p == pendiente;
        }

        private double? CalcularPendiente(Tuple<double, double> p1, Tuple<double, double> p2)
        {
            return (p1.Item1 - p2.Item1) == 0 ? (double?)null : (p1.Item2 - p2.Item2) / (p1.Item1 - p2.Item1);
        }
    }
}

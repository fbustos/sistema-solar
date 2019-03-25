using System;
using System.Collections.Generic;
using Entities.Models;

namespace Contracts
{
    public interface IPronosticoService
    {
        IEnumerable<Pronostico> GetAll();
        Pronostico GetByDia(int dia);
        IEnumerable<Pronostico> GetByClimas(string[] climas);
        void DeleteAll();
        void PronosticarClima(IEnumerable<Planeta> planetas, int anios, DateTime fecha, int? jobId);
    }
}

using Entities.Models;
using System.Collections.Generic;

namespace Contracts
{
    public interface IPlanetaService
    {
        void CreatePlanetas(IEnumerable<Planeta> planetas);
        void DeleteAll();
        IEnumerable<Planeta> GetAll();
    }
}

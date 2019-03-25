using Entities.Models;
using System.Collections.Generic;

namespace Contracts
{
    public interface IJobService
    {
        void Run(IEnumerable<Planeta> planetas = null, int anios = 10, string fechaInicio = null);
    }
}

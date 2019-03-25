using Entities.Models;
using System.Collections.Generic;

namespace Contracts
{
    public interface IJobService
    {
        Job Run(IEnumerable<Planeta> planetas = null, int anios = 10, string fechaInicio = null);
    }
}

using Entities.Models;
using System.Collections.Generic;

namespace Entities
{
    public class MyConfig
    {
        public IEnumerable<Planeta> Planetas { get; set; }

        public JobConfig JobConfig { get; set; }
    }

    public class JobConfig
    {
        public int Anios { get; set; }

        public string FechaInicio { get; set; }

        public bool Activo { get; set; }
    }
}

using System.Threading.Tasks;
using Entities.Models;

namespace Contracts
{
    public interface IPronosticoService
    {
        void RunJob();
        Task RunJobAsync();
    }
}

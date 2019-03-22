namespace Contracts
{
    public interface IRepositoryWrapper
    {
        IPronosticoRepository Pronostico { get; }

        IPlanetaRepository Planeta { get; }
    }
}

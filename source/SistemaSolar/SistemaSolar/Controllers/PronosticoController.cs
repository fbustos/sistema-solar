using System;
using Contracts;
using Entities;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace SistemaSolar.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class PronosticoController : Controller
    {
        private readonly ILogger _logger;
        private readonly IPronosticoService _pronosticoService;
        private readonly IJobService _jobService;
        private readonly IMemoryCache _memoryCache;

        public PronosticoController(ILogger<PronosticoController> logger, IPronosticoService pronosticoService, IJobService jobService, IMemoryCache memoryCache)
        {
            _logger = logger;
            _pronosticoService = pronosticoService;
            _memoryCache = memoryCache;
            _jobService = jobService;
        }

        /// <summary>
        /// Obtiene el pronóstico de los próximos 10 años, previamente calculados por el job
        /// </summary>
        /// <param></param>  
        /// <returns>El listado de climas</returns>
        /// <response code="200">Retorna todos los pronosticos</response>
        /// <response code="500">Si hubo un error durante la ejecución</response>  
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public IActionResult Get()
        {
            try
            {
                var pronosticos = _pronosticoService.GetAll();
                _logger.LogInformation($"Returned all pronosticos from database.");

                return Ok(pronosticos);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAll action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Obtiene el clima de un día en particular
        /// </summary>
        /// <param name="dia">Pronostico para este dia en particular</param>  
        /// <returns>El clima del dia seleccionado</returns>
        /// <response code="200">El pronostico del día seleccionado</response>
        /// <response code="404">Si el día no fue encontrado</response>
        /// <response code="500">Si hubo un error durante la ejecución</response>
        [HttpGet("clima")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult Get([FromQuery(Name = "dia")]int dia)
        {
            try
            {
                Pronostico pronostico;
                if (_memoryCache.TryGetValue(dia, out pronostico))
                {
                    _logger.LogInformation($"Clima obtenido de cache ", pronostico);
                }
                else
                {
                    pronostico = _pronosticoService.GetByDia(dia);
                    if (pronostico != null)
                    {
                        _memoryCache.Set(dia, pronostico, DateTimeOffset.UtcNow.AddMinutes(30));
                    }
                    else
                    {
                        return NotFound(dia);
                    }
                }

                return Ok(pronostico);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside Get action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Obtiene todos los dias de clima "Sequia"
        /// </summary>
        /// <param></param>  
        /// <returns>Listado de dias con climas de sequia</returns>
        /// <response code="200">El pronostico con los dias de clima Sequia</response>
        /// <response code="404">Si no se encontraron climas secos</response>
        /// <response code="500">Si hubo un error durante la ejecución</response>
        [HttpGet("sequia")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult GetDiasDeSequia()
        {
            try
            {
                var climas = new string[] { ClimaConstants.Sequia };
                var pronosticos = _pronosticoService.GetByClimas(climas);

                if (pronosticos == null)
                {
                    return NotFound(ClimaConstants.Sequia);
                }

                return Ok(pronosticos);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetDiasDeSequia action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Obtiene todos los dias de clima "Lluvia" especificando los que tienen picos de lluvia con el estado "Lluvia intensa"
        /// </summary>
        /// <param></param>  
        /// <returns>Listado de dias con climas de lluvia</returns>
        /// <response code="200">El pronostico con los dias de clima Lluvia</response>
        /// <response code="404">Si no se encontraron climas lluviosos</response>
        /// <response code="500">Si hubo un error durante la ejecución</response>
        [HttpGet("lluvia")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult GetDiasDeLluvia()
        {
            try
            {
                var climas = new string[] { ClimaConstants.Lluvia, ClimaConstants.LluviaIntensa };
                var pronosticos = _pronosticoService.GetByClimas(climas);

                if (pronosticos == null)
                {
                    return NotFound(ClimaConstants.Lluvia);
                }

                return Ok(pronosticos);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetDiasDeLluvia action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Obtiene todos los dias de clima "Optimo"
        /// </summary>
        /// <param></param>  
        /// <returns>Listado de dias con climas de Optimas condiciones</returns>
        /// <response code="200">El pronostico con los dias de clima Optimo</response>
        /// <response code="404">Si no se encontraron climas optimos</response>
        /// <response code="500">Si hubo un error durante la ejecución</response>
        [HttpGet("optimo")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult GetDiasOptimos([FromQuery(Name = "dia")]int dia)
        {
            try
            {
                var climas = new string[] { ClimaConstants.Optimo };
                var pronosticos = _pronosticoService.GetByClimas(climas);

                if (pronosticos == null)
                {
                    return NotFound(ClimaConstants.Optimo);
                }

                return Ok(pronosticos);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetDiasOptimos action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Corre el job para pronosticar el clima por las cantidad de años solicitados
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Run
        ///     {
        ///        "years": 10
        ///     }
        ///
        /// </remarks>
        /// <param name="years">Años a pronosticar</param>  
        /// <returns></returns>
        /// <response code="200">Si se ha pronosticado exitosamente</response>
        /// <response code="400">Si hubo un error durante la ejecución</response>
        [HttpPost("Run")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult RunJob([FromBody]int years)
        {
            _logger.LogInformation("Inicio del job");

            var job = _jobService.Run(null, years);
            if (job == null)
            {
                return BadRequest(years);
            }

            return Ok(job);
        }
    }
}

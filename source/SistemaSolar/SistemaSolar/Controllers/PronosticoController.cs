using System;
using System.Collections.Generic;
using Contracts;
using Entities;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace SistemaSolar.Controllers
{
    [Route("api/[controller]")]
    public class PronosticoController : Controller
    {
        private readonly ILogger _logger;
        private readonly IPronosticoService _pronosticoService;
        private readonly IMemoryCache _memoryCache;

        public PronosticoController(ILogger<PronosticoController> logger, IPronosticoService pronosticoService, IMemoryCache memoryCache)
        {
            _logger = logger;
            _pronosticoService = pronosticoService;
            _memoryCache = memoryCache;
        }

        // GET api/pronostico
        [HttpGet]
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

        // GET api/pronostico/clima?dia=15
        [HttpGet("clima")]
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
                }

                return Ok(pronostico);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside Get action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("sequia")]
        public IActionResult GetDiasDeSequia()
        {
            try
            {
                var climas = new string[] { ClimaConstants.Sequia };
                var pronosticos = _pronosticoService.GetByClimas(climas);

                return Ok(pronosticos);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetDiasDeSequia action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("lluvia")]
        public IActionResult GetDiasDeLluvia()
        {
            try
            {
                var climas = new string[] { ClimaConstants.Lluvia, ClimaConstants.LluviaIntensa };
                var pronosticos = _pronosticoService.GetByClimas(climas);

                return Ok(pronosticos);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetDiasDeLluvia action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("optimo")]
        public IActionResult GetDiasOptimos([FromQuery(Name = "dia")]int dia)
        {
            try
            {
                var climas = new string[] { ClimaConstants.Optimo };
                var pronosticos = _pronosticoService.GetByClimas(climas);

                return Ok(pronosticos);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetDiasOptimos action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
            _logger.LogInformation("Value added");
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

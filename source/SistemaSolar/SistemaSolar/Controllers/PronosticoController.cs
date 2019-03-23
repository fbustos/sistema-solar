using System;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SistemaSolar.Controllers
{
    [Route("api/[controller]")]
    public class PronosticoController : Controller
    {
        private readonly ILogger _logger;
        private readonly IPronosticoService _pronosticoService;

        public PronosticoController(ILogger<PronosticoController> logger, IPronosticoService pronosticoService)
        {
            _logger = logger;
            _pronosticoService = pronosticoService;
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
                var p = _pronosticoService.GetByDia(dia);
                if (p == null)
                {
                    return NotFound(p);
                }

                return Ok(p);
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
                var p = _pronosticoService.GetByClimas(climas);
                if (p == null)
                {
                    return NotFound(p);
                }

                return Ok(p);
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
                var p = _pronosticoService.GetByClimas(climas);
                if (p == null)
                {
                    return NotFound(p);
                }

                return Ok(p);
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
                var p = _pronosticoService.GetByClimas(climas);
                if (p == null)
                {
                    return NotFound(p);
                }

                return Ok(p);
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

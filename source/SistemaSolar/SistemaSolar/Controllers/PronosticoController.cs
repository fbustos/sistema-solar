using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SistemaSolar.Controllers
{
    [Route("api/[controller]")]
    public class PronosticoController : Controller
    {
        private readonly ILogger _logger;
        private readonly IRepositoryWrapper _repository;

        public PronosticoController(ILogger<PronosticoController> logger, IRepositoryWrapper repository)
        {
            _logger = logger;
            _repository = repository;
        }

        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                // Sends a message to configured loggers, including the Stackdriver logger.
                // The Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker logger will log all controller actions with
                // log level information. This log is for additional information.

                var pronosticos = _repository.Pronostico.FindAll();

                _logger.LogInformation($"Returned all pronosticos from database.");

                return Ok(pronosticos);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside FindAll action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET api/values/5
        [HttpGet("clima")]
        public IActionResult Get([FromQuery(Name = "dia")]int dia)
        {
            var p = _repository.Pronostico.FindByCondition(x => x.Dia == dia);
            return Ok(p);
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

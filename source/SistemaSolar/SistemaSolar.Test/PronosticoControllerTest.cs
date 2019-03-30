using Contracts;
using Entities;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SistemaSolar.Controllers;
using System;
using System.Collections.Generic;

namespace Tests
{
    [TestFixture]
    public class PronosticoControllerTest
    {
        private Mock<IPronosticoService> pronosticoServiceMock;
        private Mock<IJobService> jobServiceMock;
        private ILogger<PronosticoController> logger;
        private IMemoryCache memoryCache;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProvider = services.BuildServiceProvider();

            memoryCache = serviceProvider.GetService<IMemoryCache>();
            jobServiceMock = new Mock<IJobService>();
            pronosticoServiceMock = new Mock<IPronosticoService>();
            logger = Mock.Of<ILogger<PronosticoController>>();
        }

        [Test]
        public void GetAll()
        {
            var list = new List<Pronostico>();
            var pronostico = new Pronostico
            {
                Clima = ClimaConstants.Sequia,
                Dia = 2,
                Fecha = DateTime.Today,
                NivelDeLluvia = 0
            };
            list.Add(pronostico);
            pronostico = new Pronostico
            {
                Clima = ClimaConstants.Sequia,
                Dia = 4,
                Fecha = DateTime.Today,
                NivelDeLluvia = 0
            };
            list.Add(pronostico);
            pronostico = new Pronostico
            {
                Clima = ClimaConstants.Sequia,
                Dia = 5,
                Fecha = DateTime.Today,
                NivelDeLluvia = 0
            };
            list.Add(pronostico);

            pronosticoServiceMock.Setup(service => service.GetAll()).Returns(list);            

            var controller = new PronosticoController(logger, pronosticoServiceMock.Object, jobServiceMock.Object, memoryCache);
            var actionResult = controller.Get();

            // Assert
            Assert.NotNull(actionResult);
            var result = actionResult as OkObjectResult;

            Assert.NotNull(result);

            var pronosticos = result.Value as List<Pronostico>;
            Assert.AreEqual(pronosticos.Count, 3);
        }

        [Test]
        public void GetByDiaOk()
        {
            int dia = 4;
            var pronostico = new Pronostico
            {
                Clima = ClimaConstants.Sequia,
                Dia = dia,
                Fecha = DateTime.Today,
                NivelDeLluvia = 0
            };

            pronosticoServiceMock.Setup(service => service.GetByDia(dia)).Returns(pronostico);

            var controller = new PronosticoController(logger, pronosticoServiceMock.Object, jobServiceMock.Object, memoryCache);
            var actionResult = controller.Get(dia);

            // Assert
            Assert.NotNull(actionResult);
            var result = actionResult as OkObjectResult;

            Assert.NotNull(result);

            var p = result.Value as Pronostico;
            Assert.AreEqual(p.Dia, dia);
        }

        [Test]
        public void GetByDiaNotFound()
        {
            int dia = -1;

            pronosticoServiceMock.Setup(service => service.GetByDia(dia)).Returns((Pronostico)null);

            var controller = new PronosticoController(logger, pronosticoServiceMock.Object, jobServiceMock.Object, memoryCache);
            var actionResult = controller.Get(dia);

            // Assert
            Assert.NotNull(actionResult);
            var result = actionResult as NotFoundObjectResult;

            Assert.NotNull(result);
        }
    }
}

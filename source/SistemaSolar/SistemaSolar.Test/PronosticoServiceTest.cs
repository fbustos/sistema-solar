using Contracts;
using Entities;
using Entities.Models;
using Moq;
using NUnit.Framework;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Tests
{
    [TestFixture]
    public class PronisticoServiceTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetPronosticoConClimaSequia()
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

            var mock = new Mock<IPronosticoRepository>();
            var wrapper = new Mock<IRepositoryWrapper>();
            mock.Setup(p => p.FindByCondition(It.IsAny<Expression<Func<Pronostico, bool>>>())).Returns(list);
            wrapper.Setup(p => p.Pronostico).Returns(mock.Object);
            var service = new PronosticoService(null, wrapper.Object);

            var dias = service.GetByClimas(new string[] { ClimaConstants.Sequia }).ToList();
            Assert.AreEqual(dias.Count(), 3);
            Assert.True(dias.All(d => d.Clima == ClimaConstants.Sequia));
        }

        [Test]
        public void PlanetasAlineadosEntreSi()
        {
            var list = new List<Planeta>();
            var planeta = new Planeta
            {
               PlanetaId = 1,
               Posicion = 90,
               Distancia = 100
            };
            list.Add(planeta);
            planeta = new Planeta
            {
                PlanetaId = 2,
                Posicion = 270,
                Distancia = 300
            };
            list.Add(planeta);
            planeta = new Planeta
            {
                PlanetaId = 3,
                Posicion = 90,
                Distancia = 450
            };
            list.Add(planeta);

            var service = new PronosticoService(null, null);
            var alineados = service.EstanAlineados(list, out double? pendiente);

            Assert.IsTrue(alineados);
            Assert.IsNull(pendiente);
        }

        [Test]
        public void PlanetasNoAlineadosEntreSi()
        {
            var list = new List<Planeta>();
            var planeta = new Planeta
            {
                PlanetaId = 1,
                Posicion = 45,
                Distancia = 100
            };
            list.Add(planeta);
            planeta = new Planeta
            {
                PlanetaId = 2,
                Posicion = 150,
                Distancia = 300
            };
            list.Add(planeta);
            planeta = new Planeta
            {
                PlanetaId = 3,
                Posicion = 90,
                Distancia = 450
            };
            list.Add(planeta);

            var service = new PronosticoService(null, null);
            var alineados = service.EstanAlineados(list, out double? pendiente);

            Assert.IsFalse(alineados);
        }

        [Test]
        public void PlanetasAlineadosAlSol()
        {
            var planeta = new Planeta
            {
                PlanetaId = 1,
                Posicion = 90,
                Distancia = 100
            };

            var service = new PronosticoService(null, null);
            var alineados = service.AlineadosAlSol(null, planeta, new Tuple<double, double>(0, 0));

            Assert.IsTrue(alineados);
        }

        [Test]
        public void PlanetasContienenAlSol()
        {
            var list = new List<Planeta>();
            var planeta = new Planeta
            {
                PlanetaId = 1,
                Posicion = 0,
                Distancia = 100
            };
            list.Add(planeta);
            planeta = new Planeta
            {
                PlanetaId = 2,
                Posicion = 90,
                Distancia = 300
            };
            list.Add(planeta);
            planeta = new Planeta
            {
                PlanetaId = 3,
                Posicion = 200,
                Distancia = 450
            };
            list.Add(planeta);


            var service = new PronosticoService(null, null);
            var contieneAlSol = service.PlanetasContienenAlSol(list, new Tuple<double, double>(0, 0));

            Assert.IsTrue(contieneAlSol);
        }

        [Test]
        public void PlanetasContienenAlSolEjeX()
        {
            var list = new List<Planeta>();
            var planeta = new Planeta
            {
                PlanetaId = 1,
                Posicion = 180,
                Distancia = 100
            };
            list.Add(planeta);
            planeta = new Planeta
            {
                PlanetaId = 2,
                Posicion = 90,
                Distancia = 300
            };
            list.Add(planeta);
            planeta = new Planeta
            {
                PlanetaId = 3,
                Posicion = 0,
                Distancia = 450
            };
            list.Add(planeta);


            var service = new PronosticoService(null, null);
            var contieneAlSol = service.PlanetasContienenAlSol(list, new Tuple<double, double>(0, 0));

            Assert.IsTrue(contieneAlSol);
        }

        [Test]
        public void PlanetasNoContienenAlSol()
        {
            var list = new List<Planeta>();
            var planeta = new Planeta
            {
                PlanetaId = 1,
                Posicion = 60,
                Distancia = 100
            };
            list.Add(planeta);
            planeta = new Planeta
            {
                PlanetaId = 2,
                Posicion = 90,
                Distancia = 300
            };
            list.Add(planeta);
            planeta = new Planeta
            {
                PlanetaId = 3,
                Posicion = 150,
                Distancia = 450
            };
            list.Add(planeta);


            var service = new PronosticoService(null, null);
            var contieneAlSol = service.PlanetasContienenAlSol(list, new Tuple<double, double>(0, 0));

            Assert.IsFalse(contieneAlSol);
        }

        [Test]
        public void PronosticarClimaSeco()
        {
            var list = new List<Planeta>();
            var planeta = new Planeta
            {
                PlanetaId = 1,
                Posicion = 45,
                Distancia = 100
            };
            list.Add(planeta);
            planeta = new Planeta
            {
                PlanetaId = 2,
                Posicion = 225,
                Distancia = 300
            };
            list.Add(planeta);
            planeta = new Planeta
            {
                PlanetaId = 3,
                Posicion = 45,
                Distancia = 450
            };
            list.Add(planeta);


            var service = new PronosticoService(null, null);
            var clima = service.PronosticarClimaDePlanetas(list);

            Assert.AreEqual(clima, ClimaConstants.Sequia);
        }

        [Test]
        public void PronosticarClimaOptimo()
        {
            var list = new List<Planeta>();
            var planeta = new Planeta
            {
                PlanetaId = 1,
                Posicion = 45,
                Distancia = 100
            };
            list.Add(planeta);
            planeta = new Planeta
            {
                PlanetaId = 2,
                Posicion = 315,
                Distancia = 100
            };
            list.Add(planeta);
            planeta = new Planeta
            {
                PlanetaId = 3,
                Posicion = 45,
                Distancia = 100
            };
            list.Add(planeta);


            var service = new PronosticoService(null, null);
            var clima = service.PronosticarClimaDePlanetas(list);

            Assert.AreEqual(clima, ClimaConstants.Optimo);
        }

        [Test]
        public void PronosticarClimaLluvioso()
        {
            var list = new List<Planeta>();
            var planeta = new Planeta
            {
                PlanetaId = 1,
                Posicion = 0,
                Distancia = 100
            };
            list.Add(planeta);
            planeta = new Planeta
            {
                PlanetaId = 2,
                Posicion = 90,
                Distancia = 300
            };
            list.Add(planeta);
            planeta = new Planeta
            {
                PlanetaId = 3,
                Posicion = 200,
                Distancia = 450
            };
            list.Add(planeta);


            var service = new PronosticoService(null, null);
            var clima = service.PronosticarClimaDePlanetas(list);

            Assert.AreEqual(clima, ClimaConstants.Lluvia);
        }

        [Test]
        public void PronosticarClimaIndeterminado()
        {
            var list = new List<Planeta>();
            var planeta = new Planeta
            {
                PlanetaId = 1,
                Posicion = 60,
                Distancia = 100
            };
            list.Add(planeta);
            planeta = new Planeta
            {
                PlanetaId = 2,
                Posicion = 90,
                Distancia = 300
            };
            list.Add(planeta);
            planeta = new Planeta
            {
                PlanetaId = 3,
                Posicion = 150,
                Distancia = 450
            };
            list.Add(planeta);


            var service = new PronosticoService(null, null);
            var clima = service.PronosticarClimaDePlanetas(list);

            Assert.AreEqual(clima, ClimaConstants.Indeterminado);
        }
    }
}
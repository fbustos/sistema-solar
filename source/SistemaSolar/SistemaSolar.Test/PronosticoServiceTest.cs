using Contracts;
using Entities;
using Entities.Models;
using Moq;
using NUnit.Framework;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    public class PronisticoServiceTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            //var list = new List<Pronostico>();
            //var pronostico = new Pronostico
            //{
            //    Clima = ClimaConstants.Sequia,
            //    Dia = 2,
            //    Fecha = DateTime.Today,
            //    NivelDeLluvia = 0
            //};
            //list.Add(pronostico);
            //pronostico = new Pronostico
            //{   
            //    Clima = ClimaConstants.Lluvia,
            //    Dia = 3,
            //    Fecha = DateTime.Today,
            //    NivelDeLluvia = 10
            //};
            //list.Add(pronostico);
            //pronostico = new Pronostico
            //{
            //    Clima = ClimaConstants.Sequia,
            //    Dia = 4,
            //    Fecha = DateTime.Today,
            //    NivelDeLluvia = 0
            //};
            //list.Add(pronostico);
            //pronostico = new Pronostico
            //{
            //    Clima = ClimaConstants.Sequia,
            //    Dia = 5,
            //    Fecha = DateTime.Today,
            //    NivelDeLluvia = 0
            //};
            //list.Add(pronostico);

            //var mock = new Mock<IPronosticoRepository>();
            //var wrapper = new Mock<IRepositoryWrapper>();
            //mock.Setup(p => p.FindByCondition(x => x.Clima == ClimaConstants.Sequia)).Returns(list);
            //wrapper.Setup(p => p.Pronostico).Returns(mock.Object);
            //var service = new PronosticoService(null, wrapper.Object);

            //var dias = service.GetByClimas(new string[] { ClimaConstants.Sequia }).ToList();
            //Assert.AreEqual(dias.Count(), 3);
            //Assert.True(!dias.Contains(list[1]));

            //Assert.Pass();
        }
    }
}
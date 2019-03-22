﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Models
{
    [Table("planeta")]
    public class Planeta
    {
        [Key]
        public int PlanetaId { get; set; }

        public string Nombre { get; set; }

        [Required]
        public bool SentidoHorario { get; set; }

        [Required]
        public int VelocidadAngular { get; set; }

        [Required]
        public int Distancia { get; set; }

        [Required]
        public int Posicion { get; set; }

        [NotMapped]
        public Tuple<double, double> PosicionRectangular
        {
            get
            {
                double anguloRad = (Math.PI / 180.0) * this.Posicion;
                double x = this.Distancia * Math.Cos(anguloRad);
                double y = this.Distancia * Math.Sin(anguloRad);

                return new Tuple<double, double>(x, y);
            }
        }

        public void CalcularPosicion(int dias)
        {
            this.Posicion = this.SentidoHorario ? -(VelocidadAngular * dias) % 360
                                                : (VelocidadAngular * dias) % 360;
        }

        public void ActualizarPosicionUnDia()
        {
            if (this.SentidoHorario)
                this.Posicion -= VelocidadAngular;
            else
                this.Posicion += VelocidadAngular;

            this.Posicion %= 360;
        }
    }
}

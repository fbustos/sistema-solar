using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("pronostico")]
    public class Pronostico
    {
        [Key]
        public Guid PronosticoId { get; set; }

        [Required]
        public int Dia { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public string Clima { get; set; }

        [NotMapped]
        public double NivelDeLluvia { get; set; }
    }
}

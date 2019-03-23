using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("pronostico")]
    public class Pronostico
    {
        [Key]
        public int Dia { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public string Clima { get; set; }
        
        public int? JobId { get; set; }

        [NotMapped]
        public double NivelDeLluvia { get; set; }
    }
}

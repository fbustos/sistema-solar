using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("job")]
    public class Job
    {
        [Key]
        public int JobId { get; set; }
        
        public int Anios { get; set; }

        public DateTime FechaInicio { get; set; }
    }
}

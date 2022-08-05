using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExamenTeCAS.Models
{
    public class Movimiento
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es Requerido")]
        public char Tipo { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]

        public decimal Monto { get; set; }

        [StringLength(10)]
        public string Fecha { get; set; }

        [StringLength(15)]
        public string Hora { get; set; }

        public Cuenta Cuenta { get; set; }

        [NotMapped]
        public int IdCuenta { get; set; }
    }
}

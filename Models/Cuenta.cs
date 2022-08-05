using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExamenTeCAS.Models
{
    public class Cuenta
    {
        public int Id { get; set; }

        [Required(ErrorMessage = " El campo {0} es requerido")]
        [StringLength(200)]
        [Display(Name ="Nombre de la Cuenta")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Número de Cuenta")]        
        [StringLength(maximumLength:18, MinimumLength =3, ErrorMessage ="El campo {0} debe tener mínimo 3 números y máximo 18")]
        public string Numero { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]

        public decimal Saldo { get; set; }

        [StringLength(10)]
        public string Fecha { get; set; }

        [StringLength(15)]
        public string Hora { get; set; }

        public Usuario Usuario { get; set; }

        [NotMapped]
        public int IdUsuario { get; set; }

    }
}

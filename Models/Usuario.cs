using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExamenTeCAS.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Nombre(s)")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Apellido Parterno")]
        [StringLength(50)]
        public string Apaterno { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Apellido Materno")]
        [StringLength(50)]
        public string Amaterno { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name ="Correo electrónico")]
        [EmailAddress(ErrorMessage = "Ingresa un correo válido")]
        [StringLength(100)]
        public string Correo { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Teléfono")]
        [StringLength(10)]
        [Phone]
        //[StringLength(10,MinimumLength =10,ErrorMessage ="Debe contener 10 dígitos")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name ="Número de Identificación")]
        [StringLength(20)]
        public string Identificacion { get; set; }

        [StringLength(10)]
        public string FechaIngreso { get; set; }

        public string UID { get; set; }

    }
}

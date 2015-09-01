using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RecordFCS_Alt.Models
{
    [MetadataType(typeof(UsuarioMetadata))]
    public partial class Usuario
    {
        [Key]
        public Guid UsuarioID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }


        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public bool Status { get; set; }


        /* Propiedades de navegacion*/
        public virtual ICollection<Permiso> Permisos { get; set; }
        //public virtual ICollection<log_Historial> Historial { get; set; }
    }

    public class UsuarioMetadata
    {
        public Guid UsuarioID { get; set; }

        [Required]
        [Display(Name = "Nombre de Usuario")]
        [Remote("validarRegistroUnico", "Usuario", HttpMethod = "POST", ErrorMessage = "Ya existe un registro con este nombre. Intenta con otro.")]
        public string UserName { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*_])[0-9a-zA-Z!@#$%^&*_0-9]{8,128}$", ErrorMessage = "Contraseña debe contener, Mayuscula, Número, Caracter Especial !@#$%^&*_ y 8 Caracteres Mínimo.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }


        public string Nombre { get; set; }
        public string Apellido { get; set; }

        [Display(Name = "Correo Eléctronico")]
        [EmailAddress]
        [StringLength(64)]
        [DataType(DataType.EmailAddress)]
        public string Correo { get; set; }

        [Display(Name = "Estado")]
        public bool Status { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RecordFCS_Alt.Models
{
    [MetadataType(typeof(AutorMetadata))]
    public partial class Autor
    {
        [Key]
        public Guid AutorID { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string LugarNacimiento { get; set; }
        public string AnoNacimiento { get; set; }
        public string LugarMuerte { get; set; }
        public string AnoMuerte { get; set; }
        public string Observaciones { get; set; }
        public bool Status { get; set; }

        //Anteriores
        public string Temp { get; set; }


        //Virtuales
        public virtual ICollection<AutorPieza> AutorPiezas { get; set; }

    }

    public class AutorMetadata
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Requerido.")]
        //[StringLength(127)]
        [Display(Name = "Nombre(s)")]

        public string Nombre { get; set; }
        [StringLength(127)]
        [Display(Name = "Apellido(s)")]
        public string Apellido { get; set; }

        [Display(Name = "Lugar de Nacimiento")]
        public string LugarNacimiento { get; set; }

        [Display(Name = "Año de Nacimiento")]
        public string AnoNacimiento { get; set; }

        [Display(Name = "Lugar de Muerte")]
        public string LugarMuerte { get; set; }

        [Display(Name = "Año de Muerte")]
        public string AnoMuerte { get; set; }

        public string Observaciones { get; set; }

        [Display(Name = "Estado")]
        public bool Status { get; set; }

        [StringLength(63)]
        public string Temp { get; set; }
    }

}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RecordFCS_Alt.Models
{
    [MetadataType(typeof(AutorPiezaMetadata))]
    public partial class AutorPieza
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Pieza")]
        public Guid PiezaID { get; set; }

        [Key]
        [Column(Order = 2)]
        [ForeignKey("Autor")]
        public Guid AutorID { get; set; }

        public bool esPrincipal { get; set; }
        public string Prefijo { get; set; }
        public bool Status { get; set; }

        //Virtuales
        public virtual Pieza Pieza { get; set; }
        public virtual Autor Autor { get; set; }
    }

    public class AutorPiezaMetadata
    {
        [Display(Name = "Estado")]
        public bool Status { get; set; }

        [Display(Name = "Principal")]
        public bool esPrincipal { get; set; }
        
        [Display(Name = "Prefijo")]
        public string Prefijo { get; set; }
    }
}
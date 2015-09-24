using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RecordFCS_Alt.Models
{
    public class MovimientoResponsable
    {
        [Key]
        public int MovimientoResponsableID { get; set; }

        //[ForeignKey("Movimiento")]
        //public Guid MovimientoID { get; set; }

        public string Nombre { get; set; }
        public string Institucion { get; set; }
        public string FechaSalida { get; set; }

        //public virtual Movimiento Movimiento { get; set; }

    }
}
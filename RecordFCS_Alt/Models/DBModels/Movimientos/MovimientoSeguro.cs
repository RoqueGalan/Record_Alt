using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RecordFCS_Alt.Models
{
    public class MovimientoSeguro
    {
        [Key]
        public int MovimientoSeguroID { get; set; }


        public string AseguradorNombre { get; set; }
        public string NoPoliza { get; set; }
        public string FechaInicial { get; set; }
        public string FechaFinal { get; set; }

        //virtual
    }
}
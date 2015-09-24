using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RecordFCS_Alt.Models
{
    public class MovimientoExposicion
    {
        [Key]
        public int MovimientoExposicionID { get; set; }


        public string Titulo { get; set; }
        public string Curador { get; set; }
        public string FechaInicial { get; set; }
        public string FechaFinal { get; set; }

    }
}
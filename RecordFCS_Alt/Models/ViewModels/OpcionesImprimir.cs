using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RecordFCS_Alt.Models.ViewsModel
{
    public class OpcionesImprimir
    {
        public string NombreLista { get; set; }
        public int NoElementos { get; set; }
        public string Accion { get; set; }

        public int NoColumnas { get; set; }
        public int MostrarDatos { get; set; }
        public bool IncluirPiezas { get; set; }
        public int Unir { get; set; }
        public bool Ubicacion { get; set; }
        public bool Fecha { get; set; }

        public int Linea { get; set; }

        public int NombreLogotipo { get; set; }
        public bool FondoAgua { get; set; }

    }
}
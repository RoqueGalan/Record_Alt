using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RecordFCS_Alt.Models.ViewsModel
{
    public class itemListaBasicaDefault
    {
        public long ObraID { get; set; }
        public long PiezaID { get; set; }
        public string Obra_clave { get; set; }
        public string Pieza_clave { get; set; }
        public string Titulo { get; set; }
        public string Tecnica { get; set; }
        public string Medida { get; set; }
        public string Fecha { get; set; }
        public string Ubicacion { get; set; }
        public string Autor { get; set; }
        public string RutaImagen { get; set; }
    }
}
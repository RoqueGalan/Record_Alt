using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RecordFCS_Alt.Models.Migraciones
{
    public class RowArchivo
    {
        public int ArchivoID { get; set; }
        public int FICHA_NO { get; set; }
        public string Clasificacion { get; set; }
        public string Fondo { get; set; }
        public string Nombredelfondo { get; set; }
        public string Carpeta { get; set; }
        public string Caja { get; set; }
        public string Legajo { get; set; }
        public string Fojas { get; set; }
        public string Documento { get; set; }
        public string LugaryFecha { get; set; }
        public string Tipodedocumento { get; set; }
        public string Firmadopor { get; set; }
        public string Asunto1 { get; set; }
        public string Asunto2 { get; set; }
        public string URLFicha { get; set; }
        public int? NoImag { get; set; }
        public string URLImagen { get; set; }

        public string Tema { get; set; }
        public string Dirigidoa { get; set; }

    }
}
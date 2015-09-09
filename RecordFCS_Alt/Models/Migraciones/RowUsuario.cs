using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RecordFCS_Alt.Models.Migraciones
{
    public class RowUsuario
    {

        public string  cve_usuario { get; set; }
        public string login { get; set; }
        public string Puesto_Clave { get; set; }
        public string nombre { get; set; }
        public string Departamento_Clave { get; set; }
        public string password { get; set; }
        public string a_paterno { get; set; }
        public string a_materno { get; set; }
        public string email { get; set; }
        public string estatus { get; set; }
        public string cve_permiso { get; set; }
        public string passwordOK { get; set; }
        public string permisos { get; set; }
        public string cmonitoreo { get; set; }


    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RecordFCS_Alt.Models
{
    [MetadataType(typeof(PermisoMetadata))]
    public partial class Permiso
    {
        //Llaves Primarias
        [Key]
        [Column(Order = 1)]
        [ForeignKey("TipoPermiso")]
        public Guid TipoPermisoID { get; set; }

        [Key]
        [Column(Order = 2)]
        [ForeignKey("Usuario")]
        public Guid UsuarioID { get; set; }
        public bool Status { get; set; }


        //Virtuales
        public virtual Usuario Usuario { get; set; }
        public virtual TipoPermiso TipoPermiso { get; set; }
    }

    public class PermisoMetadata
    {

        public Guid TipoPermisoID { get; set; }

        public Guid UsuarioID { get; set; }

        [Display(Name = "Estado")]
        public bool Status { get; set; }

    }
}
﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RecordFCS_Alt.Models
{
    [MetadataType(typeof(TipoPermisoMetadata))]
    public partial class TipoPermiso
    {
        [Key]
        public Guid TipoPermisoID { get; set; }
        public string Clave { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Status { get; set; }

        //virtual
        public virtual ICollection<Permiso> Permisos { get; set; }
    }

    public class TipoPermisoMetadata
    {
        public Guid TipoPermisoID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Requerido.")]
        [StringLength(127)]
        [Display(Name = "Clave")]
        [Remote("EsUnico", "TipoPermiso", HttpMethod = "POST", AdditionalFields = "TipoPermisoID", ErrorMessage = "Ya existe, intenta otro nombre.")]
        public string Clave { get; set; }
        
        public string Nombre { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Display(Name = "Estado")]
        public bool Status { get; set; }

    }
}
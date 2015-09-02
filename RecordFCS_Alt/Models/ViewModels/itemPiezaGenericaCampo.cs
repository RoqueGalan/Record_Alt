using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RecordFCS_Alt.Models.ViewsModel
{
    public class itemPiezaGenericaCampo
    {
        [ForeignKey("itemPiezaGenerica")]
        public Guid PiezaID { get; set; }
        public string NombreCampo { get; set; }
        public string ValorCampo { get; set; }

        public int Orden { get; set; }

        public virtual itemPiezaGenerica itemPiezaGenerica { get; set; }
    }
}
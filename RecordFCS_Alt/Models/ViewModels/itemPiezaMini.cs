using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RecordFCS_Alt.Models.ViewsModel
{
    public class itemPiezaMini
    {
        public Guid PiezaID { get; set; }
        public Guid ObraID { get; set; }
        public string FolioPieza { get; set; }
        public string FolioObra { get; set; }
        public string NombrePieza { get; set; }
        public string NombreObra { get; set; }
        public bool esPrincipal { get; set; }

        public string RutaImagenMini { get; set; }
        public Guid? ImagenID { get; set; }

        public List<Guid> ListaPiezasHijas { get; set; }
        public virtual List<itemPiezaMiniAtt> Atributos { get; set; }
    }


    public class itemPiezaMiniAtt
    {
        public Guid PiezaID { get; set; }
        public Guid AtributoID { get; set; }
        public int Orden { get; set; }

        public string Nombre { get; set; }

        public virtual List<itemPiezaMiniAttValor> Valores { get; set; }
    }

    public class itemPiezaMiniAttValor
    {
        public Guid? AtributoPiezaID { get; set; }
        public int Orden { get; set; }
        public string Valor { get; set; }
    }
}
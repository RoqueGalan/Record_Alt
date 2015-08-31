using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RecordFCS_Alt.Models.ViewModels
{
    public class Item_Ficha
    {
        public int Orden { get; set; }
        public string Titulo { get; set; }
        public bool EsGenerico { get; set; }
        public bool EsLista { get; set; }
        public bool EsMultipleValor { get; set; }
        public string TablaSQL { get; set; }

        public virtual List<Item_Ficha_Valor> Valores {get; set;}

    }


    public class Item_Ficha_Valor
    {
        public virtual ICollection<Item_Ficha_Boton> Botones { get; set; }

    }



    public class Item_Ficha_Boton
    {
        public int Orden { get; set; }
        public string Btn_Texto { get; set; }
        public string Btn_Clase { get; set; }
        public string Btn_Parametros { get; set; }

        public string  Btn_Icono { get; set; }

        //<span class = "btn btn-danger editar">        </span>
        //<i class = "fa fa-edit"></i>
    }


}
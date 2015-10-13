using RecordFCS_Alt.Models;
using RecordFCS_Alt.Models.ViewsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RecordFCS_Alt.Controllers
{

    public class PiezaApiController : ApiController
    {
        private RecordFCSContext db = new RecordFCSContext();

        [HttpGet]
        [ActionName("ConsultarPiezaMaestra")]
        public List<itemPiezaMini> ConsultarPiezaMaestra()
        {
            List<itemPiezaMini> lista = new List<itemPiezaMini>();

            var x = db.Piezas.Where(a => a.TipoPieza.EsPrincipal).OrderBy(a => a.Obra.LetraFolio.Nombre == "A").ThenBy(a => a.Obra.NumeroFolio).Take(10).ToList();

            foreach (var pieza in x)
            {
                lista.Add( new itemPiezaMini()
                {
                    ObraID = pieza.ObraID,
                    PiezaID = pieza.PiezaID,
                    FolioObra = pieza.Obra.LetraFolio.Nombre + pieza.Obra.NumeroFolio,
                    FolioPieza = pieza.ImprimirFolio(),
                    NombreObra = pieza.Obra.TipoObra.Nombre,
                    NombrePieza = pieza.TipoPieza.Nombre,
                    esPrincipal = pieza.TipoPieza.EsPrincipal,
                    ListaPiezasHijas = new List<Guid>(),
                    Atributos = new List<itemPiezaMiniAtt>()
                });

            }
            return lista;
        }

    }
}

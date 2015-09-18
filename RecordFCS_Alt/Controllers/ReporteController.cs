using RecordFCS_Alt.Helpers.Seguridad;
using RecordFCS_Alt.Models;
using RecordFCS_Alt.Models.ViewsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace RecordFCS_Alt.Controllers
{
    public class ReporteController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: Reporte
        [CustomAuthorize(permiso = "repList")]
        public ActionResult Index()
        {
            //Mostrar los atributos que sean lista para tratarlos
            var lista = db.TipoAtributos.Where(a => a.EsLista && a.Status && a.Nombre != "Fotográfia").OrderBy(a => a.Nombre);

            return View(lista);
        }

        [CustomAuthorize(permiso = "repList")]
        public ActionResult ReporteLista(Guid? tipoAtributoID, Guid? subID = null)
        {
            if (tipoAtributoID == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            TipoAtributo tipoAtt = db.TipoAtributos.Find(tipoAtributoID);

            if (tipoAtt == null)
                return HttpNotFound();

            List<ItemReporte> lista = new List<ItemReporte>();
            List<ItemObraReporte> listaObraSinCampo = new List<ItemObraReporte>();

            int totalRegistrosEnCatalogo = 0;

            //total de piezas maestras con el atributo
            int totalPiezasObligatorias = db.Piezas.Where(a => a.TipoPieza.EsPrincipal && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
            int subTotalPiezasConCampo = 0; //piezas con campo + multiple
            
            int subTotalPiezasSinCampo = 0; // piezas sin campo

            int subTotalPiezasMultiples = 0; // piezas multiples

            

            if (tipoAtt.EsLista)
            {

                if (tipoAtt.EsGenerico)
                {

                }
                else
                {
                    //generar un switch por cada tabla que necesite un reporte
                    switch (tipoAtt.TablaSQL)
                    {
                        case "Autor":
                            totalRegistrosEnCatalogo = db.Autores.Count();

                            //PIEZAS CON ATRIBUTO
                                foreach (var item in db.Autores.Select(a => new { a.AutorID, Nombre = a.Nombre + " " + a.Apellido, Total = a.AutorPiezas.Where(b => b.Pieza.TipoPieza.EsPrincipal && b.Pieza.TipoPieza.Atributos.Any(c => c.TipoAtributoID == tipoAtt.TipoAtributoID)).Count() }).OrderBy(a => a.Nombre))
                                {
                                    if (item.Total > 0)
                                    {
                                        lista.Add(new ItemReporte()
                                        {
                                            Nombre = item.Nombre,
                                            Cantidad = item.Total,
                                            Porcentaje = Math.Round((double)(item.Total * 100) / totalRegistrosEnCatalogo, 3)
                                        });

                                        subTotalPiezasConCampo += item.Total;
                                    }
                                }
                            
                            //PIEZAS FALTANTES DEL ATRIBUTO
                            // 0, sin definir

                                var i = 0;
                                foreach (var item in db.Piezas.Where(a => a.TipoPieza.EsPrincipal && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID && !a.AutorPiezas.Any(c => c.PiezaID == a.PiezaID))).Select(a => new { a.ObraID, LetraFolioNombre = a.Obra.LetraFolio.Nombre, a.Obra.NumeroFolio, AttTitulo = a.AtributoPiezas.FirstOrDefault(b => b.Atributo.TipoAtributo.Temp == "titulo").Valor }).OrderBy(a => a.LetraFolioNombre).ThenBy(a => a.NumeroFolio).ToList())
                                {
                                    if (i < 100)
                                    {
                                        listaObraSinCampo.Add(new ItemObraReporte()
                                        {
                                            ObraID = item.ObraID,
                                            Clave = item.LetraFolioNombre + item.NumeroFolio,
                                            Titulo = item.AttTitulo
                                        });
                                    }

                                    subTotalPiezasSinCampo++;
                                }
                            
                            break;
                        case "Ubicacion":
                        case "TipoTecnica":
                        case "TipoMedida":
                        default:
                            //Error
                            break;
                    }
                }

            }
            else
            {
                //No es lista
                //error
            }


            /*Vista*/
            
            ViewBag.Titulo = tipoAtt.Nombre;


            

            //listas
            ViewBag.listaMaestra = lista;
            ViewBag.listaObraSinCampo = listaObraSinCampo;
            return PartialView("_ReporteLista");

        }

    }
}
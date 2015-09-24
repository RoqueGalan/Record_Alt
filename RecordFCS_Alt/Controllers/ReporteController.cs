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

            ViewBag.listaTipoMedida = db.TipoMedidas.Where(a => a.Status).OrderBy(a => a.Nombre).ToList();
            ViewBag.listaTipoTecnica = db.TipoTecnicas.Where(a => a.Status).OrderBy(a => a.Nombre).ToList();
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

            //total de piezas maestras con el atributo
            int totalPiezasObligatorias = db.Piezas.Where(a => a.TipoPieza.EsPrincipal && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count();
            int subTotalPiezasConCampo = 0; //piezas con campo + multiple

            int subTotalPiezasSinCampo = 0; // piezas sin campo

            int subTotalPiezasMultiples = 0; // piezas multiples

            List<ItemReporte> listaComprobarConRegistros = new List<ItemReporte>();
            List<ItemObraReporte> listaComprobarSinRegistros = new List<ItemObraReporte>();

            string subTitulo = "";

            var i = 0;

            if (tipoAtt.EsLista)
            {

                if (tipoAtt.EsGenerico)
                {
                    //PIEZAS CON ATRIBUTO
                    listaComprobarConRegistros = db.ListaValores.Where(a => a.TipoAtributoID == tipoAtt.TipoAtributoID && a.Status).Select(a => new ItemReporte() { Nombre = a.Valor, Cantidad = a.AtributoPiezas.Where(b => b.Pieza.TipoPieza.EsPrincipal && b.Pieza.TipoPieza.Atributos.Any(c => c.TipoAtributoID == tipoAtt.TipoAtributoID)).Count() }).OrderBy(a => a.Nombre).ToList();
                    //PIEZAS FALTANTES DEL ATRIBUTO
                    // 0, sin definir
                    listaComprobarSinRegistros = db.Piezas.Where(a => a.TipoPieza.EsPrincipal && a.TipoPieza.Atributos.Any(b=> b.TipoAtributoID == tipoAtt.TipoAtributoID) && a.AtributoPiezas.Where(c=> c.Atributo.TipoAtributoID == tipoAtt.TipoAtributoID).Count() == 0).OrderBy(a => a.Obra.LetraFolio.Nombre).ThenBy(a => a.Obra.NumeroFolio).Select(a => new ItemObraReporte() { ObraID = a.ObraID, Clave = a.Obra.LetraFolio.Nombre + a.Obra.NumeroFolio, Titulo = a.AtributoPiezas.FirstOrDefault(b => b.Atributo.TipoAtributo.Temp == "titulo").Valor }).ToList();
                }
                else
                {
                    //generar un switch por cada tabla que necesite un reporte
                    switch (tipoAtt.TablaSQL)
                    {
                        case "Autor":
                            //PIEZAS CON ATRIBUTO
                            listaComprobarConRegistros = db.Autores.Select(a => new ItemReporte() { Nombre = a.Nombre + " " + a.Apellido, Cantidad = a.AutorPiezas.Where(b => b.Pieza.TipoPieza.EsPrincipal && b.Pieza.TipoPieza.Atributos.Any(c => c.TipoAtributoID == tipoAtt.TipoAtributoID)).Count() }).OrderBy(a => a.Nombre).ToList();
                            //PIEZAS FALTANTES DEL ATRIBUTO
                            // 0, sin definir
                            listaComprobarSinRegistros = db.Piezas.Where(a => a.TipoPieza.EsPrincipal && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID && !a.AutorPiezas.Any(c => c.PiezaID == a.PiezaID))).OrderBy(a => a.Obra.LetraFolio.Nombre).ThenBy(a => a.Obra.NumeroFolio).Select(a => new ItemObraReporte() { ObraID = a.ObraID, Clave = a.Obra.LetraFolio.Nombre + a.Obra.NumeroFolio, Titulo = a.AtributoPiezas.FirstOrDefault(b => b.Atributo.TipoAtributo.Temp == "titulo").Valor }).ToList();

                            break;
                        case "Ubicacion":
                            //PIEZAS CON ATRIBUTO
                            listaComprobarConRegistros = db.Ubicaciones.Select(a => new ItemReporte { Nombre = a.Nombre, Cantidad = a.Piezas.Where(b => b.TipoPieza.EsPrincipal && b.TipoPieza.Atributos.Any(c => c.TipoAtributoID == tipoAtt.TipoAtributoID)).Count() }).OrderBy(a => a.Nombre).ToList();
                            //PIEZAS FALTANTES DEL ATRIBUTO
                            // 0, sin definir
                            listaComprobarSinRegistros = db.Piezas.Where(a => a.TipoPieza.EsPrincipal && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID) && a.UbicacionID == null).OrderBy(a => a.Obra.LetraFolio.Nombre).ThenBy(a => a.Obra.NumeroFolio).Select(a => new ItemObraReporte() { ObraID = a.ObraID, Clave = a.Obra.LetraFolio.Nombre + a.Obra.NumeroFolio, Titulo = a.AtributoPiezas.FirstOrDefault(b => b.Atributo.TipoAtributo.Temp == "titulo").Valor }).ToList();

                            break;
                        case "TipoTecnica":
                        //subID
                            var tipoTecnica = db.TipoTecnicas.Find(subID);
                            if (tipoTecnica != null)
                            {
                                subTitulo = " " + tipoTecnica.Nombre;
                                //PIEZAS CON ATRIBUTO
                                listaComprobarConRegistros = tipoTecnica.Tecnicas.Select(a => new ItemReporte() {Nombre = a.Descripcion, Cantidad = a.TecnicaPiezas.Where(b=> b.Pieza.TipoPieza.EsPrincipal && b.Pieza.TipoPieza.Atributos.Any(c=> c.TipoAtributoID == tipoAtt.TipoAtributoID)).Count() }).OrderBy(a=> a.Nombre).ToList();
                                //PIEZAS FALTANTES DEL ATRIBUTO
                                // 0, sin definir
                                listaComprobarSinRegistros = db.Piezas.Where(a => a.TipoPieza.EsPrincipal && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID) && !a.TecnicaPiezas.Any(c => c.PiezaID == a.PiezaID && c.TipoTecnicaID == tipoTecnica.TipoTecnicaID)).OrderBy(a => a.Obra.LetraFolio.Nombre).ThenBy(a => a.Obra.NumeroFolio).Select(a => new ItemObraReporte() { ObraID = a.ObraID, Clave = a.Obra.LetraFolio.Nombre + a.Obra.NumeroFolio, Titulo = a.AtributoPiezas.FirstOrDefault(b => b.Atributo.TipoAtributo.Temp == "titulo").Valor }).ToList();
                            }
                            break;
                        case "TipoMedida":
                        //subID
                            var tipoMedida = db.TipoMedidas.Find(subID);
                            if (tipoMedida != null)
                            {
                                subTitulo = " " + tipoMedida.Nombre;
                                //PIEZAS CON ATRIBUTO
                                listaComprobarConRegistros.Add(new ItemReporte() { Nombre = tipoMedida.Nombre, Cantidad = tipoMedida.Medidas.Where( a=> a.Pieza.TipoPieza.EsPrincipal && a.Pieza.TipoPieza.Atributos.Any(b=> b.TipoAtributoID == tipoAtt.TipoAtributoID)).Count()});
                                //PIEZAS FALTANTES DEL ATRIBUTO
                                // 0, sin definir
                                listaComprobarSinRegistros = db.Piezas.Where(a => a.TipoPieza.EsPrincipal && a.TipoPieza.Atributos.Any(b => b.TipoAtributoID == tipoAtt.TipoAtributoID) && !a.MedidaPiezas.Any(c => c.PiezaID == a.PiezaID && c.TipoMedidaID == tipoMedida.TipoMedidaID)).OrderBy(a => a.Obra.LetraFolio.Nombre).ThenBy(a => a.Obra.NumeroFolio).Select(a => new ItemObraReporte() { ObraID = a.ObraID, Clave = a.Obra.LetraFolio.Nombre + a.Obra.NumeroFolio, Titulo = a.AtributoPiezas.FirstOrDefault(b => b.Atributo.TipoAtributo.Temp == "titulo").Valor }).ToList();
                            }
                            break;
                        default:
                            //Error
                            subTitulo = " - E R R O R -";
                            break;
                    }
                }

            }
            else
            {
                //No es lista
                //error
            }


            var total = 0;
            if (tipoAtt.TablaSQL == "TipoMedida" || tipoAtt.TablaSQL == "TipoTecnica")
            {
                total = listaComprobarConRegistros.Sum(a => a.Cantidad) + listaComprobarSinRegistros.Count();
            }
            else
            {
                total = listaComprobarConRegistros.Sum(a => a.Cantidad);
            }

            //PIEZAS CON ATRIBUTO
            foreach (var item in listaComprobarConRegistros)
            {
                if (item.Cantidad > 0)
                {
                    lista.Add(new ItemReporte()
                    {
                        Nombre = item.Nombre,
                        Cantidad = item.Cantidad,
                        Porcentaje = Math.Round((double)(item.Cantidad * 100) / total, 3)
                    });

                    subTotalPiezasConCampo += item.Cantidad;
                }
            }



            //PIEZAS FALTANTES DEL ATRIBUTO
            // 0, sin definir
            foreach (var item in listaComprobarSinRegistros)
            {
                if (i < 100)
                {
                    listaObraSinCampo.Add(new ItemObraReporte()
                    {
                        ObraID = item.ObraID,
                        Clave = item.Clave,
                        Titulo = item.Titulo
                    });
                    i++;
                }

                subTotalPiezasSinCampo++;
            }

            //(1)totalPiezasObligatorias // 100
            //(2)subTotalPiezasConCampo // 80
            //(3)subTotalPiezasSinCampo // 30
            // (4)subTotalPiezasMultiples // 10

            //(4) = ((2)+(3)) - (1)
            //if (4) < 0 ? (4) * -1 : (4);



            subTotalPiezasMultiples = ((subTotalPiezasConCampo) + (subTotalPiezasSinCampo));
            subTotalPiezasMultiples = subTotalPiezasMultiples == 0 ? subTotalPiezasMultiples : subTotalPiezasMultiples - (totalPiezasObligatorias);
            subTotalPiezasMultiples = (subTotalPiezasMultiples) > 0 ? (subTotalPiezasMultiples) * -1 : (subTotalPiezasMultiples);


            /*Vista*/

            ViewBag.Titulo = tipoAtt.Nombre + subTitulo;

            ViewBag.totalPiezasObligatorias = totalPiezasObligatorias;
            ViewBag.subTotalPiezasConCampo = subTotalPiezasConCampo;
            ViewBag.subTotalPiezasSinCampo = subTotalPiezasSinCampo;
            ViewBag.subTotalPiezasMultiples = subTotalPiezasMultiples;


            //listas
            ViewBag.listaMaestra = lista;
            ViewBag.listaObraSinCampo = listaObraSinCampo;
            return PartialView("_ReporteLista");

        }

    }
}
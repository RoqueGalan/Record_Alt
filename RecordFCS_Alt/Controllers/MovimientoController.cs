using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RecordFCS_Alt.Models;
using PagedList;
using RecordFCS_Alt.Helpers.Seguridad;
using RecordFCS_Alt.Models.ViewsModel;

namespace RecordFCS_Alt.Controllers
{
    public class MovimientoController : Controller
    {
        private RecordFCSContext db = new RecordFCSContext();
                [CustomAuthorize(permiso = "")]
        // GET: Movimiento
        public ActionResult Index()
        {
            ViewBag.TipoMovimientoID = new SelectList(db.TipoMovimientos, "TipoMovimientoID", "Nombre");

            var mov = new Movimiento()
            {
                HaciaExposicion = false,
                FechaHoraMovimiento = DateTime.Now
            };

            return View(mov);
        }


        // GET: Movimiento/Crear
                [CustomAuthorize(permiso = "")]
        public ActionResult Crear(Guid? TipoMovimientoID, bool? HaciaExposicion)
        {
            bool HaciaExposicionValor = HaciaExposicion == null || HaciaExposicion == false ? false : true;

            bool esOK = true;

            var listaMovAceptados = new List<string>() { "externo", "ingreso", "salida", "traslado" };

            var tipoMov = db.TipoMovimientos.Find(TipoMovimientoID);

            esOK = tipoMov == null ? false : true;


            esOK = esOK ? listaMovAceptados.Any(a => a == tipoMov.Nombre.ToLower()) : false;


            if (esOK)
            {

                int UltimoFolioMov = db.Movimientos.Select(a => a.FolioMovimiento).OrderByDescending(a => a).FirstOrDefault();

                var mov = new Movimiento()
                {
                    EstadoMovimiento = EstadoMovimiento.EnRegistro,
                    FolioMovimiento = UltimoFolioMov + 1,
                    HaciaExposicion = HaciaExposicionValor,
                    TipoMovimiento = tipoMov,
                    TipoMovimientoID = tipoMov.TipoMovimientoID,
                    ColeccionTexto = "Museo Soumaya"
                };

                if (HaciaExposicionValor)
                {
                    mov.MovimientoExposicion = new MovimientoExposicion();
                    
                }

                switch (tipoMov.Nombre.ToLower())
                {
                    case "externo":
                        mov.MovimientoSolicitante = new MovimientoSolicitante();
                        mov.MovimientoAutorizacion = new MovimientoAutorizacion();
                        break;
                    case "ingreso":
                        mov.MovimientoSolicitante = new MovimientoSolicitante();
                        mov.MovimientoResponsable = new MovimientoResponsable();
                        mov.MovimientoAutorizacion = new MovimientoAutorizacion();
                        mov.MovimientoTransporte = new MovimientoTransporte();
                        mov.MovimientoSeguro = new MovimientoSeguro();
                        break;
                    case "salida":
                        mov.MovimientoSolicitante = new MovimientoSolicitante();
                        mov.MovimientoResponsable = new MovimientoResponsable();
                        mov.MovimientoAutorizacion = new MovimientoAutorizacion();
                        mov.MovimientoTransporte = new MovimientoTransporte();
                        mov.MovimientoSeguro = new MovimientoSeguro();
                        break;
                    case "traslado":
                        mov.MovimientoSolicitante = new MovimientoSolicitante();
                         mov.MovimientoAutorizacion = new MovimientoAutorizacion();
                        mov.MovimientoTransporte = new MovimientoTransporte();
                        break;

                    default:
                        //mov.MovimientoSolicitante = new MovimientoSolicitante();
                        //if (HaciaExposicion)
                        //    mov.MovimientoExposicion = new MovimientoExposicion();
                        //mov.MovimientoResponsable = new MovimientoResponsable();
                        //mov.MovimientoAutorizacion = new MovimientoAutorizacion();
                        //mov.MovimientoTransporte = new MovimientoTransporte();
                        //mov.MovimientoSeguro = new MovimientoSeguro();
                        esOK = false;
                        break;
                }


                if (esOK)
                {
                    mov.FechaRegistro = DateTime.Now;
                    mov.FechaHoraMovimiento = DateTime.Now;

                    if (HaciaExposicionValor)
                        mov.MovimientoExposicion.FechaInicial = DateTime.Now;
  

                    var listaUbicaciones = db.Ubicaciones.Where(a => a.Status).Select(a => new { a.Nombre, a.UbicacionID }).OrderBy(a => a.Nombre);
                    var listaUsuarios = db.Usuarios.Where(a => a.Status).Select(a => new { Nombre = a.Nombre + " " + a.Apellido, a.UsuarioID }).OrderBy(a => a.Nombre);

                    var tipoAttGuion = db.TipoAtributos.FirstOrDefault(a => a.Temp == "guion_clave");
                    var listaGuiones = tipoAttGuion.ListaValores.Where(a => a.Status).Select(a => new { Nombre = a.Valor, GuionID = a.ListaValorID }).OrderBy(a => a.Nombre);
                    var listaLetras = db.LetraFolios.Select(a => new { a.LetraFolioID, Nombre = a.Nombre, a.Status }).Where(a => a.Status).OrderBy(a => a.Nombre);


                    ViewBag.NombreMovimiento = tipoMov.Nombre.ToLower();

                    ViewBag.MovimientoAutorizacion_Usuario1ID = new SelectList(listaUsuarios, "UsuarioID", "Nombre");
                    ViewBag.MovimientoAutorizacion_Usuario2ID = new SelectList(listaUsuarios, "UsuarioID", "Nombre");


                    ViewBag.UbicacionDestinoID = new SelectList(listaUbicaciones, "UbicacionID", "Nombre");
                    ViewBag.UbicacionOrigenID = new SelectList(listaUbicaciones, "UbicacionID", "Nombre");

                    ViewBag.GuionID = new SelectList(listaGuiones, "GuionID", "Nombre");
                    ViewBag.LetraFolioID = new SelectList(listaLetras, "LetraFolioID", "Nombre", listaLetras.FirstOrDefault().LetraFolioID);

                    return PartialView("_Crear", mov);
                }
            }


            ViewBag.Mensaje = "Se encontro un error, solicite asistencia técnica.";
            return PartialView("_Error");
        }

        // POST: Movimiento/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "")]
        public ActionResult Create([Bind(Include = "MovientoID,FolioMovimiento,TipoMovimientoID,EstadoMovimiento,FechaRegistro,FechaSalida,FechaRet,UbicacionOrigenID,UbicacionDestinoID,FechaMovimiento,HoraMovimiento,MinutoMovimiento,ColeccionTexto,Observaciones,MovimientoSolicitanteID,MovimientoResponsableID,MovimientoExposicionID,MovimientoAutorizacionID,MovimientoTransporteID,MovimientoSeguroID")] Movimiento movimiento)
        {
            if (ModelState.IsValid)
            {
                movimiento.MovientoID = Guid.NewGuid();
                db.Movimientos.Add(movimiento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MovimientoAutorizacionID = new SelectList(db.MovimientoAutorizaciones, "MovimientoAutorizacionID", "FechaAutorizacion1", movimiento.MovimientoAutorizacionID);
            ViewBag.MovimientoExposicionID = new SelectList(db.MovimientoExposiciones, "MovimientoExposicionID", "Titulo", movimiento.MovimientoExposicionID);
            ViewBag.MovimientoResponsableID = new SelectList(db.MovimientoResponsables, "MovimientoResponsableID", "Nombre", movimiento.MovimientoResponsableID);
            ViewBag.MovimientoSeguroID = new SelectList(db.MovimientoSeguros, "MovimientoSeguroID", "AseguradorNombre", movimiento.MovimientoSeguroID);
            ViewBag.MovimientoSolicitanteID = new SelectList(db.MovimientoSolicitante, "MovimientoSolicitanteID", "Nombre", movimiento.MovimientoSolicitanteID);
            ViewBag.TipoMovimientoID = new SelectList(db.TipoMovimientos, "TipoMovimientoID", "Nombre", movimiento.TipoMovimientoID);
            ViewBag.MovimientoTransporteID = new SelectList(db.MovimientoTransporte, "MovimientoTransporteID", "EmpresaNombre", movimiento.MovimientoTransporteID);
            ViewBag.UbicacionDestinoID = new SelectList(db.Ubicaciones, "UbicacionID", "Nombre", movimiento.UbicacionDestinoID);
            ViewBag.UbicacionOrigenID = new SelectList(db.Ubicaciones, "UbicacionID", "Nombre", movimiento.UbicacionOrigenID);
            return View(movimiento);
        }

        // GET: Movimiento/Edit/5
                [CustomAuthorize(permiso = "")]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movimiento movimiento = db.Movimientos.Find(id);
            if (movimiento == null)
            {
                return HttpNotFound();
            }
            ViewBag.MovimientoAutorizacionID = new SelectList(db.MovimientoAutorizaciones, "MovimientoAutorizacionID", "FechaAutorizacion1", movimiento.MovimientoAutorizacionID);
            ViewBag.MovimientoExposicionID = new SelectList(db.MovimientoExposiciones, "MovimientoExposicionID", "Titulo", movimiento.MovimientoExposicionID);
            ViewBag.MovimientoResponsableID = new SelectList(db.MovimientoResponsables, "MovimientoResponsableID", "Nombre", movimiento.MovimientoResponsableID);
            ViewBag.MovimientoSeguroID = new SelectList(db.MovimientoSeguros, "MovimientoSeguroID", "AseguradorNombre", movimiento.MovimientoSeguroID);
            ViewBag.MovimientoSolicitanteID = new SelectList(db.MovimientoSolicitante, "MovimientoSolicitanteID", "Nombre", movimiento.MovimientoSolicitanteID);
            ViewBag.TipoMovimientoID = new SelectList(db.TipoMovimientos, "TipoMovimientoID", "Nombre", movimiento.TipoMovimientoID);
            ViewBag.MovimientoTransporteID = new SelectList(db.MovimientoTransporte, "MovimientoTransporteID", "EmpresaNombre", movimiento.MovimientoTransporteID);
            ViewBag.UbicacionDestinoID = new SelectList(db.Ubicaciones, "UbicacionID", "Nombre", movimiento.UbicacionDestinoID);
            ViewBag.UbicacionOrigenID = new SelectList(db.Ubicaciones, "UbicacionID", "Nombre", movimiento.UbicacionOrigenID);
            return View(movimiento);
        }

        // POST: Movimiento/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "")]
        public ActionResult Edit([Bind(Include = "MovientoID,FolioMovimiento,TipoMovimientoID,EstadoMovimiento,FechaRegistro,FechaSalida,FechaRet,UbicacionOrigenID,UbicacionDestinoID,FechaMovimiento,HoraMovimiento,MinutoMovimiento,ColeccionTexto,Observaciones,MovimientoSolicitanteID,MovimientoResponsableID,MovimientoExposicionID,MovimientoAutorizacionID,MovimientoTransporteID,MovimientoSeguroID")] Movimiento movimiento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movimiento).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MovimientoAutorizacionID = new SelectList(db.MovimientoAutorizaciones, "MovimientoAutorizacionID", "FechaAutorizacion1", movimiento.MovimientoAutorizacionID);
            ViewBag.MovimientoExposicionID = new SelectList(db.MovimientoExposiciones, "MovimientoExposicionID", "Titulo", movimiento.MovimientoExposicionID);
            ViewBag.MovimientoResponsableID = new SelectList(db.MovimientoResponsables, "MovimientoResponsableID", "Nombre", movimiento.MovimientoResponsableID);
            ViewBag.MovimientoSeguroID = new SelectList(db.MovimientoSeguros, "MovimientoSeguroID", "AseguradorNombre", movimiento.MovimientoSeguroID);
            ViewBag.MovimientoSolicitanteID = new SelectList(db.MovimientoSolicitante, "MovimientoSolicitanteID", "Nombre", movimiento.MovimientoSolicitanteID);
            ViewBag.TipoMovimientoID = new SelectList(db.TipoMovimientos, "TipoMovimientoID", "Nombre", movimiento.TipoMovimientoID);
            ViewBag.MovimientoTransporteID = new SelectList(db.MovimientoTransporte, "MovimientoTransporteID", "EmpresaNombre", movimiento.MovimientoTransporteID);
            ViewBag.UbicacionDestinoID = new SelectList(db.Ubicaciones, "UbicacionID", "Nombre", movimiento.UbicacionDestinoID);
            ViewBag.UbicacionOrigenID = new SelectList(db.Ubicaciones, "UbicacionID", "Nombre", movimiento.UbicacionOrigenID);
            return View(movimiento);
        }

        // GET: Movimiento/Delete/5
                [CustomAuthorize(permiso = "")]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movimiento movimiento = db.Movimientos.Find(id);
            if (movimiento == null)
            {
                return HttpNotFound();
            }
            return View(movimiento);
        }

        // POST: Movimiento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "")]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Movimiento movimiento = db.Movimientos.Find(id);
            db.Movimientos.Remove(movimiento);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

                [CustomAuthorize(permiso = "")]
        public ActionResult BuscarPiezas(int LetraFolioID, Guid? GuionID, string tipo = "", int? pagina = null, string claves = "")
        {
            int pagTamano = 5;
            int pagIndex = 1;
            pagIndex = pagina.HasValue ? Convert.ToInt32(pagina) : 1;

            List<Pieza> listaPiezas = new List<Pieza>();
            IPagedList<Guid> paginaPiezasIDs;

            if (tipo == "CargarGuion")
            {
                //GuionID = ListaValorID
                var listaValor = db.ListaValores.Find(GuionID);

                listaPiezas = listaPiezas.Where(a => a.AtributoPiezas.Any(b => b.ListaValor.TipoAtributoID == listaValor.TipoAtributo.TipoAtributoID && b.ListaValorID == listaValor.ListaValorID)).ToList();
            }
            else
            {
                var letra = db.LetraFolios.Find(LetraFolioID);

                listaPiezas = db.Piezas.Where(a => a.Obra.LetraFolioID == letra.LetraFolioID).ToList();

                if (!String.IsNullOrEmpty(claves))
                {
                    List<int> listaClaves = new List<int>();
                    string[] listaClavesFormTemp = claves.Split(',');

                    foreach (var clavesTemp in listaClavesFormTemp)
                    {
                        if (clavesTemp.Contains("-"))
                        {
                            string[] claveTemp = clavesTemp.Split('-');
                            var claveInicio = Convert.ToInt32(claveTemp[0]);
                            var claveFinal = Convert.ToInt32(claveTemp[1]);
                            int temp = 0;

                            if (claveInicio > claveFinal)
                            {
                                temp = claveInicio;
                                claveInicio = claveFinal;
                                claveFinal = temp;
                            }

                            for (int i = claveInicio; i <= claveFinal; i++)
                                listaClaves.Add(i);
                        }
                        else
                        {
                            listaClaves.Add(Convert.ToInt32(clavesTemp));
                        }
                    }
                    listaPiezas = listaPiezas.Where(a => a.Obra.LetraFolioID == letra.LetraFolioID && listaClaves.Contains(a.Obra.NumeroFolio)).ToList();
                }

            }

            listaPiezas = listaPiezas.Where(a => a.TipoPieza.EsPrincipal).OrderBy(a => a.Obra.LetraFolio.Nombre).ThenBy(a => a.Obra.NumeroFolio).ToList();
            paginaPiezasIDs = listaPiezas.Select(x => x.PiezaID).ToList().ToPagedList(pagIndex, pagTamano);

            ViewBag.tipo = tipo;
            return PartialView("_ResultadosBusqueda", paginaPiezasIDs);
        }

        [CustomAuthorize(permiso = "")]
                public ActionResult FichaMini(Guid? id, string tipo = "guion", string listaNombre = "listaTemp")
        {
            //var lista = Session[listaNombre] == null ? new List<itemPiezaMini>() : (List<itemPiezaMini>)Session[listaNombre];

            string tipoCarusel = "miniThumb";
            string vista = "_FichaMini";

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Pieza pieza = db.Piezas.Find(id);

            if (pieza == null)
                return HttpNotFound();

            //extraer los campos del tipo de obra

            switch (tipo.ToLower())
            {
                case "guion":
                    tipo = "Guion";
                    tipoCarusel = "miniThumb";
                    vista = "_FichaMini";
                    break;
            }


            var listaAttributosFichaCompleta = pieza.TipoPieza.Atributos.Where(a => a.Status && a.MostrarAtributos.Any(b => b.TipoMostrar.Nombre == tipo && b.Status) && a.TipoAtributo.Status).OrderBy(a => a.Orden).ToList();

            pieza.TipoPieza.TipoPiezasHijas = pieza.TipoPieza.TipoPiezasHijas.Where(a => a.Status).OrderBy(a => a.Orden).ToList();

            ViewBag.listaAttributosFichaCompleta = listaAttributosFichaCompleta;


            ViewBag.TipoFicha = tipo;
            ViewBag.tipoCarusel = tipoCarusel;
            ViewBag.listaNombre = listaNombre;


            pieza.PiezasHijas = pieza.PiezasHijas.OrderBy(a => a.SubFolio).ToList();



            return PartialView(vista, pieza);
        }


        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

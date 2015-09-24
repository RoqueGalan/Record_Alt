using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RecordFCS_Alt.Models;

namespace RecordFCS_Alt.Controllers
{
    public class MovimientoController : Controller
    {
        private RecordFCSContext db = new RecordFCSContext();

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

                int UltimoFolioMov  = db.Movimientos.Select(a=> a.FolioMovimiento).OrderByDescending(a=> a).FirstOrDefault();

                var mov = new Movimiento()
                {
                    EstadoMovimiento = EstadoMovimiento.EnRegistro,
                    FolioMovimiento = UltimoFolioMov + 1,
                    HaciaExposicion = HaciaExposicionValor,
                    TipoMovimiento = tipoMov,
                    TipoMovimientoID = tipoMov.TipoMovimientoID,
                    ColeccionTexto = "Museo Soumaya"
                };


                switch (tipoMov.Nombre.ToLower())
                {
                    case "externo":
                        mov.MovimientoSolicitante = new MovimientoSolicitante();
                        if (HaciaExposicionValor)
                            mov.MovimientoExposicion = new MovimientoExposicion();
                        mov.MovimientoAutorizacion = new MovimientoAutorizacion();
                        break;
                    case "ingreso":
                        mov.MovimientoSolicitante = new MovimientoSolicitante();
                        if (HaciaExposicionValor)
                            mov.MovimientoExposicion = new MovimientoExposicion();
                        mov.MovimientoResponsable = new MovimientoResponsable();
                        mov.MovimientoAutorizacion = new MovimientoAutorizacion();
                        mov.MovimientoTransporte = new MovimientoTransporte();
                        mov.MovimientoSeguro = new MovimientoSeguro();
                        break;
                    case "salida":
                        mov.MovimientoSolicitante = new MovimientoSolicitante();
                        if (HaciaExposicionValor)
                            mov.MovimientoExposicion = new MovimientoExposicion();
                        mov.MovimientoResponsable = new MovimientoResponsable();
                        mov.MovimientoAutorizacion = new MovimientoAutorizacion();
                        mov.MovimientoTransporte = new MovimientoTransporte();
                        mov.MovimientoSeguro = new MovimientoSeguro();
                        break;
                    case "traslado":
                        mov.MovimientoSolicitante = new MovimientoSolicitante();
                        if (HaciaExposicionValor)
                            mov.MovimientoExposicion = new MovimientoExposicion();
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

                    var listaUbicaciones = db.Ubicaciones.Where(a => a.Status).Select(a => new { a.Nombre, a.UbicacionID }).OrderBy(a => a.Nombre);


                    ViewBag.NombreMovimiento = tipoMov.Nombre.ToLower();

                    ViewBag.UbicacionDestinoID = new SelectList(listaUbicaciones, "UbicacionID", "Nombre");
                    ViewBag.UbicacionOrigenID = new SelectList(listaUbicaciones, "UbicacionID", "Nombre");

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
        public ActionResult DeleteConfirmed(Guid id)
        {
            Movimiento movimiento = db.Movimientos.Find(id);
            db.Movimientos.Remove(movimiento);
            db.SaveChanges();
            return RedirectToAction("Index");
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

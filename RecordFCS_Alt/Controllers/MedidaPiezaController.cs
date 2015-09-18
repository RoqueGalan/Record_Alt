using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RecordFCS_Alt.Models;
using RecordFCS_Alt.Helpers.Seguridad;

namespace RecordFCS_Alt.Controllers
{
    public class MedidaPiezaController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // POST: MedidaPieza/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "attPNew")]
        public ActionResult Crear([Bind(Include = "PiezaID,TipoMedidaID,Altura,Anchura,Profundidad,Diametro,Diametro2,UMLongitud,Peso,UMMasa,Otra,Status,Temp")] MedidaPieza medidaPieza, Guid AtributoID)
        {
            string renderID = "tipoMedida_" + medidaPieza.PiezaID + "_";

            var tipoMedida = db.TipoMedidas.Find(medidaPieza.TipoMedidaID);
            string texto = "";
            if (ModelState.IsValid)
            {
                db.MedidaPiezas.Add(medidaPieza);
                db.SaveChanges();

                AlertaSuccess(string.Format("Médida: <b>{0}</b> se agregó.", tipoMedida.Nombre), true);

                return Json(new { success = true, renderID = renderID, texto = texto, guardar = true });

            }

            var pieza = db.Piezas.Find(medidaPieza.PiezaID);
            var tipoMedidasExistentes = pieza.MedidaPiezas.Select(a => a.TipoMedidaID);

            ViewBag.TipoMedidaID = new SelectList(db.TipoMedidas.Where(a => a.Status && !tipoMedidasExistentes.Contains(a.TipoMedidaID)).OrderBy(a => a.Nombre).ToList(), "TipoMedidaID", "Nombre", medidaPieza.TipoMedidaID);
            return PartialView("_Crear", medidaPieza);
        }


        // POST: MedidaPieza/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "attPEdit")]
        public ActionResult Editar(MedidaPieza medidaPieza, Guid AtributoID, Guid LlaveID)
        {
            string renderID = "tipoMedida_" + medidaPieza.PiezaID + "_" + LlaveID;

            string texto = "";
            bool guardar = false;
            var tipoMedida = db.TipoMedidas.Find(medidaPieza.TipoMedidaID);


            if (ModelState.IsValid)
            {
                guardar = true;
                db.Entry(medidaPieza).State = EntityState.Modified;
                db.SaveChanges();

                AlertaSuccess(string.Format("Médida: <b>{0}</b> se actualizo.", tipoMedida.Nombre), true);

                //realizar logica de mostrar medidas
                texto = "Falta implementar la logica de mostrar medidas";
                return Json(new { success = true, renderID = renderID, texto = texto, guardar = guardar, tipo = "Autor" });
            }

            ViewBag.NombreMedida = tipoMedida.Nombre;
            return PartialView("_Editar", medidaPieza);
        }

        // GET: MedidaPieza/Delete/5
        [CustomAuthorize(permiso = "attPDel")]
        public ActionResult Eliminar(Guid? id, Guid? TipoMedidaID)
        {
            if (id == null && TipoMedidaID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedidaPieza medidaPieza = db.MedidaPiezas.Find(id, TipoMedidaID);
            if (medidaPieza == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Eliminar", medidaPieza);
        }

        // POST: MedidaPieza/Delete/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "attPDel")]
        public ActionResult EliminarConfirmado(Guid id, Guid TipoMedidaID)
        {
            string btnValue = Request.Form["accionx"];

            MedidaPieza medidaPieza = db.MedidaPiezas.Find(id, TipoMedidaID);

            var NombreTexto = medidaPieza.TipoMedida.Nombre;

            switch (btnValue)
            {
                case "eliminar":
                    db.MedidaPiezas.Remove(medidaPieza);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", NombreTexto), true);
                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;
            }

            return Json(new { success = true, guardar = false });
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

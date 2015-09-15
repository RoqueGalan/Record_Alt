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
    public class TecnicaPiezaController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();


        // POST: TecnicaPieza/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear([Bind(Include = "PiezaID,TipoTecnicaID,Status,TecnicaID")] TecnicaPieza tecnicaPieza, Guid AtributoID)
        {
            string renderID = "tipoTecnica_" + tecnicaPieza.PiezaID + "_";
            string texto = "";
            bool guardar = false;
            string valor = Request.Form["id_" + AtributoID].ToString();

            tecnicaPieza.TecnicaID = new Guid(valor);

            if (db.TecnicaPiezas.Where(a => a.PiezaID == tecnicaPieza.PiezaID && a.TecnicaID == tecnicaPieza.TecnicaID).Count() == 0)
            {
                guardar = true;

                var tecnica = db.Tecnicas.Find(tecnicaPieza.TecnicaID);

                texto = tecnica.Descripcion;

                AlertaSuccess(string.Format("Técnica: <b>{0}</b> se agregó.", tecnica.Descripcion), true);
                tecnicaPieza.TecnicaID = tecnica.TecnicaID;

            }
            else
            {
                guardar = false;
                //alerta ya existe
            }




            if (guardar)
            {
                db.TecnicaPiezas.Add(tecnicaPieza);
                db.SaveChanges();

                renderID += tecnicaPieza.TecnicaID;
            }

            return Json(new { success = true, renderID = renderID, texto = texto, guardar = guardar });

        }


        // POST: TecnicaPieza/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "PiezaID,TipoTecnicaID,Status,TecnicaID")] TecnicaPieza tecnicaPieza)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tecnicaPieza).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PiezaID = new SelectList(db.Piezas, "PiezaID", "SubFolio", tecnicaPieza.PiezaID);
            ViewBag.TecnicaID = new SelectList(db.Tecnicas, "TecnicaID", "ClaveSigla", tecnicaPieza.TecnicaID);
            ViewBag.TipoTecnicaID = new SelectList(db.TipoTecnicas, "TipoTecnicaID", "Nombre", tecnicaPieza.TipoTecnicaID);
            return View(tecnicaPieza);
        }

        // GET: TecnicaPieza/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TecnicaPieza tecnicaPieza = db.TecnicaPiezas.Find(id);
            if (tecnicaPieza == null)
            {
                return HttpNotFound();
            }
            return View(tecnicaPieza);
        }

        // POST: TecnicaPieza/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            TecnicaPieza tecnicaPieza = db.TecnicaPiezas.Find(id);
            db.TecnicaPiezas.Remove(tecnicaPieza);
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

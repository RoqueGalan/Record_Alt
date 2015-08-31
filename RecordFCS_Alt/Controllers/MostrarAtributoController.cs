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
    public class MostrarAtributoController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: MostrarAtributo/Editar/5
        public ActionResult Editar(Guid? id, Guid? AtributoID)
        {
            if (id == null || AtributoID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MostrarAtributo mostrarAtributo = db.MostrarAtributos.Find(id, AtributoID);
            if (mostrarAtributo == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Editar", mostrarAtributo);
        }

        // POST: MostrarAtributo/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "TipoMostrarID,AtributoID,Orden,InicioHTML,FinHTML,Status")] MostrarAtributo mostrarAtributo)
        {
            if (ModelState.IsValid)
            {

                db.Entry(mostrarAtributo).State = EntityState.Modified;
                db.SaveChanges();

                var TipoPiezaID = db.Atributos.FirstOrDefault(a => a.AtributoID == mostrarAtributo.AtributoID).TipoPiezaID;
                var Nombre = db.TipoMostarlos.FirstOrDefault(a => a.TipoMostrarID == mostrarAtributo.TipoMostrarID).Nombre;


                AlertaInfo(string.Format("Mostrar en: <b>{0}</b> editado.", Nombre), true);

                string url = Url.Action("Lista", "Atributo", new { id = TipoPiezaID });

                return Json(new { success = true, url = url });
            }

            return PartialView("_Editar", mostrarAtributo);
        }

        // GET: MostrarAtributo/Eliminar/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MostrarAtributo mostrarAtributo = db.MostrarAtributos.Find(id);
            if (mostrarAtributo == null)
            {
                return HttpNotFound();
            }
            return View(mostrarAtributo);
        }

        // POST: MostrarAtributo/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            MostrarAtributo mostrarAtributo = db.MostrarAtributos.Find(id);
            db.MostrarAtributos.Remove(mostrarAtributo);
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

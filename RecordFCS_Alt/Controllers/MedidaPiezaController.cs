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
    public class MedidaPiezaController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: MedidaPieza
        public ActionResult Index()
        {
            var medidaPiezas = db.MedidaPiezas.Include(m => m.Pieza).Include(m => m.TipoMedida);
            return View(medidaPiezas.ToList());
        }

        // GET: MedidaPieza/Detalles/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedidaPieza medidaPieza = db.MedidaPiezas.Find(id);
            if (medidaPieza == null)
            {
                return HttpNotFound();
            }
            return View(medidaPieza);
        }

        // GET: MedidaPieza/Create
        public ActionResult Create()
        {
            ViewBag.PiezaID = new SelectList(db.Piezas, "PiezaID", "SubFolio");
            ViewBag.TipoMedidaID = new SelectList(db.TipoMedidas, "TipoMedidaID", "Nombre");
            return View();
        }

        // POST: MedidaPieza/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PiezaID,TipoMedidaID,Altura,Anchura,Profundidad,Diametro,Diametro2,UMLongitud,Peso,UMMasa,Otra,Status,Temp")] MedidaPieza medidaPieza)
        {
            if (ModelState.IsValid)
            {
                medidaPieza.PiezaID = Guid.NewGuid();
                db.MedidaPiezas.Add(medidaPieza);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PiezaID = new SelectList(db.Piezas, "PiezaID", "SubFolio", medidaPieza.PiezaID);
            ViewBag.TipoMedidaID = new SelectList(db.TipoMedidas, "TipoMedidaID", "Nombre", medidaPieza.TipoMedidaID);
            return View(medidaPieza);
        }

        // GET: MedidaPieza/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedidaPieza medidaPieza = db.MedidaPiezas.Find(id);
            if (medidaPieza == null)
            {
                return HttpNotFound();
            }
            ViewBag.PiezaID = new SelectList(db.Piezas, "PiezaID", "SubFolio", medidaPieza.PiezaID);
            ViewBag.TipoMedidaID = new SelectList(db.TipoMedidas, "TipoMedidaID", "Nombre", medidaPieza.TipoMedidaID);
            return View(medidaPieza);
        }

        // POST: MedidaPieza/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PiezaID,TipoMedidaID,Altura,Anchura,Profundidad,Diametro,Diametro2,UMLongitud,Peso,UMMasa,Otra,Status,Temp")] MedidaPieza medidaPieza)
        {
            if (ModelState.IsValid)
            {
                db.Entry(medidaPieza).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PiezaID = new SelectList(db.Piezas, "PiezaID", "SubFolio", medidaPieza.PiezaID);
            ViewBag.TipoMedidaID = new SelectList(db.TipoMedidas, "TipoMedidaID", "Nombre", medidaPieza.TipoMedidaID);
            return View(medidaPieza);
        }

        // GET: MedidaPieza/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedidaPieza medidaPieza = db.MedidaPiezas.Find(id);
            if (medidaPieza == null)
            {
                return HttpNotFound();
            }
            return View(medidaPieza);
        }

        // POST: MedidaPieza/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            MedidaPieza medidaPieza = db.MedidaPiezas.Find(id);
            db.MedidaPiezas.Remove(medidaPieza);
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

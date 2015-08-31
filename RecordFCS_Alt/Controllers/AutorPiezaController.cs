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
    public class AutorPiezaController : Controller
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: AutorPieza/Detalles/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AutorPieza autorPieza = db.AutorPiezas.Find(id);
            if (autorPieza == null)
            {
                return HttpNotFound();
            }
            return View(autorPieza);
        }

        // GET: AutorPieza/Create
        public ActionResult Create()
        {
            ViewBag.AutorID = new SelectList(db.Autores, "AutorID", "Nombre");
            ViewBag.PiezaID = new SelectList(db.Piezas, "PiezaID", "SubFolio");
            return View();
        }

        // POST: AutorPieza/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PiezaID,AutorID,Status")] AutorPieza autorPieza)
        {
            if (ModelState.IsValid)
            {
                autorPieza.PiezaID = Guid.NewGuid();
                db.AutorPiezas.Add(autorPieza);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AutorID = new SelectList(db.Autores, "AutorID", "Nombre", autorPieza.AutorID);
            ViewBag.PiezaID = new SelectList(db.Piezas, "PiezaID", "SubFolio", autorPieza.PiezaID);
            return View(autorPieza);
        }

        // GET: AutorPieza/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AutorPieza autorPieza = db.AutorPiezas.Find(id);
            if (autorPieza == null)
            {
                return HttpNotFound();
            }
            ViewBag.AutorID = new SelectList(db.Autores, "AutorID", "Nombre", autorPieza.AutorID);
            ViewBag.PiezaID = new SelectList(db.Piezas, "PiezaID", "SubFolio", autorPieza.PiezaID);
            return View(autorPieza);
        }

        // POST: AutorPieza/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PiezaID,AutorID,Status")] AutorPieza autorPieza)
        {
            if (ModelState.IsValid)
            {
                db.Entry(autorPieza).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AutorID = new SelectList(db.Autores, "AutorID", "Nombre", autorPieza.AutorID);
            ViewBag.PiezaID = new SelectList(db.Piezas, "PiezaID", "SubFolio", autorPieza.PiezaID);
            return View(autorPieza);
        }

        // GET: AutorPieza/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AutorPieza autorPieza = db.AutorPiezas.Find(id);
            if (autorPieza == null)
            {
                return HttpNotFound();
            }
            return View(autorPieza);
        }

        // POST: AutorPieza/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            AutorPieza autorPieza = db.AutorPiezas.Find(id);
            db.AutorPiezas.Remove(autorPieza);
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

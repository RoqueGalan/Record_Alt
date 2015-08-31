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
    public class ImagenPiezaController : Controller
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: ImagenPieza
        public ActionResult Index()
        {
            var imagenPiezas = db.ImagenPiezas.Include(i => i.Pieza);
            return View(imagenPiezas.ToList());
        }

        // GET: ImagenPieza/Detalles/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ImagenPieza imagenPieza = db.ImagenPiezas.Find(id);
            if (imagenPieza == null)
            {
                return HttpNotFound();
            }
            return View(imagenPieza);
        }

        // GET: ImagenPieza/Create
        public ActionResult Create()
        {
            ViewBag.PiezaID = new SelectList(db.Piezas, "PiezaID", "SubFolio");
            return View();
        }

        // POST: ImagenPieza/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ImagenPiezaID,Orden,Titulo,Descripcion,EsPrincipal,RutaParcial,NombreImagen,Status,PiezaID")] ImagenPieza imagenPieza)
        {
            if (ModelState.IsValid)
            {
                imagenPieza.ImagenPiezaID = Guid.NewGuid();
                db.ImagenPiezas.Add(imagenPieza);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PiezaID = new SelectList(db.Piezas, "PiezaID", "SubFolio", imagenPieza.PiezaID);
            return View(imagenPieza);
        }

        // GET: ImagenPieza/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ImagenPieza imagenPieza = db.ImagenPiezas.Find(id);
            if (imagenPieza == null)
            {
                return HttpNotFound();
            }
            ViewBag.PiezaID = new SelectList(db.Piezas, "PiezaID", "SubFolio", imagenPieza.PiezaID);
            return View(imagenPieza);
        }

        // POST: ImagenPieza/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ImagenPiezaID,Orden,Titulo,Descripcion,EsPrincipal,RutaParcial,NombreImagen,Status,PiezaID")] ImagenPieza imagenPieza)
        {
            if (ModelState.IsValid)
            {
                db.Entry(imagenPieza).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PiezaID = new SelectList(db.Piezas, "PiezaID", "SubFolio", imagenPieza.PiezaID);
            return View(imagenPieza);
        }

        // GET: ImagenPieza/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ImagenPieza imagenPieza = db.ImagenPiezas.Find(id);
            if (imagenPieza == null)
            {
                return HttpNotFound();
            }
            return View(imagenPieza);
        }

        // POST: ImagenPieza/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ImagenPieza imagenPieza = db.ImagenPiezas.Find(id);
            db.ImagenPiezas.Remove(imagenPieza);
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

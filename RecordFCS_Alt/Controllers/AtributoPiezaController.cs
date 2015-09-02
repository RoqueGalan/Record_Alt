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
    public class AtributoPiezaController : Controller
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: AtributoPieza
        public ActionResult Index()
        {
            var atributoPiezas = db.AtributoPiezas.Include(a => a.Atributo).Include(a => a.ListaValor).Include(a => a.Pieza);
            return View(atributoPiezas.ToList());
        }

        // GET: AtributoPieza/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AtributoPieza atributoPieza = db.AtributoPiezas.Find(id);
            if (atributoPieza == null)
            {
                return HttpNotFound();
            }
            return View(atributoPieza);
        }

        // GET: AtributoPieza/Create
        public ActionResult Crear(Guid? id)
        {
            // AtributoPiezaID = id




            ViewBag.AtributoID = new SelectList(db.Atributos, "AtributoID", "NombreAlterno");
            ViewBag.ListaValorID = new SelectList(db.ListaValores, "ListaValorID", "Valor");
            ViewBag.PiezaID = new SelectList(db.Piezas, "PiezaID", "SubFolio");
            return PartialView("_Crear");
        }

        // POST: AtributoPieza/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AtributoPiezaID,Valor,Status,PiezaID,AtributoID,ListaValorID")] AtributoPieza atributoPieza)
        {
            if (ModelState.IsValid)
            {
                atributoPieza.AtributoPiezaID = Guid.NewGuid();
                db.AtributoPiezas.Add(atributoPieza);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AtributoID = new SelectList(db.Atributos, "AtributoID", "NombreAlterno", atributoPieza.AtributoID);
            ViewBag.ListaValorID = new SelectList(db.ListaValores, "ListaValorID", "Valor", atributoPieza.ListaValorID);
            ViewBag.PiezaID = new SelectList(db.Piezas, "PiezaID", "SubFolio", atributoPieza.PiezaID);
            return View(atributoPieza);
        }

        // GET: AtributoPieza/Edit/5
        public ActionResult Editar(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AtributoPieza atributoPieza = db.AtributoPiezas.Find(id);
            
            if (atributoPieza == null)
            {
                return HttpNotFound();
            }

            if (atributoPieza.Atributo.TipoAtributo.EsLista)
            {
             
                //ViewBag.ListaValorID = new SelectList(att)
            }


            return PartialView("_Editar",atributoPieza);
        }

        // POST: AtributoPieza/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AtributoPiezaID,Valor,Status,PiezaID,AtributoID,ListaValorID")] AtributoPieza atributoPieza)
        {
            if (ModelState.IsValid)
            {
                db.Entry(atributoPieza).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AtributoID = new SelectList(db.Atributos, "AtributoID", "NombreAlterno", atributoPieza.AtributoID);
            ViewBag.ListaValorID = new SelectList(db.ListaValores, "ListaValorID", "Valor", atributoPieza.ListaValorID);
            ViewBag.PiezaID = new SelectList(db.Piezas, "PiezaID", "SubFolio", atributoPieza.PiezaID);
            return View(atributoPieza);
        }

        // GET: AtributoPieza/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AtributoPieza atributoPieza = db.AtributoPiezas.Find(id);
            if (atributoPieza == null)
            {
                return HttpNotFound();
            }
            return View(atributoPieza);
        }

        // POST: AtributoPieza/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            AtributoPieza atributoPieza = db.AtributoPiezas.Find(id);
            db.AtributoPiezas.Remove(atributoPieza);
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

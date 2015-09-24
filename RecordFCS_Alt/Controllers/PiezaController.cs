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
    public class PiezaController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();


        // GET: Pieza/Detalles/5
        [CustomAuthorize(permiso = "")]

        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pieza pieza = db.Piezas.Find(id);
            if (pieza == null)
            {
                return HttpNotFound();
            }
            return View(pieza);
        }

        // GET: Pieza/Crear
        [CustomAuthorize(permiso = "")]

        public ActionResult Create()
        {
            ViewBag.ObraID = new SelectList(db.Obras, "ObraID", "Temp");
            ViewBag.PiezaPadreID = new SelectList(db.Piezas, "PiezaID", "SubFolio");
            ViewBag.TipoPiezaID = new SelectList(db.TipoPiezas, "TipoPiezaID", "Nombre");
            ViewBag.UbicacionID = new SelectList(db.Ubicaciones, "UbicacionID", "Nombre");
            return View();
        }

        // POST: Pieza/Crear
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "")]

        public ActionResult Create([Bind(Include = "PiezaID,SubFolio,FechaRegistro,Status,ObraID,TipoPiezaID,UbicacionID,PiezaPadreID,Temp")] Pieza pieza)
        {
            if (ModelState.IsValid)
            {
                pieza.PiezaID = Guid.NewGuid();
                db.Piezas.Add(pieza);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ObraID = new SelectList(db.Obras, "ObraID", "Temp", pieza.ObraID);
            ViewBag.PiezaPadreID = new SelectList(db.Piezas, "PiezaID", "SubFolio", pieza.PiezaPadreID);
            ViewBag.TipoPiezaID = new SelectList(db.TipoPiezas, "TipoPiezaID", "Nombre", pieza.TipoPiezaID);
            ViewBag.UbicacionID = new SelectList(db.Ubicaciones, "UbicacionID", "Nombre", pieza.UbicacionID);
            return View(pieza);
        }

        // GET: Pieza/Editar/5
        [CustomAuthorize(permiso = "")]

        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pieza pieza = db.Piezas.Find(id);
            if (pieza == null)
            {
                return HttpNotFound();
            }
            ViewBag.ObraID = new SelectList(db.Obras, "ObraID", "Temp", pieza.ObraID);
            ViewBag.PiezaPadreID = new SelectList(db.Piezas, "PiezaID", "SubFolio", pieza.PiezaPadreID);
            ViewBag.TipoPiezaID = new SelectList(db.TipoPiezas, "TipoPiezaID", "Nombre", pieza.TipoPiezaID);
            ViewBag.UbicacionID = new SelectList(db.Ubicaciones, "UbicacionID", "Nombre", pieza.UbicacionID);
            return View(pieza);
        }

        // POST: Pieza/Editar/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "")]

        public ActionResult Edit([Bind(Include = "PiezaID,SubFolio,FechaRegistro,Status,ObraID,TipoPiezaID,UbicacionID,PiezaPadreID,Temp")] Pieza pieza)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pieza).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ObraID = new SelectList(db.Obras, "ObraID", "Temp", pieza.ObraID);
            ViewBag.PiezaPadreID = new SelectList(db.Piezas, "PiezaID", "SubFolio", pieza.PiezaPadreID);
            ViewBag.TipoPiezaID = new SelectList(db.TipoPiezas, "TipoPiezaID", "Nombre", pieza.TipoPiezaID);
            ViewBag.UbicacionID = new SelectList(db.Ubicaciones, "UbicacionID", "Nombre", pieza.UbicacionID);
            return View(pieza);
        }

        // GET: Pieza/Eliminar/5
        [CustomAuthorize(permiso = "")]

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pieza pieza = db.Piezas.Find(id);
            if (pieza == null)
            {
                return HttpNotFound();
            }
            return View(pieza);
        }

        // POST: Pieza/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "")]

        public ActionResult DeleteConfirmed(Guid id)
        {
            Pieza pieza = db.Piezas.Find(id);
            db.Piezas.Remove(pieza);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [CustomAuthorize(permiso = "")]

        public ActionResult Ficha(Guid? id, string tipo = "basica")
        {

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Pieza pieza = db.Piezas.Find(id);

            if (pieza == null)
                return HttpNotFound();

            //extraer los campos del tipo de obra
            tipo = tipo.ToLower();
            tipo = tipo == "completa" ? "Ficha Completa" : tipo == "basica" ? "Ficha Básica" : "Ficha Básica";

            var listaAttributosFichaCompleta = pieza.TipoPieza.Atributos.Where(a => a.Status && a.MostrarAtributos.Any(b => b.TipoMostrar.Nombre == tipo && b.Status) && a.TipoAtributo.Status).OrderBy(a => a.Orden).ToList();

            ViewBag.listaAttributosFichaCompleta = listaAttributosFichaCompleta;


            ViewBag.TipoFicha = tipo;

            ViewBag.esCompleta = tipo == "Ficha Completa" ? true : false;

            return PartialView("_Ficha", pieza);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "attUbiEdit")]
        public ActionResult CrearUbicacion([Bind(Include = "PiezaID,SubFolio,FechaRegistro,Status,ObraID,TipoPiezaID,UbicacionID,PiezaPadreID,Temp")] Pieza pieza, Guid AtributoID)
        {
            string renderID = "ubicacion_" + pieza.PiezaID + "_";

            string texto = "";
            bool guardar = false;

            string valor = Request.Form["id_" + AtributoID].ToString();

            var ubicacion = db.Ubicaciones.Find(new Guid(valor));

            if (ubicacion != null)
            {
                texto = ubicacion.Nombre;
                pieza.UbicacionID = ubicacion.UbicacionID;
            }

            if (ModelState.IsValid)
            {
                guardar = true;
                db.Entry(pieza).State = EntityState.Modified;
                db.SaveChanges();

                renderID += ubicacion.UbicacionID;
                texto = ubicacion.Nombre;
                
                AlertaSuccess(string.Format("{0}: <b>{1}</b> se creó.", "Ubicación",ubicacion.Nombre), true);

                return Json(new { success = true, renderID = renderID, texto = texto, guardar = guardar });

            }

            ViewBag.UbicacionID = new SelectList(db.Ubicaciones.Where(a => a.Status).OrderBy(a => a.Nombre).Take(100).ToList(), "UbicacionID", "Nombre", pieza.UbicacionID);

            return PartialView("_CrearUbicacion", pieza);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "attUbiEdit")]
        public ActionResult EditarUbicacion([Bind(Include = "PiezaID,SubFolio,FechaRegistro,Status,ObraID,TipoPiezaID,UbicacionID,PiezaPadreID,Temp")] Pieza pieza, Guid AtributoID, Guid LlaveID)
        {
            //validar errores y devolverlos a la vista
            string renderID = "ubicacion_" + pieza.PiezaID + "_" + LlaveID;

            string texto = "";
            bool guardar = false;


            var ubicacionAnterior = db.Ubicaciones.Find(LlaveID);

            if (ubicacionAnterior == null)
            {
                guardar = false;
            }
            else
            {


                string valor = Request.Form["id_" + AtributoID].ToString();
                var ubicacion = db.Ubicaciones.Find(new Guid(valor));

                if (ubicacion != null)
                {
                    texto = ubicacion.Nombre;
                    pieza.UbicacionID = ubicacion.UbicacionID;
                }

                if (ModelState.IsValid)
                {
                    guardar = true;
                    db.Entry(pieza).State = EntityState.Modified;
                    db.SaveChanges();

                    renderID += ubicacion.UbicacionID;

                    texto = ubicacion.Nombre;
                    AlertaSuccess(string.Format("{0}: <b>{1}</b> se actualizo a <b>{2}</b>.", "Ubicación", ubicacionAnterior.Nombre, ubicacion.Nombre), true);


                    return Json(new { success = true, renderID = renderID, texto = texto, guardar = guardar });
                }
            }





            ViewBag.UbicacionID = new SelectList(db.Ubicaciones.Where(a => a.Status).OrderBy(a => a.Nombre).Take(100).ToList(), "UbicacionID", "Nombre", pieza.UbicacionID);

            return PartialView("_EditarUbicacion", pieza);
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

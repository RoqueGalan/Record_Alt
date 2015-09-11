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
    public class TipoObraController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: TipoObra
        [CustomAuthorize(permiso = "")]
        public ActionResult Index()
        {
            return View();
        }

        [CustomAuthorize(permiso = "")]
        public ActionResult Lista()
        {
            var lista = db.TipoObras.OrderBy(a => a.Nombre).ToList();

            ViewBag.totalRegistros = lista.Count;

            return PartialView("_Lista", lista);
        }

        // GET: TipoObra/Detalles/5
        [CustomAuthorize(permiso = "")]
        public ActionResult Detalles(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoObra tipoObra = db.TipoObras.Find(id);
            if (tipoObra == null)
            {
                return HttpNotFound();
            }

            return View(tipoObra);
        }

        // GET: TipoObra/Crear
        [CustomAuthorize(permiso = "")]
        public ActionResult Crear()
        {
            var tipoObra = new TipoObra()
            {
                Status = true
            };
            return PartialView("_Crear", tipoObra);
        }

        // POST: TipoObra/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "")]
        public ActionResult Crear([Bind(Include = "TipoObraID,Nombre,Descripcion,Status,Temp")] TipoObra tipoObra)
        {
            //revalidar el nombre
            if (db.TipoObras.FirstOrDefault(a => a.Nombre == tipoObra.Nombre) != null)
                ModelState.AddModelError("Nombre", "Nombre ya existe.");

            if (ModelState.IsValid)
            {
                tipoObra.TipoObraID = Guid.NewGuid();
                db.TipoObras.Add(tipoObra);
                db.SaveChanges();

                AlertaSuccess(string.Format("Tipo de Obra: <b>{0}</b> creada.", tipoObra.Nombre), true);

                //Crear la pieza Maestra

                TipoPieza tipoPieza = new TipoPieza()
                {
                    TipoPiezaID = Guid.NewGuid(),
                    TipoObraID = tipoObra.TipoObraID,
                    Nombre = "Principal",
                    Prefijo = "A",
                    Orden = 1,
                    Descripcion = "Pieza principal de la Obra",
                    EsPrincipal = true,
                    Status = true
                };

                db.TipoPiezas.Add(tipoPieza);
                db.SaveChanges();

                AlertaSuccess(string.Format("Tipo de Pieza : <b>{0}</b> creada.", tipoPieza.Nombre), true);

                string url = Url.Action("Lista", "TipoObra");
                return Json(new { success = true, url = url });
            }

            return PartialView("_Crear", tipoObra);
        }

        // GET: TipoObra/Editar/5
        [CustomAuthorize(permiso = "")]
        public ActionResult Editar(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoObra tipoObra = db.TipoObras.Find(id);
            if (tipoObra == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Editar", tipoObra);
        }

        // POST: TipoObra/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "")]
        public ActionResult Editar([Bind(Include = "TipoObraID,Nombre,Descripcion,Status,Temp")] TipoObra tipoObra)
        {
            //revalidar el nombre
            var tp = db.TipoObras.Select(a => new { a.TipoObraID, a.Nombre }).SingleOrDefault(a => a.Nombre == tipoObra.Nombre);

            if (tp != null)
                if (tp.TipoObraID != tipoObra.TipoObraID)
                    ModelState.AddModelError("Nombre", "Nombre ya existe.");


            if (ModelState.IsValid)
            {
                db.Entry(tipoObra).State = EntityState.Modified;
                db.SaveChanges();

                AlertaInfo(string.Format("Tipo de Obra: <b>{0}</b> se editó.", tipoObra.Nombre), true);
                string url = Url.Action("Lista", "TipoObra");
                return Json(new { success = true, url = url });
            }

            return PartialView("_Editar", tipoObra);
        }

        // GET: TipoObra/Eliminar/5
        [CustomAuthorize(permiso = "")]
        public ActionResult Eliminar(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoObra tipoObra = db.TipoObras.Find(id);
            if (tipoObra == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Eliminar", tipoObra);
        }

        // POST: TipoObra/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "")]
        public ActionResult EliminarConfirmado(Guid id)
        {
            string btnValue = Request.Form["accionx"];

            TipoObra tipoObra = db.TipoObras.Find(id);

            switch (btnValue)
            {
                case "deshabilitar":
                    tipoObra.Status = false;
                    db.Entry(tipoObra).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", tipoObra.Nombre), true);
                    break;
                case "eliminar":
                    db.TipoObras.Remove(tipoObra);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", tipoObra.Nombre), true);
                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;
            }


            string url = Url.Action("Lista", "TipoObra");
            return Json(new { success = true, url = url });
        }



        public JsonResult EsUnico(string Nombre, Guid? TipoObraID)
        {
            bool x = false;

            if (TipoObraID == null)
            {
                x = db.TipoObras.SingleOrDefault(a => a.Nombre == Nombre) == null ? true : false;
            }
            else
            {
                var tp = db.TipoObras.Select(a => new { a.TipoObraID, a.Nombre }).SingleOrDefault(a => a.Nombre == Nombre);
                x = tp == null ? true : tp.TipoObraID == TipoObraID ? true : false;
            }

            return Json(x);
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

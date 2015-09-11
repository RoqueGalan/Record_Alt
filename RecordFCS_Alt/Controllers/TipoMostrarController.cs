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
    public class TipoMostrarController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: TipoMostrar
        [CustomAuthorize(permiso = "")]
        public ActionResult Index()
        {
            return View();
        }


        [CustomAuthorize(permiso = "")]
        public ActionResult Lista()
        {
            var lista = db.TipoMostarlos.OrderBy(a => a.Nombre).ToList();

            ViewBag.totalRegistros = lista.Count;

            return PartialView("_Lista", lista);
        }


        // GET: TipoMostrar/Crear
        [CustomAuthorize(permiso = "")]
        public ActionResult Crear()
        {
            var tipoMostrar = new TipoMostrar()
            {
                Status = true
            };

            return PartialView("_Crear", tipoMostrar);
        }

        // POST: TipoMostrar/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "")]
        public ActionResult Crear([Bind(Include = "TipoMostrarID,Nombre,Status")] TipoMostrar tipoMostrar)
        {

            if (ModelState.IsValid)
            {
                tipoMostrar.TipoMostrarID = Guid.NewGuid();
                db.TipoMostarlos.Add(tipoMostrar);
                db.SaveChanges();

                AlertaSuccess(string.Format("Tipo de Mostrar: <b>{0}</b> creado.", tipoMostrar.Nombre), true);

                string url = Url.Action("Lista", "TipoMostrar");
                return Json(new { success = true, url = url });
            }

            return PartialView("_Crear", tipoMostrar);
        }

        // GET: TipoMostrar/Editar/5
        [CustomAuthorize(permiso = "")]
        public ActionResult Editar(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoMostrar tipoMostrar = db.TipoMostarlos.Find(id);
            if (tipoMostrar == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Editar", tipoMostrar);
        }

        // POST: TipoMostrar/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "")]
        public ActionResult Editar([Bind(Include = "TipoMostrarID,Nombre,Status")] TipoMostrar tipoMostrar)
        {
            var tm = db.TipoMostarlos.Select(a => new { a.Nombre, a.TipoMostrarID }).SingleOrDefault(a => a.Nombre == tipoMostrar.Nombre);

            if (tm != null)
                if (tm.TipoMostrarID != tipoMostrar.TipoMostrarID)
                    ModelState.AddModelError("Nombre", "Nombre ya existe.");


            if (ModelState.IsValid)
            {
                db.Entry(tipoMostrar).State = EntityState.Modified;
                db.SaveChanges();

                AlertaInfo(string.Format("Tipo de Mostrar: <b>{0}</b> se editó.", tipoMostrar.Nombre), true);
                string url = Url.Action("Lista", "TipoMostrar");
                return Json(new { success = true, url = url });
            }

            return PartialView("_Editar", tipoMostrar);
        }

        // GET: TipoMostrar/Eliminar/5
        [CustomAuthorize(permiso = "")]
        public ActionResult Eliminar(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoMostrar tipoMostrar = db.TipoMostarlos.Find(id);
            if (tipoMostrar == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Eliminar", tipoMostrar);
        }

        // POST: TipoMostrar/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "")]
        public ActionResult EliminarConfirmado(Guid id)
        {
            string btnValue = Request.Form["accionx"];

            TipoMostrar tipoMostrar = db.TipoMostarlos.Find(id);

            switch (btnValue)
            {
                case "deshabilitar":
                    tipoMostrar.Status = false;
                    db.Entry(tipoMostrar).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", tipoMostrar.Nombre), true);
                    break;
                case "eliminar":
                    db.TipoMostarlos.Remove(tipoMostrar);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", tipoMostrar.Nombre), true);
                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;
            }


            string url = Url.Action("Lista", "TipoMostrar");
            return Json(new { success = true, url = url });
        }


        public JsonResult EsUnico(string Nombre, Guid? TipoMostrarID)
        {
            bool x = false;
            
            var tm = db.TipoMostarlos.Select(a => new { a.TipoMostrarID, a.Nombre }).SingleOrDefault(a => a.Nombre == Nombre);

            x = tm == null ? true : tm.TipoMostrarID == TipoMostrarID ? true : false;

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

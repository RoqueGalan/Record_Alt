using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RecordFCS_Alt.Models;
using PagedList;
using RecordFCS_Alt.Helpers.Seguridad;

namespace RecordFCS_Alt.Controllers
{
    public class LetraFolioController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: LetraFolio
        [CustomAuthorize(permiso = "leFolList")]
        public ActionResult Index()
        {
            return View();
        }

        // GET: LetraFolio/Detalles/5
        [CustomAuthorize(permiso = "")]
        public ActionResult Lista(string FiltroActual, string Busqueda, int? Pagina)
        {
            if (Busqueda != null) Pagina = 1;
            else Busqueda = FiltroActual;

            ViewBag.FiltroActual = Busqueda;

            var lista = db.LetraFolios.Select(a => a);

            if (!String.IsNullOrEmpty(Busqueda))
            {
                Busqueda = Busqueda.ToLower();
                lista = lista.Where(a => a.Nombre.ToLower().Contains(Busqueda));
            }

            lista = lista.OrderBy(a => a.Nombre);

            //paginador
            int registrosPorPagina = 25;
            int pagActual = 1;
            pagActual = Pagina.HasValue ? Convert.ToInt32(Pagina) : 1;

            IPagedList<LetraFolio> listaPagina = lista.ToPagedList(pagActual, registrosPorPagina);

            return PartialView("_Lista", listaPagina);
        }


        // GET: LetraFolio/Crear
        [CustomAuthorize(permiso = "leFolNew")]
        public ActionResult Crear()
        {
            var lf = new LetraFolio()
            {
                Status = true
            };

            return PartialView("_Crear", lf);
        }

        // POST: LetraFolio/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "leFolNew")]
        public ActionResult Crear([Bind(Include = "LetraFolioID,Nombre,Descripcion,Status")] LetraFolio letraFolio)
        {
            //validar el nombre
            var lf = db.LetraFolios.Select(a => new { a.Nombre, a.LetraFolioID }).FirstOrDefault(a => a.Nombre == letraFolio.Nombre);
            if (lf != null)
                ModelState.AddModelError("Nombre", "Nombre ya existe.");


            if (ModelState.IsValid)
            {
                db.LetraFolios.Add(letraFolio);
                db.SaveChanges();

                AlertaSuccess(string.Format("Letra de Folio: <b>{0}</b> creada.", letraFolio.Nombre), true);

                string url = Url.Action("Lista", "LetraFolio");
                return Json(new { success = true, url = url });
            }

            return View(letraFolio);
        }

        // GET: LetraFolio/Editar/5
        [CustomAuthorize(permiso = "leFolEdit")]
        public ActionResult Editar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LetraFolio letraFolio = db.LetraFolios.Find(id);
            if (letraFolio == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Editar", letraFolio);
        }

        // POST: LetraFolio/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "leFolEdit")]
        public ActionResult Editar([Bind(Include = "LetraFolioID,Nombre,Descripcion,Status")] LetraFolio letraFolio)
        {
            //validar el nombre
            var lf = db.LetraFolios.Select(a => new { a.Nombre, a.LetraFolioID }).FirstOrDefault(a => a.Nombre == letraFolio.Nombre);

            if (lf != null)
                if (lf.LetraFolioID != letraFolio.LetraFolioID)
                    ModelState.AddModelError("Nombre", "Nombre ya existe.");

            if (ModelState.IsValid)
            {
                db.Entry(letraFolio).State = EntityState.Modified;
                db.SaveChanges();

                AlertaInfo(string.Format("Letra de Folio: <b>{0}</b> se editó.", letraFolio.Nombre), true);
                string url = Url.Action("Lista", "LetraFolio");
                return Json(new { success = true, url = url });

            }

            return PartialView("_Editar", letraFolio);
        }

        // GET: LetraFolio/Eliminar/5
        [CustomAuthorize(permiso = "leFolDel")]
        public ActionResult Eliminar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LetraFolio letraFolio = db.LetraFolios.Find(id);
            if (letraFolio == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Eliminar", letraFolio);
        }

        // POST: LetraFolio/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "leFolDel")]
        public ActionResult EliminarConfirmado(int id)
        {
            string btnValue = Request.Form["accionx"];

            LetraFolio letraFolio = db.LetraFolios.Find(id);

            switch (btnValue)
            {
                case "deshabilitar":
                    letraFolio.Status = false;
                    db.Entry(letraFolio).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", letraFolio.Nombre), true);
                    break;
                case "eliminar":
                    db.LetraFolios.Remove(letraFolio);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", letraFolio.Nombre), true);
                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;
            }

            string url = Url.Action("Lista", "LetraFolio");
            return Json(new { success = true, url = url });
        }

        [CustomAuthorize(permiso = "")]
        public JsonResult EsUnico(string Nombre, int? LetraFolioID)
        {
            bool x = false;

            var lf = db.LetraFolios.Select(a => new { a.LetraFolioID, a.Nombre }).SingleOrDefault(a => a.Nombre == Nombre);

            x = lf == null ? true : lf.LetraFolioID == LetraFolioID ? true : false;

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

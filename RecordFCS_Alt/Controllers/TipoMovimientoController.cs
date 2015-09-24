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
using PagedList;

namespace RecordFCS_Alt.Controllers
{
    public class TipoMovimientoController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: TipoMovimiento
        [CustomAuthorize(permiso = "tMovList")]
        public ActionResult Index()
        {
            return View();
        }

        [CustomAuthorize(permiso = "")]
        public ActionResult Lista(string FiltroActual, string Busqueda, int? Pagina)
        {
            if (Busqueda != null) Pagina = 1;
            else Busqueda = FiltroActual;

            ViewBag.FiltroActual = Busqueda;

            var lista = db.TipoMovimientos.Select(a => a);

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

            IPagedList<TipoMovimiento> listaPagina = lista.ToPagedList(pagActual, registrosPorPagina);

            return PartialView("_Lista", listaPagina);
        }


        // GET: TipoMovimiento/Crear
        [CustomAuthorize(permiso = "tMovNew")]
        public ActionResult Crear()
        {
            var tmov = new TipoMovimiento()
            {
                Status = true
            };
            return PartialView("_Crear", tmov);
        }

        // POST: TipoMovimiento/Crear
        [CustomAuthorize(permiso = "tMovNew")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear([Bind(Include = "TipoMovimientoID,Nombre,Status")] TipoMovimiento tipoMovimiento)
        {
            //validar el nombre
            var tmov = db.TipoMovimientos.Select(a => new { a.Nombre, a.TipoMovimientoID }).FirstOrDefault(a => a.Nombre == tipoMovimiento.Nombre);

            if (tmov != null)
                ModelState.AddModelError("Nombre", "Nombre ya existe.");


            if (ModelState.IsValid)
            {
                tipoMovimiento.TipoMovimientoID = Guid.NewGuid();
                db.TipoMovimientos.Add(tipoMovimiento);
                db.SaveChanges();

                AlertaSuccess(string.Format("Tipo de Movimiento: <b>{0}</b> creado.", tipoMovimiento.Nombre), true);

                string url = Url.Action("Lista", "TipoMovimiento");
                return Json(new { success = true, url = url });
            }

            return PartialView("_Crear", tipoMovimiento);
        }

        // GET: TipoMovimiento/Editar/5
        [CustomAuthorize(permiso = "tMovEdit")]
        public ActionResult Editar(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoMovimiento tipoMovimiento = db.TipoMovimientos.Find(id);
            if (tipoMovimiento == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Editar", tipoMovimiento);
        }

        // POST: TipoMovimiento/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "tMovEdit")]
        public ActionResult Editar([Bind(Include = "TipoMovimientoID,Nombre,Status")] TipoMovimiento tipoMovimiento)
        {
            //validar el nombre
            var tmov = db.TipoMovimientos.Select(a => new { a.Nombre, a.TipoMovimientoID }).FirstOrDefault(a => a.Nombre == tipoMovimiento.Nombre);

            if (tmov != null)
                if (tmov.TipoMovimientoID != tipoMovimiento.TipoMovimientoID)
                    ModelState.AddModelError("Nombre", "Nombre ya existe.");



            if (ModelState.IsValid)
            {
                db.Entry(tipoMovimiento).State = EntityState.Modified;
                db.SaveChanges();

                AlertaInfo(string.Format("Tipo de Movimiento: <b>{0}</b> se editó.", tipoMovimiento.Nombre), true);
                string url = Url.Action("Lista", "TipoMovimiento");
                return Json(new { success = true, url = url });
            }

            return PartialView("_Editar", tipoMovimiento);
        }

        // GET: TipoMovimiento/Eliminar/5
        [CustomAuthorize(permiso = "tMovDel")]
        public ActionResult Eliminar(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoMovimiento tipoMovimiento = db.TipoMovimientos.Find(id);
            if (tipoMovimiento == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Eliminar", tipoMovimiento);
        }

        // POST: TipoMovimiento/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "tMovDel")]
        public ActionResult EliminarConfirmado(Guid id)
        {
            string btnValue = Request.Form["accionx"];
            string Nombre = "";

            TipoMovimiento tipoMovimiento = db.TipoMovimientos.Find(id);

            Nombre = tipoMovimiento.Nombre;

            switch (btnValue)
            {
                case "deshabilitar":
                    tipoMovimiento.Status = false;
                    db.Entry(tipoMovimiento).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", Nombre), true);
                    break;

                case "eliminar":
                    db.TipoMovimientos.Remove(tipoMovimiento);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", Nombre), true);
                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;
            }

            string url = Url.Action("Lista", "TipoMovimiento");
            return Json(new { success = true, url = url });
        }


        [CustomAuthorize(permiso = "")]
        public JsonResult EsUnico(string Nombre, Guid? TipoMovimientoID)
        {
            bool x = false;

            var tm = db.TipoMovimientos.Select(a => new { a.TipoMovimientoID, a.Nombre }).SingleOrDefault(a => a.Nombre == Nombre);

            x = tm == null ? true : tm.TipoMovimientoID == TipoMovimientoID ? true : false;

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

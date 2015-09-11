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
    public class UbicacionController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: Ubicacion
        [CustomAuthorize(permiso = "")]
        public ActionResult Index()
        {
            return View();
        }

        // GET: Ubicacion/Lista
        [CustomAuthorize(permiso = "")]
        public ActionResult Lista(string FiltroActual, string Busqueda, int? Pagina)
        {
            if (Busqueda != null) Pagina = 1;
            else Busqueda = FiltroActual;

            ViewBag.FiltroActual = Busqueda;

            var lista = db.Ubicaciones.Select(a=> a);

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

            IPagedList<Ubicacion> listaPagina = lista.ToPagedList(pagActual, registrosPorPagina);

            return PartialView("_Lista", listaPagina);
        }

        // GET: Ubicacion/Crear
        [CustomAuthorize(permiso = "")]
        public ActionResult Crear(bool EsRegistroObra = false)
        {
            var ubicacion = new Ubicacion()
            {
                Status = true
            };

            ViewBag.EsRegistroObra = EsRegistroObra;


            return PartialView("_Crear", ubicacion);
        }

        // POST: Ubicacion/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "")]
        public ActionResult Crear([Bind(Include = "UbicacionID,Nombre,Descripcion,Status,Temp")] Ubicacion ubicacion, bool EsRegistroObra = false)
        {
            //validar el nombre
            var ubi = db.Ubicaciones.Select(a => new { a.Nombre, a.UbicacionID }).FirstOrDefault(a => a.Nombre == ubicacion.Nombre);

            if (ubi != null)
                    ModelState.AddModelError("Nombre", "Nombre ya existe.");


            if (ModelState.IsValid)
            {
                ubicacion.UbicacionID = Guid.NewGuid();
                db.Ubicaciones.Add(ubicacion);
                db.SaveChanges();

                AlertaSuccess(string.Format("Ubicación: <b>{0}</b> creada.", ubicacion.Nombre), true);

                

                if (EsRegistroObra)
                {
                    return Json(new { success = true, nombre = ubicacion.Nombre, ubicacionID = ubicacion.UbicacionID });

                }
                else
                {
                   string url = Url.Action("Lista", "Ubicacion");
                return Json(new { success = true, url = url });
                }
                
            }

            ViewBag.EsRegistroObra = EsRegistroObra;

            return PartialView("_Crear", ubicacion);
        }

        // GET: Ubicacion/Editar/5
        [CustomAuthorize(permiso = "")]
        public ActionResult Editar(Guid? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Ubicacion ubicacion = db.Ubicaciones.Find(id);

            if (ubicacion == null) return HttpNotFound();

            return PartialView("_Editar", ubicacion);
        }

        // POST: Ubicacion/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "")]
        public ActionResult Editar([Bind(Include = "UbicacionID,Nombre,Descripcion,Status,Temp")] Ubicacion ubicacion)
        {
            //validar el nombre
            var ubi = db.Ubicaciones.Select(a => new { a.Nombre, a.UbicacionID }).FirstOrDefault(a => a.Nombre == ubicacion.Nombre);

            if (ubi != null)
                if (ubi.UbicacionID != ubicacion.UbicacionID)
                    ModelState.AddModelError("Nombre", "Nombre ya existe.");


            if (ModelState.IsValid)
            {
                db.Entry(ubicacion).State = EntityState.Modified;
                db.SaveChanges();

                AlertaInfo(string.Format("Ubicación: <b>{0}</b> se editó.", ubicacion.Nombre), true);
                string url = Url.Action("Lista", "Ubicacion");
                return Json(new { success = true, url = url });
            }

            return PartialView("_Editar", ubicacion);
        }

        // GET: Ubicacion/Eliminar/5
        [CustomAuthorize(permiso = "")]
        public ActionResult Eliminar(Guid? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Ubicacion ubicacion = db.Ubicaciones.Find(id);

            if (ubicacion == null) return HttpNotFound();

            return PartialView("_Eliminar", ubicacion);
        }

        // POST: Ubicacion/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "")]
        public ActionResult EliminarConfirmado(Guid id)
        {
            string btnValue = Request.Form["accionx"];

            Ubicacion ubicacion = db.Ubicaciones.Find(id);
            switch (btnValue)
            {
                case "deshabilitar":
                    ubicacion.Status = false;
                    db.Entry(ubicacion).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", ubicacion.Nombre), true);
                    break;
                case "eliminar":
                    db.Ubicaciones.Remove(ubicacion);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", ubicacion.Nombre), true);
                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;
            }


            string url = Url.Action("Lista", "Ubicacion");
            return Json(new { success = true, url = url });
        }

        [CustomAuthorize(permiso = "")]
        public ActionResult GenerarLista(string Filtro = "", string TipoLista = "option")
        {

            List<Ubicacion> lista = db.Ubicaciones.Select(a => a).Where(a => a.Status).ToList();

            if (!String.IsNullOrEmpty(Filtro))
            {
                Filtro = Filtro.ToLower();
                lista = lista.Where(a => a.Nombre.ToLower().Contains(Filtro)).ToList();
            }

            lista = lista.Select(a => new Ubicacion() { UbicacionID = a.UbicacionID, Nombre = a.Nombre }).OrderBy(a => a.Nombre).ToList();


            switch (TipoLista)
            {
                case "Select":
                case "select":
                case "SELECT":

                    ViewBag.UbicacionId = new SelectList(lista, "UbicacionID", "Nombre");
                    return PartialView("_ListaSelect");

                default:
                    return Json(lista, JsonRequestBehavior.AllowGet);
            }

        }




        public JsonResult EsUnico(string Nombre, Guid? UbicacionID)
        {
            bool x = false;

            var ubi = db.Ubicaciones.Select(a => new { a.UbicacionID, a.Nombre }).SingleOrDefault(a => a.Nombre == Nombre);

            x = ubi == null ? true : ubi.UbicacionID == UbicacionID ? true : false;

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

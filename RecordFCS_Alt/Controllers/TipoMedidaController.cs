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

namespace RecordFCS_Alt.Controllers
{
    public class TipoMedidaController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: TipoMedida
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Lista(string FiltroActual, string Busqueda, int? Pagina)
        {
            if (Busqueda != null) Pagina = 1;
            else Busqueda = FiltroActual;

            ViewBag.FiltroActual = Busqueda;

            var lista = db.TipoMedidas.Select(a => a);

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

            IPagedList<TipoMedida> listaPagina = lista.ToPagedList(pagActual, registrosPorPagina);

            return PartialView("_Lista", listaPagina);
        }


        // GET: TipoMedida/_Crear
        public ActionResult Crear()
        {
            var tm = new TipoMedida()
            {
                Status = true
            };

            return PartialView("_Crear", tm);
        }

        // POST: TipoMedida/_Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear([Bind(Include = "TipoMedidaID,Nombre,Descripcion,Status,Temp")] TipoMedida tipoMedida)
        {
            //validar el nombre
            var ubi = db.TipoMedidas.Select(a => new { a.Nombre, a.TipoMedidaID }).FirstOrDefault(a => a.Nombre == tipoMedida.Nombre);

            if (ubi != null)
                    ModelState.AddModelError("Nombre", "Nombre ya existe.");


            if (ModelState.IsValid)
            {
                tipoMedida.TipoMedidaID = Guid.NewGuid();
                db.TipoMedidas.Add(tipoMedida);
                db.SaveChanges();

                AlertaSuccess(string.Format("Tipo de Médida: <b>{0}</b> creada.", tipoMedida.Nombre), true);

                string url = Url.Action("Lista", "TipoMedida");
                return Json(new { success = true, url = url });
            }

            return PartialView("_Crear", tipoMedida);
        }

        // GET: TipoMedida/Editar/5
        public ActionResult Editar(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoMedida tipoMedida = db.TipoMedidas.Find(id);
            if (tipoMedida == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Editar", tipoMedida);
        }

        // POST: TipoMedida/Editar/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "TipoMedidaID,Nombre,Descripcion,Status,Temp")] TipoMedida tipoMedida)
        {
            //validar el nombre
            var tm = db.TipoMedidas.Select(a => new { a.Nombre, a.TipoMedidaID }).FirstOrDefault(a => a.Nombre == tipoMedida.Nombre);

            if (tm != null)
                if (tm.TipoMedidaID != tipoMedida.TipoMedidaID)
                    ModelState.AddModelError("Nombre", "Nombre ya existe.");


            if (ModelState.IsValid)
            {
                db.Entry(tipoMedida).State = EntityState.Modified;
                db.SaveChanges();

                AlertaInfo(string.Format("Tipo de Médida: <b>{0}</b> se editó.", tipoMedida.Nombre), true);
                string url = Url.Action("Lista", "TipoMedida");
                return Json(new { success = true, url = url });
            }
            return PartialView("_Editar", tipoMedida);
        }

        // GET: TipoMedida/Eliminar/5
        public ActionResult Eliminar(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoMedida tipoMedida = db.TipoMedidas.Find(id);
            if (tipoMedida == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Eliminar", tipoMedida);
        }

        // POST: TipoMedida/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarConfirmado(Guid id)
        {
            string btnValue = Request.Form["accionx"];

            TipoMedida tipoMedida = db.TipoMedidas.Find(id);

            switch (btnValue)
            {
                case "deshabilitar":
                    tipoMedida.Status = false;
                    db.Entry(tipoMedida).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", tipoMedida.Nombre), true);
                    break;
                case "eliminar":
                    db.TipoMedidas.Remove(tipoMedida);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", tipoMedida.Nombre), true);
                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;
            }


            string url = Url.Action("Lista", "TipoMedida");
            return Json(new { success = true, url = url });
        }

        public JsonResult EsUnico(string Nombre, Guid? TipoMedidaID)
        {
            bool x = false;

            var tm = db.TipoMedidas.Select(a => new { a.TipoMedidaID, a.Nombre }).SingleOrDefault(a => a.Nombre == Nombre);

            x = tm == null ? true : tm.TipoMedidaID == TipoMedidaID ? true : false;

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

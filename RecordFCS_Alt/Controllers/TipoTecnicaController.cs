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
    public class TipoTecnicaController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: TipoTecnica
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Detalles(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            TipoTecnica tipoTecnica = db.TipoTecnicas.Find(id);
            if (tipoTecnica == null)
            {
                return HttpNotFound();
            }
            return View("Detalles", tipoTecnica);
        }




        // GET: TipoTecnica/Lista
        public ActionResult Lista(string FiltroActual, string Busqueda, int? Pagina)
        {
            if (Busqueda != null) Pagina = 1;
            else Busqueda = FiltroActual;

            ViewBag.FiltroActual = Busqueda;

            var lista = db.TipoTecnicas.Select(a => a);

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

            IPagedList<TipoTecnica> listaPagina = lista.ToPagedList(pagActual, registrosPorPagina);

            return PartialView("_Lista", listaPagina);
        }


        // GET: TipoTecnica/Crear
        public ActionResult Crear()
        {
            var tt = new TipoTecnica()
            {
                Status = true
            };

            return PartialView("_Crear", tt);
        }

        // POST: TipoTecnica/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear([Bind(Include = "TipoTecnicaID,Nombre,Descripcion,Status,Temp")] TipoTecnica tipoTecnica)
        {
            //validar el nombre
            var tt = db.TipoTecnicas.Select(a => new { a.Nombre, a.TipoTecnicaID }).FirstOrDefault(a => a.Nombre == tipoTecnica.Nombre);

            if (tt != null)
                ModelState.AddModelError("Nombre", "Nombre ya existe.");


            if (ModelState.IsValid)
            {
                tipoTecnica.TipoTecnicaID = Guid.NewGuid();
                db.TipoTecnicas.Add(tipoTecnica);
                db.SaveChanges();

                AlertaSuccess(string.Format("Tipo de Técnica: <b>{0}</b> creada.", tipoTecnica.Nombre), true);

                string url = Url.Action("Lista", "TipoTecnica");
                return Json(new { success = true, url = url });
            }

            return PartialView("_Crear", tipoTecnica);
        }

        // GET: TipoTecnica/Editar/5
        public ActionResult Editar(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoTecnica tipoTecnica = db.TipoTecnicas.Find(id);
            if (tipoTecnica == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Editar", tipoTecnica);
        }

        // POST: TipoTecnica/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "TipoTecnicaID,Nombre,Descripcion,Status,Temp")] TipoTecnica tipoTecnica)
        {
            //validar el nombre
            var tt = db.TipoTecnicas.Select(a => new { a.Nombre, a.TipoTecnicaID }).FirstOrDefault(a => a.Nombre == tipoTecnica.Nombre);

            if (tt != null)
                if (tt.TipoTecnicaID != tipoTecnica.TipoTecnicaID)
                    ModelState.AddModelError("Nombre", "Nombre ya existe.");


            if (ModelState.IsValid)
            {
                db.Entry(tipoTecnica).State = EntityState.Modified;
                db.SaveChanges();

                AlertaInfo(string.Format("Tipo de Técnica: <b>{0}</b> se editó.", tipoTecnica.Nombre), true);
                string url = Url.Action("Lista", "TipoTecnica");
                return Json(new { success = true, url = url });
            }

            return PartialView("_Editar", tipoTecnica);
        }

        // GET: TipoTecnica/Eliminar/5
        public ActionResult Eliminar(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoTecnica tipoTecnica = db.TipoTecnicas.Find(id);
            if (tipoTecnica == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Eliminar", tipoTecnica);
        }

        // POST: TipoTecnica/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarConfirmado(Guid id)
        {
            string btnValue = Request.Form["accionx"];

            TipoTecnica tt = db.TipoTecnicas.Find(id);

            switch (btnValue)
            {
                case "deshabilitar":
                    tt.Status = false;
                    db.Entry(tt).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", tt.Nombre), true);
                    break;
                case "eliminar":
                    db.TipoTecnicas.Remove(tt);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", tt.Nombre), true);
                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;
            }


            string url = Url.Action("Lista", "TipoTecnica");
            return Json(new { success = true, url = url });
        }


        public JsonResult EsUnico(string Nombre, Guid? TipoTecnicaID)
        {
            bool x = false;

            var tt = db.TipoTecnicas.Select(a => new { a.TipoTecnicaID, a.Nombre }).SingleOrDefault(a => a.Nombre == Nombre);

            x = tt == null ? true : tt.TipoTecnicaID == TipoTecnicaID ? true : false;

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

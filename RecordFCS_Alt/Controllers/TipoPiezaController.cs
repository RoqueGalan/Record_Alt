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
    public class TipoPiezaController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        [CustomAuthorize(permiso = "")]
        public ActionResult Index()
        {
            //redireccionar a Tipo de Obras
            return RedirectToAction("Index", "TipoObra");
        }

        // GET: TipoPieza
        //root = true --> mostrar lista de piezas maestras
        [CustomAuthorize(permiso = "")]
        public ActionResult Lista(Guid? id, bool esRoot = false)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            //id = TipoPiezaID
            IEnumerable<TipoPieza> lista = null;

            if (esRoot)
            {
                TipoObra tipoObra = db.TipoObras.Find(id);
                if (tipoObra == null) return HttpNotFound();

                lista = tipoObra.TipoPiezas.Where(a => a.EsPrincipal).OrderBy(a => a.Orden);

            }
            else
            {
                TipoPieza tipoPiezaPadre = db.TipoPiezas.Find(id);
                if (tipoPiezaPadre == null) return HttpNotFound();

                lista = tipoPiezaPadre.TipoPiezasHijas.OrderBy(a => a.Orden);

                ViewBag.nombre = tipoPiezaPadre.Nombre;

            }


            ViewBag.totalRegistros = lista.Count();
            ViewBag.id = id;
            ViewBag.root = esRoot;


            return PartialView("_Lista", lista.ToList());
        }

        [CustomAuthorize(permiso = "")]
        public ActionResult ListaSelect(Guid? id, bool esRoot = false)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            //id = TipoPiezaID
            IEnumerable<TipoPieza> lista = null;

            if (esRoot)
            {
                TipoObra tipoObra = db.TipoObras.Find(id);
                if (tipoObra == null) return HttpNotFound();

                lista = tipoObra.TipoPiezas.Where(a => a.EsPrincipal && a.Status && a.TipoPiezaPadreID == null).OrderBy(a => a.Nombre);
            }
            else
            {
                TipoPieza tipoPiezaPadre = db.TipoPiezas.Find(id);

                if (tipoPiezaPadre == null) return HttpNotFound();

                lista = tipoPiezaPadre.TipoPiezasHijas.Where(a => a.Status).OrderBy(a => a.Nombre);

            }


            ViewBag.totalRegistros = lista.Count();

            ViewBag.TipoPiezaID = new SelectList(lista.Select(a => new { a.TipoPiezaID, Nombre = a.Nombre + " - " + a.Descripcion }).ToList(), "TipoPiezaID", "Nombre");


            return PartialView("_Registro_inputSelect");
        }

        // GET: TipoPieza/Detalles/5
        [CustomAuthorize(permiso = "")]
        public ActionResult Detalles(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoPieza tipoPieza = db.TipoPiezas.Find(id);
            if (tipoPieza == null)
            {
                return HttpNotFound();
            }
            return View(tipoPieza);
        }

        // GET: TipoPieza/Crear
        [CustomAuthorize(permiso = "")]
        public ActionResult Crear(Guid? id, bool principal = false)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            TipoPieza tp = new TipoPieza();

            if (principal)
            {
                //id = TipoObraID
                TipoObra tipoObra = db.TipoObras.Find(id);
                if (tipoObra == null)
                    return HttpNotFound();

                tp.TipoObraID = tipoObra.TipoObraID;
                tp.Orden = tipoObra.TipoPiezas.Where(a => a.EsPrincipal).Count() + 1;
                tp.Status = true;
                tp.EsPrincipal = true;
            }
            else
            {
                //id = TipoPiezaPadreID
                TipoPieza tipoPiezaPadre = db.TipoPiezas.Find(id);
                if (tipoPiezaPadre == null)
                    return HttpNotFound();

                tp.TipoObraID = tipoPiezaPadre.TipoObraID;
                tp.Orden = tipoPiezaPadre.TipoPiezasHijas.Count + 1;
                tp.TipoPiezaPadreID = tipoPiezaPadre.TipoPiezaID;
                tp.Status = true;
                tp.EsPrincipal = false;
            }

            return PartialView("_Crear", tp);
        }

        // POST: TipoPieza/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "")]
        public ActionResult Crear([Bind(Include = "TipoPiezaID,Nombre,Descripcion,Prefijo,Orden,EsPrincipal,Status,TipoObraID,TipoPiezaPadreID,Temp")] TipoPieza tipoPieza)
        {
            var tp = db.TipoPiezas.Select(a => new { a.TipoObraID, a.TipoPiezaPadreID, a.TipoPiezaID, a.Nombre }).FirstOrDefault(a => a.Nombre == tipoPieza.Nombre && a.TipoObraID == tipoPieza.TipoObraID && a.TipoPiezaPadreID == tipoPieza.TipoPiezaPadreID);

            if (tp != null)
                if (tp.TipoPiezaID != tipoPieza.TipoPiezaID)
                    ModelState.AddModelError("Nombre", "Nombre ya existe.");


            if (ModelState.IsValid)
            {
                tipoPieza.TipoPiezaID = Guid.NewGuid();
                db.TipoPiezas.Add(tipoPieza);
                db.SaveChanges();

                AlertaSuccess(string.Format("Tipo de Pieza: <b>{0}</b> creada.", tipoPieza.Nombre), true);

                //principal true enviar tipoobraid



                string url = "";

                if (tipoPieza.TipoPiezaPadreID == null)
                    url = Url.Action("Lista", "TipoPieza", new { id = tipoPieza.TipoObraID, esRoot = true });
                else
                    url = Url.Action("Lista", "TipoPieza", new { id = tipoPieza.TipoPiezaPadreID });


                return Json(new { success = true, url = url });

            }

            return PartialView("_Crear", tipoPieza);
        }

        // GET: TipoPieza/Editar/5
        [CustomAuthorize(permiso = "")]
        public ActionResult Editar(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoPieza tipoPieza = db.TipoPiezas.Find(id);
            if (tipoPieza == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Editar", tipoPieza);
        }

        // POST: TipoPieza/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "")]
        public ActionResult Editar([Bind(Include = "TipoPiezaID,Nombre,Descripcion,Prefijo,Orden,EsPrincipal,Status,TipoObraID,TipoPiezaPadreID,Temp")] TipoPieza tipoPieza)
        {
            var tp = db.TipoPiezas.Select(a => new { a.TipoObraID, a.TipoPiezaPadreID, a.TipoPiezaID, a.Nombre }).FirstOrDefault(a => a.Nombre == tipoPieza.Nombre && a.TipoObraID == tipoPieza.TipoObraID && a.TipoPiezaPadreID == tipoPieza.TipoPiezaPadreID);

            if (tp != null)
                if (tp.TipoPiezaID != tipoPieza.TipoPiezaID)
                    ModelState.AddModelError("Nombre", "Nombre ya existe.");

            if (ModelState.IsValid)
            {
                db.Entry(tipoPieza).State = EntityState.Modified;
                db.SaveChanges();

                AlertaInfo(string.Format("Tipo de Pieza: <b>{0}</b> se editó.", tipoPieza.Nombre), true);


                string url = "";

                if (tipoPieza.TipoPiezaPadreID == null)
                    url = Url.Action("Lista", "TipoPieza", new { id = tipoPieza.TipoObraID, esRoot = true });
                else
                    url = Url.Action("Lista", "TipoPieza", new { id = tipoPieza.TipoPiezaPadreID });

                return Json(new { success = true, url = url });
            }

            return PartialView("_Editar", tipoPieza);
        }

        // GET: TipoPieza/Eliminar/5
        [CustomAuthorize(permiso = "")]
        public ActionResult Eliminar(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoPieza tipoPieza = db.TipoPiezas.Find(id);
            if (tipoPieza == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Eliminar", tipoPieza);
        }


        // POST: TipoPieza/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "")]
        public ActionResult EliminarConfirmado(Guid id)
        {
            string btnValue = Request.Form["accionx"];

            TipoPieza tipoPieza = db.TipoPiezas.Find(id);

            var toID = tipoPieza.TipoObraID;
            var tppID = tipoPieza.TipoPiezaPadreID;

            switch (btnValue)
            {
                case "deshabilitar":
                    tipoPieza.Status = false;
                    db.Entry(tipoPieza).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", tipoPieza.Nombre), true);
                    break;
                case "eliminar":
                    db.TipoPiezas.Remove(tipoPieza);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", tipoPieza.Nombre), true);
                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;
            }


            string url = "";

            if (tppID == null)
                url = Url.Action("Lista", "TipoPieza", new { id = toID, esRoot = true });
            else
                url = Url.Action("Lista", "TipoPieza", new { id = tppID });


            return Json(new { success = true, url = url });
        }


        public JsonResult EsUnico(string Nombre, Guid? TipoPiezaID, Guid? TipoObraID, Guid? TipoPiezaPadreID)
        {
            bool x = false;

            var tp = db.TipoPiezas.SingleOrDefault(a => a.Nombre == Nombre && a.TipoObraID == TipoObraID && a.TipoPiezaPadreID == TipoPiezaPadreID);
            x = tp == null ? true : tp.TipoPiezaID == TipoPiezaID ? true : false;


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

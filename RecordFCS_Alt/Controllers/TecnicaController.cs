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
    public class TecnicaController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: Tecnica
        [CustomAuthorize(permiso = "")]
        public ActionResult Index()
        {
            //redireccionar a Tipo de Tecnicas
            return RedirectToAction("Index", "TipoTecnica");
        }

        [CustomAuthorize(permiso = "")]
        public ActionResult Lista(Guid? id, string FiltroActual, string Busqueda, int? Pagina)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            TipoTecnica ttecnica = db.TipoTecnicas.Find(id);

            if (ttecnica == null)
                return HttpNotFound();

            if (Busqueda != null) Pagina = 1;
            else Busqueda = FiltroActual;

            ViewBag.FiltroActual = Busqueda;

            var lista = ttecnica.Tecnicas.Select(a => a);

            if (!String.IsNullOrEmpty(Busqueda))
            {
                Busqueda = Busqueda.ToLower();
                lista = lista.Where(a => a.Descripcion.ToLower().Contains(Busqueda));
            }

            lista = lista.OrderBy(a => a.TecnicaPadreID).ThenBy(b => b.Descripcion);

            ViewBag.TipoTecnicaID = ttecnica.TipoTecnicaID;
            ViewBag.Nombre = ttecnica.Nombre;

            //paginador
            int registrosPorPagina = 25;
            int pagActual = 1;
            pagActual = Pagina.HasValue ? Convert.ToInt32(Pagina) : 1;

            IPagedList<Tecnica> listaPagina = lista.ToPagedList(pagActual, registrosPorPagina);

            return PartialView("_Lista", listaPagina);
        }

        // GET: Tecnica/Crear
        [CustomAuthorize(permiso = "tecNew")]
        public ActionResult Crear(Guid? id, bool EsRegistroObra = false)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            TipoTecnica ttecnica = db.TipoTecnicas.Find(id);

            if (ttecnica == null)
                return HttpNotFound();

            Tecnica tecnica = new Tecnica()
            {
                TipoTecnicaID = ttecnica.TipoTecnicaID,
                Status = true
            };

            var lista = ttecnica.Tecnicas.Where(a => a.Status && !String.IsNullOrWhiteSpace(a.ClaveSigla) && !String.IsNullOrWhiteSpace(a.ClaveTexto) && !String.IsNullOrWhiteSpace(a.MatriculaSigla)).Select(a => new { a.TecnicaID, Nombre = a.ClaveTexto + " " + a.Descripcion });

            ViewBag.total = lista.Count();

            ViewBag.TecnicaPadreID = new SelectList(lista, "TecnicaID", "Nombre");

            ViewBag.EsRegistroObra = EsRegistroObra;


            return PartialView("_Crear", tecnica);
        }

        // POST: Tecnica/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "tecNew")]
        public ActionResult Crear([Bind(Include = "TecnicaID,ClaveSigla,ClaveTexto,MatriculaSigla,Descripcion,Status,TipoTecnicaID,TecnicaPadreID,Temp1,Temp2")] Tecnica tecnica, bool EsRegistroObra = false)
        {
            var tec = db.Tecnicas.Select(a => new { a.Descripcion, a.TipoTecnicaID, a.TecnicaID, a.TecnicaPadreID }).FirstOrDefault(a => a.Descripcion == tecnica.Descripcion && a.TipoTecnicaID == tecnica.TipoTecnicaID && a.TecnicaPadreID == tecnica.TecnicaPadreID);

            if (tec != null)
                ModelState.AddModelError("Descripcion", "Ya existe, intenta de nuevo.");



            if (ModelState.IsValid)
            {
                tecnica.TecnicaID = Guid.NewGuid();
                db.Tecnicas.Add(tecnica);
                db.SaveChanges();

                AlertaSuccess(string.Format("Técnica: <b>{0}</b> creada.", tecnica.Descripcion), true);

                if (EsRegistroObra)
                {
                    return Json(new { success = true, descripcion = tecnica.Descripcion, tecnicaID = tecnica.TecnicaID, tipoTecnicaID = tecnica.TipoTecnicaID });

                }
                else
                {
                    string url = Url.Action("Lista", "Tecnica", new { id = tecnica.TipoTecnicaID });
                    return Json(new { success = true, url = url });
                }

            }

            var ttecnica = db.TipoTecnicas.Find(tecnica.TipoTecnicaID);

            var lista = ttecnica.Tecnicas.Where(a => a.Status && !String.IsNullOrWhiteSpace(a.ClaveSigla) && !String.IsNullOrWhiteSpace(a.ClaveTexto) && !String.IsNullOrWhiteSpace(a.MatriculaSigla)).Select(a => new { a.TecnicaID, Nombre = a.ClaveTexto + " " + a.Descripcion });

            ViewBag.total = lista.Count();

            ViewBag.TecnicaPadreID = new SelectList(lista, "TecnicaID", "Nombre", tecnica.TecnicaPadreID);

            ViewBag.EsRegistroObra = EsRegistroObra;


            return PartialView("_Crear", tecnica);

        }

        // GET: Tecnica/Editar/5
        [CustomAuthorize(permiso = "tecEdit")]
        public ActionResult Editar(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tecnica tecnica = db.Tecnicas.Find(id);
            if (tecnica == null)
            {
                return HttpNotFound();
            }



            var lista = tecnica.TipoTecnica.Tecnicas.Where(a => a.Status && !String.IsNullOrWhiteSpace(a.ClaveSigla) && !String.IsNullOrWhiteSpace(a.ClaveTexto) && !String.IsNullOrWhiteSpace(a.MatriculaSigla)).Select(a => new { a.TecnicaID, Nombre = a.ClaveTexto + " " + a.Descripcion });

            ViewBag.total = lista.Count();

            ViewBag.TecnicaPadreID = new SelectList(lista, "TecnicaID", "Nombre", tecnica.TecnicaPadreID);


            return PartialView("_Editar", tecnica);
        }

        // POST: Tecnica/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "tecEdit")]
        public ActionResult Editar([Bind(Include = "TecnicaID,ClaveSigla,ClaveTexto,MatriculaSigla,Descripcion,Status,TipoTecnicaID,TecnicaPadreID,Temp1,Temp2")] Tecnica tecnica)
        {
            var tec = db.Tecnicas.Select(a => new { a.Descripcion, a.TipoTecnicaID, a.TecnicaID, a.TecnicaPadreID }).FirstOrDefault(a => a.Descripcion == tecnica.Descripcion && a.TipoTecnicaID == tecnica.TipoTecnicaID && a.TecnicaPadreID == tecnica.TecnicaPadreID);

            if (tec != null)
                if (tec.TecnicaID != tecnica.TecnicaID)
                    ModelState.AddModelError("Descripcion", "Ya existe, intenta de nuevo.");


            if (ModelState.IsValid)
            {
                db.Entry(tecnica).State = EntityState.Modified;
                db.SaveChanges();

                AlertaInfo(string.Format("Técnica: <b>{0}</b> se editó.", tecnica.Descripcion), true);

                string url = Url.Action("Lista", "Tecnica", new { id = tecnica.TipoTecnicaID });
                return Json(new { success = true, url = url });
            }

            var ttecnica = db.TipoTecnicas.Find(tecnica.TipoTecnicaID);

            var lista = ttecnica.Tecnicas.Where(a => a.Status && !String.IsNullOrWhiteSpace(a.ClaveSigla) && !String.IsNullOrWhiteSpace(a.ClaveTexto) && !String.IsNullOrWhiteSpace(a.MatriculaSigla)).Select(a => new { a.TecnicaID, Nombre = a.ClaveTexto + " " + a.Descripcion });

            ViewBag.total = lista.Count();

            ViewBag.TecnicaPadreID = new SelectList(lista, "TecnicaID", "Nombre", tecnica.TecnicaPadreID);


            return PartialView("_Editar", tecnica);
        }

        // GET: Tecnica/Eliminar/5
        [CustomAuthorize(permiso = "tecDel")]
        public ActionResult Eliminar(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tecnica tecnica = db.Tecnicas.Find(id);
            if (tecnica == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Eliminar", tecnica);
        }

        // POST: Tecnica/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "tecDel")]
        public ActionResult EliminarConfirmado(Guid id)
        {
            string btnValue = Request.Form["accionx"];

            Tecnica tecnica = db.Tecnicas.Find(id);


            switch (btnValue)
            {
                case "deshabilitar":
                    tecnica.Status = false;
                    db.Entry(tecnica).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", tecnica.Descripcion), true);
                    break;
                case "eliminar":
                    db.Tecnicas.Remove(tecnica);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", tecnica.Descripcion), true);
                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;
            }

            string url = Url.Action("Lista", "Tecnica", new { id = tecnica.TipoTecnicaID });
            return Json(new { success = true, url = url });
        }

        [CustomAuthorize(permiso = "")]
        public JsonResult EsUnico(string Descripcion, Guid? TipoTecnicaID, Guid? TecnicaPadreID, Guid? TecnicaID)
        {
            bool x = false;

            var tp = db.Tecnicas.SingleOrDefault(a => a.Descripcion == Descripcion && a.TipoTecnicaID == TipoTecnicaID && a.TecnicaPadreID == TecnicaPadreID);
            x = tp == null ? true : tp.TecnicaID == TecnicaID ? true : false;


            return Json(x);
        }

        [CustomAuthorize(permiso = "")]
        public ActionResult GenerarLista(Guid? id, string Filtro = "", string TipoLista = "option")
        {


            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            TipoTecnica tipoTecnica = db.TipoTecnicas.Find(id);

            if (tipoTecnica == null) return HttpNotFound();

            List<Tecnica> lista = tipoTecnica.Tecnicas.Select(a => a).Where(a => a.Status && !String.IsNullOrWhiteSpace(a.Descripcion)).ToList();

            if (!String.IsNullOrEmpty(Filtro))
            {
                Filtro = Filtro.ToLower();
                lista = lista.Where(a => a.Descripcion.ToLower().Contains(Filtro)).ToList();
            }

            lista = lista.Select(a => new Tecnica() { TecnicaID = a.TecnicaID, Descripcion = a.Descripcion, TipoTecnicaID = a.TipoTecnicaID }).OrderBy(a => a.Descripcion).ToList();


            switch (TipoLista)
            {
                case "Select":
                case "select":
                case "SELECT":


                    ViewBag.ListaValorID = new SelectList(lista, "TecnicaID", "Descripcion");
                    ViewBag.TipoTecnicaID = tipoTecnica.TipoTecnicaID;

                    return PartialView("_ListaSelect");


                default:

                    return Json(lista, JsonRequestBehavior.AllowGet);
            }

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

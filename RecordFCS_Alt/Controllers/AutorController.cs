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
    public class AutorController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: Autor
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Lista(string FiltroActual, string Busqueda, int? Pagina)
        {

            if (Busqueda != null)
                Pagina = 1;
            else
                Busqueda = FiltroActual;

            ViewBag.FiltroActual = Busqueda;


            var lista = db.Autores.Select(a => a);

            if (!String.IsNullOrEmpty(Busqueda))
            {
                Busqueda = Busqueda.ToLower();
                lista = lista.Where(a => a.Nombre.ToLower().Contains(Busqueda) || a.Apellido.ToLower().Contains(Busqueda));
            }

            lista = lista.OrderBy(a => a.Nombre);

            //paginador
            int registrosPorPagina = 25;
            int pagActual = 1;
            pagActual = Pagina.HasValue ? Convert.ToInt32(Pagina) : 1;

            IPagedList<Autor> listaPagina = lista.ToPagedList(pagActual, registrosPorPagina);

            return PartialView("_Lista", listaPagina);
        }

        // GET: Autor/Crear
        public ActionResult Crear(bool EsRegistroObra = false)
        {
            var autor = new Autor()
            {
                Status = true
            };

            ViewBag.EsRegistroObra = EsRegistroObra;


            return PartialView("_Crear", autor);
        }

        // POST: Autor/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear([Bind(Include = "AutorID,Nombre,Apellido,LugarNacimiento,AnoNacimiento,LugarMuerte,AnoMuerte,Observaciones,Status,Temp")] Autor autor, bool EsRegistroObra)
        {
            //validar el nombre
            var aut = db.Autores.Select(a => new { a.Nombre, a.Apellido, a.AutorID }).FirstOrDefault(a => a.Nombre == autor.Nombre && a.Apellido == autor.Apellido);

            if (aut != null)
                ModelState.AddModelError("Nombre", "Nombre ya existe.");

            if (ModelState.IsValid)
            {
                autor.AutorID = Guid.NewGuid();
                db.Autores.Add(autor);
                db.SaveChanges();

                AlertaSuccess(string.Format("Autor: <b>{0} {1}</b> creado.", autor.Nombre, autor.Apellido), true);

                if (EsRegistroObra)
                {
                    return Json(new { success = true, nombre = autor.Nombre + " " + autor.Apellido, autorID = autor.AutorID });
                }
                else
                {
                    string url = Url.Action("Lista", "Autor");
                    return Json(new { success = true, url = url });
                }

            }

            ViewBag.EsRegistroObra = EsRegistroObra;

            return PartialView("_Crear", autor);
        }

        // GET: Autor/Editar/5
        public ActionResult Editar(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Autor autor = db.Autores.Find(id);
            if (autor == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Editar", autor);
        }

        // POST: Autor/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "AutorID,Nombre,Apellido,LugarNacimiento,AnoNacimiento,LugarMuerte,AnoMuerte,Observaciones,Status,Temp")] Autor autor)
        {
            //validar el nombre
            var aut = db.Autores.Select(a => new { a.Nombre, a.Apellido, a.AutorID }).FirstOrDefault(a => a.Nombre == autor.Nombre && a.Apellido == autor.Apellido);

            if (aut != null)
                if (aut.AutorID != autor.AutorID)
                    ModelState.AddModelError("Nombre", "Nombre ya existe.");

            if (ModelState.IsValid)
            {
                db.Entry(autor).State = EntityState.Modified;
                db.SaveChanges();

                AlertaInfo(string.Format("Autor: <b>{0} {1}</b> se editó.", autor.Nombre, autor.Apellido), true);
                string url = Url.Action("Lista", "Autor");
                return Json(new { success = true, url = url });
            }

            return PartialView("_Editar", autor);
        }

        // GET: Autor/Eliminar/5
        public ActionResult Eliminar(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Autor autor = db.Autores.Find(id);
            if (autor == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Eliminar", autor);
        }

        // POST: Autor/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarConfirmado(Guid id)
        {
            string btnValue = Request.Form["accionx"];

            Autor aut = db.Autores.Find(id);

            switch (btnValue)
            {
                case "deshabilitar":
                    aut.Status = false;
                    db.Entry(aut).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0} {1}</b>", aut.Nombre, aut.Apellido), true);
                    break;
                case "eliminar":
                    db.Autores.Remove(aut);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0} {1}</b>", aut.Nombre, aut.Apellido), true);
                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;
            }


            string url = Url.Action("Lista", "Autor");
            return Json(new { success = true, url = url });
        }


        public ActionResult GenerarLista(string Filtro = "", string TipoLista = "option")
        {

            List<Autor> lista = db.Autores.Select(a => a).Where(a => a.Status).ToList();


            if (!String.IsNullOrEmpty(Filtro))
            {
                Filtro = Filtro.ToLower();
                lista = lista.Where(a => a.Nombre.ToLower().Contains(Filtro)).ToList();
            }

            lista = lista.Select(a => new Autor() { AutorID = a.AutorID, Nombre = a.Nombre + " " + a.Apellido }).OrderBy(a => a.Nombre).ToList();


            switch (TipoLista)
            {
                case "Select":
                case "select":
                case "SELECT":
                    ViewBag.AutorID = new SelectList(lista, "AutorID", "Nombre");
                    return PartialView("_ListaSelect");

                default:

                    return Json(lista, JsonRequestBehavior.AllowGet);
            }

        }


        public JsonResult EsUnico(string Nombre, string Apellido, Guid? AutorID)
        {
            bool x = false;

            var aut = db.Autores.Select(a => new { a.AutorID, a.Nombre, a.Apellido }).SingleOrDefault(a => a.Nombre == Nombre && a.Apellido == Apellido);

            x = aut == null ? true : aut.AutorID == AutorID ? true : false;

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

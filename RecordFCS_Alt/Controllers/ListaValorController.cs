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
    public class ListaValorController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: ListaValor
        public ActionResult Index()
        {
            return RedirectToAction("Index", "TipoAtributo");

        }

        // GET: ListaValor/Detalles/5
        public ActionResult Lista(Guid? id, string FiltroActual, string Busqueda, int? Pagina)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            TipoAtributo tipoatt = db.TipoAtributos.Find(id);

            if (tipoatt == null) return HttpNotFound();

            if (Busqueda != null) Pagina = 1;
            else Busqueda = FiltroActual;

            ViewBag.FiltroActual = Busqueda;

            var lista = tipoatt.ListaValores.Select(a => a);

            if (!String.IsNullOrEmpty(Busqueda))
            {
                Busqueda = Busqueda.ToLower();
                lista = lista.Where(a => a.Valor.ToLower().Contains(Busqueda));
            }

            lista = lista.OrderBy(a => a.Valor);

            ViewBag.TipoAtributoID = tipoatt.TipoAtributoID;
            ViewBag.Nombre = tipoatt.Nombre;

            //paginador
            int registrosPorPagina = 25;
            int pagActual = 1;
            pagActual = Pagina.HasValue ? Convert.ToInt32(Pagina) : 1;

            IPagedList<ListaValor> listaPagina = lista.ToPagedList(pagActual, registrosPorPagina);

            return PartialView("_Lista", listaPagina);
        }

        // GET: ListaValor/Crear
        public ActionResult Crear(Guid? id, bool EsRegistroObra = false)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            TipoAtributo tipoatt = db.TipoAtributos.Find(id);

            if (tipoatt == null) return HttpNotFound();

            ListaValor listaValor = new ListaValor()
            {
                Status = true,
                TipoAtributoID = tipoatt.TipoAtributoID,
                TipoAtributo = tipoatt
            };

            ViewBag.EsRegistroObra = EsRegistroObra;
            
            return PartialView("_Crear", listaValor);
        }

        // POST: ListaValor/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear([Bind(Include = "ListaValorID,Valor,Status,Temp,TipoAtributoID")] ListaValor listaValor, bool EsRegistroObra)
        {
            var lv = db.ListaValores.Select(a => new { a.ListaValorID, a.Valor, a.TipoAtributoID }).FirstOrDefault(a => a.Valor == listaValor.Valor && a.TipoAtributoID == listaValor.TipoAtributoID);

            if (lv != null)
                ModelState.AddModelError("Valor", "Ya existe, intenta de nuevo.");

            if (ModelState.IsValid)
            {
                listaValor.ListaValorID = Guid.NewGuid();
                db.ListaValores.Add(listaValor);
                db.SaveChanges();

                var tipoatt = db.TipoAtributos.Find(listaValor.TipoAtributoID);

                AlertaSuccess(string.Format("{0}: <b>{1}</b> creado.", tipoatt.Nombre, listaValor.Valor), true);


                if (EsRegistroObra)
                {
                    return Json(new { success = true, valor = listaValor.Valor, tipoAtributoID = listaValor.TipoAtributoID, listaValorID = listaValor.ListaValorID });

                }
                else
                {
                    string url = Url.Action("Lista", "ListaValor", new { id = listaValor.TipoAtributoID });
                    return Json(new { success = true, url = url });
                }
                
            }

            ViewBag.EsRegistroObra = EsRegistroObra;

            return PartialView("_Crear", listaValor);
        }

        // GET: ListaValor/Editar/5
        public ActionResult Editar(Guid? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ListaValor listaValor = db.ListaValores.Find(id);

            if (listaValor == null) return HttpNotFound();

            return PartialView("_Editar", listaValor);
        }

        // POST: ListaValor/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "ListaValorID,Valor,Status,Temp,TipoAtributoID")] ListaValor listaValor)
        {
            var lv = db.ListaValores.Select(a => new { a.Valor, a.TipoAtributoID, a.ListaValorID }).FirstOrDefault(a => a.Valor == listaValor.Valor && a.TipoAtributoID == listaValor.TipoAtributoID);

            if (lv != null)
                if (lv.ListaValorID != listaValor.ListaValorID)
                    ModelState.AddModelError("Valor", "Ya existe, intenta de nuevo.");

            var tipoatt = db.TipoAtributos.Find(listaValor.TipoAtributoID);

            if (ModelState.IsValid)
            {
                db.Entry(listaValor).State = EntityState.Modified;
                db.SaveChanges();


                AlertaInfo(string.Format("{0}: <b>{1}</b> se editó.", tipoatt.Nombre, listaValor.Valor), true);

                string url = Url.Action("Lista", "ListaValor", new { id = listaValor.TipoAtributoID });
                return Json(new { success = true, url = url });
            }

            listaValor.TipoAtributo = tipoatt;

            return PartialView("_Editar", listaValor);
        }

        // GET: ListaValor/Eliminar/5
        public ActionResult Eliminar(Guid? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ListaValor listaValor = db.ListaValores.Find(id);

            if (listaValor == null) return HttpNotFound();

            return PartialView("_Eliminar", listaValor);
        }

        // POST: ListaValor/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarConfirmado(Guid id)
        {
            string btnValue = Request.Form["accionx"];

            ListaValor listaValor = db.ListaValores.Find(id);


            switch (btnValue)
            {
                case "deshabilitar":
                    listaValor.Status = false;
                    db.Entry(listaValor).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", listaValor.Valor), true);
                    break;
                case "eliminar":
                    db.ListaValores.Remove(listaValor);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", listaValor.Valor), true);
                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;
            }

            string url = Url.Action("Lista", "Tecnica", new { id = listaValor.TipoAtributoID });
            return Json(new { success = true, url = url });
        }



        public ActionResult GenerarLista(Guid? id, string Filtro = "", string TipoLista = "option")
        {


            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            TipoAtributo tipoatt = db.TipoAtributos.Find(id);

            if (tipoatt == null) return HttpNotFound();

            List<ListaValor> lista = tipoatt.ListaValores.Select(a => a).Where(a => a.Status).ToList();

            switch (TipoLista)
            {
                case "Select":
                case "select":
                case "SELECT":

                    if (!String.IsNullOrEmpty(Filtro))
                    {
                        Filtro = Filtro.ToLower();
                        lista = lista.Where(a => a.Valor.ToLower().Contains(Filtro)).ToList();
                    }

                    lista = lista.Select(a => new ListaValor() { ListaValorID = a.ListaValorID, Valor = a.Valor, TipoAtributoID = a.TipoAtributoID }).OrderBy(a => a.Valor).ToList();

                    ViewBag.ListaValorID = new SelectList(lista, "ListaValorID", "Valor");

                    return PartialView("_ListaSelect");


                default:

                    if (!String.IsNullOrEmpty(Filtro))
                    {
                        Filtro = Filtro.ToLower();
                        lista = lista.Where(a => a.Valor.ToLower().Contains(Filtro)).ToList();
                    }

                    
                    var x =  lista.Select(a => new { ListaValorID = a.ListaValorID, Valor = a.Valor }).OrderBy(a => a.Valor).ToList();

                    return Json(x, JsonRequestBehavior.AllowGet);
            }

        }


        public JsonResult EsUnico(string Valor, Guid? TipoAtributoID, Guid? ListaValorID)
        {
            bool x = false;

            var lv = db.ListaValores.SingleOrDefault(a => a.Valor == Valor && a.TipoAtributoID == TipoAtributoID);
            x = lv == null ? true : lv.ListaValorID == ListaValorID ? true : false;


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

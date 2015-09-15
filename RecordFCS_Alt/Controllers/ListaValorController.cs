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
    public class ListaValorController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: ListaValor
        [CustomAuthorize(permiso = "")]
        public ActionResult Index()
        {
            return RedirectToAction("Index", "TipoAtributo");

        }

        // GET: ListaValor/Detalles/5
        [CustomAuthorize(permiso = "")]
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
        [CustomAuthorize(permiso = "catNew")]
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
        [CustomAuthorize(permiso = "catNew")]
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
        [CustomAuthorize(permiso = "catEdit")]
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
        [CustomAuthorize(permiso = "catEdit")]
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
        [CustomAuthorize(permiso = "catDel")]
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
        [CustomAuthorize(permiso = "catDel")]
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


        [CustomAuthorize(permiso = "")]
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


                    var x = lista.Select(a => new { ListaValorID = a.ListaValorID, Valor = a.Valor }).OrderBy(a => a.Valor).ToList();

                    return Json(x, JsonRequestBehavior.AllowGet);
            }

        }

        [CustomAuthorize(permiso = "")]
        public JsonResult EsUnico(string Valor, Guid? TipoAtributoID, Guid? ListaValorID)
        {
            bool x = false;

            var lv = db.ListaValores.SingleOrDefault(a => a.Valor == Valor && a.TipoAtributoID == TipoAtributoID);
            x = lv == null ? true : lv.ListaValorID == ListaValorID ? true : false;


            return Json(x);
        }


        [CustomAuthorize(permiso = "")]
        public ActionResult ListaString(Guid idTipoAtributo, string busqueda, bool exacta)
        {
            TempData["listaValores"] = null;
            List<string> lista = new List<string>();

            if (idTipoAtributo == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            TipoAtributo tipoatt = db.TipoAtributos.Find(idTipoAtributo);

            if (tipoatt == null) return HttpNotFound();


            if (tipoatt.EsLista)
            {
                List<ListaValor> listado;

                if (exacta)
                {
                    listado = tipoatt.ListaValores.Where(a => a.Valor == busqueda).OrderBy(b => b.Valor).Take(10).ToList();
                }
                else
                {
                    listado = tipoatt.ListaValores.Where(a => a.Valor.Contains(busqueda)).OrderBy(b => b.Valor).Take(10).ToList();
                }

                foreach (var item in listado)
                {
                    lista.Add(item.Valor);
                }
            }
            else
            {
                List<AtributoPieza> campos;

                if (exacta)
                {
                    campos = db.AtributoPiezas.Where(a => a.Valor == busqueda && a.Atributo.TipoAtributoID == tipoatt.TipoAtributoID).OrderBy(b => b.Valor).Take(10).ToList();
                }
                else
                {
                    campos = db.AtributoPiezas.Where(a => a.Valor.Contains(busqueda) && a.Atributo.TipoAtributoID == tipoatt.TipoAtributoID).OrderBy(b => b.Valor).Take(10).ToList();

                }

                foreach (var item in campos)
                {
                    lista.Add(item.Valor);
                }
            }

            TempData["listaValores"] = lista.ToList();

            return RedirectToAction("RenderListaCoincidencias", "Buscador");
        }

        //POST: Buscador/AgregarFiltro
        [CustomAuthorize(permiso = "")]
        public ActionResult GenerarFiltro(Guid TipoAtributoID, string Filtro, string PalabraExacta)
        {
            var tipoAtt = db.TipoAtributos.Find(TipoAtributoID);

            if (tipoAtt != null || !String.IsNullOrEmpty(Filtro))
            {
                //extraer nombre para el LABEL
                ViewBag.nombre = tipoAtt.Nombre;
                ViewBag.id = tipoAtt.TipoAtributoID;
                ViewBag.valor = Filtro;

                if (PalabraExacta == "true")
                {
                    ViewBag.exacto = "checked";
                }

                return PartialView("_CampoMenuFiltro");
            }

            return PartialView("_CampoMenuFiltroError");
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RecordFCS_Alt.Models;

namespace RecordFCS_Alt.Controllers
{
    public class TipoAtributoController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: TipoAtributo
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Lista()
        {
            var lista = db.TipoAtributos.OrderBy(a => a.Orden).ToList();

            ViewBag.totalRegistros = lista.Count;

            return PartialView("_Lista", lista);
        }

        //// GET: TipoAtributo/Detalles/5
        public ActionResult Detalles(Guid? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            TipoAtributo tipoAtributo = db.TipoAtributos.Find(id);

            if (tipoAtributo == null) return HttpNotFound();
            if (!tipoAtributo.EsLista) return HttpNotFound();




            return View(tipoAtributo);
        }

        // GET: TipoAtributo/Crear
        public ActionResult Crear()
        {
            //Valores default
            var tipoAtt = new TipoAtributo()
            {
                DatoCS = "string",
                EsGenerico = true,
                EnBuscador = false,
                Orden = db.TipoAtributos.Count() + 1,
                EsLista = false,
                EsMultipleValor = false,
                PerteneceA = "Pieza",
                TablaSQL = "ListaValor",
                Status = true
            };

            return PartialView("_Crear", tipoAtt);
        }

        // POST: TipoAtributo/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(TipoAtributo tipoAtt)
        {
            //revalidar campo unico

            if (ModelState.IsValid)
            {
                tipoAtt.TipoAtributoID = Guid.NewGuid();
                db.TipoAtributos.Add(tipoAtt);
                db.SaveChanges();

                AlertaSuccess(string.Format("Tipo de Atributo: <b>{0}</b> creado.", tipoAtt.Nombre), true);

                string url = Url.Action("Lista", "TipoAtributo");
                return Json(new { success = true, url = url });
            }

            return PartialView("_Crear", tipoAtt);
        }

        // GET: TipoAtributo/Editar/5
        public ActionResult Editar(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoAtributo tipoAtt = db.TipoAtributos.Find(id);
            if (tipoAtt == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Editar", tipoAtt);
        }

        // POST: TipoAtributo/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(TipoAtributo tipoAtributo)
        {
            var ta = db.TipoAtributos.Select(a => new { a.TipoAtributoID, a.Nombre }).SingleOrDefault(a => a.Nombre == tipoAtributo.Nombre);


            if (ta != null)
                if (ta.TipoAtributoID != tipoAtributo.TipoAtributoID)
                    ModelState.AddModelError("Nombre", "Nombre ya existe.");



            if (ModelState.IsValid)
            {
                db.Entry(tipoAtributo).State = EntityState.Modified;
                db.SaveChanges();


                AlertaInfo(string.Format("Tipo de Atributo: <b>{0}</b> se editó.", tipoAtributo.Nombre), true);
                string url = Url.Action("Lista", "TipoAtributo");
                return Json(new { success = true, url = url });
            }

            return PartialView("_Editar", tipoAtributo);
        }

        // GET: TipoAtributo/Eliminar/5
        public ActionResult Eliminar(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoAtributo tipoAtributo = db.TipoAtributos.Find(id);
            if (tipoAtributo == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Eliminar", tipoAtributo);
        }

        // POST: TipoAtributo/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarConfirmado(Guid id)
        {
            string btnValue = Request.Form["accionx"];

            TipoAtributo tipoAtributo = db.TipoAtributos.Find(id);

            switch (btnValue)
            {
                case "deshabilitar":
                    tipoAtributo.Status = false;
                    db.Entry(tipoAtributo).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", tipoAtributo.Nombre), true);
                    break;
                case "eliminar":
                    db.TipoAtributos.Remove(tipoAtributo);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", tipoAtributo.Nombre), true);
                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;
            }


            string url = Url.Action("Lista", "TipoAtributo");
            return Json(new { success = true, url = url });
        }



        public JsonResult EsUnico(string Nombre, Guid? TipoAtributoID)
        {
            bool x = false;


            var ta = db.TipoAtributos.Select(a => new { a.TipoAtributoID, a.Nombre }).SingleOrDefault(a => a.Nombre == Nombre);

            x = ta == null ? true : ta.TipoAtributoID == TipoAtributoID ? true : false;

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

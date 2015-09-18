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
    public class AutorPiezaController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // POST: AutorPieza/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "attPNew")]
        public ActionResult Crear([Bind(Include = "PiezaID,AutorID,Status")] AutorPieza autorPieza, Guid AtributoID)
        {

            string renderID = "autor_" + autorPieza.PiezaID + "_";

            string texto = "";
            bool guardar = false;

            string valor = Request.Form["id_" + AtributoID].ToString();

            autorPieza.AutorID = new Guid(valor);

            //no existe el ListaValorID entonces actualizar el AtributoPiezaID con el ListaValorID
            if (db.AutorPiezas.Where(a => a.PiezaID == autorPieza.PiezaID && a.AutorID == autorPieza.AutorID).Count() == 0)
            {
                guardar = true;

                var autor = db.Autores.Find(autorPieza.AutorID);

                texto = autor.Nombre + " " + autor.Apellido;

                AlertaSuccess(string.Format("Autor: <b>{0}</b> se agregó.", autor.Nombre + "" + autor.Apellido), true);
                autorPieza.AutorID = autor.AutorID;

            }
            else
            {
                guardar = false;
                //alerta ya existe
            }


            if (guardar)
            {
                db.AutorPiezas.Add(autorPieza);
                db.SaveChanges();
                renderID += autorPieza.AutorID;
            }


            return Json(new { success = true, renderID = renderID, texto = texto, guardar = guardar });

        }


        // POST: AutorPieza/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "attPEdit")]
        public ActionResult Editar(AutorPieza autorPieza, Guid AtributoID, Guid LlaveID)
        {
            //validar errores y devolverlos a la vista
            string renderID = "autor_" + autorPieza.PiezaID + "_" + LlaveID;

            string texto = "";
            bool guardar = false;


            var autorPiezaAnterior = db.AutorPiezas.Find(autorPieza.PiezaID, LlaveID);

            if (autorPiezaAnterior == null)
            {
                guardar = false;
                //alerta no existe el atributo
            }
            else
            {
                string valor = Request.Form["id_" + AtributoID].ToString();

                autorPieza.AutorID = new Guid(valor);

                guardar = true;

                //no existe el ListaValorID entonces actualizar el AtributoPiezaID con el ListaValorID
                if (db.AutorPiezas.Where(a => a.PiezaID == autorPieza.PiezaID && a.AutorID == autorPieza.AutorID).Count() == 0)
                {

                    var autor = db.Autores.Find(autorPieza.AutorID);

                    var textoNombreAutor = autor.Nombre + " " + autor.Apellido;
                    var textoPrefijo = autorPieza.Prefijo == "" || autorPieza == null ? "" : autorPieza.Prefijo + ": ";

                    texto = string.Format("<span><b>{0}</b></span>{1}", textoPrefijo, textoNombreAutor);

                    AlertaSuccess(string.Format("Autor: <b>{0}</b> se actualizo a <b>{1}</b>.", autorPiezaAnterior.Autor.Nombre + "" + autorPiezaAnterior.Autor.Apellido, autor.Nombre + " " + autor.Apellido), true);
                    autorPieza.AutorID = autor.AutorID;

                }
                else
                {
                    //alerta ya existe
                    AlertaSuccess(string.Format("Autor: <b>{0}</b> se actualizo.", autorPiezaAnterior.Autor.Nombre + "" + autorPiezaAnterior.Autor.Apellido), true);

                }
            }

            if (guardar)
            {
                db.AutorPiezas.Remove(autorPiezaAnterior);
                db.SaveChanges();
                db.AutorPiezas.Add(autorPieza);
                db.SaveChanges();
            }


            return Json(new { success = true, renderID = renderID, texto = texto, guardar = guardar, tipo = "Autor" });
        }


        [CustomAuthorize(permiso = "attPDel")]
        public ActionResult Eliminar(Guid? id, Guid? AutorID)
        {
            if (id == null && AutorID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AutorPieza autorPieza = db.AutorPiezas.Find(id, AutorID);
            if (autorPieza == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Eliminar", autorPieza);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "attPDel")]
        public ActionResult EliminarConfirmado(Guid id, Guid AutorID)
        {
            string btnValue = Request.Form["accionx"];

            AutorPieza autorPieza = db.AutorPiezas.Find(id, AutorID);

            var NombreTexto = autorPieza.Autor.Nombre + " " + autorPieza.Autor.Apellido;

            switch (btnValue)
            {
                case "eliminar":
                    db.AutorPiezas.Remove(autorPieza);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", NombreTexto), true);
                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;
            }

            return Json(new { success = true, guardar = false });
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

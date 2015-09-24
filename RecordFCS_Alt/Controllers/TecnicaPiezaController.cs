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
    public class TecnicaPiezaController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();


        // POST: TecnicaPieza/Crear
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "attPNew")]
        public ActionResult Crear([Bind(Include = "PiezaID,TipoTecnicaID,Status,TecnicaID")] TecnicaPieza tecnicaPieza, Guid AtributoID)
        {
            string renderID = "tipoTecnica_" + tecnicaPieza.PiezaID + "_";
            string texto = "";
            bool guardar = false;
            string valor = Request.Form["id_" + AtributoID].ToString();

            tecnicaPieza.TecnicaID = new Guid(valor);

            if (db.TecnicaPiezas.Where(a => a.PiezaID == tecnicaPieza.PiezaID && a.TecnicaID == tecnicaPieza.TecnicaID).Count() == 0)
            {
                guardar = true;

                var tecnica = db.Tecnicas.Find(tecnicaPieza.TecnicaID);

                texto = tecnica.Descripcion;

                AlertaSuccess(string.Format("Técnica: <b>{0}</b> se agregó.", tecnica.Descripcion), true);
                tecnicaPieza.TecnicaID = tecnica.TecnicaID;

            }
            else
            {
                guardar = false;
                //alerta ya existe
            }




            if (guardar)
            {
                db.TecnicaPiezas.Add(tecnicaPieza);
                db.SaveChanges();

                renderID += tecnicaPieza.TecnicaID;
            }

            return Json(new { success = true, renderID = renderID, texto = texto, guardar = guardar });

        }


        // POST: TecnicaPieza/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "attPEdit")]
        public ActionResult Editar([Bind(Include = "PiezaID,TipoTecnicaID,Status,TecnicaID")] TecnicaPieza tecnicaPieza, Guid AtributoID, Guid LlaveID)
        {
            //validar errores y devolverlos a la vista

            //llave = TipoTecnicaID
            string renderID = "tipoTecnica_" + tecnicaPieza.PiezaID + "_" + LlaveID;

            string texto = "";
            bool guardar = false;


            var tecnicaPiezaAnterior = db.TecnicaPiezas.Find(tecnicaPieza.PiezaID, LlaveID);

            if (tecnicaPiezaAnterior == null)
            {
                guardar = false;
            }
            else
            {
                string valor = Request.Form["id_" + AtributoID].ToString();

                tecnicaPieza.TecnicaID = new Guid(valor);

                //no existe el entonces actualizar el AtributoPiezaID con el ListaValorID
                if (db.TecnicaPiezas.Where(a => a.PiezaID == tecnicaPieza.PiezaID && a.TipoTecnicaID == tecnicaPieza.TipoTecnicaID && a.TecnicaID == tecnicaPieza.TecnicaID).Count() == 0)
                {
                    guardar = true;

                    var tecnica = db.Tecnicas.Find(tecnicaPieza.TecnicaID);

                    texto = string.Format("<span><b>{0}: </b></span> {1}", tecnicaPiezaAnterior.TipoTecnica.Nombre, tecnica.Descripcion);

                    AlertaSuccess(string.Format("Técnica: <b>{0}</b> se actualizo a <b>{1}</b>.", tecnicaPiezaAnterior.Tecnica.Descripcion, tecnica.Descripcion), true);
                    tecnicaPieza.TecnicaID = tecnica.TecnicaID;
                }
                else
                {
                    guardar = false;
                    //alerta ya existe
                }


            }

            if (guardar)
            {
                db.TecnicaPiezas.Remove(tecnicaPiezaAnterior);
                db.SaveChanges();
                db.TecnicaPiezas.Add(tecnicaPieza);
                db.SaveChanges();
            }

            return Json(new { success = true, renderID = renderID, texto = texto, guardar = guardar });

        }

        // GET: TecnicaPieza/Eliminar/5
        [CustomAuthorize(permiso = "attPDel")]
        public ActionResult Eliminar(Guid? id, Guid? TipoTecnicaID)
        {
            if (id == null && TipoTecnicaID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TecnicaPieza tecnicaPieza = db.TecnicaPiezas.Find(id, TipoTecnicaID);
            if (tecnicaPieza == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Eliminar", tecnicaPieza);
        }

        // POST: TecnicaPieza/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "attPDel")]
        public ActionResult EliminarConfirmado(Guid id, Guid TipoTecnicaID)
        {
            string btnValue = Request.Form["accionx"];

            TecnicaPieza tecnicaPieza = db.TecnicaPiezas.Find(id, TipoTecnicaID);

            var NombreTexto = tecnicaPieza.Tecnica.Descripcion;

            switch (btnValue)
            {
                case "eliminar":
                    db.TecnicaPiezas.Remove(tecnicaPieza);
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

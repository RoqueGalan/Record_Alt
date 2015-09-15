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
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
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
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "attPEdit")]
        public ActionResult Editar(AutorPieza autorPieza, Guid AtributoID, Guid LlaveID)
        {
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

                //no existe el ListaValorID entonces actualizar el AtributoPiezaID con el ListaValorID
                if (db.AutorPiezas.Where(a => a.PiezaID == autorPieza.PiezaID && a.AutorID == autorPieza.AutorID).Count() == 0)
                {
                    guardar = true;

                    var autor = db.Autores.Find(autorPieza.AutorID);

                    texto = autor.Nombre + " " + autor.Apellido;

                    AlertaSuccess(string.Format("Autor: <b>{0}</b> se actualizo a <b>{1}</b>.", autorPiezaAnterior.Autor.Nombre + "" + autorPiezaAnterior.Autor.Apellido, autor.Nombre + " " + autor.Apellido), true);
                    autorPieza.AutorID = autor.AutorID;

                }
                else
                {
                    guardar = false;
                    //alerta ya existe
                }
            }

            if (guardar)
            {
                db.AutorPiezas.Remove(autorPiezaAnterior);
                db.SaveChanges();
                db.AutorPiezas.Add(autorPieza);
                db.SaveChanges();
            }


            return Json(new { success = true, renderID = renderID, texto = texto, guardar = guardar });
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

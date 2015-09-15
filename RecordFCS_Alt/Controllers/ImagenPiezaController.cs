using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RecordFCS_Alt.Models;
using System.IO;
using RecordFCS_Alt.Helpers;
using RecordFCS_Alt.Helpers.Seguridad;
using System.Drawing;

namespace RecordFCS_Alt.Controllers
{
    public class ImagenPiezaController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();
        
        [CustomAuthorize(permiso = "")]
        public ActionResult Carrusel(Guid? id, bool status = false, string tipo = "original")
        {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }

            Pieza pieza = db.Piezas.Find(id);

            if (pieza == null) { return HttpNotFound(); }


            if (status)
            {
                pieza.ImagenPiezas = pieza.ImagenPiezas.Where(i => i.Status == status).OrderBy(i => i.Orden).ToList();
            }
            else
            {
                pieza.ImagenPiezas = pieza.ImagenPiezas.OrderBy(i => i.Orden).ToList();
            }



            var vista = "_Carrusel";

            if (tipo == "thumb")
            {
                vista = "_CarruselThumb";
            }

            ViewBag.PiezaID = id;

            ViewBag.NoImagenes = pieza.ImagenPiezas.Count;

            ViewBag.CarruselID = "Carrusel_" + id;

            return PartialView(vista, pieza.ImagenPiezas);
        }



        [CustomAuthorize(permiso = "imgNew")]
        public ActionResult Crear(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var pieza = db.Piezas.Find(id);
            if (pieza == null)
            {
                return HttpNotFound();
            }

            var imagenPieza = new ImagenPieza()
            {
                PiezaID = pieza.PiezaID,
                Status = true
            };

            var listaImagenes = db.ImagenPiezas.Where(ip => ip.PiezaID == pieza.PiezaID).ToList();
            //GENERAR EN AUTOMATICO LA NUMERACION
            if (listaImagenes.Count == 0)
            {
                imagenPieza.Orden = 1;
                imagenPieza.EsPrincipal = true;
            }
            else
            {
                imagenPieza.Orden = listaImagenes.Count + 1;
                imagenPieza.EsPrincipal = false;
            }

            return PartialView("_Crear", imagenPieza);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "imgNew")]
        public ActionResult Crear(ImagenPieza imagenPieza, HttpPostedFileBase FileImagen)
        {

            var FileImageForm = FileImagen;



            if (ModelState.IsValid)
            {


                //guardar la imagen en carpeta
                string extension = Path.GetExtension(FileImageForm.FileName);
                imagenPieza.ImagenPiezaID = Guid.NewGuid();

                imagenPieza.NombreImagen = Guid.NewGuid().ToString() + extension;

                imagenPieza.RutaParcial = "/Content/img/pieza/";





                var rutaGuardar_Original = Server.MapPath(imagenPieza.Ruta);

                FileImageForm.SaveAs(rutaGuardar_Original);

                FileImageForm.InputStream.Dispose(); 
                FileImageForm.InputStream.Close(); 
                GC.Collect();

                ////Generar la mini
                Thumbnail mini = new Thumbnail()
                {
                    OrigenSrc = rutaGuardar_Original,
                    DestinoSrc = Server.MapPath(imagenPieza.RutaMini),
                    LimiteAnchoAlto = 250
                };

                mini.GuardarThumbnail();

                //add a la lista de imagenes
                

                //guardar en db
                db.ImagenPiezas.Add(imagenPieza);
                db.SaveChanges();

                AlertaSuccess(string.Format("Se guardo imagen <b>{0}</b>", imagenPieza.Titulo), true);


                string url = Url.Action("Carrusel", "ImagenPieza", new { id = imagenPieza.PiezaID, status = false, tipo = "thumb" });
                return Json(new { success = true, url = url, modelo = "ImagenPieza", lista = "lista", idPieza = imagenPieza.PiezaID });
            }

            return PartialView("_Crear", imagenPieza);
        }


        [CustomAuthorize(permiso = "imgEdit")]
        public ActionResult Editar(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ImagenPieza imagenPieza = db.ImagenPiezas.Find(id);
            if (imagenPieza == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Editar", imagenPieza);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "imgEdit")]
        public ActionResult Editar(ImagenPieza imagenPieza, HttpPostedFileBase FileImagen)
        {

            if (ModelState.IsValid)
            {
                if (FileImagen != null)
                {

                    FileInfo infoThumb = new FileInfo(Server.MapPath("~" + imagenPieza.RutaMini));
                    if (infoThumb.Exists)
                        infoThumb.Delete();

                    FileInfo infoOriginal = new FileInfo(Server.MapPath("~" + imagenPieza.Ruta));
                    if (infoOriginal.Exists)
                        infoOriginal.Delete();

                    infoOriginal = null;
                    infoThumb = null;

                    string extension = Path.GetExtension(FileImagen.FileName);


                    imagenPieza.RutaParcial = "/Content/img/pieza/";
                    imagenPieza.NombreImagen = Guid.NewGuid().ToString() + extension;
                    imagenPieza.Ruta = imagenPieza.RutaParcial + imagenPieza.NombreImagen;
                    imagenPieza.RutaMini = imagenPieza.RutaParcial + "thumb/" + imagenPieza.NombreImagen;
                    var rutaGuardar_Original = Server.MapPath(imagenPieza.Ruta);
                    FileImagen.SaveAs(rutaGuardar_Original);
                    //Generar la mini
                    Thumbnail mini = new Thumbnail()
                    {
                        OrigenSrc = rutaGuardar_Original,
                        DestinoSrc = Server.MapPath(imagenPieza.RutaMini),
                        LimiteAnchoAlto = 250
                    };

                    mini.GuardarThumbnail();

                }

                db.Entry(imagenPieza).State = EntityState.Modified;
                db.SaveChanges();

                AlertaInfo(string.Format("Se edito imagen <b>{0}</b>", imagenPieza.Titulo), true);

                string url = Url.Action("Carrusel", "ImagenPieza", new { id = imagenPieza.PiezaID, status = false, tipo = "thumb" });
                return Json(new { success = true, url = url, modelo = "ImagenPieza", lista = "lista", idPieza = imagenPieza.PiezaID });

            }

            return PartialView("_Editar", imagenPieza);
        }


        // GET: ImagenPieza/Eliminar/5
        [CustomAuthorize(permiso = "imgDel")]
        public ActionResult Eliminar(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ImagenPieza imagenPieza = db.ImagenPiezas.Find(id);
            if (imagenPieza == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Eliminar", imagenPieza);
        }


        // POST: ImagenPieza/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "imgDel")]
        public ActionResult EliminarConfirmado(Guid id)
        {
            string btnValue = Request.Form["accionx"];

            var imagenPieza = db.ImagenPiezas.Find(id);

            switch (btnValue)
            {
                case "deshabilitar":
                    imagenPieza.Status = false;
                    db.Entry(imagenPieza).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", imagenPieza.Titulo), true);

                    break;
                case "eliminar":
                    db.ImagenPiezas.Remove(imagenPieza);
                    db.SaveChanges();

                    //------------ Eliminar el archivo normal y el thumbnail

                    FileInfo infoThumb = new FileInfo(Server.MapPath("~" + imagenPieza.RutaMini));
                    if (infoThumb.Exists)
                        infoThumb.Delete();
                    else
                        AlertaWarning(string.Format("No se encontro imagen miniatura"), true);

                    FileInfo infoOriginal = new FileInfo(Server.MapPath("~" + imagenPieza.Ruta));
                    if (infoOriginal.Exists)
                        infoOriginal.Delete();
                    else
                        AlertaWarning(string.Format("No se encontro imagen original"), true);


                    // ----------------------------------

                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", imagenPieza.Titulo), true);

                    break;
                default:
                    AlertaDanger(string.Format("No seleccionaste ninguna accion"), true);
                    break;

            }

            string url = Url.Action("Carrusel", "ImagenPieza", new { id = imagenPieza.PiezaID, status = false, tipo = "thumb" });
            return Json(new { success = true, url = url, modelo = "ImagenPieza", lista = "lista", idPieza = imagenPieza.PiezaID });
        }



        //// GET: ImagenPieza/Detalles/5
        //public ActionResult Details(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ImagenPieza imagenPieza = db.ImagenPiezas.Find(id);
        //    if (imagenPieza == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(imagenPieza);
        //}

       

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

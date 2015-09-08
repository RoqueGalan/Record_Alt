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

namespace RecordFCS_Alt.Controllers
{
    public class ImagenPiezaController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

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
        public ActionResult Crear(ImagenPieza imagenPieza, HttpPostedFileBase FileImagen)
        {

            if (ModelState.IsValid)
            {


                //guardar la imagen en carpeta
                string extension = Path.GetExtension(FileImagen.FileName);
                imagenPieza.ImagenPiezaID = Guid.NewGuid();

                imagenPieza.NombreImagen = Guid.NewGuid().ToString() + extension;

                imagenPieza.RutaParcial = "/Content/img/pieza/";





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

                //add a la lista de imagenes
                FileImagen = null;
                mini = null;


                //guardar en db
                db.ImagenPiezas.Add(imagenPieza);
                db.SaveChanges();



                string url = Url.Action("Carrusel", "ImagenPieza", new { id = imagenPieza.PiezaID, status = false, tipo = "thumb" });
                return Json(new { success = true, url = url, modelo = "ImagenPieza", lista = "lista", idPieza = imagenPieza.PiezaID });
            }

            return PartialView("_Crear", imagenPieza);
        }



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

                string url = Url.Action("Carrusel", "ImagenPieza", new { id = imagenPieza.PiezaID, status = false, tipo = "thumb" });
                return Json(new { success = true, url = url, modelo = "ImagenPieza", lista = "lista", idPieza = imagenPieza.PiezaID });

            }

            return PartialView("_Editar", imagenPieza);
        }


        // GET: ImagenPieza/Eliminar/5
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



        // GET: ImagenPieza/Detalles/5
        public ActionResult Details(Guid? id)
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
            return View(imagenPieza);
        }

        // GET: ImagenPieza/Create
        public ActionResult Create()
        {
            ViewBag.PiezaID = new SelectList(db.Piezas, "PiezaID", "SubFolio");
            return View();
        }

        // POST: ImagenPieza/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ImagenPiezaID,Orden,Titulo,Descripcion,EsPrincipal,RutaParcial,NombreImagen,Status,PiezaID")] ImagenPieza imagenPieza)
        {
            if (ModelState.IsValid)
            {
                imagenPieza.ImagenPiezaID = Guid.NewGuid();
                db.ImagenPiezas.Add(imagenPieza);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PiezaID = new SelectList(db.Piezas, "PiezaID", "SubFolio", imagenPieza.PiezaID);
            return View(imagenPieza);
        }

        // GET: ImagenPieza/Edit/5
        public ActionResult Edit(Guid? id)
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
            ViewBag.PiezaID = new SelectList(db.Piezas, "PiezaID", "SubFolio", imagenPieza.PiezaID);
            return View(imagenPieza);
        }

        // POST: ImagenPieza/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ImagenPiezaID,Orden,Titulo,Descripcion,EsPrincipal,RutaParcial,NombreImagen,Status,PiezaID")] ImagenPieza imagenPieza)
        {
            if (ModelState.IsValid)
            {
                db.Entry(imagenPieza).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PiezaID = new SelectList(db.Piezas, "PiezaID", "SubFolio", imagenPieza.PiezaID);
            return View(imagenPieza);
        }

        // GET: ImagenPieza/Delete/5
        public ActionResult Delete(Guid? id)
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
            return View(imagenPieza);
        }

        // POST: ImagenPieza/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ImagenPieza imagenPieza = db.ImagenPiezas.Find(id);
            db.ImagenPiezas.Remove(imagenPieza);
            db.SaveChanges();
            return RedirectToAction("Index");
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

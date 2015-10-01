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
using System.Text.RegularExpressions;
using System.IO;
using RecordFCS_Alt.Helpers;

namespace RecordFCS_Alt.Controllers
{
    public class PiezaController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: Pieza/Crear
        [CustomAuthorize(permiso = "pNew")]
        public ActionResult Crear(Guid? id, Guid? TipoPiezaID)
        {
            if (id == null || TipoPiezaID == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Pieza p = db.Piezas.Find(id);
            TipoPieza tp = db.TipoPiezas.Find(TipoPiezaID);

            if (p == null || tp == null)
                return HttpNotFound();

            //Calcular el numero del folio
            int numero = p.PiezasHijas.Where(a => a.TipoPiezaID == tp.TipoPiezaID).Count() + 1;
            string numeroTexto = numero > 1 ? numero.ToString() : "";

            Pieza pieza = new Pieza()
            {
                FechaRegistro = DateTime.Now,
                ObraID = p.ObraID,
                Obra = p.Obra,
                PiezaPadre = p,
                PiezaPadreID = p.PiezaID,
                Status = true,
                TipoPiezaID = tp.TipoPiezaID,
                TipoPieza = tp,
                SubFolio = tp.Prefijo + numeroTexto
            };




            return PartialView("_Crear", pieza);
        }

        // POST: Pieza/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "pNew")]
        public ActionResult Crear([Bind(Include = "PiezaID,SubFolio,FechaRegistro,Status,ObraID,TipoPiezaID,UbicacionID,PiezaPadreID,Temp")] Pieza pieza)
        {
            var Formulario = Request.Form;

            Pieza piezaPadre = db.Piezas.Find(pieza.PiezaPadreID);
            TipoPieza tipoPieza = db.TipoPiezas.Find(pieza.TipoPiezaID);


            //Calcular el numero del folio


            pieza.PiezaID = Guid.NewGuid();
            pieza.FechaRegistro = DateTime.Now;


            var listaAttRegistro = tipoPieza.Atributos.Where(a => a.Status && a.MostrarAtributos.Any(b => b.TipoMostrar.Nombre == "Registro" && b.Status) && a.TipoAtributo.Status).OrderBy(a => a.Orden).ToList();

            List<AtributoPieza> listaAdd_AttGen = new List<AtributoPieza>();
            List<AutorPieza> listaAdd_AttAutor = new List<AutorPieza>();
            List<ImagenPieza> listaAdd_AttImg = new List<ImagenPieza>();
            List<TecnicaPieza> listaAdd_AttTec = new List<TecnicaPieza>();
            List<MedidaPieza> listaAdd_AttMed = new List<MedidaPieza>();
            Ubicacion ubicacionAdd = null;


            List<string> listaKey;

            /*
             * Extraer los registros del formulario dependiendo el tipo de Atributo
             * 
             * IMAGEN
             *      SIMPLE
             *          id_####################_File        (File)
             *          id_####################_Titulo      (Input)
             *          id_####################_Descripcion (Input)
             *          
             
             *          
             
             */






            foreach (var att in listaAttRegistro)
            {
                var tipoAtt = att.TipoAtributo;

                if (tipoAtt.EsGenerico)
                {
                    /*
                     * GENERICO
                     *      LISTA
                     *          SIMPLE
                     *              id_#################### (Select)
                     *          MULTI
                     *              id_####################_#################### (Input)
                     */
                    if (tipoAtt.EsLista)
                    {

                        if (tipoAtt.EsMultipleValor)
                            listaKey = Formulario.AllKeys.Where(k => k.StartsWith("id_" + att.AtributoID + "_")).ToList();
                        else
                            listaKey = Formulario.AllKeys.Where(k => k.StartsWith("id_" + att.AtributoID)).ToList();

                        //buscar en form todas las llaves que correspondan al id_xxxxxxxxxxxxxx_xxxxxxxxxxxxxx
                        foreach (string key in listaKey)
                        {
                            var addOk = true;
                            string valor = Formulario[key];


                            addOk = String.IsNullOrWhiteSpace(valor) ? false : true;

                            //validar el valorID, buscar el valor
                            Guid valorID = addOk ? new Guid(valor) : new Guid(new Byte[16]);


                            addOk = !addOk ? addOk : listaAdd_AttGen.Where(a => a.AtributoID == att.AtributoID && a.ListaValorID == valorID).FirstOrDefault() == null ? true : false;

                            addOk = !addOk ? addOk : db.ListaValores.Where(a => a.TipoAtributoID == tipoAtt.TipoAtributoID && a.Status && a.ListaValorID == valorID).FirstOrDefault() == null ? false : true;

                            if (addOk)
                                listaAdd_AttGen.Add(new AtributoPieza()
                                {
                                    AtributoPiezaID = Guid.NewGuid(),
                                    AtributoID = att.AtributoID,
                                    PiezaID = pieza.PiezaID,
                                    Status = true,
                                    ListaValorID = valorID
                                });
                        }
                    }
                    else
                    {
                        /*
                         * GENERICO
                         *    CAMPO
                         *        SIMPLE  
                         *            id_#################### (Input)
                         *        MULTI   
                         *            id_####################_##### (Input)
                         */

                        if (tipoAtt.EsMultipleValor)
                            listaKey = Formulario.AllKeys.Where(k => k.StartsWith("id_" + att.AtributoID + "_")).ToList();
                        else
                            listaKey = Formulario.AllKeys.Where(k => k.StartsWith("id_" + att.AtributoID)).ToList();


                        //buscar en form todas las llaves que correspondan al id_xxxxxxxxxxxxxx
                        foreach (string key in listaKey)
                        {
                            var addOk = true;
                            string valor = Formulario[key];

                            //validar el campo, quitar espacios en blanco, bla bla bla
                            valor = valor.Trim(); // quitar espacios en inicio y fin
                            valor = Regex.Replace(valor, @"\s+", " "); //quitar espacios de sobra

                            addOk = String.IsNullOrWhiteSpace(valor) ? false : true;
                            addOk = !addOk ? addOk : listaAdd_AttGen.Where(a => a.AtributoID == att.AtributoID && a.Valor == valor).FirstOrDefault() == null ? true : false;

                            if (addOk)
                                listaAdd_AttGen.Add(new AtributoPieza()
                                {
                                    AtributoPiezaID = Guid.NewGuid(),
                                    AtributoID = att.AtributoID,
                                    PiezaID = pieza.PiezaID,
                                    Status = true,
                                    Valor = valor
                                });

                        }
                    }
                }
                else
                {
                    switch (tipoAtt.TablaSQL)
                    {
                        case "Autor":
                            /*
                                * AUTOR
                                *      MULTIPLE
                                *          id_####################_####################            (Input)
                                *          id_####################_####################_prefijo    (Input)
                            */
                            //filtrar id_#######
                            listaKey = Formulario.AllKeys.Where(k => k.StartsWith("id_" + att.AtributoID + "_")).ToList();

                            ///filtrar: ignorar los _prefijo
                            listaKey = listaKey.Where(k => !k.EndsWith("_prefijo")).ToList();

                            //buscar en form todas las llaves que correspondan al id_xxxxxxxxxxxxxx_xxxxxxxxxxxxxx
                            foreach (string key in listaKey)
                            {
                                var addOk = true;
                                string text_autorID = Formulario[key];
                                string text_prefijo = Formulario[key + "_prefijo"];

                                addOk = String.IsNullOrWhiteSpace(text_autorID) ? false : true;

                                //validar el valorID, buscar el valor
                                Guid autorID = addOk ? new Guid(text_autorID) : new Guid(new Byte[16]);

                                addOk = !addOk ? addOk : listaAdd_AttAutor.Where(a => a.AutorID == autorID).FirstOrDefault() == null ? true : false;

                                addOk = !addOk ? addOk : db.Autores.Where(a => a.Status && a.AutorID == autorID).FirstOrDefault() == null ? false : true;

                                if (addOk)
                                {
                                    var autorPieza = new AutorPieza()
                                    {
                                        AutorID = autorID,
                                        PiezaID = pieza.PiezaID,
                                        esPrincipal = false,
                                        Prefijo = text_prefijo,
                                        Status = true
                                    };

                                    //validar si es principal
                                    if (autorPieza.Prefijo.ToLower() == "principal")
                                        autorPieza.esPrincipal = listaAdd_AttAutor.Where(a => a.esPrincipal).Count() == 0 ? true : false;

                                    listaAdd_AttAutor.Add(autorPieza);
                                }
                            }
                            break;


                        case "Ubicacion":
                            /*
                                * UBICACION
                                *      SIMPLE  
                                *          id_####################     (select)
                            */

                            listaKey = Formulario.AllKeys.Where(k => k.StartsWith("id_" + att.AtributoID)).ToList();

                            //buscar en form todas las llaves que correspondan al id_xxxxxxxxxxxxxx_xxxxxxxxxxxxxx
                            foreach (string key in listaKey)
                            {
                                var addOk = true;
                                string texto_ubicacionID = Formulario[key];

                                addOk = String.IsNullOrWhiteSpace(texto_ubicacionID) ? false : true;

                                //validar el valorID, buscar el valor
                                Guid ubicacionID = addOk ? new Guid(texto_ubicacionID) : new Guid(new Byte[16]);

                                addOk = !addOk ? addOk : ubicacionAdd == null ? true : false;

                                addOk = !addOk ? addOk : db.Ubicaciones.Where(a => a.Status && a.UbicacionID == ubicacionID).FirstOrDefault() == null ? false : true;

                                if (addOk)
                                    pieza.UbicacionID = ubicacionID;
                            }
                            break;

                        case "TipoTecnica":
                            /*
                                * TECNICA
                                *      SIMPLE
                                *          id_####################     (Select)
                             */

                            listaKey = Formulario.AllKeys.Where(k => k.StartsWith("id_" + att.AtributoID)).ToList();

                            //buscar en form todas las llaves que correspondan al id_xxxxxxxxxxxxxx_xxxxxxxxxxxxxx
                            foreach (string key in listaKey)
                            {
                                var addOk = true;
                                string texto_TecnicaID = Formulario[key];

                                addOk = String.IsNullOrWhiteSpace(texto_TecnicaID) ? false : true;

                                //validar el valorID, buscar el valor
                                Guid tecnicaID = addOk ? new Guid(texto_TecnicaID) : new Guid(new Byte[16]);

                                addOk = !addOk ? addOk : listaAdd_AttTec.Where(a => a.TecnicaID == tecnicaID).FirstOrDefault() == null ? true : false;

                                addOk = !addOk ? addOk : db.Tecnicas.Where(a => a.TecnicaID == tecnicaID && a.Status).FirstOrDefault() == null ? false : true;

                                if (addOk)
                                {
                                    var tecnica = db.Tecnicas.Where(a => a.TecnicaID == tecnicaID && a.Status).FirstOrDefault();

                                    listaAdd_AttTec.Add(new TecnicaPieza()
                                    {
                                        PiezaID = pieza.PiezaID,
                                        Status = true,
                                        TecnicaID = tecnica.TecnicaID,
                                        TipoTecnicaID = tecnica.TipoTecnicaID
                                    });
                                }
                            }


                            break;

                        case "TipoMedida":
                            /* 
                                * TIPO MEDIDA
                                *      SIMPLE
                                *          id_####################                 (Select)(TipoMedida)
                                *          id_####################_UML             (Select)
                                *          id_####################_Altura          (input)
                                *          id_####################_Anchura         (input)
                                *          id_####################_Profundidad     (input)
                                *          id_####################_Diametro        (input)
                                *          id_####################_Diametro2       (input)
                            */

                            listaKey = Formulario.AllKeys.Where(k => k == "TipoMedidaID").ToList();

                            //buscar en form todas las llaves que correspondan al id_xxxxxxxxxxxxxx_xxxxxxxxxxxxxx
                            foreach (string key in listaKey)
                            {
                                var addOk = true;
                                string texto_TipoMedidaID = Formulario[key];

                                addOk = String.IsNullOrWhiteSpace(texto_TipoMedidaID) ? false : true;

                                //validar el valorID, buscar el valor
                                Guid tipoMedidaID = addOk ? new Guid(texto_TipoMedidaID) : new Guid(new Byte[16]);

                                addOk = !addOk ? addOk : listaAdd_AttMed.Where(a => a.TipoMedidaID == tipoMedidaID).FirstOrDefault() == null ? true : false;

                                addOk = !addOk ? addOk : db.TipoMedidas.Where(a => a.TipoMedidaID == tipoMedidaID && a.Status).FirstOrDefault() == null ? false : true;

                                if (addOk)
                                {
                                    var medidaPieza = new MedidaPieza()
                                    {
                                        PiezaID = pieza.PiezaID,
                                        Status = true,
                                        TipoMedidaID = tipoMedidaID
                                    };

                                    string text_UML = String.IsNullOrWhiteSpace(Formulario["id_" + att.AtributoID + "_UML"]) ? "cm" : Formulario["id_" + att.AtributoID + "_UML"];
                                    string text_Altura = String.IsNullOrWhiteSpace(Formulario["id_" + att.AtributoID + "_Altura"]) ? "0" : Formulario["id_" + att.AtributoID + "_Altura"];
                                    string text_Anchura = String.IsNullOrWhiteSpace(Formulario["id_" + att.AtributoID + "_Anchura"]) ? "0" : Formulario["id_" + att.AtributoID + "_Anchura"];
                                    string text_Profundidad = String.IsNullOrWhiteSpace(Formulario["id_" + att.AtributoID + "_Profundidad"]) ? "0" : Formulario["id_" + att.AtributoID + "_Profundidad"];
                                    string text_Diametro = String.IsNullOrWhiteSpace(Formulario["id_" + att.AtributoID + "_Diametro"]) ? "0" : Formulario["id_" + att.AtributoID + "_Diametro"];
                                    string text_Diametro2 = String.IsNullOrWhiteSpace(Formulario["id_" + att.AtributoID + "_Diametro2"]) ? "0" : Formulario["id_" + att.AtributoID + "_Diametro2"];

                                    if (text_Altura == "0") medidaPieza.Altura = Convert.ToDouble("text_Altura");
                                    if (text_Anchura == "0") medidaPieza.Anchura = Convert.ToDouble("text_Anchura");
                                    if (text_Altura == "0") medidaPieza.Profundidad = Convert.ToDouble("text_Profundidad");
                                    if (text_Altura == "0") medidaPieza.Diametro = Convert.ToDouble("text_Diametro");
                                    if (text_Altura == "0") medidaPieza.Diametro2 = Convert.ToDouble("text_Diametro2");

                                    switch (text_UML)
                                    {
                                        case "pulgada": medidaPieza.UMLongitud = UMLongitud.pulgada; break;
                                        case "dc": medidaPieza.UMLongitud = UMLongitud.dc; break;
                                        case "m": medidaPieza.UMLongitud = UMLongitud.m; break;
                                        case "dam": medidaPieza.UMLongitud = UMLongitud.dam; break;
                                        case "mm": medidaPieza.UMLongitud = UMLongitud.mm; break;
                                        case "hm": medidaPieza.UMLongitud = UMLongitud.hm; break;
                                        case "km": medidaPieza.UMLongitud = UMLongitud.km; break;
                                        default: medidaPieza.UMLongitud = UMLongitud.cm; break;

                                    }

                                    listaAdd_AttMed.Add(medidaPieza);
                                }
                            }



                            break;

                        case "ImagenPieza":

                            listaKey = Request.Files.AllKeys.Where(k => k == "id_" + att.AtributoID + "_File").ToList();

                            //buscar en form todas las llaves que correspondan al id_xxxxxxxxxxxxxx_xxxxxxxxxxxxxx
                            foreach (string key in listaKey)
                            {
                                HttpPostedFileBase FileImageForm = Request.Files[key];

                                string texto_Titulo = Formulario["id_" + att.AtributoID + "_Titulo"];
                                string texto_Descripcion = Formulario["id_" + att.AtributoID + "_Descripcion"];
                                string extension = Path.GetExtension(FileImageForm.FileName);

                                var imgGuid = Guid.NewGuid();

                                ImagenPieza imagenPieza = new ImagenPieza()
                                {
                                    PiezaID = pieza.PiezaID,
                                    ImagenPiezaID = imgGuid,
                                    Titulo = texto_Titulo,
                                    Descripcion = texto_Descripcion,
                                    EsPrincipal = true,
                                    Orden = 1,
                                    Status = true,
                                    RutaParcial = "/Content/img/pieza/",
                                    NombreImagen = imgGuid.ToString() + extension,
                                };

                                var rutaGuardar_Original = Server.MapPath(imagenPieza.Ruta);

                                FileImageForm.SaveAs(rutaGuardar_Original);


                                FileImageForm.InputStream.Dispose();
                                FileImageForm.InputStream.Close();
                                GC.Collect();

                                //Generar la mini
                                Thumbnail mini = new Thumbnail()
                                {
                                    OrigenSrc = rutaGuardar_Original,
                                    DestinoSrc = Server.MapPath(imagenPieza.RutaMini),
                                    LimiteAnchoAlto = 250
                                };

                                mini.GuardarThumbnail();

                                //add a la lista de imagenes

                                listaAdd_AttImg.Add(imagenPieza);
                            }

                            break;

                        default:
                            AlertaDanger(String.Format("No se pudo guardar el campo, {0}.", att.NombreAlterno));
                            break;
                    }

                }



            }



            if (ModelState.IsValid)
            {

                int numero = piezaPadre.PiezasHijas.Where(a => a.TipoPiezaID == tipoPieza.TipoPiezaID).Count() + 1;
                string numeroTexto = numero > 1 ? numero.ToString() : "";
                pieza.SubFolio = tipoPieza.Prefijo + numeroTexto;

                //Guardar la pieza
                db.Piezas.Add(pieza);
                db.SaveChanges();

                //Guardar sus atributos
                db.AtributoPiezas.AddRange(listaAdd_AttGen);
                db.AutorPiezas.AddRange(listaAdd_AttAutor);
                db.ImagenPiezas.AddRange(listaAdd_AttImg);
                db.TecnicaPiezas.AddRange(listaAdd_AttTec);
                db.MedidaPiezas.AddRange(listaAdd_AttMed);

                db.SaveChanges();

                return Json(new { success = true });
            }

            return Json(new { success = false });

        }

        //// GET: Pieza/Editar/5
        //[CustomAuthorize(permiso = "")]

        //public ActionResult Edit(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Pieza pieza = db.Piezas.Find(id);
        //    if (pieza == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.ObraID = new SelectList(db.Obras, "ObraID", "Temp", pieza.ObraID);
        //    ViewBag.PiezaPadreID = new SelectList(db.Piezas, "PiezaID", "SubFolio", pieza.PiezaPadreID);
        //    ViewBag.TipoPiezaID = new SelectList(db.TipoPiezas, "TipoPiezaID", "Nombre", pieza.TipoPiezaID);
        //    ViewBag.UbicacionID = new SelectList(db.Ubicaciones, "UbicacionID", "Nombre", pieza.UbicacionID);
        //    return View(pieza);
        //}

        //// POST: Pieza/Editar/5
        //// Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        //// más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[CustomAuthorize(permiso = "")]

        //public ActionResult Edit([Bind(Include = "PiezaID,SubFolio,FechaRegistro,Status,ObraID,TipoPiezaID,UbicacionID,PiezaPadreID,Temp")] Pieza pieza)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(pieza).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.ObraID = new SelectList(db.Obras, "ObraID", "Temp", pieza.ObraID);
        //    ViewBag.PiezaPadreID = new SelectList(db.Piezas, "PiezaID", "SubFolio", pieza.PiezaPadreID);
        //    ViewBag.TipoPiezaID = new SelectList(db.TipoPiezas, "TipoPiezaID", "Nombre", pieza.TipoPiezaID);
        //    ViewBag.UbicacionID = new SelectList(db.Ubicaciones, "UbicacionID", "Nombre", pieza.UbicacionID);
        //    return View(pieza);
        //}

        // GET: Pieza/Eliminar/5
        [CustomAuthorize(permiso = "pDel")]
        public ActionResult Eliminar(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pieza pieza = db.Piezas.Find(id);
            if (pieza == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Eliminar", pieza);
        }

        // POST: Pieza/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "pDel")]
        public ActionResult EliminarConfirmado(Guid id)
        {
            string btnValue = Request.Form["accionx"];
            Pieza pieza = db.Piezas.Find(id);

            string folioPieza = pieza.ImprimirFolio();
            switch (btnValue)
            {
                case "deshabilitar":
                    pieza.Status = false;
                    db.Entry(pieza).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", folioPieza), true);

                    break;
                case "eliminar":
                    db.Piezas.Remove(pieza);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", folioPieza), true);

                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;

            }
            return Json(new { success = true });
        }


        [CustomAuthorize(permiso = "")]
        public ActionResult Ficha(Guid? id, string tipo = "basica")
        {
            string tipoCarusel = "thumb";
            string vista = "_Ficha";

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Pieza pieza = db.Piezas.Find(id);

            if (pieza == null)
                return HttpNotFound();

            //extraer los campos del tipo de obra
            
            switch (tipo.ToLower())
            {
                case "completa":
                    tipo = "Ficha Completa";
                    vista = "_Ficha";
                    break;
                case "basica":
                    tipo = "Ficha Básica";
                    vista = "_Ficha";
                    break;

                case "guion":
                    tipo = "Guion";
                    tipoCarusel = "miniThumb";
                    vista = "_FichaMini";
                    break;
            }


            var listaAttributosFichaCompleta = pieza.TipoPieza.Atributos.Where(a => a.Status && a.MostrarAtributos.Any(b => b.TipoMostrar.Nombre == tipo && b.Status) && a.TipoAtributo.Status).OrderBy(a => a.Orden).ToList();

            pieza.TipoPieza.TipoPiezasHijas = pieza.TipoPieza.TipoPiezasHijas.Where(a => a.Status).OrderBy(a => a.Orden).ToList();

            ViewBag.listaAttributosFichaCompleta = listaAttributosFichaCompleta;


            ViewBag.TipoFicha = tipo;
            ViewBag.tipoCarusel = tipoCarusel;

            ViewBag.esCompleta = tipo == "Ficha Completa" ? true : false;


            pieza.PiezasHijas = pieza.PiezasHijas.OrderBy(a => a.SubFolio).ToList();



            return PartialView(vista, pieza);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "attUbiEdit")]
        public ActionResult CrearUbicacion([Bind(Include = "PiezaID,SubFolio,FechaRegistro,Status,ObraID,TipoPiezaID,UbicacionID,PiezaPadreID,Temp")] Pieza pieza, Guid AtributoID)
        {
            string renderID = "ubicacion_" + pieza.PiezaID + "_";

            string texto = "";
            bool guardar = false;

            string valor = Request.Form["id_" + AtributoID].ToString();

            var ubicacion = db.Ubicaciones.Find(new Guid(valor));

            if (ubicacion != null)
            {
                texto = ubicacion.Nombre;
                pieza.UbicacionID = ubicacion.UbicacionID;
            }

            if (ModelState.IsValid)
            {
                guardar = true;
                db.Entry(pieza).State = EntityState.Modified;
                db.SaveChanges();

                renderID += ubicacion.UbicacionID;
                texto = ubicacion.Nombre;

                AlertaSuccess(string.Format("{0}: <b>{1}</b> se creó.", "Ubicación", ubicacion.Nombre), true);

                return Json(new { success = true, renderID = renderID, texto = texto, guardar = guardar });

            }

            ViewBag.UbicacionID = new SelectList(db.Ubicaciones.Where(a => a.Status).OrderBy(a => a.Nombre).Take(100).ToList(), "UbicacionID", "Nombre", pieza.UbicacionID);

            return PartialView("_CrearUbicacion", pieza);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "attUbiEdit")]
        public ActionResult EditarUbicacion([Bind(Include = "PiezaID,SubFolio,FechaRegistro,Status,ObraID,TipoPiezaID,UbicacionID,PiezaPadreID,Temp")] Pieza pieza, Guid AtributoID, Guid LlaveID)
        {
            //validar errores y devolverlos a la vista
            string renderID = "ubicacion_" + pieza.PiezaID + "_" + LlaveID;

            string texto = "";
            bool guardar = false;


            var ubicacionAnterior = db.Ubicaciones.Find(LlaveID);

            if (ubicacionAnterior == null)
            {
                guardar = false;
            }
            else
            {


                string valor = Request.Form["id_" + AtributoID].ToString();
                var ubicacion = db.Ubicaciones.Find(new Guid(valor));

                if (ubicacion != null)
                {
                    texto = ubicacion.Nombre;
                    pieza.UbicacionID = ubicacion.UbicacionID;
                }

                if (ModelState.IsValid)
                {
                    guardar = true;
                    db.Entry(pieza).State = EntityState.Modified;
                    db.SaveChanges();

                    renderID += ubicacion.UbicacionID;

                    texto = ubicacion.Nombre;
                    AlertaSuccess(string.Format("{0}: <b>{1}</b> se actualizo a <b>{2}</b>.", "Ubicación", ubicacionAnterior.Nombre, ubicacion.Nombre), true);


                    return Json(new { success = true, renderID = renderID, texto = texto, guardar = guardar });
                }
            }





            ViewBag.UbicacionID = new SelectList(db.Ubicaciones.Where(a => a.Status).OrderBy(a => a.Nombre).Take(100).ToList(), "UbicacionID", "Nombre", pieza.UbicacionID);

            return PartialView("_EditarUbicacion", pieza);
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

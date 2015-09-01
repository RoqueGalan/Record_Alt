using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RecordFCS_Alt.Models;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using RecordFCS_Alt.Helpers;

namespace RecordFCS_Alt.Controllers
{
    public class ObraController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: Obra
        public ActionResult Index()
        {
            return View(db.Obras.ToList());
        }

        //GET: Obra/Detalles/5
        public ActionResult Detalles(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Obra obra = db.Obras.Find(id);

            if (obra == null)
                return HttpNotFound();

            ViewBag.FolioCompleto = obra.LetraFolio.Nombre + "" + obra.NumeroFolio;

            return View(obra);
        }

        // GET: Obra/Registrar
        public ActionResult Registrar()
        {
            var listaLetras = db.LetraFolios.Select(a => new { a.LetraFolioID, Nombre = a.Nombre + " - " + a.Descripcion, a.Status }).Where(a => a.Status).OrderBy(a => a.Nombre);
            var listaTipoObras = db.TipoObras.Select(a => new { a.TipoObraID, Nombre = a.Nombre + " - " + a.Descripcion, a.Status }).Where(a => a.Status).OrderBy(a => a.Nombre);
            ViewBag.LetraFolioID = new SelectList(listaLetras, "LetraFolioID", "Nombre", listaLetras.FirstOrDefault().LetraFolioID);
            ViewBag.TipoObraID = new SelectList(listaTipoObras, "TipoObraID", "Nombre");

            return View();
        }


        public int DarFolioValido(int LetraFolioID, int Numero, bool segunda = false)
        {
            int timeDormirMiliSeg = 2000; //2 segundo
            Thread.Sleep(timeDormirMiliSeg);

            var existe = true;

            int i = Numero - 1 <= 0 ? 0 : Numero - 1;
            int? temp = null;

            do
            {
                i++;
                temp = db.Obras.Where(a => a.LetraFolioID == LetraFolioID && a.NumeroFolio == i).Select(a => a.NumeroFolio).FirstOrDefault();

                //cuando num = 0 es por que no existe 
                existe = temp == null || temp == 0 ? false : true;

            } while (existe);

            //revalidar 2da vez 
            if (!segunda)
                Numero = DarFolioValido(LetraFolioID, i, true);
            else
                Numero = i;

            return Numero;
        }

        // POST: Obra/Registrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registrar(Guid? TipoObraID, int? LetraFolioID, Guid? TipoPiezaID)
        {
            var Formulario = Request.Form;


            if (TipoObraID == null || LetraFolioID == null || TipoPiezaID == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var letra = db.LetraFolios.Find(LetraFolioID);
            var tipoObra = db.TipoObras.Find(TipoObraID);
            var tipoPieza = tipoObra.TipoPiezas.FirstOrDefault(a => a.TipoPiezaID == TipoPiezaID);

            if (tipoObra == null || letra == null || tipoPieza == null)
                return HttpNotFound();



            var obra = new Obra()
            {
                FechaRegistro = DateTime.Now,
                TipoObraID = tipoObra.TipoObraID,
                LetraFolioID = letra.LetraFolioID,
                Status = false,
                NumeroFolio = 1
            };

            obra.ObraID = Guid.NewGuid();






            //Crear pieza
            Pieza pieza = new Pieza()
            {
                PiezaID = Guid.NewGuid(),
                FechaRegistro = obra.FechaRegistro,
                ObraID = obra.ObraID,
                Status = false,
                PiezaPadreID = null, // null = Principal o Maestra
                TipoPiezaID = tipoPieza.TipoPiezaID,
                SubFolio = tipoPieza.Prefijo
            };

            //lista de atributos de registro
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
                                HttpPostedFileBase FileImagen = Request.Files[key];

                                string texto_Titulo = Formulario["id_" + att.AtributoID + "_Titulo"];
                                string texto_Descripcion = Formulario["id_" + att.AtributoID + "_Descripcion"];
                                string extension = Path.GetExtension(FileImagen.FileName);

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
                //validar el numero de folio
                obra.NumeroFolio = DarFolioValido(obra.LetraFolioID, obra.NumeroFolio);

                //Guardar la obra
                db.Obras.Add(obra);
                db.SaveChanges();

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

                return RedirectToAction("Detalles", "Obra", new { id = obra.ObraID });

            }

            return Json(new { success = false });
        }

        // GET: Obra/Editar/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Obra obra = db.Obras.Find(id);
            if (obra == null)
            {
                return HttpNotFound();
            }
            ViewBag.LetraFolioID = new SelectList(db.LetraFolios, "LetraFolioID", "Nombre", obra.LetraFolioID);
            ViewBag.TipoObraID = new SelectList(db.TipoObras, "TipoObraID", "Nombre", obra.TipoObraID);
            return View(obra);
        }

        // POST: Obra/Editar/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ObraID,NumeroFolio,FechaRegistro,TipoObraID,LetraFolioID,Status,Temp")] Obra obra)
        {
            if (ModelState.IsValid)
            {
                db.Entry(obra).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LetraFolioID = new SelectList(db.LetraFolios, "LetraFolioID", "Nombre", obra.LetraFolioID);
            ViewBag.TipoObraID = new SelectList(db.TipoObras, "TipoObraID", "Nombre", obra.TipoObraID);
            return View(obra);
        }

        // GET: Obra/Eliminar/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Obra obra = db.Obras.Find(id);
            if (obra == null)
            {
                return HttpNotFound();
            }
            return View(obra);
        }

        // POST: Obra/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Obra obra = db.Obras.Find(id);
            db.Obras.Remove(obra);
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

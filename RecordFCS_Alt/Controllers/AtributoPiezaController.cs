using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RecordFCS_Alt.Models;
using System.Text.RegularExpressions;
using RecordFCS_Alt.Helpers.Seguridad;

namespace RecordFCS_Alt.Controllers
{
    public class AtributoPiezaController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();


        // GET: AtributoPieza/Create
        [CustomAuthorize(permiso = "attPNew")]
        public ActionResult Crear(Guid? id, Guid? AtributoID)
        {
            // AtributoPiezaID = id
            //id PiezaID
            //atributoID
            //AtributoPiezaID || TablaID

            PartialViewResult _vista = null;


            if (id == null || AtributoID == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Atributo att = db.Atributos.Find(AtributoID);

            if (att == null)
                return HttpNotFound();

            Pieza pieza = db.Piezas.Find(id);

            if (pieza == null)
                return HttpNotFound();

            if (att.TipoAtributo.EsGenerico)
            {
                AtributoPieza attPieza = new AtributoPieza()
                {
                    AtributoID = att.AtributoID,
                    PiezaID = pieza.PiezaID,
                    Status = true
                };

                if (att.TipoAtributo.EsLista)
                {

                    List<ListaValor> lista = new List<ListaValor>();
                    lista.Add(attPieza.ListaValor); //agregar valor por si no viene en los primeros 500
                    lista.AddRange(att.TipoAtributo.ListaValores.Where(a => a.Status && !String.IsNullOrWhiteSpace(a.Valor)).OrderBy(a => a.Valor).Take(100).ToList());

                    ViewBag.ListaValorID = new SelectList(lista, "ListaValorID", "Valor", attPieza.ListaValorID);

                    _vista = PartialView("_CrearGenericoLista", attPieza);
                }
                else
                {
                    _vista = PartialView("_CrearGenericoCampo", attPieza);
                }
            }
            else
            {
                switch (att.TipoAtributo.TablaSQL)
                {
                    case "Autor":
                        List<Autor> listaAutores = new List<Autor>();

                        var piezaAutor = new AutorPieza()
                        {
                            Status = true,
                            PiezaID = pieza.PiezaID
                        };

                        listaAutores.AddRange(db.Autores.Where(a => a.Status).OrderBy(a => a.Nombre).Take(100).ToList());

                        ViewBag.AutorID = new SelectList(listaAutores.Select(a => new { Nombre = a.Nombre + " " + a.Apellido, a.AutorID }), "AutorID", "Nombre");

                        _vista = PartialView("~/Views/AutorPieza/_Crear.cshtml", piezaAutor);

                        break;
                    case "Ubicacion":
                        //var listaUbicaciones = db.Ubicaciones.Where(a => a.Status).OrderBy(a => a.Nombre).Select(a => new { Nombre = a.Nombre, a.UbicacionID }).ToList();
                        //ViewBag.UbicacionID = new SelectList(listaUbicaciones, "UbicacionID", "Nombre");

                        //_vista = PartialView("~/Views/Ubicacion/_CampoRegistro.cshtml");
                        break;

                    case "TipoTecnica":
                        var piezaTecnica = new TecnicaPieza()
                        {
                            Status = true,
                            PiezaID = pieza.PiezaID
                        };
                        var tipoTecnicasExistentes = pieza.TecnicaPiezas.Select(a => a.TipoTecnicaID);

                        var listaTipoTecnicas = db.TipoTecnicas.Where(a => a.Status && !tipoTecnicasExistentes.Contains(a.TipoTecnicaID)).OrderBy(a => a.Nombre).Select(a => new { Nombre = a.Nombre, a.TipoTecnicaID }).ToList();

                        ViewBag.TipoTecnicaID = new SelectList(listaTipoTecnicas, "TipoTecnicaID", "Nombre");

                        _vista = PartialView("~/Views/TecnicaPieza/_Crear.cshtml", piezaTecnica);
                        break;

                    case "TipoMedida":
                        //var listaTipoMedidas = db.TipoMedidas.Where(a => a.Status).OrderBy(a => a.Nombre).Select(a => new { a.Nombre, a.TipoMedidaID }).ToList();
                        //var listaUML = from UMLongitud e in Enum.GetValues(typeof(UMLongitud))
                        //               select new { ID = e, Nombre = e.ToString() };

                        //ViewBag.TipoMedidaID = new SelectList(listaTipoMedidas, "TipoMedidaID", "Nombre");
                        //ViewData["id_" + AtributoID + "_UML"] = new SelectList(listaUML, "ID", "Nombre");

                        //_vista = PartialView("~/Views/TipoMedida/_CampoRegistro.cshtml");
                        break;

                    default:
                        //_vista = PartialView("_ErrorCampo");
                        break;

                }

            }


            ViewBag.EsMultipleValor = att.TipoAtributo.EsMultipleValor;
            ViewBag.ParametrosHTML = att.TipoAtributo.HTMLParametros;
            ViewBag.TipoAtributoID = att.TipoAtributo.TipoAtributoID;
            ViewBag.NombreAtt = att.TipoAtributo.Nombre;
            ViewBag.id = id;
            ViewBag.AtributoID = AtributoID;

            switch (att.TipoAtributo.DatoCS)
            {
                case "double":
                case "Double":
                case "int":
                case "float":
                case "int32":
                case "int64":
                case "decimal": ViewBag.TipoInput = "number"; break;
                case "date": ViewBag.TipoInput = "date"; break;
                case "time": ViewBag.TipoInput = "time"; break;
                case "datetime": ViewBag.TipoInput = "datetime"; break;
                case "datetime-local": ViewBag.TipoInput = "datetime-local"; break;
                case "month": ViewBag.TipoInput = "month"; break;
                case "week": ViewBag.TipoInput = "week"; break;
                case "color": ViewBag.TipoInput = "color"; break;
                case "email": ViewBag.TipoInput = "email"; break;
                case "url": ViewBag.TipoInput = "url"; break;
                case "tel": ViewBag.TipoInput = "tel"; break;
                case "range": ViewBag.TipoInput = "range"; break;
                default: ViewBag.TipoInput = "text"; break;
            }


            return _vista;
        }

        // POST: AtributoPieza/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "attPNew")]
        public ActionResult Crear([Bind(Include = "AtributoPiezaID,Valor,Status,PiezaID,AtributoID,ListaValorID")] AtributoPieza atributoPieza)
        {

            Guid renderID = atributoPieza.AtributoPiezaID;
            string texto = "";
            bool guardar = false;



            var att = db.Atributos.Find(atributoPieza.AtributoID);

            if (att == null)
            {
                guardar = false;
                //alerta no existe el atributo
            }
            else
            {

                if (att.TipoAtributo.EsLista)
                {
                    string valor = Request.Form["id_" + atributoPieza.AtributoID].ToString();

                    atributoPieza.ListaValorID = new Guid(valor);

                    //no existe el ListaValorID entonces actualizar el AtributoPiezaID con el ListaValorID
                    if (db.AtributoPiezas.Where(a => a.AtributoID == atributoPieza.AtributoID && a.PiezaID == atributoPieza.PiezaID && a.ListaValorID == atributoPieza.ListaValorID).Count() == 0)
                    {
                        guardar = true;

                        var listaValor = db.ListaValores.FirstOrDefault(a => a.ListaValorID == atributoPieza.ListaValorID);

                        texto = listaValor.Valor;

                        AlertaSuccess(string.Format("{0}: <b>{1}</b> se creó.", att.NombreAlterno, listaValor.Valor), true);
                    }
                    else
                    {
                        guardar = false;
                        //Alerta ya existe
                    }

                }
                else
                {
                    //campo texto
                    //tratar el atributoPieza.Valor , quitar espacios extras
                    atributoPieza.Valor = Regex.Replace(atributoPieza.Valor.Trim(), @"\s+", " ");

                    if (db.AtributoPiezas.Where(a => a.AtributoID == atributoPieza.AtributoID && a.PiezaID == atributoPieza.PiezaID && a.Valor == atributoPieza.Valor).Count() == 0)
                    {
                        guardar = true;
                        texto = atributoPieza.Valor;

                        AlertaSuccess(string.Format("{0}: <b>{1}</b> se creó.", att.NombreAlterno, texto), true);


                    }
                    else
                    {
                        guardar = false;
                        //alerta ya existe
                    }

                }
            }

            if (guardar)
            {
                atributoPieza.AtributoPiezaID = Guid.NewGuid();

                atributoPieza.AtributoPiezaID = Guid.NewGuid();
                db.AtributoPiezas.Add(atributoPieza);
                db.SaveChanges();

                renderID = atributoPieza.AtributoPiezaID;
            }




            return Json(new { success = true, renderID = "valor_" + renderID, texto = texto, guardar = guardar });

        }

        // GET: AtributoPieza/Edit/5
        [CustomAuthorize(permiso = "attPEdit")]
        public ActionResult Editar(Guid? id, Guid? AtributoID, Guid? LLaveID)
        {


            //id PiezaID
            //atributoID
            //AtributoPiezaID || TablaID

            PartialViewResult _vista = null;


            if (id == null || AtributoID == null || LLaveID == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Atributo att = db.Atributos.Find(AtributoID);

            if (att == null)
                return HttpNotFound();

            if (att.TipoAtributo.EsGenerico)
            {
                AtributoPieza attPieza = db.AtributoPiezas.Find(LLaveID);

                if (att.TipoAtributo.EsLista)
                {

                    List<ListaValor> lista = new List<ListaValor>();
                    lista.Add(attPieza.ListaValor); //agregar valor por si no viene en los primeros 500
                    lista.AddRange(att.TipoAtributo.ListaValores.Where(a => a.Status && !String.IsNullOrWhiteSpace(a.Valor)).OrderBy(a => a.Valor).Take(100).ToList());


                    ViewBag.ListaValorID = new SelectList(lista, "ListaValorID", "Valor", attPieza.ListaValorID);

                    _vista = PartialView("_EditarGenericoLista", attPieza);
                }
                else
                {

                    _vista = PartialView("_EditarGenericoCampo", attPieza);
                }
            }
            else
            {
                switch (att.TipoAtributo.TablaSQL)
                {
                    case "Autor":
                        List<Autor> listaAutores = new List<Autor>();
                        var piezaAutor = db.AutorPiezas.Find(id, LLaveID);

                        listaAutores.Add(piezaAutor.Autor);
                        listaAutores.AddRange(db.Autores.Where(a => a.Status).OrderBy(a => a.Nombre).Take(100).ToList());

                        ViewBag.AutorID = new SelectList(listaAutores.Select(a => new { Nombre = a.Nombre + " " + a.Apellido, a.AutorID }), "AutorID", "Nombre", piezaAutor.Autor.AutorID);

                        _vista = PartialView("~/Views/AutorPieza/_Editar.cshtml", piezaAutor);

                        break;
                    case "Ubicacion":
                        //var listaUbicaciones = db.Ubicaciones.Where(a => a.Status).OrderBy(a => a.Nombre).Select(a => new { Nombre = a.Nombre, a.UbicacionID }).ToList();
                        //ViewBag.UbicacionID = new SelectList(listaUbicaciones, "UbicacionID", "Nombre");

                        //_vista = PartialView("~/Views/Ubicacion/_CampoRegistro.cshtml");
                        break;

                    case "TipoTecnica":
                        List<Tecnica> listaTecnicas = new List<Tecnica>();
                        var piezaTecnica = db.TecnicaPiezas.Find(id, LLaveID);

                        //Select TipoTecnica
                        var listaTipoTecnicas = db.TipoTecnicas.Where(a => a.TipoTecnicaID == piezaTecnica.TipoTecnicaID).OrderBy(a => a.Nombre).Select(a => new { Nombre = a.Nombre, a.TipoTecnicaID }).ToList();
                        ViewBag.TipoTecnicaID = new SelectList(listaTipoTecnicas, "TipoTecnicaID", "Nombre", piezaTecnica.TipoTecnicaID);

                        //Select Tecnica
                        listaTecnicas.Add(piezaTecnica.Tecnica);
                        listaTecnicas.AddRange(db.Tecnicas.Where(a => a.Status && a.TipoTecnicaID == piezaTecnica.TipoTecnicaID).OrderBy(a => a.Descripcion).Take(100).ToList());
                        ViewBag.TecnicaID = new SelectList(listaTecnicas.Select(a => new { Nombre = a.Descripcion, a.TecnicaID }), "TecnicaID", "Nombre", piezaTecnica.TecnicaID);
                        
                        _vista = PartialView("~/Views/TecnicaPieza/_Editar.cshtml", piezaTecnica);
                        break;

                    case "TipoMedida":
                        //var listaTipoMedidas = db.TipoMedidas.Where(a => a.Status).OrderBy(a => a.Nombre).Select(a => new { a.Nombre, a.TipoMedidaID }).ToList();
                        //var listaUML = from UMLongitud e in Enum.GetValues(typeof(UMLongitud))
                        //               select new { ID = e, Nombre = e.ToString() };

                        //ViewBag.TipoMedidaID = new SelectList(listaTipoMedidas, "TipoMedidaID", "Nombre");
                        //ViewData["id_" + AtributoID + "_UML"] = new SelectList(listaUML, "ID", "Nombre");

                        //_vista = PartialView("~/Views/TipoMedida/_CampoRegistro.cshtml");
                        break;

                    default:
                        //_vista = PartialView("_ErrorCampo");
                        break;

                }

            }


            ViewBag.EsMultipleValor = att.TipoAtributo.EsMultipleValor;
            ViewBag.ParametrosHTML = att.TipoAtributo.HTMLParametros;
            ViewBag.TipoAtributoID = att.TipoAtributo.TipoAtributoID;
            ViewBag.NombreAtt = att.TipoAtributo.Nombre;
            ViewBag.id = id;
            ViewBag.AtributoID = AtributoID;
            ViewBag.LLaveID = LLaveID;

            switch (att.TipoAtributo.DatoCS)
            {
                case "double":
                case "Double":
                case "int":
                case "float":
                case "int32":
                case "int64":
                case "decimal": ViewBag.TipoInput = "number"; break;
                case "date": ViewBag.TipoInput = "date"; break;
                case "time": ViewBag.TipoInput = "time"; break;
                case "datetime": ViewBag.TipoInput = "datetime"; break;
                case "datetime-local": ViewBag.TipoInput = "datetime-local"; break;
                case "month": ViewBag.TipoInput = "month"; break;
                case "week": ViewBag.TipoInput = "week"; break;
                case "color": ViewBag.TipoInput = "color"; break;
                case "email": ViewBag.TipoInput = "email"; break;
                case "url": ViewBag.TipoInput = "url"; break;
                case "tel": ViewBag.TipoInput = "tel"; break;
                case "range": ViewBag.TipoInput = "range"; break;
                default: ViewBag.TipoInput = "text"; break;
            }


            return _vista;
        }

        // POST: AtributoPieza/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "attPEdit")]
        public ActionResult Editar([Bind(Include = "AtributoPiezaID,Valor,Status,PiezaID,AtributoID,ListaValorID")] AtributoPieza atributoPieza)
        {
            Guid renderID = atributoPieza.AtributoPiezaID;
            string texto = "";
            bool guardar = false;



            var attPiezaAnterior = db.AtributoPiezas.Find(atributoPieza.AtributoPiezaID);

            if (attPiezaAnterior == null)
            {
                guardar = false;
                //alerta no existe el atributo
            }
            else
            {

                if (attPiezaAnterior.Atributo.TipoAtributo.EsLista)
                {
                    string valor = Request.Form["id_" + atributoPieza.AtributoID].ToString();

                    atributoPieza.ListaValorID = new Guid(valor);

                    //no existe el ListaValorID entonces actualizar el AtributoPiezaID con el ListaValorID
                    if (db.AtributoPiezas.Where(a => a.AtributoID == atributoPieza.AtributoID && a.PiezaID == atributoPieza.PiezaID && a.ListaValorID == atributoPieza.ListaValorID).Count() == 0)
                    {
                        guardar = true;

                        var listaValor = db.ListaValores.FirstOrDefault(a => a.ListaValorID == atributoPieza.ListaValorID);

                        texto = listaValor.Valor;

                        AlertaSuccess(string.Format("{0}: <b>{1}</b> se actualizo a <b>{2}</b>.", attPiezaAnterior.Atributo.NombreAlterno, attPiezaAnterior.ListaValor.Valor, listaValor.Valor), true);
                        attPiezaAnterior.ListaValorID = listaValor.ListaValorID;

                    }
                    else
                    {
                        guardar = false;
                        //Alerta ya existe
                    }

                }
                else
                {
                    //campo texto
                    //tratar el atributoPieza.Valor , quitar espacios extras
                    atributoPieza.Valor = Regex.Replace(atributoPieza.Valor.Trim(), @"\s+", " ");

                    if (db.AtributoPiezas.Where(a => a.AtributoID == atributoPieza.AtributoID && a.PiezaID == atributoPieza.PiezaID && a.Valor == atributoPieza.Valor).Count() == 0)
                    {
                        guardar = true;
                        texto = atributoPieza.Valor;

                        AlertaSuccess(string.Format("{0}: <b>{1}</b> se actualizo a <b>{2}</b>.", attPiezaAnterior.Atributo.NombreAlterno, atributoPieza.Valor, attPiezaAnterior.Valor), true);
                        attPiezaAnterior.Valor = atributoPieza.Valor;


                    }
                    else
                    {
                        guardar = false;
                        //alerta ya existe
                    }

                }
            }

            if (guardar)
            {
                db.Entry(attPiezaAnterior).State = EntityState.Modified;
                db.SaveChanges();
                renderID = attPiezaAnterior.AtributoPiezaID;
            }




            return Json(new { success = true, renderID = "valor_" + renderID, texto = texto, guardar = guardar });
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

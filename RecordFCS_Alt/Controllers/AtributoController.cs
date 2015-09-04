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
    public class AtributoController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: Atributo
        public ActionResult Lista(Guid? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var tipoPieza = db.TipoPiezas.Find(id);

            if (tipoPieza == null) return HttpNotFound();

            tipoPieza.Atributos = tipoPieza.Atributos.OrderBy(a => a.Orden).ToList();

            var listaTM = db.TipoMostarlos.Where(a => a.Status).OrderBy(a => a.Nombre).ToList();

            foreach (var att in tipoPieza.Atributos)
            {
                //buscar si att esta en listaTM
                foreach (var tm in listaTM)
                {
                    var x = tm.MostrarAtributos.Where(a => a.AtributoID == att.AtributoID).FirstOrDefault();

                    if (x == null)
                    {
                        x = new MostrarAtributo()
                        {
                            TipoMostrarID = tm.TipoMostrarID,
                            AtributoID = att.AtributoID,
                            Status = false,
                            Orden = att.Orden
                        };


                        db.MostrarAtributos.Add(x);
                        db.SaveChanges();
                    }
                }

                att.MostrarAtributos = att.MostrarAtributos.Where(a => a.TipoMostrar.Status).OrderBy(a => a.TipoMostrar.Nombre).ToList();
            }

            ViewBag.totalRegistros = tipoPieza.Atributos.Count;
            ViewBag.id = tipoPieza.TipoPiezaID;
            ViewBag.nombre = tipoPieza.Nombre;
            ViewBag.listaTM = db.TipoMostarlos.Where(a => a.Status).Select(a => a.Nombre).OrderBy(a => a).ToList();


            return PartialView("_Lista", tipoPieza.Atributos);
        }

        // GET: Atributo/Crear
        public ActionResult Crear(Guid? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var tipoPieza = db.TipoPiezas.Find(id);

            if (tipoPieza == null) return HttpNotFound();

            Atributo att = new Atributo()
            {
                TipoPiezaID = tipoPieza.TipoPiezaID,
                Status = true,
                Orden = tipoPieza.Atributos.Count + 1
            };

            // llenar el select personalizado
            // solo muestra los tipos de atributos no asignados al tipo de pieza
            var listaCompleta = db.TipoAtributos.Where(a => a.Status).OrderBy(a => a.Orden).ToList().Except(tipoPieza.Atributos.Select(a => a.TipoAtributo));

            var listaTipoAtributos = new List<object>();

            foreach (var ta in listaCompleta)
            {
                var esLista = ta.EsLista ? "[Cat]" : "";
                var esMulti = ta.EsMultipleValor ? "[Multi]" : "";
                var descripcion = ta.Descripcion == null ? "" : "[" + ta.Descripcion + "]";

                listaTipoAtributos.Add(new
                {
                    TipoAtributoID = ta.TipoAtributoID,
                    Nombre = ta.Nombre + esLista + esMulti + descripcion
                });
            }
            ViewBag.TipoAtributoID = new SelectList(listaTipoAtributos, "TipoAtributoID", "Nombre");

            return PartialView("_Crear", att);
        }

        // POST: Atributo/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear([Bind(Include = "AtributoID,Orden,NombreAlterno,Status,TipoPiezaID,TipoAtributoID")] Atributo atributo)
        {
            //validar 
            if (db.Atributos.SingleOrDefault(a => a.TipoAtributoID == atributo.TipoAtributoID && a.TipoPiezaID == atributo.TipoPiezaID) != null)
                ModelState.AddModelError("TipoAtributoID", "Ya esta agregado.");


            if (ModelState.IsValid)
            {
                var tipoAtt = db.TipoAtributos.SingleOrDefault(a => a.TipoAtributoID == atributo.TipoAtributoID);

                atributo.NombreAlterno = string.IsNullOrWhiteSpace(atributo.NombreAlterno) ? tipoAtt.Nombre : atributo.NombreAlterno;
                atributo.Orden = atributo.Orden == 0 ? tipoAtt.Orden : atributo.Orden;

                atributo.AtributoID = Guid.NewGuid();
                db.Atributos.Add(atributo);
                db.SaveChanges();

                AlertaSuccess(string.Format("Atributo: <b>{0}</b> agregado.", atributo.NombreAlterno), true);

                string url = Url.Action("Lista", "Atributo", new { id = atributo.TipoPiezaID });

                return Json(new { success = true, url = url });
            }

            // llenar el select personalizado
            // solo muestra los tipos de atributos no asignados al tipo de pieza
            var tipoPieza = db.TipoPiezas.Find(atributo.TipoPiezaID);

            var listaCompleta = db.TipoAtributos.Where(a => a.Status).OrderBy(a => a.Orden).ToList().Except(tipoPieza.Atributos.Select(a => a.TipoAtributo));

            var listaTipoAtributos = new List<object>();

            foreach (var ta in listaCompleta)
            {
                var esLista = ta.EsLista ? "[Cat]" : "";
                var esMulti = ta.EsMultipleValor ? "[Multi]" : "";
                var descripcion = ta.Descripcion == null ? "" : "[" + ta.Descripcion + "]";

                listaTipoAtributos.Add(new
                {
                    TipoAtributoID = ta.TipoAtributoID,
                    Nombre = ta.Nombre + esLista + esMulti + descripcion
                });
            }
            ViewBag.TipoAtributoID = new SelectList(listaTipoAtributos, "TipoAtributoID", "Nombre");

            return PartialView("_Crear", atributo);
        }


        // GET: Atributo/Editar/5
        public ActionResult Editar(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Atributo atributo = db.Atributos.Find(id);
            if (atributo == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Editar", atributo);
        }

        // POST: Atributo/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "AtributoID,Orden,NombreAlterno,Status,TipoPiezaID,TipoAtributoID")] Atributo atributo)
        {
            if (ModelState.IsValid)
            {

                var tipoAtt = db.TipoAtributos.SingleOrDefault(a => a.TipoAtributoID == atributo.TipoAtributoID);

                atributo.NombreAlterno = string.IsNullOrWhiteSpace(atributo.NombreAlterno) ? tipoAtt.Nombre : atributo.NombreAlterno;

                db.Entry(atributo).State = EntityState.Modified;
                db.SaveChanges();



                AlertaInfo(string.Format("Atributo: <b>{0}</b> editado.", atributo.NombreAlterno), true);

                string url = Url.Action("Lista", "Atributo", new { id = atributo.TipoPiezaID });

                return Json(new { success = true, url = url });
            }

            return PartialView("_Editar", atributo);
        }

        // GET: Atributo/Eliminar/5
        public ActionResult Eliminar(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Atributo atributo = db.Atributos.Find(id);
            if (atributo == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Eliminar", atributo);
        }

        // POST: Atributo/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarConfirmado(Guid id)
        {
            string btnValue = Request.Form["accionx"];

            Atributo atributo = db.Atributos.Find(id);

            switch (btnValue)
            {
                case "deshabilitar":
                    atributo.Status = false;
                    db.Entry(atributo).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", atributo.NombreAlterno), true);
                    break;
                case "eliminar":
                    db.Atributos.Remove(atributo);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", atributo.NombreAlterno), true);
                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;
            }

            string url = Url.Action("Lista", "Atributo", new { id = atributo.TipoPiezaID });

            return Json(new { success = true, url = url });
        }

        public JsonResult EsUnico(Guid? TipoAtributoID, Guid? TipoPiezaID, Guid? AtributoID)
        {
            bool x = false;

            var att = db.Atributos.SingleOrDefault(a => a.TipoAtributoID == TipoAtributoID && a.TipoPiezaID == TipoPiezaID);
            x = att == null ? true : att.AtributoID == AtributoID ? true : false;

            return Json(x);
        }


        public ActionResult RegistroFormulario(Guid? id)
        {
            //buscar el tipo de pieza
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            TipoPieza tipoPieza = db.TipoPiezas.Find(id);

            if (tipoPieza == null) return HttpNotFound();

            var lista = tipoPieza.Atributos.Where(a => a.Status && a.MostrarAtributos.Any(b => b.TipoMostrar.Nombre == "Registro" && b.Status) && a.TipoAtributo.Status).OrderBy(a => a.Orden);

            var Nombre = "";

            if (tipoPieza.EsPrincipal)
                Nombre = tipoPieza.TipoObra.Nombre;
            else
                Nombre = tipoPieza.Nombre;


            ViewBag.Nombre = Nombre;

            //listar todos sus campos
            return PartialView("_Registro_form", lista);
        }


        public ActionResult GenerarCampoRegistro(Guid? id, Guid? AtributoID)
        {
            PartialViewResult _vista = null;

            if (id == null || AtributoID == null)
                _vista = PartialView("_ErrorCampo");

            TipoAtributo tipoAtt = db.TipoAtributos.Find(id);

            if (tipoAtt == null)
                _vista = PartialView("_ErrorCampo");



            if (tipoAtt.EsGenerico)
            {
                if (tipoAtt.EsLista)
                {
                    var lista = tipoAtt.ListaValores.Where(a => a.Status && !String.IsNullOrWhiteSpace(a.Valor)).OrderBy(a => a.Valor).Select(a => new { a.Valor, a.ListaValorID, a.TipoAtributoID }).ToList().Take(100);

                    ViewBag.ListaValorID = new SelectList(lista, "ListaValorID", "Valor");
                    
                    _vista = PartialView("_GenericoLista");
                }
                else
                {
                    //_vista = PartialView("_GenericoCampo","AutorPieza");
                    _vista = PartialView("_GenericoCampo");
                }
            }
            else
            {
                switch (tipoAtt.TablaSQL)
                {
                    case "Autor":
                        var listaAutores = db.Autores.Where(a => a.Status).OrderBy(a => a.Nombre).Select(a => new { Nombre = a.Nombre + " " + a.Apellido, a.AutorID }).ToList().Take(100);
                        ViewBag.AutorID = new SelectList(listaAutores, "AutorID", "Nombre");

                        _vista = PartialView("~/Views/AutorPieza/_CampoRegistro.cshtml");
                        break;
                    case "Ubicacion":
                        var listaUbicaciones = db.Ubicaciones.Where(a => a.Status).OrderBy(a => a.Nombre).Select(a => new { Nombre = a.Nombre, a.UbicacionID }).ToList().Take(100);
                        ViewBag.UbicacionID = new SelectList(listaUbicaciones, "UbicacionID", "Nombre");

                        _vista = PartialView("~/Views/Ubicacion/_CampoRegistro.cshtml");
                        break;

                    case "TipoTecnica":
                        var listaTipoTecnicas = db.TipoTecnicas.Where(a => a.Status).OrderBy(a => a.Nombre).Select(a => new { Nombre = a.Nombre, a.TipoTecnicaID }).ToList();
                        ViewBag.TipoTecnicaID = new SelectList(listaTipoTecnicas, "TipoTecnicaID", "Nombre");

                        _vista = PartialView("~/Views/Tecnica/_CampoRegistro.cshtml");
                        break;

                    case "TipoMedida":
                        var listaTipoMedidas = db.TipoMedidas.Where(a => a.Status).OrderBy(a => a.Nombre).Select(a => new { a.Nombre, a.TipoMedidaID}).ToList();
                        var listaUML = from UMLongitud e in Enum.GetValues(typeof(UMLongitud))
                                       select new {ID = e, Nombre = e.ToString()};

                       ViewBag.TipoMedidaID = new SelectList(listaTipoMedidas, "TipoMedidaID", "Nombre" );
                        ViewData["id_" + AtributoID + "_UML"] = new SelectList(listaUML, "ID", "Nombre");

                        _vista = PartialView("~/Views/TipoMedida/_CampoRegistro.cshtml");
                        break;

                    case "ImagenPieza":
                        _vista = PartialView("~/Views/ImagenPieza/_CampoRegistro.cshtml");
                        break;
                    default:
                        _vista = PartialView("_ErrorCampo");
                        break;
                }
                

            }


            ViewBag.EsMultipleValor = tipoAtt.EsMultipleValor;
            ViewBag.AtributoID = AtributoID;
            ViewBag.ParametrosHTML = tipoAtt.HTMLParametros;
            ViewBag.TipoAtributoID = tipoAtt.TipoAtributoID;
            ViewBag.NombreAtt = tipoAtt.Nombre;

            switch (tipoAtt.DatoCS)
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

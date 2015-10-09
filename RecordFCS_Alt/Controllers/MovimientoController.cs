using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RecordFCS_Alt.Models;
using PagedList;
using RecordFCS_Alt.Helpers.Seguridad;
using RecordFCS_Alt.Models.ViewsModel;
using System.Globalization;

namespace RecordFCS_Alt.Controllers
{
    public class MovimientoController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();
        [CustomAuthorize(permiso = "")]
        // GET: Movimiento
        public ActionResult Index()
        {
            ViewBag.TipoMovimientoID = new SelectList(db.TipoMovimientos, "TipoMovimientoID", "Nombre");

            var mov = new Movimiento()
            {
                HaciaExposicion = false,
                FechaHoraMovimiento = DateTime.Now
            };

            var listaUbicaciones = db.Ubicaciones.Where(a => a.Status).Select(a => new { a.Nombre, a.UbicacionID }).OrderBy(a => a.Nombre);

            ViewBag.UbicacionDestinoID = new SelectList(listaUbicaciones, "UbicacionID", "Nombre");
            ViewBag.UbicacionOrigenID = new SelectList(listaUbicaciones, "UbicacionID", "Nombre");


            return View(mov);
        }


        public List<string> getListaCamposMostrar(string tipoMovimiento, EstadoMovimiento estado)
        {

            List<string> lista = new List<string>();


            //lista = new List<string>() { 
            //                "movhaciaExpo","movFolio","movTipo","movStatus","movFechaRet", "observa", //movimiento 1
            //                "odOrigen","odDestino","odFecha","odColeccion", //movimiento 2
            //                "solNombreSol","solCargoSol","solInstitucion","solNombreRepre","solCargoRepre","solSede","SolPaisEdo","solPeticionRec","solDicCondEspa","solCartaAcep","solContratoCom","solRevGuion","solCondicConserv","solFacilReport","solDictSegu","solListAval","solTramFianza","solPoliSeg","solPoliCartas", //solicitante
            //                "expoTitulo","espoCurador","expoFechaIni","expoFechaFIn",//exposicion
            //                "resNombre","resInstitu","resFechaSal", //responsable
            //                "autUsuario1","autUsuario2","autFecha1","autFecha2", //autorizacion
            //                "transNombre","transRecorr","transHorario","transNota", //transporte
            //                "segNombre","segPoliza","segFechaIni","segFechaFin" //seguro
            //            };

            switch (tipoMovimiento.ToLower())
            {
                case "ingreso":
                    if (estado == EstadoMovimiento.EnRegistro)
                        lista = new List<string>() { 
                            "movhaciaExpo","movFolio","movTipo","movStatus", "observa", //movimiento 1"odOrigen","odDestino","odFecha","odColeccion", //movimiento 2
                            "solNombreSol","solCargoSol","solInstitucion","solNombreRepre","solCargoRepre","solSede","SolPaisEdo","solPeticionRec","solDicCondEspa","solCartaAcep","solContratoCom","solRevGuion","solCondicConserv","solFacilReport","solDictSegu","solListAval","solTramFianza","solPoliSeg","solPoliCartas", //solicitante
                            "expoTitulo","espoCurador","expoFechaIni","expoFechaFIn",//exposicion
                            "resNombre","resInstitu", //responsable
                            //"autUsuario1","autUsuario2","autFecha1","autFecha2", //autorizacion
                            "transNombre","transRecorr","transHorario","transNota", //transporte
                            "segNombre","segPoliza","segFechaIni","segFechaFin" //seguro
                        };

                    break;
                case "externo":
                    if (estado == EstadoMovimiento.EnRegistro)
                        lista = new List<string>() { 
                            "movhaciaExpo","movFolio","movTipo","movStatus", "observa", //movimiento 1
                            "odOrigen","odDestino","odFecha","odColeccion", //movimiento 2
                            "solNombreSol","solCargoSol", //solicitante
                            "expoTitulo","espoCurador","expoFechaIni","expoFechaFIn",//exposicion
                            //"resNombre","resInstitu","resFechaSal", //responsable
                            //"autUsuario1","autUsuario2","autFecha1","autFecha2", //autorizacion
                            //"transNombre","transRecorr","transHorario","transNota", //transporte
                            //"segNombre","segPoliza","segFechaIni","segFechaFin" //seguro
                        };

                    break;

                case "salida":
                    if (estado == EstadoMovimiento.EnRegistro)
                        lista = new List<string>() { 
                            "movhaciaExpo","movFolio","movTipo","movStatus", "observa", //movimiento 1
                            "odOrigen","odDestino","odFecha","odColeccion", //movimiento 2
                            "solNombreSol","solCargoSol","solInstitucion","solNombreRepre","solCargoRepre","solSede","SolPaisEdo","solPeticionRec","solDicCondEspa","solCartaAcep","solContratoCom","solRevGuion","solCondicConserv","solFacilReport","solDictSegu","solListAval","solTramFianza","solPoliSeg","solPoliCartas", //solicitante
                            "expoTitulo","espoCurador","expoFechaIni","expoFechaFIn",//exposicion
                            "resNombre","resInstitu", //responsable
                            //"autUsuario1","autUsuario2","autFecha1","autFecha2", //autorizacion
                            "transNombre","transRecorr","transHorario","transNota", //transporte
                            "segNombre","segPoliza","segFechaIni","segFechaFin" //seguro
                        };

                    break;

                case "traslado":
                    if (estado == EstadoMovimiento.EnRegistro)
                        lista = new List<string>() { 
                            "movhaciaExpo","movFolio","movTipo","movStatus", "observa", //movimiento 1
                            "odOrigen","odDestino","odFecha","odColeccion", //movimiento 2
                            "solNombreSol","solCargoSol","solInstitucion","solNombreRepre","solCargoRepre","solSede","solRevGuion","solCondicConserv","solPoliSeg","solPoliCartas", //solicitante
                            "expoTitulo","espoCurador","expoFechaIni","expoFechaFIn",//exposicion
                            //"resNombre","resInstitu","resFechaSal", //responsable
                            //"autUsuario1","autUsuario2","autFecha1","autFecha2", //autorizacion
                            "transNombre","transRecorr","transHorario","transNota", //transporte
                            //"segNombre","segPoliza","segFechaIni","segFechaFin" //seguro
                        };

                    break;
            }



            return lista;
        }


        // GET: Movimiento/Crear
        [CustomAuthorize(permiso = "")]
        public ActionResult Crear(Guid? TipoMovimientoID, bool? HaciaExposicion)
        {

            bool HaciaExposicionValor = HaciaExposicion == null || HaciaExposicion == false ? false : true;

            bool esOK = true;

            var listaMovAceptados = new List<string>() { "externo", "ingreso", "salida", "traslado" };

            var tipoMov = db.TipoMovimientos.Find(TipoMovimientoID);

            esOK = tipoMov == null ? false : true;


            esOK = esOK ? listaMovAceptados.Any(a => a == tipoMov.Nombre.ToLower()) : false;

            List<string> listaCampos = new List<string>();

            if (esOK)
            {

                int UltimoFolioMov = db.Movimientos.Select(a => a.FolioMovimiento).OrderByDescending(a => a).FirstOrDefault();

                var mov = new Movimiento()
                {
                    EstadoMovimiento = EstadoMovimiento.EnRegistro,
                    FolioMovimiento = UltimoFolioMov + 1,
                    HaciaExposicion = HaciaExposicionValor,
                    TipoMovimiento = tipoMov,
                    TipoMovimientoID = tipoMov.TipoMovimientoID,
                    ColeccionTexto = "Museo Soumaya"
                };

                if (HaciaExposicionValor)
                {
                    mov.MovimientoExposicion = new MovimientoExposicion();

                }

                listaCampos = getListaCamposMostrar(tipoMov.Nombre, EstadoMovimiento.EnRegistro);

                switch (tipoMov.Nombre.ToLower())
                {
                    case "externo":
                        mov.MovimientoSolicitante = new MovimientoSolicitante();
                        //mov.MovimientoAutorizacion = new MovimientoAutorizacion();
                        break;
                    case "ingreso":
                        mov.MovimientoSolicitante = new MovimientoSolicitante();
                        mov.MovimientoResponsable = new MovimientoResponsable();
                        //mov.MovimientoAutorizacion = new MovimientoAutorizacion();
                        mov.MovimientoTransporte = new MovimientoTransporte();
                        mov.MovimientoSeguro = new MovimientoSeguro();

                        break;
                    case "salida":
                        mov.MovimientoSolicitante = new MovimientoSolicitante();
                        mov.MovimientoResponsable = new MovimientoResponsable();
                        //mov.MovimientoAutorizacion = new MovimientoAutorizacion();
                        mov.MovimientoTransporte = new MovimientoTransporte();
                        mov.MovimientoSeguro = new MovimientoSeguro();

                        break;
                    case "traslado":
                        mov.MovimientoSolicitante = new MovimientoSolicitante();
                        //mov.MovimientoAutorizacion = new MovimientoAutorizacion();
                        mov.MovimientoTransporte = new MovimientoTransporte();
                        break;

                    default:
                        //mov.MovimientoSolicitante = new MovimientoSolicitante();
                        //if (HaciaExposicion)
                        //    mov.MovimientoExposicion = new MovimientoExposicion();
                        //mov.MovimientoResponsable = new MovimientoResponsable();
                        //mov.MovimientoAutorizacion = new MovimientoAutorizacion();
                        //mov.MovimientoTransporte = new MovimientoTransporte();
                        //mov.MovimientoSeguro = new MovimientoSeguro();
                        esOK = false;
                        break;
                }


                if (esOK)
                {
                    mov.FechaRegistro = DateTime.Now;
                    mov.FechaHoraMovimiento = DateTime.Now;

                    if (HaciaExposicionValor)
                        mov.MovimientoExposicion.FechaInicial = DateTime.Now;


                    var listaUbicaciones = db.Ubicaciones.Where(a => a.Status).Select(a => new { a.Nombre, a.UbicacionID }).OrderBy(a => a.Nombre);
                    var listaUsuarios = db.Usuarios.Where(a => a.Status).Select(a => new { Nombre = a.Nombre + " " + a.Apellido, a.UsuarioID }).OrderBy(a => a.Nombre);

                    var tipoAttGuion = db.TipoAtributos.FirstOrDefault(a => a.Temp == "guion_clave");
                    var listaGuiones = tipoAttGuion.ListaValores.Where(a => a.Status).Select(a => new { Nombre = a.Valor, GuionID = a.ListaValorID }).OrderBy(a => a.Nombre);
                    var listaLetras = db.LetraFolios.Select(a => new { a.LetraFolioID, Nombre = a.Nombre, a.Status }).Where(a => a.Status).OrderBy(a => a.Nombre);


                    ViewBag.NombreMovimiento = tipoMov.Nombre.ToLower();

                    //ViewBag.MovimientoAutorizacion_Usuario1ID = new SelectList(listaUsuarios, "UsuarioID", "Nombre");
                    //ViewBag.MovimientoAutorizacion_Usuario2ID = new SelectList(listaUsuarios, "UsuarioID", "Nombre");


                    ViewBag.UbicacionDestinoID = new SelectList(listaUbicaciones, "UbicacionID", "Nombre");
                    ViewBag.UbicacionOrigenID = new SelectList(listaUbicaciones, "UbicacionID", "Nombre");

                    ViewBag.GuionID = new SelectList(listaGuiones, "GuionID", "Nombre");
                    ViewBag.LetraFolioID = new SelectList(listaLetras, "LetraFolioID", "Nombre", listaLetras.FirstOrDefault().LetraFolioID);
                    ViewBag.listaCampos = listaCampos;
                    Session["listaMov"] = new List<Guid>();

                    return PartialView("_Crear", mov);
                }
            }


            ViewBag.Mensaje = "Se encontro un error, solicite asistencia técnica.";
            return PartialView("_Error");
        }

        // POST: Movimiento/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "")]
        public ActionResult Crear(Movimiento mov)
        {

            //validar los campos:
            var listaGuidSessionPiezas = Session["listaMov"] == null ? new List<Guid>() : (List<Guid>)Session["listaMov"];
            var listaMovimientoPiezas = new List<MovimientoPieza>();

            bool todoOk = true;

            todoOk = (listaGuidSessionPiezas.Count == 0) ? false : true;

            if (todoOk)
            {
                //lista piezas
                var listaIDLLaves = Request.Form.AllKeys.Where(a => a.StartsWith("addPiezaID_")).ToList();

                foreach (var keyID in listaIDLLaves)
                {
                    var addOk = true;
                    string valor = Request.Form[keyID];
                    addOk = String.IsNullOrWhiteSpace(valor) ? false : true;
                    //validar el valorID, buscar el valor
                    Guid valorID = addOk ? new Guid(valor) : new Guid(new Byte[16]);

                    addOk = !addOk ? addOk : db.Piezas.Where(a => a.PiezaID == valorID).FirstOrDefault() == null ? false : true;

                    if (addOk)
                    {
                        listaMovimientoPiezas.Add(new MovimientoPieza()
                        {
                            PiezaID = valorID,
                            estaDisponible = false,
                        });
                    }

                }
            }
            else
            {
                ModelState.AddModelError("", "Movimiento sin piezas");
            }


            //validar fechas



            if (ModelState.IsValid)
            {
                //mov.MovientoID = Guid.NewGuid();
                //var movNew = new Movimiento()
                //{
                //    ColeccionTexto = mov.ColeccionTexto,
                //    EstadoMovimiento = mov.EstadoMovimiento,
                //    FechaHoraMovimiento = mov.FechaHoraMovimiento,
                //    FechaRegistro = mov.FechaRegistro,
                //    FechaRet = mov.FechaRet,
                //    FolioMovimiento = UltimoFolioMov + 1,
                //    HaciaExposicion = mov.HaciaExposicion,
                //    MovientoID = Guid.NewGuid(),
                //    Observaciones = mov.Observaciones,
                //    TipoMovimientoID = mov.TipoMovimientoID,
                //    UbicacionDestinoID = mov.UbicacionDestinoID,
                //    UbicacionOrigenID = mov.UbicacionOrigenID,
                //};

                if (mov.MovimientoAutorizacion != null)
                {
                    db.MovimientoAutorizaciones.Add(mov.MovimientoAutorizacion);
                    db.SaveChanges();
                    mov.MovimientoAutorizacionID = mov.MovimientoAutorizacion.MovimientoAutorizacionID;
                }


                if (mov.MovimientoExposicion != null)
                {
                    db.MovimientoExposiciones.Add(mov.MovimientoExposicion);
                    db.SaveChanges();
                    mov.MovimientoExposicionID = mov.MovimientoExposicion.MovimientoExposicionID;

                }

                if (mov.MovimientoResponsable != null)
                {
                    db.MovimientoResponsables.Add(mov.MovimientoResponsable);
                    db.SaveChanges();
                    mov.MovimientoResponsableID = mov.MovimientoResponsable.MovimientoResponsableID;

                }


                if (mov.MovimientoSeguro != null)
                {
                    db.MovimientoSeguros.Add(mov.MovimientoSeguro);
                    db.SaveChanges();
                    mov.MovimientoSeguroID = mov.MovimientoSeguro.MovimientoSeguroID;

                }


                if (mov.MovimientoSolicitante != null)
                {
                    db.MovimientoSolicitante.Add(mov.MovimientoSolicitante);
                    db.SaveChanges();
                    mov.MovimientoSolicitanteID = mov.MovimientoSolicitante.MovimientoSolicitanteID;

                }


                if (mov.MovimientoTransporte != null)
                {
                    db.MovimientoTransporte.Add(mov.MovimientoTransporte);
                    db.SaveChanges();
                    mov.MovimientoTransporteID = mov.MovimientoTransporte.MovimientoTransporteID;

                }

                int UltimoFolioMov = db.Movimientos.Select(a => a.FolioMovimiento).OrderByDescending(a => a).FirstOrDefault();
                mov.FolioMovimiento = UltimoFolioMov + 1;
                mov.MovientoID = Guid.NewGuid();

                db.Movimientos.Add(mov);
                db.SaveChanges();

                //agregar las piezas

                foreach (var item in listaMovimientoPiezas)
                {
                    item.MovimientoID = mov.MovientoID;
                    item.estaDisponible = false;
                }

                db.MovimientoPiezas.AddRange(listaMovimientoPiezas);
                db.SaveChanges();

                return RedirectToAction("Index");
            }


            //var success = false;
            bool HaciaExposicionValor = mov.HaciaExposicion;

            bool esOK = true;

            var listaMovAceptados = new List<string>() { "externo", "ingreso", "salida", "traslado" };

            var tipoMov = db.TipoMovimientos.Find(mov.TipoMovimientoID);

            esOK = tipoMov == null ? false : true;
            esOK = esOK ? listaMovAceptados.Any(a => a == tipoMov.Nombre.ToLower()) : false;

            List<string> listaCampos = new List<string>();

            int UltimoFolioMovc = db.Movimientos.Select(a => a.FolioMovimiento).OrderByDescending(a => a).FirstOrDefault();

            mov.EstadoMovimiento = EstadoMovimiento.EnRegistro;
            mov.FolioMovimiento = UltimoFolioMovc + 1;
            mov.TipoMovimiento = tipoMov;
            mov.TipoMovimientoID = tipoMov.TipoMovimientoID;


            if (HaciaExposicionValor && mov.MovimientoExposicion == null)
                mov.MovimientoExposicion = new MovimientoExposicion();

            listaCampos = getListaCamposMostrar(tipoMov.Nombre, EstadoMovimiento.EnRegistro);

            switch (tipoMov.Nombre.ToLower())
            {
                case "externo":
                    if (mov.MovimientoSolicitante == null)
                        mov.MovimientoSolicitante = new MovimientoSolicitante();

                    //mov.MovimientoAutorizacion = new MovimientoAutorizacion();
                    break;
                case "ingreso":
                    if (mov.MovimientoSolicitante == null)
                        mov.MovimientoSolicitante = new MovimientoSolicitante();


                    if (mov.MovimientoResponsable == null)
                        mov.MovimientoResponsable = new MovimientoResponsable();

                    //mov.MovimientoAutorizacion = new MovimientoAutorizacion();

                    if (mov.MovimientoTransporte == null)
                        mov.MovimientoTransporte = new MovimientoTransporte();


                    if (mov.MovimientoSeguro == null)
                        mov.MovimientoSeguro = new MovimientoSeguro();
                    break;
                case "salida":
                    if (mov.MovimientoSolicitante == null)
                        mov.MovimientoSolicitante = new MovimientoSolicitante();
                    if (mov.MovimientoResponsable == null)
                        mov.MovimientoResponsable = new MovimientoResponsable();

                    //mov.MovimientoAutorizacion = new MovimientoAutorizacion();
                    if (mov.MovimientoTransporte == null)
                        mov.MovimientoTransporte = new MovimientoTransporte();
                    if (mov.MovimientoSeguro == null)
                        mov.MovimientoSeguro = new MovimientoSeguro();

                    break;
                case "traslado":
                    if (mov.MovimientoSolicitante == null)
                        mov.MovimientoSolicitante = new MovimientoSolicitante();
                    //mov.MovimientoAutorizacion = new MovimientoAutorizacion();
                    if (mov.MovimientoTransporte == null)
                        mov.MovimientoTransporte = new MovimientoTransporte();

                    break;

                default:
                    //mov.MovimientoSolicitante = new MovimientoSolicitante();
                    //if (HaciaExposicion)
                    //    mov.MovimientoExposicion = new MovimientoExposicion();
                    //mov.MovimientoResponsable = new MovimientoResponsable();
                    //mov.MovimientoAutorizacion = new MovimientoAutorizacion();
                    //mov.MovimientoTransporte = new MovimientoTransporte();
                    //mov.MovimientoSeguro = new MovimientoSeguro();
                    esOK = false;
                    break;
            }


            if (esOK)
            {
                mov.FechaRegistro = DateTime.Now;
                //mov.FechaHoraMovimiento = DateTime.Now;


                var listaUbicaciones = db.Ubicaciones.Where(a => a.Status).Select(a => new { a.Nombre, a.UbicacionID }).OrderBy(a => a.Nombre);
                var listaUsuarios = db.Usuarios.Where(a => a.Status).Select(a => new { Nombre = a.Nombre + " " + a.Apellido, a.UsuarioID }).OrderBy(a => a.Nombre);

                var tipoAttGuion = db.TipoAtributos.FirstOrDefault(a => a.Temp == "guion_clave");
                var listaGuiones = tipoAttGuion.ListaValores.Where(a => a.Status).Select(a => new { Nombre = a.Valor, GuionID = a.ListaValorID }).OrderBy(a => a.Nombre);
                var listaLetras = db.LetraFolios.Select(a => new { a.LetraFolioID, Nombre = a.Nombre, a.Status }).Where(a => a.Status).OrderBy(a => a.Nombre);


                ViewBag.NombreMovimiento = tipoMov.Nombre.ToLower();

                //ViewBag.MovimientoAutorizacion_Usuario1ID = new SelectList(listaUsuarios, "UsuarioID", "Nombre");
                //ViewBag.MovimientoAutorizacion_Usuario2ID = new SelectList(listaUsuarios, "UsuarioID", "Nombre");


                ViewBag.UbicacionDestinoID = new SelectList(listaUbicaciones, "UbicacionID", "Nombre", mov.UbicacionDestinoID);
                ViewBag.UbicacionOrigenID = new SelectList(listaUbicaciones, "UbicacionID", "Nombre", mov.UbicacionOrigenID);

                ViewBag.GuionID = new SelectList(listaGuiones, "GuionID", "Nombre");
                ViewBag.LetraFolioID = new SelectList(listaLetras, "LetraFolioID", "Nombre", listaLetras.FirstOrDefault().LetraFolioID);
                ViewBag.listaCampos = listaCampos;

            }

            return PartialView("_Crear", mov);

        }

        // GET: Movimiento/Edit/5
        [CustomAuthorize(permiso = "")]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movimiento movimiento = db.Movimientos.Find(id);
            if (movimiento == null)
            {
                return HttpNotFound();
            }
            ViewBag.MovimientoAutorizacionID = new SelectList(db.MovimientoAutorizaciones, "MovimientoAutorizacionID", "FechaAutorizacion1", movimiento.MovimientoAutorizacionID);
            ViewBag.MovimientoExposicionID = new SelectList(db.MovimientoExposiciones, "MovimientoExposicionID", "Titulo", movimiento.MovimientoExposicionID);
            ViewBag.MovimientoResponsableID = new SelectList(db.MovimientoResponsables, "MovimientoResponsableID", "Nombre", movimiento.MovimientoResponsableID);
            ViewBag.MovimientoSeguroID = new SelectList(db.MovimientoSeguros, "MovimientoSeguroID", "AseguradorNombre", movimiento.MovimientoSeguroID);
            ViewBag.MovimientoSolicitanteID = new SelectList(db.MovimientoSolicitante, "MovimientoSolicitanteID", "Nombre", movimiento.MovimientoSolicitanteID);
            ViewBag.TipoMovimientoID = new SelectList(db.TipoMovimientos, "TipoMovimientoID", "Nombre", movimiento.TipoMovimientoID);
            ViewBag.MovimientoTransporteID = new SelectList(db.MovimientoTransporte, "MovimientoTransporteID", "EmpresaNombre", movimiento.MovimientoTransporteID);
            ViewBag.UbicacionDestinoID = new SelectList(db.Ubicaciones, "UbicacionID", "Nombre", movimiento.UbicacionDestinoID);
            ViewBag.UbicacionOrigenID = new SelectList(db.Ubicaciones, "UbicacionID", "Nombre", movimiento.UbicacionOrigenID);
            return View(movimiento);
        }

        // POST: Movimiento/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "")]
        public ActionResult Edit([Bind(Include = "MovientoID,FolioMovimiento,TipoMovimientoID,EstadoMovimiento,FechaRegistro,FechaSalida,FechaRet,UbicacionOrigenID,UbicacionDestinoID,FechaMovimiento,HoraMovimiento,MinutoMovimiento,ColeccionTexto,Observaciones,MovimientoSolicitanteID,MovimientoResponsableID,MovimientoExposicionID,MovimientoAutorizacionID,MovimientoTransporteID,MovimientoSeguroID")] Movimiento movimiento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movimiento).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MovimientoAutorizacionID = new SelectList(db.MovimientoAutorizaciones, "MovimientoAutorizacionID", "FechaAutorizacion1", movimiento.MovimientoAutorizacionID);
            ViewBag.MovimientoExposicionID = new SelectList(db.MovimientoExposiciones, "MovimientoExposicionID", "Titulo", movimiento.MovimientoExposicionID);
            ViewBag.MovimientoResponsableID = new SelectList(db.MovimientoResponsables, "MovimientoResponsableID", "Nombre", movimiento.MovimientoResponsableID);
            ViewBag.MovimientoSeguroID = new SelectList(db.MovimientoSeguros, "MovimientoSeguroID", "AseguradorNombre", movimiento.MovimientoSeguroID);
            ViewBag.MovimientoSolicitanteID = new SelectList(db.MovimientoSolicitante, "MovimientoSolicitanteID", "Nombre", movimiento.MovimientoSolicitanteID);
            ViewBag.TipoMovimientoID = new SelectList(db.TipoMovimientos, "TipoMovimientoID", "Nombre", movimiento.TipoMovimientoID);
            ViewBag.MovimientoTransporteID = new SelectList(db.MovimientoTransporte, "MovimientoTransporteID", "EmpresaNombre", movimiento.MovimientoTransporteID);
            ViewBag.UbicacionDestinoID = new SelectList(db.Ubicaciones, "UbicacionID", "Nombre", movimiento.UbicacionDestinoID);
            ViewBag.UbicacionOrigenID = new SelectList(db.Ubicaciones, "UbicacionID", "Nombre", movimiento.UbicacionOrigenID);
            return View(movimiento);
        }

        // GET: Movimiento/Delete/5
        [CustomAuthorize(permiso = "")]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movimiento movimiento = db.Movimientos.Find(id);
            if (movimiento == null)
            {
                return HttpNotFound();
            }
            return View(movimiento);
        }

        // POST: Movimiento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "")]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Movimiento movimiento = db.Movimientos.Find(id);
            db.Movimientos.Remove(movimiento);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [CustomAuthorize(permiso = "")]
        public ActionResult BuscarMovimiento(int? FolioMovimiento, string FechaInicial, string FechaFinal, Guid? UbicacionOrigenID, Guid? UbicacionDestinoID, int? pagina = null)
        {
            int pagTamano = 5;
            int pagIndex = 1;
            pagIndex = pagina.HasValue ? Convert.ToInt32(pagina) : 1;


            IQueryable<Movimiento> listaMovimientos = db.Movimientos;
            IPagedList<Movimiento> listaMovimientosEnPagina;

            //FolioMovimiento = 0
            if (FolioMovimiento != null && FolioMovimiento > 0)
            {
                //realizar la busqueda exclisiva del movimiento
                listaMovimientos = listaMovimientos.Where(a => a.FolioMovimiento == FolioMovimiento);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(FechaInicial))
                {
                    DateTime Fecha = DateTime.ParseExact(FechaInicial,"dd/MM/yyyy hh:mm tt",CultureInfo.InvariantCulture);

                   listaMovimientos = listaMovimientos.Where(a => DateTime.Compare(a.FechaHoraMovimiento.Value, Fecha) > 0);

                }

                if (!string.IsNullOrWhiteSpace(FechaFinal))
                {

                    DateTime Fecha = DateTime.ParseExact(FechaFinal, "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture);

                    listaMovimientos = listaMovimientos.Where(a => DateTime.Compare(a.FechaHoraMovimiento.Value, Fecha) < 0);

                    //listaMovimientos = listaMovimientos.Where(a => DateTime.Compare(a.FechaHoraMovimiento.Value, FechaFinal.Value) < 0);

                }

                if (UbicacionOrigenID != null)
                    listaMovimientos = listaMovimientos.Where(a => a.UbicacionOrigenID == UbicacionOrigenID);

                if (UbicacionDestinoID != null)
                    listaMovimientos = listaMovimientos.Where(a => a.UbicacionDestinoID == UbicacionDestinoID);

            }

            listaMovimientosEnPagina = listaMovimientos.OrderBy(a=> a.FechaHoraMovimiento).Select(x => x).ToList().ToPagedList(pagIndex, pagTamano);

            return PartialView("_Lista", listaMovimientosEnPagina);
        }

        [CustomAuthorize(permiso = "")]
        public ActionResult BuscarPiezas(int LetraFolioID, Guid? GuionID, Guid? UbicacionOrigenID, string tipo = "", int? pagina = null, string claves = "", string listaNombre = "listaTemp")
        {
            int pagTamano = 5;
            int pagIndex = 1;
            pagIndex = pagina.HasValue ? Convert.ToInt32(pagina) : 1;

            List<Pieza> listaPiezas = new List<Pieza>();
            IPagedList<Guid> paginaPiezasIDs;

            bool recargarListaTemp = false;

            if (tipo == "CargarGuion")
            {
                //GuionID = ListaValorID
                var listaValor = db.ListaValores.Find(GuionID);

                listaPiezas = listaValor.AtributoPiezas.Select(a => a.Pieza).ToList();

                //cargar el guion a la listaTemp
                foreach (var item in listaPiezas)
                {
                    if (!addPiezaValidacion(item.PiezaID, UbicacionOrigenID, listaNombre))
                    {
                        AlertaDefault(string.Format("Pieza [{0}]: No se puedo agregar.", item.ImprimirFolio()), true);
                    }
                }

                recargarListaTemp = true;
            }
            else
            {
                var letra = db.LetraFolios.Find(LetraFolioID);

                listaPiezas = db.Piezas.Where(a => a.Obra.LetraFolioID == letra.LetraFolioID).ToList();

                if (!String.IsNullOrEmpty(claves))
                {
                    List<int> listaClaves = new List<int>();
                    string[] listaClavesFormTemp = claves.Split(',');

                    foreach (var clavesTemp in listaClavesFormTemp)
                    {
                        if (clavesTemp.Contains("-"))
                        {
                            string[] claveTemp = clavesTemp.Split('-');
                            var claveInicio = Convert.ToInt32(claveTemp[0]);
                            var claveFinal = Convert.ToInt32(claveTemp[1]);
                            int temp = 0;

                            if (claveInicio > claveFinal)
                            {
                                temp = claveInicio;
                                claveInicio = claveFinal;
                                claveFinal = temp;
                            }

                            for (int i = claveInicio; i <= claveFinal; i++)
                                listaClaves.Add(i);
                        }
                        else
                        {
                            listaClaves.Add(Convert.ToInt32(clavesTemp));
                        }
                    }
                    listaPiezas = listaPiezas.Where(a => a.Obra.LetraFolioID == letra.LetraFolioID && listaClaves.Contains(a.Obra.NumeroFolio)).ToList();
                }

            }

            listaPiezas = listaPiezas.Where(a => a.TipoPieza.EsPrincipal).OrderBy(a => a.Obra.LetraFolio.Nombre).ThenBy(a => a.Obra.NumeroFolio).ToList();
            paginaPiezasIDs = listaPiezas.Select(x => x.PiezaID).ToList().ToPagedList(pagIndex, pagTamano);

            ViewBag.tipo = tipo;
            ViewBag.UbicacionOrigenID = UbicacionOrigenID;
            ViewBag.recargarListaTemp = recargarListaTemp;

            return PartialView("_ResultadosBusqueda", paginaPiezasIDs);
        }

        [CustomAuthorize(permiso = "")]
        public ActionResult FichaMini(Guid? id, Guid? UbicacionOrigenID, string tipoAttBuscar = "guion", string listaNombre = "listaTemp", bool esBusqueda = false)
        {
            var lista = Session[listaNombre] == null ? new List<Guid>() : (List<Guid>)Session[listaNombre];



            string tipoCarusel = "miniThumb";
            string vista = "_FichaMini";

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Pieza pieza = db.Piezas.Find(id);

            if (pieza == null)
                return HttpNotFound();

            itemPiezaMini piezaMini = new itemPiezaMini()
            {
                ObraID = pieza.ObraID,
                PiezaID = pieza.PiezaID,
                FolioObra = pieza.Obra.LetraFolio.Nombre + pieza.Obra.NumeroFolio,
                FolioPieza = pieza.ImprimirFolio(),
                NombreObra = pieza.Obra.TipoObra.Nombre,
                NombrePieza = pieza.TipoPieza.Nombre,
                esPrincipal = pieza.TipoPieza.EsPrincipal,
                esBusqueda = esBusqueda,
                ListaPiezasHijas = new List<Guid>(),
                Atributos = new List<itemPiezaMiniAtt>(),
                UbicacionOrigenID = UbicacionOrigenID,
            };


            piezaMini.enLista = lista.Any(a => a == piezaMini.PiezaID);

            if (esBusqueda)
            {
                if (UbicacionOrigenID == null)
                {
                    piezaMini.esValida = false;
                }
                else
                {
                    if (pieza.UbicacionID == null)
                        piezaMini.esValida = true;
                    else
                        if (pieza.UbicacionID == UbicacionOrigenID)
                            piezaMini.esValida = true;
                }
            }
            else
            {
                piezaMini.esValida = piezaMini.enLista;
            }

            if (pieza.MovimientoPiezas.Where(a => !a.estaDisponible).Count() > 0)
                piezaMini.esValida = false;

            //extraer los campos del tipo de obra

            switch (tipoAttBuscar.ToLower())
            {
                case "guion":
                    tipoAttBuscar = "Guion";
                    tipoCarusel = "miniThumb";
                    vista = "_FichaMini";
                    break;
            }


            var listaAttributosFicha = pieza.TipoPieza.Atributos.Where(a => a.Status && a.MostrarAtributos.Any(b => b.TipoMostrar.Nombre == tipoAttBuscar && b.Status) && a.TipoAtributo.Status).OrderBy(a => a.Orden).ToList();


            //llenar los attFicha
            foreach (var att in listaAttributosFicha)
            {
                var tipoAtt = att.TipoAtributo;

                var attFicha = new itemPiezaMiniAtt()
                {
                    Nombre = att.NombreAlterno,
                    Orden = att.Orden,
                    PiezaID = piezaMini.PiezaID,
                    AtributoID = att.AtributoID,
                    Valores = new List<itemPiezaMiniAttValor>()
                };

                if (tipoAtt.EsGenerico)
                {
                    var lista_AttPieza = pieza.AtributoPiezas.Where(a => a.Atributo == att).ToList();

                    if (lista_AttPieza.Count > 0)
                    {
                        foreach (var item in lista_AttPieza)
                        {
                            var attValor = new itemPiezaMiniAttValor()
                            {
                                AtributoPiezaID = item.AtributoPiezaID,
                                Orden = attFicha.Valores.Count + 1
                            };

                            if (tipoAtt.EsLista)
                            {
                                attValor.Valor = item.ListaValor.Valor;
                            }
                            else
                            {
                                attValor.Valor = item.Valor;
                            }

                            if (!string.IsNullOrWhiteSpace(attValor.Valor))
                            {
                                attFicha.Valores.Add(attValor);
                            }
                        }
                    }
                }
                else
                {
                    switch (tipoAtt.TablaSQL)
                    {
                        case "Autor":
                            var lista_AttAutor = pieza.AutorPiezas.Where(a => a.Status).OrderByDescending(a => a.esPrincipal).ThenBy(a => a.Prefijo).ThenBy(a => a.Autor.Nombre).ToList();
                            if (lista_AttAutor.Count > 0)
                            {
                                foreach (var item in lista_AttAutor)
                                {
                                    var attValor = new itemPiezaMiniAttValor()
                                    {
                                        AtributoPiezaID = item.AutorID,
                                        Orden = attFicha.Valores.Count + 1
                                    };

                                    attValor.Valor = string.IsNullOrWhiteSpace(item.Prefijo) ? "" : item.Prefijo + ": " + item.Autor.Nombre + " " + item.Autor.Apellido;

                                    if (!string.IsNullOrWhiteSpace(item.Autor.Nombre + " " + item.Autor.Apellido))
                                    {
                                        attFicha.Valores.Add(attValor);
                                    }
                                }
                            }
                            break;

                        case "Ubicacion":
                            if (pieza.UbicacionID != null)
                            {
                                var attValor = new itemPiezaMiniAttValor()
                                {
                                    AtributoPiezaID = pieza.UbicacionID,
                                    Orden = 1,
                                    Valor = pieza.Ubicacion.Nombre
                                };

                                if (!string.IsNullOrWhiteSpace(attValor.Valor))
                                {
                                    attFicha.Valores.Add(attValor);
                                }

                            }
                            break;

                        case "TipoTecnica":
                            var lista_Tecnicas = pieza.TecnicaPiezas.Where(a => a.Status).OrderBy(a => a.TipoTecnica.Nombre).ToList();

                            if (lista_Tecnicas.Count > 0)
                            {
                                foreach (var item in lista_Tecnicas)
                                {
                                    var attValor = new itemPiezaMiniAttValor()
                                    {
                                        AtributoPiezaID = item.TecnicaID,
                                        Orden = attFicha.Valores.Count + 1
                                    };

                                    attValor.Valor = item.TipoTecnica.Nombre + ": " + item.Tecnica.Descripcion;

                                    if (!string.IsNullOrWhiteSpace(item.Tecnica.Descripcion))
                                    {
                                        attFicha.Valores.Add(attValor);
                                    }
                                }
                            }
                            break;

                        case "TipoMedida":
                            var lista_Medidas = pieza.MedidaPiezas.Where(a => a.Status).OrderBy(a => a.TipoMedida.Nombre).ToList();
                            if (lista_Medidas.Count > 0)
                            {
                                foreach (var item in lista_Medidas)
                                {
                                    var attValor = new itemPiezaMiniAttValor()
                                    {
                                        AtributoPiezaID = item.TipoMedidaID,
                                        Orden = attFicha.Valores.Count + 1
                                    };

                                    string medidaTexto = "";
                                    bool existe0 = false;
                                    bool existe1 = false;

                                    //1x
                                    existe0 = item.Altura.HasValue ? true : false;
                                    existe1 = item.Anchura.HasValue ? true : false;

                                    medidaTexto += existe0 ? item.Altura.ToString() : "";
                                    medidaTexto += existe0 && existe1 ? "x" : "";
                                    existe0 = existe1;
                                    existe1 = item.Profundidad.HasValue ? true : false;

                                    //2x
                                    medidaTexto += medidaTexto.EndsWith("x") ? "" : medidaTexto.Length > 0 && existe0 ? "x" : "";
                                    medidaTexto += existe0 ? item.Anchura.ToString() : "";
                                    medidaTexto += existe0 && existe1 ? "x" : "";
                                    existe0 = existe1;
                                    existe1 = item.Diametro.HasValue ? true : false;

                                    //3x
                                    medidaTexto += medidaTexto.EndsWith("x") ? "" : medidaTexto.Length > 0 && existe0 ? "x" : "";
                                    medidaTexto += existe0 ? item.Profundidad.ToString() : "";
                                    medidaTexto += existe0 && existe1 ? "x" : "";
                                    existe0 = existe1;
                                    existe1 = item.Diametro2.HasValue ? true : false;

                                    //4Øx
                                    medidaTexto += medidaTexto.EndsWith("x") ? "" : medidaTexto.Length > 0 && existe0 ? "x" : "";
                                    medidaTexto += existe0 ? item.Diametro.ToString() + "Ø" : "";
                                    medidaTexto += existe0 && existe1 ? "x" : "";
                                    existe0 = existe1;
                                    existe1 = item.UMLongitud.HasValue ? true : false;

                                    //cm
                                    medidaTexto += medidaTexto.EndsWith("x") ? "" : medidaTexto.Length > 0 && existe0 ? "x" : "";
                                    medidaTexto += existe0 ? item.Diametro2.ToString() + "Ø" : "";
                                    medidaTexto += existe1 && medidaTexto.Length > 0 ? item.UMLongitud.ToString() + " " : " ";
                                    existe0 = item.Peso.HasValue ? true : false;

                                    //6
                                    medidaTexto += medidaTexto.EndsWith(" ") && existe0 ? item.Peso.ToString() + item.UMMasa : "";
                                    existe0 = item.Otra == null || item.Otra == "" ? false : true;

                                    //otra
                                    medidaTexto += existe0 ? medidaTexto.Length > 0 ? ", " + item.Otra : item.Otra : "";

                                    attValor.Valor = medidaTexto;

                                    if (!string.IsNullOrWhiteSpace(attValor.Valor))
                                    {
                                        attFicha.Valores.Add(attValor);
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }

                if (attFicha.Valores.Count > 0)
                {
                    piezaMini.Atributos.Add(attFicha);
                }
            }

            var imagen = pieza.ImagenPiezas.OrderBy(a => a.Orden).FirstOrDefault(a => a.Status && a.EsPrincipal);

            string textSinImagen = pieza.TipoPieza.EsPrincipal ? "holder.js/100x80/text:404" : "holder.js/80x60/text:404";
            string classTamano = pieza.TipoPieza.EsPrincipal ? "imagenExtraMini" : "imagenSExtraMini";

            if (imagen != null)
            {
                piezaMini.ImagenID = imagen.ImagenPiezaID;
                piezaMini.RutaImagenMini = imagen.RutaMini;
            }


            piezaMini.ListaPiezasHijas.AddRange(pieza.PiezasHijas.Where(a => a.Status).OrderBy(a => a.SubFolio).Select(a => a.PiezaID));

            //pieza.TipoPieza.TipoPiezasHijas = pieza.TipoPieza.TipoPiezasHijas.Where(a => a.Status).OrderBy(a => a.Orden).ToList();

            //ViewBag.listaAttributosFichaCompleta = listaAttributosFicha;


            ViewBag.TipoFicha = tipoAttBuscar;
            ViewBag.tipoCarusel = tipoCarusel;
            ViewBag.listaNombre = listaNombre;
            ViewBag.esPrincipal = pieza.TipoPieza.EsPrincipal;
            ViewBag.textSinImagen = textSinImagen;
            ViewBag.classTamano = classTamano;

            //pieza.PiezasHijas = pieza.PiezasHijas.OrderBy(a => a.SubFolio).ToList();
            return PartialView("~/Views/Movimiento/" + vista + ".cshtml", piezaMini);
        }


        public ActionResult AddPieza(Guid? id, Guid? UbicacionID, string listaNombre = "listaTemp")
        {
            bool success = true;

            if (id == null)
            {
                success = false;
                AlertaDefault(string.Format("Pieza: No existe."), true);
            };

            if (UbicacionID == null)
            {
                success = false;
                AlertaDefault(string.Format("Ubicación de origen: No existe."), true);
            }


            if (success)
            {
                Pieza pieza = db.Piezas.Find(id);

                if (pieza == null)
                {
                    success = false;
                    AlertaDefault(string.Format("Pieza: No existe."), true);
                }

                if (success)
                {
                    if (pieza.MovimientoPiezas.Where(a => !a.estaDisponible).Count() > 0)
                    {
                        success = false;
                        AlertaDefault(string.Format("Pieza: No disponible."), true);
                    }
                }

                if (success)
                {

                    Ubicacion ubicacion = db.Ubicaciones.Find(UbicacionID);

                    if (ubicacion == null)
                    {
                        success = false;
                        AlertaDefault(string.Format("Ubicación: No existe."), true);
                    }

                    if (success)
                    {
                        //la ubicacion vacia es que no se le a asignado ninguna ubicacion actual.
                        if (pieza.UbicacionID == null)
                        {
                            success = true;
                        }
                        else
                        {
                            if (pieza.UbicacionID == ubicacion.UbicacionID)
                            {
                                success = true;
                            }
                            else
                            {
                                success = false;
                                AlertaDefault(string.Format("Pieza [{0}]: Las ubicaciones no coinciden.", pieza.ImprimirFolio()), true);
                            }
                        }

                        if (success)
                        {
                            var lista = Session[listaNombre] == null ? new List<Guid>() : (List<Guid>)Session[listaNombre];

                            if (!lista.Any(a => a == pieza.PiezaID))
                            {
                                lista.Add(pieza.PiezaID);
                                Session[listaNombre] = lista;
                            }
                        }
                    }
                }
            }

            return Json(new { success = success, piezaID = id, lista = listaNombre }, JsonRequestBehavior.AllowGet);
        }


        public bool addPiezaValidacion(Guid id, Guid? UbicacionOrigenID, string listaNombre = "listaTemp")
        {
            bool esOK = true;

            Pieza pieza = db.Piezas.Find(id);

            if (pieza == null)
                esOK = false;


            if (esOK)
            {
                esOK = (pieza.MovimientoPiezas.Where(a => !a.estaDisponible).Count() > 0) ? false : true;
            }

            if (esOK)
            {
                Ubicacion ubicacion = db.Ubicaciones.Find(UbicacionOrigenID);

                if (ubicacion == null)
                    esOK = false;

                //la ubicacion vacia es que no se le a asignado ninguna ubicacion actual.
                if (esOK && pieza.UbicacionID != null)
                    if (pieza.UbicacionID != ubicacion.UbicacionID)
                        esOK = false;
            }


            if (esOK)
            {
                var lista = Session[listaNombre] == null ? new List<Guid>() : (List<Guid>)Session[listaNombre];

                if (!lista.Any(a => a == pieza.PiezaID))
                {
                    lista.Add(pieza.PiezaID);
                    Session[listaNombre] = lista;
                }
            }

            return esOK;
        }

        public ActionResult RenderLista(Guid? UbicacionOrigenID, string listaNombre = "listaTemp")
        {
            var listadePiezasSeleccionadas = Session[listaNombre] == null ? new List<Guid>() : (List<Guid>)Session[listaNombre];

            //revalidar la lista, por si la ubicacion origen fue modificada
            listadePiezasSeleccionadas = db.Piezas.Where(a => listadePiezasSeleccionadas.Any(b => b == a.PiezaID) && (a.UbicacionID == null || a.UbicacionID == UbicacionOrigenID)).Select(a => a.PiezaID).ToList();

            var listaPiezasPrincipales = db.Piezas.Where(a => a.Obra.Piezas.Any(b => listadePiezasSeleccionadas.Any(c => c == b.PiezaID)) && a.TipoPieza.EsPrincipal).OrderBy(a => a.Obra.LetraFolio.Nombre).ThenBy(a => a.Obra.NumeroFolio).Select(a => a.PiezaID).ToList();

            ViewBag.Fecha = DateTime.Now;
            ViewBag.UbicacionOrigenID = UbicacionOrigenID;

            Session[listaNombre] = listadePiezasSeleccionadas;

            return PartialView("_ListaPiezaTemporal", listaPiezasPrincipales);
        }

        public void DelPieza(Guid id, string listaNombre = "listaTemp")
        {
            var lista = Session[listaNombre] == null ? new List<Guid>() : (List<Guid>)Session[listaNombre];

            var pieza = db.Piezas.Find(id);

            if (lista.Any(a => a == id))
            {
                if (lista.Remove(id))
                {
                    AlertaWarning(string.Format("Se quito la pieza <b>{0}</b>", pieza.ImprimirFolio()), true);
                    Session[listaNombre] = lista;
                }
                else
                {
                    AlertaInverse(string.Format("No se puede quitar la pieza <b>{0}</b>", pieza.ImprimirFolio()), true);
                }

            }
        }





        public void EliminarLista(string listaNombre = "listaTemp")
        {
            Session.Remove(listaNombre);
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

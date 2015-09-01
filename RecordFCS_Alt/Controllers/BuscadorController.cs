using RecordFCS_Alt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RecordFCS_Alt.Controllers
{
    public class BuscadorController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: Buscador
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult MenuFiltros(string rutaVista = "_ResultadosBusqueda")
        {
            ViewBag.rutaVista = rutaVista;
            return PartialView("_MenuFiltros");
        }

        //GET: Buscador/AgregarFiltro
        public ActionResult AgregarFiltro()
        {
            var listaTipoAtt = db.TipoAtributos.Where(a => a.Status == true && a.EnBuscador == true).OrderBy(a => a.Orden);

            ViewBag.TipoAtributoID = new SelectList(listaTipoAtt, "TipoAtributoID", "Nombre");


            return PartialView("_AgregarFiltro");
        }


        public ActionResult RenderBuscarCampo(Guid? idTipoAtributo)
        {

            var tipoAtt = db.TipoAtributos.Find(idTipoAtributo);
            PartialViewResult _vista = null;

            if (tipoAtt.EsGenerico)
            {
                _vista = PartialView("~/Views/ListaValor/_CampoBuscador.cshtml");
            }

            ViewBag.TipoAtributoID = tipoAtt.TipoAtributoID;

            return _vista;

        }
    }
}
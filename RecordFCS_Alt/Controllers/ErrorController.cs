using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RecordFCS_Alt.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error/400
        public ActionResult SolicitudIncorrecta()
        {
            Response.StatusCode = 400;

            ViewBag.NoError = 400;
            ViewBag.TituloError = "Solicitud Incorrecta.";
            ViewBag.DescripcionError = "Contacte al administrador si persiste el error.";

            //Enlace
            ViewBag.LinkURLError = Url.Action("Index", "Home", null);
            ViewBag.LinkIconoError = "fa fa-home fa-align-vertical";
            ViewBag.LinkTituloError = " Inicio";

            return View("ErrorGeneral");
        }


        // GET: Error/401
        public ActionResult AccesoDenegado()
        {
            Response.StatusCode = 401;

            ViewBag.NoError = 401;
            ViewBag.TituloError = "Lo sentimos , no cuentas con los permisos para acceder.";
            ViewBag.DescripcionError = "Contacte al administrador si persiste el error.";

            //Enlace
            ViewBag.LinkURLError = Url.Action("Index", "Home", null);
            ViewBag.LinkIconoError = "fa fa-home fa-align-vertical";
            ViewBag.LinkTituloError = " Inicio";

            return View("ErrorGeneral");
        }


        // GET: Error/404
        public ActionResult NoEncontrado()
        {
            Response.StatusCode = 404;

            ViewBag.NoError = 404;
            ViewBag.TituloError = "Lo sentimos, la página solicitada no se encontro!";
            ViewBag.DescripcionError = "Contacte al administrador si persiste el error.";

            //Enlace
            ViewBag.LinkURLError = Url.Action("Index", "Home", null);
            ViewBag.LinkIconoError = "fa fa-home fa-align-vertical";
            ViewBag.LinkTituloError = " Inicio";

            return View("ErrorGeneral");
        }

        // GET: Error/405
        public ActionResult MetodoNoPermitido()
        {
            Response.StatusCode = 405;

            ViewBag.NoError = 405;
            ViewBag.TituloError = "El método de acceso al recurso no es permitido";

            ViewBag.DescripcionError = "Contacte al administrador si persiste el error.";

            ViewBag.LinkURLError = Url.Action("Index", "Home", null);
            ViewBag.LinkIconoError = " fa fa-home fa-align-vertical";
            ViewBag.LinkTituloError = " Inicio";

            return View("ErrorGeneral");
        }

        // GET: Error/408
        public ActionResult TiempoEsperaAgotado()
        {
            Response.StatusCode = 408;

            ViewBag.NoError = 408;
            ViewBag.TituloError = "Tiempo de espera agotado";

            ViewBag.DescripcionError = "Contacte al administrador si persiste el error.";

            ViewBag.LinkURLError = Url.Action("Index", "Home", null);
            ViewBag.LinkIconoError = " fa fa-home fa-align-vertical";
            ViewBag.LinkTituloError = " Inicio";

            return View("ErrorGeneral");
        }

        // GET: Error/500
        public ActionResult Error()
        {
            Response.StatusCode = 500;

            ViewBag.NoError = 500;
            ViewBag.TituloError = "Error del sistema.";
            ViewBag.DescripcionError = "Contacte al administrador si persiste el error.";

            //Enlace
            ViewBag.LinkURLError = Url.Action("Index", "Home", null);
            ViewBag.LinkIconoError = "fa fa-home fa-align-vertical";
            ViewBag.LinkTituloError = " Inicio";

            return View("ErrorGeneral");
        }




    }
}
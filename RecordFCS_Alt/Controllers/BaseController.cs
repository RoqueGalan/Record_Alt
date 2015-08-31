using RecordFCS_Alt.Helpers;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace RecordFCS_Alt.Controllers
{
    public class BaseController : Controller
    {

        /* ALERTAS */
        public void AlertaSuccess(string mensaje, bool descartable = false)
        {
            AddAlerta(AlertaEstilos.Success, mensaje, descartable);
        }

        public void AlertaInfo(string mensaje, bool descartable = false)
        {
            AddAlerta(AlertaEstilos.Info, mensaje, descartable);
        }

        public void AlertaWarning(string mensaje, bool descartable = false)
        {
            AddAlerta(AlertaEstilos.Warning, mensaje, descartable);
        }

        public void AlertaDanger(string mensaje, bool descartable = false)
        {
            AddAlerta(AlertaEstilos.Danger, mensaje, descartable);
        }

        public void AlertaInverse(string mensaje, bool descartable = false)
        {
            AddAlerta(AlertaEstilos.Inverse, mensaje, descartable);
        }

        public void AlertaDefault(string mensaje, bool descartable = false)
        {
            AddAlerta(AlertaEstilos.Default, mensaje, descartable);
        }



        private void AddAlerta(string alertaEstilo, string mensaje, bool descartable)
        {
            var alertas = TempData.ContainsKey(Alerta.TempDataKey)
                ? (List<Alerta>)TempData[Alerta.TempDataKey]
                : new List<Alerta>();

            alertas.Add(new Alerta
            {
                AlertaEstilo = alertaEstilo,
                Mensaje = mensaje,
                Descartable = descartable
            });

            TempData[Alerta.TempDataKey] = alertas;
        }

        public ActionResult _Alertas()
        {
            return PartialView("_Alertas");
        }


    }
}
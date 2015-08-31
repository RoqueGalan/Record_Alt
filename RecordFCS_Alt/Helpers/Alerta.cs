using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RecordFCS_Alt.Helpers
{
    public class Alerta
    {
        public const string TempDataKey = "TempDataAlerts";

        public string AlertaEstilo { get; set; }
        public string Mensaje { get; set; }
        public bool Descartable { get; set; }
    }

    public static class AlertaEstilos
    {
        public const string Success = "success";
        public const string Info = "info";
        public const string Warning = "warning";
        public const string Danger = "danger";
        public const string Inverse = "inverse";
        public const string Default = "default";
    }
}
using System;
using System.Web;
using System.Web.Mvc;

namespace RecordFCS_Alt.Helpers.Seguridad
{
    public abstract class BaseViewPage : WebViewPage
    {
        protected virtual new CustomPrincipal User
        {
            get { return HttpContext.Current.User as CustomPrincipal; }
        }

        protected virtual Boolean IsAuthenticated
        {
            get
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                { return true; }
                else
                { return false; }

            }
        }
        public override void Execute()
        {
        }




    }


    public abstract class BaseViewPage<TModel> : WebViewPage<TModel>
    {
        public virtual new CustomPrincipal User
        {
            get
            {
                return HttpContext.Current.User as CustomPrincipal;
            }
        }

        protected virtual Boolean IsAuthenticated
        {
            get
            {
                return HttpContext.Current.User.Identity.IsAuthenticated;
            }
        }

        public override void Execute()
        {
        }
    }
}
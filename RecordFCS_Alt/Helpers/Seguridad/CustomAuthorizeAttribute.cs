using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;

namespace RecordFCS_Alt.Helpers.Seguridad
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public string permiso { get; set; }

        public virtual CustomPrincipal User
        {
            get { return HttpContext.Current.User as CustomPrincipal; }

        }


        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);

            if (!isAuthorized)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(permiso))
            {
                permiso = permiso.Replace(" ", "");
                string[] roles = permiso.Split(',');

                if (User.ListaRoles.Any(r => roles.Any(rs => rs.Contains(r))))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }




        //public override void OnAuthorization(AuthorizationContext filterContext)
        //{
        //    if (filterContext.HttpContext.Request.IsAuthenticated)
        //    {
        //        var authorizedUsers = ConfigurationManager.AppSettings[UsersConfigKey];
        //        var authorizedRoles = ConfigurationManager.AppSettings[RolesConfigKey];

        //        Users = String.IsNullOrEmpty(Users) ? authorizedUsers : Users;
        //        Roles = String.IsNullOrEmpty(Roles) ? authorizedRoles : Roles;

        //        if (!String.IsNullOrEmpty(Roles))
        //        {
        //            if (!User.IsInRole(Roles))
        //            {
        //                filterContext.Result = new RedirectToRouteResult(new
        //             RouteValueDictionary(new { controller = "Error", action = "AccesoDenegado" }));

        //                // base.OnAuthorization(filterContext); //returns to login url
        //            }
        //        }

        //        if (!String.IsNullOrEmpty(Users))
        //        {
        //            if (!Users.Contains(User.UsuarioID.ToString()))
        //            {
        //                filterContext.Result = new RedirectToRouteResult(new
        //             RouteValueDictionary(new { controller = "Error", action = "AccesoDenegado" }));

        //                // base.OnAuthorization(filterContext); //returns to login url
        //            }
        //        }
        //    }
        //    else
        //    {
        //        filterContext.Result = new RedirectToRouteResult(new
        //             RouteValueDictionary(new { controller = "Error", action = "AccesoDenegado" }));
        //    }

        //}
    }
}
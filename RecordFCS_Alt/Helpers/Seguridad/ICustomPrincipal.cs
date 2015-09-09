using System;
using System.Security.Principal;
using System.Linq;
using System.Web;

namespace RecordFCS_Alt.Helpers.Seguridad
{
    interface ICustomPrincipal : IPrincipal
    {
        Guid UsuarioID { get; set; }
        string Nombre { get; set; }
        string Apellido { get; set; }
        string Tiempo { get; set; }
        string[] ListaRoles { get; set; }
    }

    public class CustomPrincipal : ICustomPrincipal
    {
        public IIdentity Identity { get; private set; }
        public bool IsInRole(string rol)
        {
            bool retorno = false;
            rol = rol.Replace(" ", "");
            string[] roles = rol.Split(',');
            //Separar rol en roles[]

            foreach (var r in roles)
            {
                if (ListaRoles.SingleOrDefault(a => a == r) != null)
                {
                    retorno = true;
                    break;
                }
            }

            return retorno;
        }

        public CustomPrincipal(string userName)
        {
            this.Identity = new GenericIdentity(userName);
        }

        public Guid UsuarioID { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Tiempo { get; set; }

        public string[] ListaRoles { get; set; }

    }


    public class CustomPrincipalSerializeModel
    {
        public Guid UsuarioID { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Tiempo { get; set; }

        public string[] ListaRoles { get; set; }
    }

}
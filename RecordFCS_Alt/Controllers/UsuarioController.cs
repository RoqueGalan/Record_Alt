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
using System.Text;
using System.Web.Security;
using RecordFCS_Alt.Helpers.Seguridad;
using Newtonsoft.Json;
using System.Text.RegularExpressions;


namespace RecordFCS_Alt.Controllers
{
    public class UsuarioController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: Usuario
        [CustomAuthorize(permiso = "usrLis,usrAllEdit")]
        public ActionResult Index()
        {

            return View();
        }

        // GET: Usuario/IniciarSesion
        [AllowAnonymous]
        public ActionResult IniciarSesion()
        {
            var usuario = new Usuario();

            return PartialView("_IniciarSesion", usuario);
        }

        // POST: Usuario/IniciarSesion
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult IniciarSesion([Bind(Include = "UserName,Password")] Usuario usuario, bool Recordarme)
        {
            //validar que usuario y contraseña existan
            usuario.Password = EncriptaPass(usuario.Password);


            if (db.Usuarios.FirstOrDefault(a => a.UserName == usuario.UserName && a.Password == usuario.Password) != null)
            {
                //el usuario es correcto
                var usuarioX = db.Usuarios.FirstOrDefault(u => u.UserName == usuario.UserName);
                //var tiempoActual = DateTime.Now;
                //var tiempoFin = DateTime.Now.AddMinutes(2);

                CustomPrincipalSerializeModel serializeModel = new CustomPrincipalSerializeModel();
                serializeModel.UsuarioID = usuarioX.UsuarioID;
                serializeModel.Nombre = usuarioX.Nombre;
                serializeModel.Apellido = usuarioX.Apellido;
                //serializeModel.Tiempo = tiempoFin.Hour + ":" + tiempoFin.Minute + ":" + tiempoFin.Second;
                serializeModel.ListaRoles = usuarioX.Permisos.Where(a => a.Status).Select(a => a.TipoPermiso.Clave).ToArray();

             

                string userData = JsonConvert.SerializeObject(serializeModel);



                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                    1, 
                    usuarioX.UserName,
                    DateTime.Now, 
                    DateTime.Now.AddHours(5),
                    Recordarme,
                    userData,
                    "/");


                string encTicket = FormsAuthentication.Encrypt(authTicket);
                int maxByteSize = 4000; // Max Cookie Size is 4096 including Cookie Name, Expiry, etc...  
                var tamTicket = System.Text.ASCIIEncoding.ASCII.GetByteCount(encTicket);
                if (tamTicket > maxByteSize)
                {
                    // Raise the alarm that the cookie is going to get rejected by the browser  
                    AlertaDanger("Se supero el limite de permisos.",true);

                }


                HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket) { Expires = authTicket.Expiration };
                Response.Cookies.Add(faCookie);


                if (usuario.Password == EncriptaPass("Record@2015"))
                {
                    AlertaWarning("Recuerda cambiar tu contraseña por una personal, con el fin de tener mayor seguridad.", true);
                }

                string url = Url.Action("Index", "Home");
                return Json(new { success = true, url = url, modelo = "Usuario" });
            }

            //activar el error para 
            if (!String.IsNullOrEmpty(usuario.UserName) || !String.IsNullOrWhiteSpace(usuario.UserName))
            {
                ModelState.Remove("UserName");

            }

            ModelState.Remove("Password");
            ModelState.AddModelError("Password", "La contraseña es incorrecta");
            usuario.Password = "";

            return PartialView("_IniciarSesion", usuario);
        }

        [AllowAnonymous]
        public ActionResult CerrarSesion()
        {
            AlertaWarning("Sesión concluida.");
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home", null);
        }

                

        // GET: Usuario/Lista
        [CustomAuthorize(permiso = "")]
        public ActionResult Lista(string FiltroActual, string Busqueda, int? Pagina)
        {
            if (Busqueda != null) Pagina = 1;
            else Busqueda = FiltroActual;

            ViewBag.FiltroActual = Busqueda;

            var lista = db.Usuarios.Select(a => a);

            if (!String.IsNullOrEmpty(Busqueda))
            {
                Busqueda = Busqueda.ToLower();
                lista = lista.Where(a => a.Nombre.ToLower().Contains(Busqueda) || a.Apellido.ToLower().Contains(Busqueda) || a.UserName.ToLower().Contains(Busqueda));
            }

            lista = lista.OrderBy(a => a.Nombre).ThenBy(a=> a.Apellido);


            //paginador
            int registrosPorPagina = 25;
            int pagActual = 1;
            pagActual = Pagina.HasValue ? Convert.ToInt32(Pagina) : 1;

            IPagedList<Usuario> listaPagina = lista.ToPagedList(pagActual, registrosPorPagina);

            return PartialView("_Lista", listaPagina);
        }

        // GET: Usuario/Detalles/5
        [CustomAuthorize(permiso = "usrDeta,usrAllEdit")]
        public ActionResult Detalles(Guid? id)
        {

            //validar que si tienes los permisos para editar todos los usuarios

            if (!User.IsInRole("usrAllEdit"))
            {
                //comprobar que solo estoy accediendo a mi perfil
                id = User.UsuarioID;
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            return View(usuario);
        }

        // GET: Usuario/Crear
        [CustomAuthorize(permiso = "usrNew,usrAllEdit")]
        public ActionResult Crear()
        {
            Usuario usuario = new Usuario()
            {
                Password = "Record@2015",
                ConfirmPassword = "Record@2015",
            };

            ViewBag.PassDefault = "Record@2015";

            return PartialView("_Crear", usuario);
        }

        // POST: Usuario/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "usrNew,usrAllEdit")]
        public ActionResult Crear([Bind(Include = "UsuarioID,UserName,Password,Nombre,Apellido,Correo,Status,ConfirmPassword")] Usuario usuario)
        {

            if (usuario.Password != usuario.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "La Contraseña no coincide");
            }

            if (ModelState.IsValid)
            {
                //volver a revalidar, ya que hay error de validacion en el cliente
                if (db.Usuarios.Where(a => a.UserName == usuario.UserName).Count() > 0)
                {
                    ModelState.AddModelError("UserName", "Ya existe un registro con este nombre. Intenta con otro.");
                    return PartialView("_Crear", usuario);
                }
                usuario.UsuarioID = Guid.NewGuid();
                usuario.Status = true;

                usuario.Password = EncriptaPass(usuario.Password);

                db.Usuarios.Add(usuario);
                db.SaveChanges();

                AlertaSuccess(string.Format("Usuario: <b>{0}</b> se creo con exitó.", usuario.UserName), true);
                string url = Url.Action("Lista", "Usuario");
                return Json(new { success = true, url = url, modelo = "Usuario" });
            }

            ViewBag.PassDefault = "Record_2015";

            return PartialView("_Crear", usuario);
        }

        // GET: Usuario/Editar/5
        [CustomAuthorize(permiso = "usrEdit,usrAllEdit")]
        public ActionResult Editar(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            usuario.ConfirmPassword = usuario.Password;
            return PartialView("_Editar", usuario);
        }

        // POST: Usuario/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "usrEdit,usrAllEdit")]
        public ActionResult Editar(Usuario usuario)
        {
            var passOld = db.Usuarios.Select(a => new { a.Password, a.UsuarioID }).FirstOrDefault(a => a.UsuarioID == usuario.UsuarioID);
            var passForm = Regex.Replace(usuario.Password.ToString().Trim(), @"\s+", " ");

            if (passForm != passOld.Password)
            {
                usuario.Password = EncriptaPass(passForm);
            }

            usuario.ConfirmPassword = usuario.Password;

            try
            {
                var addr = new System.Net.Mail.MailAddress(usuario.Correo);

            }
            catch
            {
                usuario.Correo = null;
            }


            if (ModelState.IsValid)
            {
                db.Entry(usuario).State = EntityState.Modified;

                db.SaveChanges();

                AlertaSuccess(string.Format("Usuario: <b>{0}</b> se edito con exitó.", usuario.UserName), true);

                var url = Url.Action("Lista", "Usuario");
                return Json(new { success = true, url = url });
            }


            return PartialView("_Editar", usuario);
        }

        // GET: Usuario/Eliminar/5
        [CustomAuthorize(permiso = "usrDel,usrAllEdit")]
        public ActionResult Eliminar(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Eliminar", usuario);
        }

        // POST: Usuario/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "usrDel,usrAllEdit")]
        public ActionResult EliminarConfirmado(Guid id)
        {
            string btnValue = Request.Form["accionx"];
            Usuario usuario = db.Usuarios.Find(id);

            switch (btnValue)
            {
                case "deshabilitar":
                    usuario.Status = false;
                    db.Entry(usuario).State = EntityState.Modified;
                    db.SaveChanges();
                    AlertaDefault(string.Format("Se deshabilito <b>{0}</b>", usuario.UserName), true);

                    break;
                case "eliminar":
                    db.Usuarios.Remove(usuario);
                    db.SaveChanges();
                    AlertaDanger(string.Format("Se elimino <b>{0}</b>", usuario.UserName), true);

                    break;
                default:
                    AlertaDanger(string.Format("Ocurrio un error."), true);
                    break;

            }
            string url = Url.Action("Lista", "Usuario");
            return Json(new { success = true, url = url });
        }

        [HttpPost]
        [CustomAuthorize]
        public JsonResult validarRegistroUnico(string UserName, Guid UsuarioID)
        {
            var usuario = db.Usuarios.Where(a => a.UserName == UserName).FirstOrDefault();
            bool x = false;
            if (usuario == null)
            {
                x = true;
            }
            else
            {
                if (usuario.UsuarioID == UsuarioID)
                {
                    x = true;
                }
            }
            return Json(x);
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult validarCompararPassword(string Password, string ConfirmPassword)
        {
            return Json(Password == ConfirmPassword);
        }


        [AllowAnonymous]
        public ActionResult ResetPassword(string UserName = "", string Correo = "")
        {
            Usuario usuario = new Usuario();

            if (!String.IsNullOrEmpty(UserName) && !String.IsNullOrEmpty(Correo))
            {
                usuario = db.Usuarios.Single(a => a.UserName == UserName && a.Correo == Correo);
            }
            else if (!String.IsNullOrEmpty(UserName))
            {
                //Buscar por nombre de usuario
                usuario = db.Usuarios.Single(a => a.UserName == UserName);
            }
            else if (!String.IsNullOrEmpty(Correo))
            {
                //Buscar por correo
                usuario = db.Usuarios.Single(a => a.Correo == Correo);
            }
            else
            {
                //No se restablera la contraseña

            }


            if (usuario == null)
            {
                AlertaDanger("No se pudo restabler la contraseña, intentelo mas tarde.", true);
            }
            else
            {
                /* IMPLEMENTAR EL ENVIO DE LA CONTRASEÑA POR CORREO */

                AlertaSuccess("Se restablecio la contraseña", true);
                AlertaDefault(string.Format(" Usuario = {0}", usuario.UserName), true);
                AlertaDefault(string.Format(" Contraseña = {0}", usuario.Password), true);
            }

            string url = Url.Action("Index", "Home");
            return Json(new { success = true, url = url, modelo = "Usuario" });
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        /// <summary>
        /// Encripta una cadena (se utiliza para los passwords de los usuarios)
        /// </summary>
        /// <param name="cont">Cadena a encriptar</param>
        /// <returns>Regresa la cadena encriptada</returns>
        /// <remarks></remarks>
        public static string EncriptaPass(string cont)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = Encoding.UTF8.GetBytes(cont);
            bs = x.ComputeHash(bs);
            StringBuilder s = new StringBuilder();
            for (Int16 i = 0; i <= bs.Length - 1; i++)
            {
                s.Append(bs[i].ToString("x2").ToLower());
            }
            return s.ToString();
        }



    }
}

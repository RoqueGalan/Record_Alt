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

namespace RecordFCS_Alt.Controllers
{
    public class UsuarioController : BaseController
    {
        private RecordFCSContext db = new RecordFCSContext();

        // GET: Usuario
        [CustomAuthorize(permiso = "UsuarioVer")]
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
        public ActionResult IniciarSesion([Bind(Include = "UserName,Password")] Usuario usuario)
        {
            //validar que usuario y contraseña existan

            if (db.Usuarios.Where(a => a.UserName == usuario.UserName && a.Password == usuario.Password).Count() == 1)
            {
                //el usuario es correcto
                var usuarioX = db.Usuarios.Where(u => u.UserName == usuario.UserName).First();

                CustomPrincipalSerializeModel serializeModel = new CustomPrincipalSerializeModel();
                serializeModel.UsuarioID = usuarioX.UsuarioID;
                serializeModel.Nombre = usuarioX.Nombre;
                serializeModel.Apellido = usuarioX.Apellido;
                serializeModel.ListaRoles = usuarioX.Permisos.Where(a => a.Status).Select(a => a.TipoPermiso.Clave).ToArray();

                string userData = JsonConvert.SerializeObject(serializeModel);

                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                     1,
                     usuarioX.UserName,
                     DateTime.Now,
                     DateTime.Now.AddMinutes(30),
                     true,
                     userData);


                string encTicket = FormsAuthentication.Encrypt(authTicket);
                HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket) { Expires = authTicket.Expiration};
                Response.Cookies.Add(faCookie);


                if (usuario.Password == "Record_2015")
                {
                    AlertaWarning("Recuerda cambiar tu contraseña por una personal, con el fin de tener mayor seguridad.");
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
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home", null);
        }


        // GET: Usuario/Lista
        [CustomAuthorize(permiso = "UsuarioVer")]
        public ActionResult Lista(int? pagina)
        {
            var usuarios = db.Usuarios.OrderBy(a => a.Nombre);

            ViewBag.totalRegistros = usuarios.Count();

            //paginador
            int pagTamano = 50;
            int pagIndex = 1;
            pagIndex = pagina.HasValue ? Convert.ToInt32(pagina) : 1;

            IPagedList<Usuario> paginaUsuarios = usuarios.ToPagedList(pagIndex, pagTamano);

            return PartialView("_Lista", paginaUsuarios);
        }

        // GET: Usuario/Detalles/5
        [CustomAuthorize(permiso = "UsuarioEdit")]
        public ActionResult Detalles(Guid? id)
        {

            //validar que si tienes los permisos para editar todos los usuarios

            if (!User.IsInRole("UsuarioTodosEdit"))
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
        [CustomAuthorize(permiso = "UsuarioCrear")]
        public ActionResult Crear()
        {
            Usuario usuario = new Usuario()
            {
                Password = "Record_2015",
                ConfirmPassword = "Record_2015",
            };

            ViewBag.PassDefault = "Record_2015";

            return PartialView("_Crear", usuario);
        }

        // POST: Usuario/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "UsuarioCrear")]
        public ActionResult Crear([Bind(Include = "UsuarioID,UserName,Password,Nombre,Apellido,Correo,Status")] Usuario usuario)
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
        [CustomAuthorize(permiso = "UsuarioEdit")]
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
            return PartialView("_Editar", usuario);
        }

        // POST: Usuario/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permiso = "UsuarioEdit")]
        public ActionResult Editar([Bind(Include = "UsuarioID,UserName,Password,Nombre,Apellido,Correo,Status")] Usuario usuario)
        {
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
        [CustomAuthorize(permiso = "UsuarioElimininar")]
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
        [CustomAuthorize(permiso = "UsuarioElimininar")]
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
        public JsonResult validarRegistroUnico(string UserName)
        {
            var lista = db.Usuarios.Where(a => a.UserName == UserName);

            return Json(lista.Count() == 0);
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
    }
}

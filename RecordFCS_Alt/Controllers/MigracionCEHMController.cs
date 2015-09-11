using RecordFCS_Alt.Helpers.Seguridad;
using RecordFCS_Alt.Models;
using RecordFCS_Alt.Models.Migraciones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace RecordFCS_Alt.Controllers
{

    public class MigracionCEHMController : Controller
    {
        private SqlConnection con1 = new SqlConnection(ConfigurationManager.ConnectionStrings["ArchivoCEHMConnection"].ConnectionString);
        private SqlConnection con2 = new SqlConnection(ConfigurationManager.ConnectionStrings["ArchivoCEHMConnection"].ConnectionString);
        private SqlConnection con3 = new SqlConnection(ConfigurationManager.ConnectionStrings["ArchivoCEHMConnection"].ConnectionString);
        private SqlConnection con4 = new SqlConnection(ConfigurationManager.ConnectionStrings["ArchivoCEHMConnection"].ConnectionString);

        private SqlConnection conRec1 = new SqlConnection(ConfigurationManager.ConnectionStrings["RecordNewConnection"].ConnectionString);
        private SqlConnection conRec2 = new SqlConnection(ConfigurationManager.ConnectionStrings["RecordNewConnection"].ConnectionString);
        private SqlConnection conRec3 = new SqlConnection(ConfigurationManager.ConnectionStrings["RecordNewConnection"].ConnectionString);




        private RecordFCSContext db = new RecordFCSContext();


        // GET: MigracionCEHM
        [CustomAuthorize(permiso = "")]
        public ActionResult Index()
        {
            return View();
        }

        [CustomAuthorize(permiso = "")]
        public ActionResult IniciarMigracion()
        {
            int NumFol = 0; //ultima registrada

            RecordFCSContext dbx = new RecordFCSContext();

            int bloqueGuardar = 500;


            Guid TipoObraID = new Guid("375ead18-18db-4a8e-bfbf-7d55ee08ff80");
            TipoObra tipoObra = dbx.TipoObras.Find(TipoObraID);

            Guid TipoPiezaID = new Guid("c84ed502-20d8-4691-9a17-2d739c2bf4da");
            TipoPieza tipoPieza = tipoObra.TipoPiezas.FirstOrDefault(a => a.TipoPiezaID == TipoPiezaID);

            LetraFolio letra = dbx.LetraFolios.SingleOrDefault(a => a.Nombre == "A");
            

            if (tipoObra != null && tipoPieza != null && letra != null)
            {

                //Extraer los atributos requeridos.
                var listaAttRegistro = tipoPieza.Atributos.Where(a => a.Status && a.MostrarAtributos.Any(b => b.TipoMostrar.Nombre == "Registro" && b.Status) && a.TipoAtributo.Status).OrderBy(a => a.Orden).ToList();

                //extraer
                con1.Open();
                string textSql1 = string.Format("SELECT * FROM [{0}]", "ArchivoFondo");
                SqlCommand sql1 = new SqlCommand(textSql1, con1);
                SqlDataReader leer1 = sql1.ExecuteReader();

                List<RowArchivo> listaArchivoCEHM = new List<RowArchivo>();

                int i = NumFol;
               
                while (leer1.Read())
                {
                    i++;
                    var rowArchivo = new RowArchivo()
                    {
                        ArchivoID = Convert.ToInt32(i),
                        Asunto1 = Regex.Replace(leer1["Asunto1"].ToString().Trim(), @"\s+", " "),
                        Asunto2 = Regex.Replace(leer1["Asunto2"].ToString().Trim(), @"\s+", " "),
                        Caja = Regex.Replace(leer1["Caja"].ToString().Trim(), @"\s+", " "),
                        Carpeta = Regex.Replace(leer1["Carpeta"].ToString().Trim(), @"\s+", " "),
                        Clasificacion = Regex.Replace(leer1["Clasificacion"].ToString().Trim(), @"\s+", " "),
                        Documento = Regex.Replace(leer1["Documento"].ToString().Trim(), @"\s+", " "),
                        FICHA_NO = Convert.ToInt32(leer1["FICHA_NO"]),
                        Firmadopor = Regex.Replace(leer1["Firmadopor"].ToString().Trim(), @"\s+", " "),
                        Fojas = Regex.Replace(leer1["Fojas"].ToString().Trim(), @"\s+", " "),
                        Fondo = Regex.Replace(leer1["Fondo"].ToString().Trim(), @"\s+", " "),
                        Legajo = Regex.Replace(leer1["Legajo"].ToString().Trim(), @"\s+", " "),
                        LugaryFecha = Regex.Replace(leer1["LugaryFecha"].ToString().Trim(), @"\s+", " "),
                        NoImag = Convert.ToInt32(leer1["NoImag"].ToString() == "" ? 0 : leer1["NoImag"]),
                        Nombredelfondo = Regex.Replace(leer1["Nombredelfondo"].ToString().Trim(), @"\s+", " "),
                        Tipodedocumento = Regex.Replace(leer1["Tipodedocumento"].ToString().Trim(), @"\s+", " "),
                        URLFicha = Regex.Replace(leer1["URLFicha"].ToString().Trim(), @"\s+", " "),
                        URLImagen = Regex.Replace(leer1["URLImagen"].ToString().Trim(), @"\s+", " "),
                        Tema = Regex.Replace(leer1["Tema"].ToString().Trim(), @"\s+", " "),
                        Dirigidoa = Regex.Replace(leer1["Dirigidoa"].ToString().Trim(), @"\s+", " ")
                        
                    };


                    if (rowArchivo.ArchivoID > 0)
                        listaArchivoCEHM.Add(rowArchivo);

                }
                con1.Close();
                leer1 = null;

                int numeroRow = 0;
                List<AtributoPieza> listaAdd_AttGen = new List<AtributoPieza>();
                List<AutorPieza> listaAdd_AttAutor = new List<AutorPieza>();


                foreach (var row in listaArchivoCEHM)
                {
                    
                    if(numeroRow == bloqueGuardar)
                    {
                        //guardar los atributos
                        dbx.AtributoPiezas.AddRange(listaAdd_AttGen);
                        dbx.AutorPiezas.AddRange(listaAdd_AttAutor);
                        dbx.SaveChanges();

                        dbx.Dispose();
                        dbx = new RecordFCSContext();
                        dbx.Configuration.AutoDetectChangesEnabled = false;

                        numeroRow = 0;
                        listaAdd_AttAutor = new List<AutorPieza>();
                        listaAdd_AttGen = new List<AtributoPieza>();

                    }

                    //tratar los att de la pieza
                    var obra = new Obra()
                    {
                        ObraID = Guid.NewGuid(),
                        FechaRegistro = DateTime.Now,
                        TipoObraID = tipoObra.TipoObraID,
                        LetraFolioID = letra.LetraFolioID,
                        Status = false,
                        NumeroFolio = row.ArchivoID
                    };
                    dbx.Obras.Add(obra);


                    //Crear pieza
                    Pieza pieza = new Pieza()
                    {
                        PiezaID = Guid.NewGuid(),
                        FechaRegistro = obra.FechaRegistro,
                        ObraID = obra.ObraID,
                        Status = false,
                        PiezaPadreID = null, // null = Principal o Maestra
                        TipoPiezaID = tipoPieza.TipoPiezaID,
                        SubFolio = tipoPieza.Prefijo
                    };
                    dbx.Piezas.Add(pieza);


                    foreach (var att in listaAttRegistro)
                    {
                        var tipoAtt = att.TipoAtributo;

                        if (tipoAtt.EsGenerico)
                        {


                            if (tipoAtt.EsLista)
                            {
                                /* 
                                 * GENERICO LISTA
                                 * Fondo - Fondo_CEHM - Fondo
                                 * Colección - Coleccion_Clave - Nombredelfondo
                                 * Legajo - Legajo_CEHM - Legajo
                                 * Fecha de ejecución - FechaEjecucion_Clave - Fecha de ejecucion
                                 */
                                var addOK = true;
                                string valorText = "";
                                switch (tipoAtt.Temp)
                                {
                                    case "Fondo_CEHM":
                                        addOK = row.Fondo == null || row.Fondo == "" ? false : true;
                                        valorText = addOK ? row.Fondo : "";
                                        break;
                                    case "Coleccion_Clave":
                                        addOK = row.Nombredelfondo == null || row.Nombredelfondo == "" ? false : true;
                                        valorText = addOK ? row.Nombredelfondo : "";
                                        break;
                                    case "Legajo_CEHM":
                                        addOK = row.Legajo == null || row.Legajo == "" ? false : true;
                                        valorText = addOK ? row.Legajo : "";
                                        break;
                                    case "FechaEjecucion_Clave":
                                        addOK = row.LugaryFecha == null || row.LugaryFecha == "" ? false : true;
                                        valorText = addOK ? row.LugaryFecha : "";
                                        break;
                                    default:
                                        addOK = false;
                                        break;
                                }


                                if (addOK)
                                {
                                    var listaValor = dbx.ListaValores.SingleOrDefault(a => a.TipoAtributoID == tipoAtt.TipoAtributoID && a.Valor == valorText);

                                    if (listaValor == null)
                                    {
                                        listaValor = new ListaValor()
                                        {
                                            ListaValorID = Guid.NewGuid(),
                                            Status = true,
                                            TipoAtributoID = tipoAtt.TipoAtributoID,
                                            Valor = valorText
                                        };
                                        dbx.ListaValores.Add(listaValor);
                                        dbx.SaveChanges();
                                    }

                                    listaAdd_AttGen.Add(new AtributoPieza()
                                    {
                                        AtributoPiezaID = Guid.NewGuid(),
                                        AtributoID = att.AtributoID,
                                        PiezaID = pieza.PiezaID,
                                        Status = true,
                                        ListaValorID = listaValor.ListaValorID
                                    });
                                }

                            }
                            else
                            {
                                if (tipoAtt.EsMultipleValor)
                                {
                                    /*
                                     * GENERICO TEXTO MULTIPLE
                                     * Descripción - descripcion
                                     * 
                                     * 
                                     * Se forma con : Asunto1, Asunto2, Tema

                                     */

                                    var addOK = true;
                                    string valorText = "";

                                    switch (tipoAtt.Temp)
                                    {
                                        case "descripcion":
                                            // Tema
                                            addOK = true;
                                            valorText = "";
                                            addOK = row.Tema == null || row.Tema == "" ? false : true;

                                            valorText = addOK ? row.Tema : "";

                                            if (addOK)
                                            {
                                                listaAdd_AttGen.Add(new AtributoPieza()
                                                {
                                                    AtributoPiezaID = Guid.NewGuid(),
                                                    AtributoID = att.AtributoID,
                                                    PiezaID = pieza.PiezaID,
                                                    Status = true,
                                                    Valor = valorText
                                                });
                                            }

                                            // Asunto1
                                            addOK = true;
                                            valorText = "";
                                            addOK = row.Asunto1 == null || row.Asunto1 == "" ? false : true;

                                            valorText = addOK ? row.Asunto1 : "";
                                            if (addOK)
                                            {
                                                listaAdd_AttGen.Add(new AtributoPieza()
                                                {
                                                    AtributoPiezaID = Guid.NewGuid(),
                                                    AtributoID = att.AtributoID,
                                                    PiezaID = pieza.PiezaID,
                                                    Status = true,
                                                    Valor = valorText
                                                });
                                            }


                                            // Asunto2
                                            addOK = true;
                                            valorText = "";
                                            addOK = row.Asunto2 == null || row.Asunto2 == "" ? false : true;

                                            valorText = addOK ? row.Asunto2 : "";
                                            if (addOK)
                                            {
                                                listaAdd_AttGen.Add(new AtributoPieza()
                                                {
                                                    AtributoPiezaID = Guid.NewGuid(),
                                                    AtributoID = att.AtributoID,
                                                    PiezaID = pieza.PiezaID,
                                                    Status = true,
                                                    Valor = valorText
                                                });
                                            }

                                            break;
                                        default:
                                            addOK = false;
                                            break;
                                    }

                                }
                                else
                                {
                                    /*
                                     * GENERICOS TEXTO
                                     * No ficha CEHM            - NoFicha_CEHM
                                     * Clasificacion CEHM       - Clasificacion_CEHM
                                     * No de caja o carpeta     - NoCajaOCarpeta_Cehm
                                     * No de documento o fojas  - NoDocFojas_CEHM
                                     * Título descriptivo       - titulo
                                     * Enlace ficha             - UrlFicha_CEHM
                                     * No de imagenes           - NoImagen_CEHM
                                     * Enlace Imagenes          - URLImagen_CEHM
                                     */

                                    var addOK = true;
                                    string valorText = "";

                                    switch (tipoAtt.Temp)
                                    {
                                        case "NoFicha_CEHM":
                                            addOK = row.FICHA_NO == 0 ? false : true;
                                            valorText = addOK ? row.FICHA_NO.ToString() : "0";
                                            break;

                                        case "Clasificacion_CEHM":
                                            addOK = row.Clasificacion == null || row.Clasificacion == "" ? false : true;
                                            valorText = addOK ? row.Clasificacion : "";
                                            break;

                                        case "NoCajaOCarpeta_Cehm":
                                            // se forma con:  caja y carpeta
                                            // queda: Caja: 1 / Carpeta: 1
                                            // queda: Caja: 1
                                            // queda: Carpeta : 1
                                            var cajaOk = false;
                                            addOK = row.Caja == null || row.Caja == "" ? false : true;
                                            valorText += addOK? "" + row.Caja : "";
                                            cajaOk = addOK;
                                            addOK = row.Carpeta == null || row.Carpeta == "" ? false : true;
                                            valorText += cajaOk && addOK? " / " : "";
                                            valorText += addOK ? "" + row.Carpeta : "";
                                            addOK = addOK || cajaOk ? true : false;
                                            break;

                                        case "NoDocFojas_CEHM":
                                            // se forma con Fojas, Documento
                                            var fojaOk = false;
                                            addOK = row.Fojas == null || row.Fojas == "" ? false : true;
                                            valorText += addOK? "" + row.Fojas : "";
                                            fojaOk = addOK;
                                            addOK = row.Documento == null || row.Documento == "" ? false : true;
                                            valorText += fojaOk && addOK? " / " : "";
                                            valorText += addOK ? "" + row.Documento : "";
                                            addOK = addOK || fojaOk ? true : false;
                                            break;

                                        case "titulo":
                                            // Tipodedocumento, Dirigidoa

                                            var tipoDocOk = false;
                                            addOK = row.Tipodedocumento == null || row.Tipodedocumento == "" ? false : true;
                                            valorText += addOK? row.Tipodedocumento : "";
                                            tipoDocOk = addOK;
                                            addOK = row.Dirigidoa == null || row.Dirigidoa == "" ? false : true;
                                            valorText += tipoDocOk && addOK ? " / " : "";
                                            valorText += addOK ? row.Dirigidoa : "";
                                            addOK = addOK || tipoDocOk ? true : false;
                                            break;
                                        case "UrlFicha_CEHM":
                                            addOK = row.URLFicha == null || row.URLFicha == "" ? false : true;
                                            valorText = addOK ? row.URLFicha : "";
                                            break;
                                        case "NoImagen_CEHM":
                                            addOK = row.NoImag == 0 ? false : true;
                                            valorText = addOK ? row.NoImag.ToString() : "0";
                                            addOK = true;
                                            break;
                                        case "URLImagen_CEHM":
                                            addOK = row.URLImagen == null || row.URLImagen == "" ? false : true;
                                            valorText = addOK ? row.URLImagen : "";
                                            break;
                                            
                                        default:
                                            addOK = false;
                                            break;
                                    }

                                    if (addOK)
                                    {
                                        listaAdd_AttGen.Add(new AtributoPieza()
                                        {
                                            AtributoPiezaID = Guid.NewGuid(),
                                            AtributoID = att.AtributoID,
                                            PiezaID = pieza.PiezaID,
                                            Status = true,
                                            Valor = valorText
                                        });

                                    }

                                }
                            }

                        }
                        else
                        {
                            /* 
                             * AUTOR LISTA MULTIPLE
                             * Firmado por
                             */


                            var addOK = true;
                            string valorText = "";

                            addOK = row.Firmadopor == null || row.Firmadopor == "" ? false : true;
                            valorText = addOK ? row.Firmadopor : "";


                            if (addOK)
                            {
                                var autor = dbx.Autores.SingleOrDefault(a => a.Nombre == valorText);

                                if (autor == null)
                                {
                                    autor = new Autor()
                                    {
                                        AutorID = Guid.NewGuid(),
                                        Status = true,
                                        Nombre = valorText
                                    };
                                    dbx.Autores.Add(autor);
                                    dbx.SaveChanges();
                                }


                                listaAdd_AttAutor.Add(new AutorPieza()
                                    {
                                        AutorID = autor.AutorID,
                                        esPrincipal = true,
                                        PiezaID = pieza.PiezaID,
                                        Status = true,
                                        Prefijo = "Principal"
                                    });
                               
                            }


                        }


                    }



                    numeroRow++;
                }

                    //guardar los atributos
                    dbx.AtributoPiezas.AddRange(listaAdd_AttGen);
                    dbx.AutorPiezas.AddRange(listaAdd_AttAutor);
                    dbx.SaveChanges();

                    dbx.Dispose();
                    dbx = new RecordFCSContext();
                    dbx.Configuration.AutoDetectChangesEnabled = false;

            }








            return View();
        }


        [CustomAuthorize(permiso = "")]
        public ActionResult IniciarUsuarios(){

            conRec1.Open();
            string textSql1 = string.Format("SELECT * FROM [{0}]", "m_usuario");
            SqlCommand sql1 = new SqlCommand(textSql1, conRec1);
            SqlDataReader leer1 = sql1.ExecuteReader();

            List<RowUsuario> listaUsuarios = new List<RowUsuario>();

            var i = 0;

            while (leer1.Read())
            {
                i++;
                var rowUsuario = new RowUsuario()
                {
                    a_materno = Regex.Replace(leer1["a_materno"].ToString().Trim(), @"\s+", " "),
                    a_paterno = Regex.Replace(leer1["a_paterno"].ToString().Trim(), @"\s+", " "),
                    cmonitoreo = Regex.Replace(leer1["cmonitoreo"].ToString().Trim(), @"\s+", " "),
                    cve_permiso = Regex.Replace(leer1["cve_permiso"].ToString().Trim(), @"\s+", " "),
                    cve_usuario = Regex.Replace(leer1["cve_usuario"].ToString().Trim(), @"\s+", " "),
                    Departamento_Clave = Regex.Replace(leer1["Departamento_Clave"].ToString().Trim(), @"\s+", " "),
                    email = Regex.Replace(leer1["email"].ToString().Trim(), @"\s+", " "),
                    estatus = Regex.Replace(leer1["estatus"].ToString().Trim(), @"\s+", " "),
                    login = Regex.Replace(leer1["login"].ToString().Trim(), @"\s+", " "),
                    nombre = Regex.Replace(leer1["nombre"].ToString().Trim(), @"\s+", " "),
                    password = Regex.Replace(leer1["password"].ToString().Trim(), @"\s+", " "),
                    passwordOK = Regex.Replace(leer1["passwordOK"].ToString().Trim(), @"\s+", " "),
                    permisos = Regex.Replace(leer1["permisos"].ToString().Trim(), @"\s+", " "),
                    Puesto_Clave = Regex.Replace(leer1["Puesto_Clave"].ToString().Trim(), @"\s+", " ")
                };


                if (rowUsuario.cve_usuario != "0")
                    listaUsuarios.Add(rowUsuario);

            }
            conRec1.Close();
            leer1 = null;

            string pass = "Record@2015";
            string passEncriptado = EncriptaPass(pass);

            foreach (var item in listaUsuarios)
            {
                var usuario = new Usuario();

                usuario.UsuarioID = Guid.NewGuid();
                

                usuario.Apellido = item.a_paterno + " " + item.a_materno;
                usuario.Apellido = Regex.Replace(usuario.Apellido.ToString().Trim(), @"\s+", " ");



                try
                {
                    var addr = new System.Net.Mail.MailAddress(item.email);
                    usuario.Correo = item.email;
                }
                catch
                {
                }




                usuario.Nombre = item.nombre;
                usuario.Password = passEncriptado;





                usuario.Status = item.estatus == "1" ? true : false;
                usuario.UserName = item.login;
                usuario.ConfirmPassword = usuario.Password;


                db.Usuarios.Add(usuario);
                db.SaveChanges();
            }


        return View();
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
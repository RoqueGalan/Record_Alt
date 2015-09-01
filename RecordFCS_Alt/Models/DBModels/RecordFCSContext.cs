using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace RecordFCS_Alt.Models
{
    public class RecordFCSContext : DbContext
    {
        public RecordFCSContext() : base("name=DefaultConnection") { }

        //Configuracion de Estructura de Obras y Piezas
        public DbSet<TipoObra> TipoObras { get; set; }
        public DbSet<TipoPieza> TipoPiezas { get; set; }
        public DbSet<Atributo> Atributos { get; set; }
        public DbSet<TipoAtributo> TipoAtributos { get; set; }
        public DbSet<MostrarAtributo> MostrarAtributos { get; set; }
        public DbSet<TipoMostrar> TipoMostarlos { get; set; }

        //Obras
        public DbSet<LetraFolio> LetraFolios { get; set; }
        public DbSet<Obra> Obras { get; set; }
        public DbSet<Pieza> Piezas { get; set; }

        //Catalogos de Pieza
        public DbSet<Ubicacion> Ubicaciones { get; set; }
        public DbSet<Autor> Autores { get; set; }
        public DbSet<Tecnica> Tecnicas { get; set; }
        public DbSet<ListaValor> ListaValores { get; set; }

        //SubCatalogos de Pieza
        public DbSet<TipoMedida> TipoMedidas { get; set; }
        public DbSet<TipoTecnica> TipoTecnicas { get; set; }

        //Atributos de Pieza
        public DbSet<AutorPieza> AutorPiezas { get; set; }
        public DbSet<ImagenPieza> ImagenPiezas { get; set; }
        public DbSet<MedidaPieza> MedidaPiezas { get; set; }
        public DbSet<TecnicaPieza> TecnicaPiezas { get; set; }
        public DbSet<AtributoPieza> AtributoPiezas { get; set; }


        //usuario
        public DbSet<TipoPermiso> TipoPermisos { get; set; }
        public DbSet<Permiso> Permisos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }






        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //restringir eliminacion en cascada
            //tipo pieza en pieza
            modelBuilder.Entity<TipoPieza>().
                HasMany(a => a.Piezas).
                WithRequired(b => b.TipoPieza).
                WillCascadeOnDelete(false);

            modelBuilder.Entity<TipoTecnica>().
                HasMany(a => a.Tecnicas).
                WithRequired(b => b.TipoTecnica).
                WillCascadeOnDelete(false);

            modelBuilder.Entity<TipoObra>().
                HasMany(a => a.Obras).
                WithRequired(b => b.TipoObra).
                WillCascadeOnDelete(false);
        }
    }
}
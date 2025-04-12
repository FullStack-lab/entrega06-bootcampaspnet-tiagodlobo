using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SGB_Lobo.Models.Context
{
    // Classe de contexto para conexão com o banco de dados
    public class BibliotecaContext : DbContext
    {
        public BibliotecaContext()
            : base("name=BibliotecaContext")
        {
        }

        // Tabelas do banco de dados
        public DbSet<Livro> Livros { get; set; }
        public DbSet<Autor> Autores { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Emprestimo> Emprestimos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Remove a pluralização automática dos nomes das tabelas
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Configura os relacionamentos entre as tabelas
            modelBuilder.Entity<Livro>()
                .HasRequired(l => l.Autor)
                .WithMany(a => a.Livros)
                .HasForeignKey(l => l.AutorId);

            modelBuilder.Entity<Livro>()
                .HasRequired(l => l.Categoria)
                .WithMany(c => c.Livros)
                .HasForeignKey(l => l.CategoriaId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
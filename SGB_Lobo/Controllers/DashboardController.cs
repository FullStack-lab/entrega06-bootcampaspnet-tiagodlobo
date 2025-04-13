using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using SGB_Lobo.AutoMapper;
using SGB_Lobo.Models.Context;
using SGB_Lobo.Models.ViewModels;
using System.Data.Entity;
using System.Collections.Generic;

namespace SGB_Lobo.Controllers
{
    public class DashboardController : Controller
    {
        private BibliotecaContext db = new BibliotecaContext();
        private readonly IMapper _mapper;

        public DashboardController()
        {
            _mapper = MapperConfig.Mapper;
        }

        public ActionResult Index()
        {
            // Estatísticas básicas
            ViewBag.TotalLivros = db.Livros.Count();
            ViewBag.LivrosDisponiveis = db.Livros.Count(l => l.Disponivel);
            ViewBag.TotalAutores = db.Autores.Count();
            ViewBag.TotalCategorias = db.Categorias.Count();
            ViewBag.EmprestimosAtivos = db.Emprestimos.Count(e => !e.DataDevolucao.HasValue);
            ViewBag.EmprestimosAtrasados = db.Emprestimos.Count(e => !e.DataDevolucao.HasValue
                && e.DataPrevistaDevolucao < System.DateTime.Now);

            // Livros mais emprestados (top 5)
            var livrosMaisEmprestados = db.Livros
                .Include(l => l.Emprestimos)
                .Select(l => new LivroMaisEmprestadoViewModel
                {
                    Titulo = l.Titulo,
                    QuantidadeEmprestimos = l.Emprestimos.Count
                })
                .OrderByDescending(l => l.QuantidadeEmprestimos)
                .Take(5)
                .ToList();

            // Usuários com mais empréstimos (top 5)
            var usuariosMaisAtivos = db.Usuarios
                .Select(u => new UsuarioMaisAtivoViewModel
                {
                    Nome = u.Nome,
                    QuantidadeEmprestimos = u.Emprestimos.Count
                })
                .OrderByDescending(u => u.QuantidadeEmprestimos)
                .Take(5)
                .ToList();

            // Total de livros por categoria
            var livrosPorCategoria = db.Categorias
                .Select(c => new LivroPorCategoriaViewModel
                {
                    Nome = c.Nome,
                    QuantidadeLivros = c.Livros.Count
                })
                .OrderByDescending(c => c.QuantidadeLivros)
                .ToList();

            ViewBag.LivrosMaisEmprestados = livrosMaisEmprestados;
            ViewBag.UsuariosMaisAtivos = usuariosMaisAtivos;
            ViewBag.LivrosPorCategoria = livrosPorCategoria;

            return View();
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
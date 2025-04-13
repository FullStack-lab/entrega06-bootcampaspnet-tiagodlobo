using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AutoMapper;
using SGB_Lobo.AutoMapper;
using SGB_Lobo.Models;
using SGB_Lobo.Models.Context;
using SGB_Lobo.Models.ViewModels;

namespace SGB_Lobo.Controllers
{
    public class LivrosController : Controller
    {
        private BibliotecaContext db = new BibliotecaContext();
        private readonly IMapper _mapper;

        public LivrosController()
        {
            _mapper = MapperConfig.Mapper;
        }

        // GET: Livros
        public ActionResult Index(string searchString, string categoria, string disponibilidade)
        {
            var livros = db.Livros
                .Include(l => l.Autor)
                .Include(l => l.Categoria)
                .AsQueryable();

            // Filtro de busca
            if (!string.IsNullOrEmpty(searchString))
            {
                string searchLower = searchString.ToLower();
                livros = livros.Where(l =>
                    l.Titulo.ToLower().Contains(searchLower) ||
                    l.Autor.Nome.ToLower().Contains(searchLower));
                ViewBag.SearchString = searchString;
            }

            // Filtro por categoria
            if (!string.IsNullOrEmpty(categoria))
            {
                livros = livros.Where(l => l.Categoria.Nome == categoria);
                ViewBag.CategoriaId = categoria;
            }

            // Filtro por disponibilidade
            if (!string.IsNullOrEmpty(disponibilidade))
            {
                bool estaDisponivel = disponibilidade == "disponiveis";
                livros = livros.Where(l => l.Disponivel == estaDisponivel);
                ViewBag.Disponibilidade = disponibilidade;
            }

            // Preparar ViewBags para os dropdowns
            ViewBag.Categorias = new SelectList(db.Categorias, "Id", "Nome");

            // Usar AutoMapper para converter para ViewModel
            var livrosViewModel = _mapper.Map<List<LivroViewModel>>(livros.ToList());

            // Retornar a view com os resultados filtrados
            return View(livrosViewModel);
        }

        // GET: livros/disponiveis
        public ActionResult Disponiveis()
        {
            var livros = db.Livros
                .Include(l => l.Autor)
                .Include(l => l.Categoria)
                .Where(l => l.Disponivel)
                .ToList();

            var livrosViewModel = _mapper.Map<List<LivroViewModel>>(livros);
            ViewBag.FiltroAtual = "Livros Disponíveis";
            return View("Index", livrosViewModel);
        }

        // GET: livros/categoria/{categoria}
        public ActionResult PorCategoria(string categoria)
        {
            var livros = db.Livros
                .Include(l => l.Autor)
                .Include(l => l.Categoria)
                .Where(l => l.Categoria.Nome.ToLower() == categoria.ToLower())
                .ToList();

            var livrosViewModel = _mapper.Map<List<LivroViewModel>>(livros);
            ViewBag.FiltroAtual = $"Categoria: {categoria}";
            return View("Index", livrosViewModel);
        }

        // GET: livros/busca/{termo}
        public ActionResult Busca(string termo)
        {
            var livros = db.Livros
                .Include(l => l.Autor)
                .Include(l => l.Categoria)
                .Where(l => l.Titulo.Contains(termo) ||
                            l.Autor.Nome.Contains(termo) ||
                            l.Categoria.Nome.Contains(termo))
                .ToList();

            var livrosViewModel = _mapper.Map<List<LivroViewModel>>(livros);
            ViewBag.FiltroAtual = $"Busca por: {termo}";
            return View("Index", livrosViewModel);
        }

        // GET: Livros/Details/{id}
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var livro = db.Livros
                .Include(l => l.Autor)
                .Include(l => l.Categoria)
                .FirstOrDefault(l => l.Id == id);

            if (livro == null)
                return HttpNotFound();

            var livroViewModel = _mapper.Map<LivroViewModel>(livro);
            return View(livroViewModel);
        }

        // GET: Livros/Create
        public ActionResult Create()
        {
            ViewBag.AutorId = new SelectList(db.Autores, "Id", "Nome");
            ViewBag.CategoriaId = new SelectList(db.Categorias, "Id", "Nome");
            return View();
        }

        // POST: Livros/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LivroViewModel livroViewModel)
        {
            if (ModelState.IsValid)
            {
                // Converter de ViewModel para Entidade
                var livro = _mapper.Map<Livro>(livroViewModel);

                db.Livros.Add(livro);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AutorId = new SelectList(db.Autores, "Id", "Nome", livroViewModel.AutorId);
            ViewBag.CategoriaId = new SelectList(db.Categorias, "Id", "Nome", livroViewModel.CategoriaId);
            return View(livroViewModel);
        }

        // GET: Livros/Edit/{id}
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var livro = db.Livros.Find(id);

            if (livro == null)
                return HttpNotFound();

            // Converter para ViewModel
            var livroViewModel = _mapper.Map<LivroViewModel>(livro);

            ViewBag.AutorId = new SelectList(db.Autores, "Id", "Nome", livroViewModel.AutorId);
            ViewBag.CategoriaId = new SelectList(db.Categorias, "Id", "Nome", livroViewModel.CategoriaId);
            return View(livroViewModel);
        }

        // POST: Livros/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LivroViewModel livroViewModel)
        {
            if (ModelState.IsValid)
            {
                // Converter de ViewModel para Entidade
                var livro = _mapper.Map<Livro>(livroViewModel);

                db.Entry(livro).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AutorId = new SelectList(db.Autores, "Id", "Nome", livroViewModel.AutorId);
            ViewBag.CategoriaId = new SelectList(db.Categorias, "Id", "Nome", livroViewModel.CategoriaId);
            return View(livroViewModel);
        }

        // GET: Livros/Delete/{id}
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var livro = db.Livros
                .Include(l => l.Autor)
                .Include(l => l.Categoria)
                .FirstOrDefault(l => l.Id == id);

            if (livro == null)
                return HttpNotFound();

            // Converter para ViewModel
            var livroViewModel = _mapper.Map<LivroViewModel>(livro);

            return View(livroViewModel);
        }

        // POST: Livros/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var livro = db.Livros.Find(id);
            if (livro == null)
                return HttpNotFound();

            try
            {
                // Verifica se o livro está emprestado
                if (db.Emprestimos.Any(e => e.LivroId == id && e.DataDevolucao == null))
                {
                    TempData["Error"] = "Não é possível excluir o livro pois ele está emprestado.";
                    return RedirectToAction("Index");
                }

                db.Livros.Remove(livro);
                db.SaveChanges();
                TempData["Success"] = "Livro excluído com sucesso!";
            }
            catch (Exception)
            {
                TempData["Error"] = "Ocorreu um erro ao excluir o livro.";
            }

            return RedirectToAction("Index");
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
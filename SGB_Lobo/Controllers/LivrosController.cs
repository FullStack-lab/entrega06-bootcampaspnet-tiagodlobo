using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SGB_Lobo.Models;
using SGB_Lobo.Models.Context;

namespace SGB_Lobo.Controllers
{
    public class LivrosController : Controller
    {
        private BibliotecaContext db = new BibliotecaContext();

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

            // Retornar a view com os resultados filtrados
            return View(livros.ToList());
        }

        // GET: livros/disponiveis
        public ActionResult Disponiveis()
        {
            var livros = db.Livros
                .Include(l => l.Autor)
                .Include(l => l.Categoria)
                .Where(l => l.Disponivel)
                .ToList();

            ViewBag.FiltroAtual = "Livros Disponíveis";
            return View("Index", livros);
        }

        // GET: livros/categoria/{categoria}
        public ActionResult PorCategoria(string categoria)
        {
            var livros = db.Livros
                .Include(l => l.Autor)
                .Include(l => l.Categoria)
                .Where(l => l.Categoria.Nome.ToLower() == categoria.ToLower())
                .ToList();

            ViewBag.FiltroAtual = $"Categoria: {categoria}";
            return View("Index", livros);
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

            ViewBag.FiltroAtual = $"Busca por: {termo}";
            return View("Index", livros);
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

            return View(livro);
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
        public ActionResult Create(Livro livro)
        {
            if (ModelState.IsValid)
            {
                db.Livros.Add(livro);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AutorId = new SelectList(db.Autores, "Id", "Nome", livro.AutorId);
            ViewBag.CategoriaId = new SelectList(db.Categorias, "Id", "Nome", livro.CategoriaId);
            return View(livro);
        }

        // GET: Livros/Edit/{id}
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var livro = db.Livros.Find(id);

            if (livro == null)
                return HttpNotFound();

            ViewBag.AutorId = new SelectList(db.Autores, "Id", "Nome", livro.AutorId);
            ViewBag.CategoriaId = new SelectList(db.Categorias, "Id", "Nome", livro.CategoriaId);
            return View(livro);
        }

        // POST: Livros/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Livro livro)
        {
            if (ModelState.IsValid)
            {
                db.Entry(livro).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AutorId = new SelectList(db.Autores, "Id", "Nome", livro.AutorId);
            ViewBag.CategoriaId = new SelectList(db.Categorias, "Id", "Nome", livro.CategoriaId);
            return View(livro);
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

            return View(livro);
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
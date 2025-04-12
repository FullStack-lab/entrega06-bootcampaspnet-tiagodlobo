using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SGB_Lobo.Models;
using SGB_Lobo.Models.Context;

namespace SGB_Lobo.Controllers
{
    public class EmprestimosController : Controller
    {
        private BibliotecaContext db = new BibliotecaContext();

        // GET: Emprestimos
        public ActionResult Index()
        {
            var emprestimos = db.Emprestimos
                .Include(e => e.Livro)
                .Include(e => e.Usuario)
                .OrderByDescending(e => e.DataEmprestimo)
                .ToList();
            return View(emprestimos);
        }

        // GET: Emprestimos/Create
        public ActionResult Create()
        {
            var livrosDisponiveis = db.Livros
                .Where(l => l.Disponivel)
                .Select(l => new
                {
                    l.Id,
                    NomeCompleto = l.Titulo + " - " + l.Autor.Nome
                });

            var usuariosAtivos = db.Usuarios
                .Where(u => u.Ativo)
                .Select(u => new
                {
                    u.Id,
                    NomeCompleto = u.Nome + " (" + u.Email + ")"
                });

            ViewBag.LivroId = new SelectList(livrosDisponiveis, "Id", "NomeCompleto");
            ViewBag.UsuarioId = new SelectList(usuariosAtivos, "Id", "NomeCompleto");
            ViewBag.DataEmprestimo = DateTime.Now;
            ViewBag.DataPrevistaDevolucao = DateTime.Now.AddDays(7);

            return View();
        }

        // POST: Emprestimos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Emprestimo emprestimo)
        {
            if (ModelState.IsValid)
            {
                var livro = db.Livros.Find(emprestimo.LivroId);
                if (livro == null || !livro.Disponivel)
                {
                    ModelState.AddModelError("", "Este livro não está disponível para empréstimo.");
                    PreparaViewBagsCreate();
                    return View(emprestimo);
                }

                livro.Disponivel = false;
                emprestimo.DataEmprestimo = DateTime.Now;

                db.Emprestimos.Add(emprestimo);
                db.SaveChanges();

                TempData["Success"] = "Empréstimo realizado com sucesso!";
                return RedirectToAction("Index");
            }

            PreparaViewBagsCreate();
            return View(emprestimo);
        }

        // GET: Emprestimos/Devolver/{id}
        public ActionResult Devolver(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var emprestimo = db.Emprestimos
                .Include(e => e.Livro)
                .Include(e => e.Usuario)
                .FirstOrDefault(e => e.Id == id);

            if (emprestimo == null)
                return HttpNotFound();

            if (emprestimo.DataDevolucao.HasValue)
            {
                TempData["Error"] = "Este livro já foi devolvido.";
                return RedirectToAction("Index");
            }

            return View(emprestimo);
        }

        // POST: Emprestimos/Devolver/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Devolver(int id)
        {
            var emprestimo = db.Emprestimos
                .Include(e => e.Livro)
                .Include(e => e.Usuario)
                .FirstOrDefault(e => e.Id == id);

            if (emprestimo == null)
                return HttpNotFound();

            if (emprestimo.DataDevolucao.HasValue)
            {
                TempData["Error"] = "Este livro já foi devolvido.";
                return RedirectToAction("Index");
            }

            emprestimo.DataDevolucao = DateTime.Now;
            emprestimo.Livro.Disponivel = true;

            db.SaveChanges();
            TempData["Success"] = "Livro devolvido com sucesso!";

            return RedirectToAction("Index");
        }

        private void PreparaViewBagsCreate()
        {
            var livrosDisponiveis = db.Livros
                .Where(l => l.Disponivel)
                .Select(l => new
                {
                    l.Id,
                    NomeCompleto = l.Titulo + " - " + l.Autor.Nome
                });

            var usuariosAtivos = db.Usuarios
                .Where(u => u.Ativo)
                .Select(u => new
                {
                    u.Id,
                    NomeCompleto = u.Nome + " (" + u.Email + ")"
                });

            ViewBag.LivroId = new SelectList(livrosDisponiveis, "Id", "NomeCompleto");
            ViewBag.UsuarioId = new SelectList(usuariosAtivos, "Id", "NomeCompleto");
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
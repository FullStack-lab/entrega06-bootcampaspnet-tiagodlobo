using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SGB_Lobo.Models;
using SGB_Lobo.Models.Context;

namespace SGB_Lobo.Controllers
{
    public class CategoriasController : Controller
    {
        private BibliotecaContext db = new BibliotecaContext();

        // GET: Categorias
        public ActionResult Index()
        {
            return View(db.Categorias.ToList());
        }

        // GET: Categorias/Details/{id}
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var categoria = db.Categorias
                .Include(c => c.Livros)
                .FirstOrDefault(c => c.Id == id);

            if (categoria == null)
                return HttpNotFound();

            return View(categoria);
        }

        // GET: Categorias/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Categorias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Categoria categoria)
        {
            // Verifica se já existe uma categoria com o mesmo nome
            if (db.Categorias.Any(c => c.Nome.ToLower() == categoria.Nome.ToLower()))
            {
                ModelState.AddModelError("Nome", "Já existe uma categoria com este nome.");
                return View(categoria);
            }

            if (ModelState.IsValid)
            {
                db.Categorias.Add(categoria);
                db.SaveChanges();
                TempData["Success"] = "Categoria criada com sucesso!";
                return RedirectToAction("Index");
            }

            return View(categoria);
        }

        // GET: Categorias/Edit/{id}
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var categoria = db.Categorias.Find(id);

            if (categoria == null)
                return HttpNotFound();

            return View(categoria);
        }

        // POST: Categorias/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Categoria categoria)
        {
            // Verifica se já existe outra categoria com o mesmo nome
            if (db.Categorias.Any(c => c.Nome.ToLower() == categoria.Nome.ToLower() && c.Id != categoria.Id))
            {
                ModelState.AddModelError("Nome", "Já existe uma categoria com este nome.");
                return View(categoria);
            }

            if (ModelState.IsValid)
            {
                db.Entry(categoria).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Categoria atualizada com sucesso!";
                return RedirectToAction("Index");
            }

            return View(categoria);
        }

        // POST: Categorias/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var categoria = db.Categorias.Find(id);
            if (categoria == null)
                return HttpNotFound();

            try
            {
                // Verifica se a categoria possui livros
                if (db.Livros.Any(l => l.CategoriaId == id))
                {
                    TempData["Error"] = "Não é possível excluir a categoria pois existem livros associados a ela.";
                    return RedirectToAction("Index");
                }

                db.Categorias.Remove(categoria);
                db.SaveChanges();
                TempData["Success"] = "Categoria excluída com sucesso!";
            }
            catch (Exception)
            {
                TempData["Error"] = "Ocorreu um erro ao excluir a categoria.";
            }

            return RedirectToAction("Index");
        }

        // Método para validação remota do nome da categoria
        [HttpPost]
        public JsonResult VerificarNome(string nome, int? id)
        {
            var categoriaExiste = db.Categorias.Any(c =>
                c.Nome.ToLower() == nome.ToLower() &&
                (!id.HasValue || c.Id != id.Value));

            return Json(!categoriaExiste);
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
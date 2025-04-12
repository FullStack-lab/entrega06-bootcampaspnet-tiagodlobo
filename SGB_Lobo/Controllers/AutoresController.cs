using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SGB_Lobo.Models;
using SGB_Lobo.Models.Context;

namespace SGB_Lobo.Controllers
{
    public class AutoresController : Controller
    {
        private BibliotecaContext db = new BibliotecaContext();

        // GET: Autores
        public ActionResult Index()
        {
            return View(db.Autores.ToList());
        }

        // GET: Autores/Details/{id}
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var autor = db.Autores
                .Include(a => a.Livros)
                .FirstOrDefault(a => a.Id == id);

            if (autor == null)
                return HttpNotFound();

            return View(autor);
        }

        // GET: Autores/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Autores/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Autor autor)
        {
            if (ModelState.IsValid)
            {
                db.Autores.Add(autor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(autor);
        }

        // GET: Autores/Edit/{id}
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var autor = db.Autores.Find(id);

            if (autor == null)
                return HttpNotFound();

            return View(autor);
        }

        // POST: Autores/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Autor autor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(autor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(autor);
        }

        // GET: Autores/Delete/{id}
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var autor = db.Autores
                .Include(a => a.Livros)
                .FirstOrDefault(a => a.Id == id);

            if (autor == null)
                return HttpNotFound();

            return View(autor);
        }

        // POST: Autores/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var autor = db.Autores.Find(id);
            if (autor == null)
                return HttpNotFound();

            try
            {
                // Verifica se o autor possui livros
                if (db.Livros.Any(l => l.AutorId == id))
                {
                    TempData["Error"] = "Não é possível excluir o autor pois existem livros associados a ele.";
                    return RedirectToAction("Index");
                }

                db.Autores.Remove(autor);
                db.SaveChanges();
                TempData["Success"] = "Autor excluído com sucesso.";
            }
            catch (Exception)
            {
                TempData["Error"] = "Ocorreu um erro ao excluir o autor.";
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
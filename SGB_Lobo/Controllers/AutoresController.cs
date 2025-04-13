using System;
using System.Collections.Generic;
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
    public class AutoresController : Controller
    {
        private BibliotecaContext db = new BibliotecaContext();
        private readonly IMapper _mapper;

        public AutoresController()
        {
            _mapper = MapperConfig.Mapper;
        }

        // GET: Autores
        public ActionResult Index()
        {
            var autores = db.Autores.ToList();
            var autoresViewModel = _mapper.Map<List<AutorViewModel>>(autores);
            return View(autoresViewModel);
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

            var autorViewModel = _mapper.Map<AutorViewModel>(autor);
            return View(autorViewModel);
        }

        // GET: Autores/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Autores/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AutorViewModel autorViewModel)
        {
            if (ModelState.IsValid)
            {
                var autor = _mapper.Map<Autor>(autorViewModel);
                db.Autores.Add(autor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(autorViewModel);
        }

        // GET: Autores/Edit/{id}
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var autor = db.Autores.Find(id);

            if (autor == null)
                return HttpNotFound();

            var autorViewModel = _mapper.Map<AutorViewModel>(autor);
            return View(autorViewModel);
        }

        // POST: Autores/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AutorViewModel autorViewModel)
        {
            if (ModelState.IsValid)
            {
                var autor = _mapper.Map<Autor>(autorViewModel);
                db.Entry(autor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(autorViewModel);
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

            var autorViewModel = _mapper.Map<AutorViewModel>(autor);
            return View(autorViewModel);
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
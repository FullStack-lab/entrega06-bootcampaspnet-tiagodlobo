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
    public class CategoriasController : Controller
    {
        private BibliotecaContext db = new BibliotecaContext();
        private readonly IMapper _mapper;

        public CategoriasController()
        {
            _mapper = MapperConfig.Mapper;
        }

        // GET: Categorias
        public ActionResult Index()
        {
            var categorias = db.Categorias.ToList();
            var categoriasViewModel = _mapper.Map<List<CategoriaViewModel>>(categorias);
            return View(categoriasViewModel);
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

            var categoriaViewModel = _mapper.Map<CategoriaViewModel>(categoria);
            return View(categoriaViewModel);
        }

        // GET: Categorias/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Categorias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CategoriaViewModel categoriaViewModel)
        {
            // Verifica se já existe uma categoria com o mesmo nome
            if (db.Categorias.Any(c => c.Nome.ToLower() == categoriaViewModel.Nome.ToLower()))
            {
                ModelState.AddModelError("Nome", "Já existe uma categoria com este nome.");
                return View(categoriaViewModel);
            }

            if (ModelState.IsValid)
            {
                var categoria = _mapper.Map<Categoria>(categoriaViewModel);
                db.Categorias.Add(categoria);
                db.SaveChanges();
                TempData["Success"] = "Categoria criada com sucesso!";
                return RedirectToAction("Index");
            }

            return View(categoriaViewModel);
        }

        // GET: Categorias/Edit/{id}
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var categoria = db.Categorias.Find(id);

            if (categoria == null)
                return HttpNotFound();

            var categoriaViewModel = _mapper.Map<CategoriaViewModel>(categoria);
            return View(categoriaViewModel);
        }

        // POST: Categorias/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CategoriaViewModel categoriaViewModel)
        {
            // Verifica se já existe outra categoria com o mesmo nome
            if (db.Categorias.Any(c => c.Nome.ToLower() == categoriaViewModel.Nome.ToLower() && c.Id != categoriaViewModel.Id))
            {
                ModelState.AddModelError("Nome", "Já existe uma categoria com este nome.");
                return View(categoriaViewModel);
            }

            if (ModelState.IsValid)
            {
                var categoria = _mapper.Map<Categoria>(categoriaViewModel);
                db.Entry(categoria).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Categoria atualizada com sucesso!";
                return RedirectToAction("Index");
            }

            return View(categoriaViewModel);
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
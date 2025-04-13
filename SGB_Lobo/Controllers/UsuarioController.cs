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
    public class UsuarioController : Controller
    {
        private BibliotecaContext db = new BibliotecaContext();
        private readonly IMapper _mapper;

        public UsuarioController()
        {
            _mapper = MapperConfig.Mapper;
        }

        // GET: Usuario
        public ActionResult Index()
        {
            return View();
        }

        // GET: Usuario/Create
        public ActionResult Create()
        {
            return View();
        }

        // GET: Usuario/Edit/5
        public ActionResult Edit(int id)
        {
            var usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            var usuarioViewModel = _mapper.Map<UsuarioViewModel>(usuario);
            ViewBag.UsuarioId = id;
            return View(usuarioViewModel);
        }

        // GET: Usuario/Details/5
        public ActionResult Details(int id)
        {
            var usuario = db.Usuarios
                .Include(u => u.Emprestimos)
                .FirstOrDefault(u => u.Id == id);

            if (usuario == null)
            {
                return HttpNotFound();
            }

            var usuarioViewModel = _mapper.Map<UsuarioViewModel>(usuario);
            ViewBag.UsuarioId = id;
            return View(usuarioViewModel);
        }

        // GET: Usuario/Delete/5
        public ActionResult Delete(int id)
        {
            var usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            var usuarioViewModel = _mapper.Map<UsuarioViewModel>(usuario);
            ViewBag.UsuarioId = id;
            return View(usuarioViewModel);
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
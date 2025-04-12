using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SGB_Lobo.Controllers
{
    public class UsuarioController : Controller
    {
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
            ViewBag.UsuarioId = id;
            return View();
        }

        // GET: Usuario/Details/5
        public ActionResult Details(int id)
        {
            ViewBag.UsuarioId = id;
            return View();
        }

        // GET: Usuario/Delete/5
        public ActionResult Delete(int id)
        {
            ViewBag.UsuarioId = id;
            return View();
        }
    }
}
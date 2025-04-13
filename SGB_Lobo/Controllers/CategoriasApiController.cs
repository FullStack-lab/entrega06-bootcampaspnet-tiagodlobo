using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using AutoMapper;
using SGB_Lobo.AutoMapper;
using SGB_Lobo.Models;
using SGB_Lobo.Models.Context;
using SGB_Lobo.Models.ViewModels;

namespace SGB_Lobo.Controllers
{
    [RoutePrefix("api/categorias")]
    public class CategoriasApiController : ApiController
    {
        private BibliotecaContext db = new BibliotecaContext();
        private readonly IMapper _mapper;

        public CategoriasApiController()
        {
            _mapper = MapperConfig.Mapper;
        }

        // GET: api/categorias
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetCategorias()
        {
            var categorias = db.Categorias.ToList();
            var categoriasViewModel = _mapper.Map<List<CategoriaViewModel>>(categorias);

            return Ok(categoriasViewModel);
        }

        // GET: api/categorias/5
        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult GetCategoria(int id)
        {
            var categoria = db.Categorias.Find(id);
            if (categoria == null)
            {
                return NotFound();
            }

            var qtdLivros = db.Livros.Count(l => l.CategoriaId == id);

            var categoriaViewModel = _mapper.Map<CategoriaViewModel>(categoria);
            categoriaViewModel.QuantidadeLivros = qtdLivros;

            return Ok(categoriaViewModel);
        }

        // POST: api/categorias
        [HttpPost]
        [Route("")]
        public IHttpActionResult PostCategoria(CategoriaViewModel categoriaViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Verificar se já existe uma categoria com o mesmo nome
                if (db.Categorias.Any(c => c.Nome == categoriaViewModel.Nome))
                {
                    return BadRequest("Já existe uma categoria com este nome.");
                }

                var categoria = _mapper.Map<Categoria>(categoriaViewModel);
                db.Categorias.Add(categoria);
                db.SaveChanges();

                categoriaViewModel.Id = categoria.Id;
                return Ok(categoriaViewModel);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT: api/categorias/5
        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult PutCategoria(int id, CategoriaViewModel categoriaViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != categoriaViewModel.Id)
            {
                return BadRequest("O ID fornecido não corresponde à categoria");
            }

            // Verificar se já existe outra categoria com o mesmo nome
            if (db.Categorias.Any(c => c.Nome.ToLower() == categoriaViewModel.Nome.ToLower() && c.Id != id))
            {
                return BadRequest("Já existe uma categoria com este nome.");
            }

            var categoria = _mapper.Map<Categoria>(categoriaViewModel);
            db.Entry(categoria).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoriaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/categorias/5
        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult DeleteCategoria(int id)
        {
            var categoria = db.Categorias.Find(id);
            if (categoria == null)
            {
                return NotFound();
            }

            // Verifica se a categoria possui livros
            if (db.Livros.Any(l => l.CategoriaId == id))
            {
                return BadRequest("Não é possível excluir a categoria pois existem livros associados a ela.");
            }

            db.Categorias.Remove(categoria);
            db.SaveChanges();

            return Ok(new { message = "Categoria excluída com sucesso" });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CategoriaExists(int id)
        {
            return db.Categorias.Count(e => e.Id == id) > 0;
        }
    }
}
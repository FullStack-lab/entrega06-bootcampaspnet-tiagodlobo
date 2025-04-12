using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using SGB_Lobo.Models;
using SGB_Lobo.Models.Context;

namespace SGB_Lobo.Controllers
{
    [RoutePrefix("api/categorias")]
    public class CategoriasApiController : ApiController
    {
        private BibliotecaContext db = new BibliotecaContext();

        // GET: api/categorias
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetCategorias()
        {
            var categorias = db.Categorias
                .Select(c => new { id = c.Id, nome = c.Nome })
                .ToList();

            return Ok(categorias);
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

            var resultado = new
            {
                id = categoria.Id,
                nome = categoria.Nome,
                qtdLivros = db.Livros.Count(l => l.CategoriaId == id)
            };

            return Ok(resultado);
        }

        // POST: api/categorias
        [HttpPost]
        [Route("")]
        public IHttpActionResult PostCategoria(Categoria categoria)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Verificar se já existe uma categoria com o mesmo nome
                if (db.Categorias.Any(c => c.Nome == categoria.Nome))
                {
                    return BadRequest("Já existe uma categoria com este nome.");
                }

                db.Categorias.Add(categoria);
                db.SaveChanges();

                // *** IMPORTANTE: Esta é a linha que precisa ser alterada ***
                // Não use CreatedAtRoute, use Ok
                return Ok(new { id = categoria.Id, nome = categoria.Nome });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT: api/categorias/5
        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult PutCategoria(int id, Categoria categoria)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != categoria.Id)
            {
                return BadRequest("O ID fornecido não corresponde à categoria");
            }

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
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Http;
using SGB_Lobo.Models;
using SGB_Lobo.Models.Context;

namespace SGB_Lobo.Controllers
{
    [RoutePrefix("api/livros")]
    public class LivrosApiController : ApiController
    {
        private BibliotecaContext db = new BibliotecaContext();

        // GET api/livros
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get(string searchString = null, int? categoriaId = null, string disponibilidade = null)
        {
            var livros = db.Livros
                .Include(l => l.Autor)
                .Include(l => l.Categoria)
                .AsQueryable();

            // Filtro por texto
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                livros = livros.Where(l =>
                    l.Titulo.ToLower().Contains(searchString) ||
                    l.Autor.Nome.ToLower().Contains(searchString));
            }

            // Filtro por categoria
            if (categoriaId.HasValue)
            {
                livros = livros.Where(l => l.CategoriaId == categoriaId.Value);
            }

            // Filtro por disponibilidade
            if (!string.IsNullOrEmpty(disponibilidade))
            {
                bool disponivel = disponibilidade == "disponivel";
                livros = livros.Where(l => l.Disponivel == disponivel);
            }

            var result = livros.Select(l => new {
                id = l.Id,
                titulo = l.Titulo,
                autor = l.Autor.Nome,
                categoria = l.Categoria.Nome,
                anoPublicacao = l.AnoPublicacao,
                disponivel = l.Disponivel
            }).ToList();

            return Ok(result);
        }

        // GET api/livros/5
        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult Get(int id)
        {
            var livro = db.Livros
                .Include(l => l.Autor)
                .Include(l => l.Categoria)
                .FirstOrDefault(l => l.Id == id);

            if (livro == null)
                return NotFound();

            var result = new
            {
                id = livro.Id,
                titulo = livro.Titulo,
                autorId = livro.AutorId,
                autor = livro.Autor.Nome,
                categoriaId = livro.CategoriaId,
                categoria = livro.Categoria.Nome,
                anoPublicacao = livro.AnoPublicacao,
                disponivel = livro.Disponivel
            };

            return Ok(result);
        }

        // POST api/livros
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] Livro livro)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            db.Livros.Add(livro);
            db.SaveChanges();

            return Created($"api/livros/{livro.Id}", new { id = livro.Id });
        }

        // PUT api/livros/5
        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult Put(int id, [FromBody] Livro livro)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != livro.Id)
                return BadRequest("ID inconsistente");

            var existingLivro = db.Livros.Find(id);
            if (existingLivro == null)
                return NotFound();

            db.Entry(existingLivro).CurrentValues.SetValues(livro);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE api/livros/5
        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult Delete(int id)
        {
            var livro = db.Livros.Find(id);
            if (livro == null)
                return NotFound();

            // Verificar se o livro pode ser excluído
            if (db.Emprestimos.Any(e => e.LivroId == id))
                return BadRequest("Não é possível excluir o livro, pois existem empréstimos relacionados.");

            db.Livros.Remove(livro);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET api/livros/disponiveis
        [HttpGet]
        [Route("disponiveis")]
        public IHttpActionResult GetDisponiveis()
        {
            var livros = db.Livros
                .Include(l => l.Autor)
                .Include(l => l.Categoria)
                .Where(l => l.Disponivel)
                .Select(l => new {
                    id = l.Id,
                    titulo = l.Titulo,
                    autor = l.Autor.Nome,
                    categoria = l.Categoria.Nome,
                    anoPublicacao = l.AnoPublicacao
                })
                .ToList();

            return Ok(livros);
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
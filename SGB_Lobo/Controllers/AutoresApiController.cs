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
    [RoutePrefix("api/autores")]
    public class AutoresApiController : ApiController
    {
        private BibliotecaContext db = new BibliotecaContext();

        // GET: api/autores
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAutores()
        {
            var autores = db.Autores
                .Select(a => new { id = a.Id, nome = a.Nome })
                .ToList();

            return Ok(autores);
        }

        // GET: api/autores/5
        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult GetAutor(int id)
        {
            var autor = db.Autores.Find(id);
            if (autor == null)
            {
                return NotFound();
            }

            var resultado = new
            {
                id = autor.Id,
                nome = autor.Nome,
                qtdLivros = db.Livros.Count(l => l.AutorId == id)
            };

            return Ok(resultado);
        }

        // POST: api/autores
        [HttpPost]
        [Route("")]
        public IHttpActionResult PostAutor(Autor autor)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                db.Autores.Add(autor);
                db.SaveChanges();

                // Retorne apenas um OK com os dados necessários
                return Ok(new { id = autor.Id });
            }
            catch (Exception ex)
            {
                // Log do erro completo
                System.Diagnostics.Debug.WriteLine("Erro ao cadastrar autor: " + ex.ToString());

                // Retorne uma mensagem genérica para o cliente
                return ResponseMessage(new System.Net.Http.HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new System.Net.Http.StringContent("Erro ao processar a requisição.")
                });
            }
        }

        // PUT: api/autores/5
        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult PutAutor(int id, Autor autor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != autor.Id)
            {
                return BadRequest("O ID fornecido não corresponde ao autor");
            }

            db.Entry(autor).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AutorExists(id))
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

        // DELETE: api/autores/5
        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult DeleteAutor(int id)
        {
            var autor = db.Autores.Find(id);
            if (autor == null)
            {
                return NotFound();
            }

            // Verifica se o autor possui livros
            if (db.Livros.Any(l => l.AutorId == id))
            {
                return BadRequest("Não é possível excluir o autor pois existem livros associados a ele.");
            }

            db.Autores.Remove(autor);
            db.SaveChanges();

            return Ok(new { message = "Autor excluído com sucesso" });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AutorExists(int id)
        {
            return db.Autores.Count(e => e.Id == id) > 0;
        }
    }
}
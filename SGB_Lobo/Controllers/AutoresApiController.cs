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
    [RoutePrefix("api/autores")]
    public class AutoresApiController : ApiController
    {
        private BibliotecaContext db = new BibliotecaContext();
        private readonly IMapper _mapper;

        public AutoresApiController()
        {
            _mapper = MapperConfig.Mapper;
        }

        // GET: api/autores
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAutores()
        {
            var autores = db.Autores.ToList();
            var autoresViewModel = _mapper.Map<List<AutorViewModel>>(autores);

            return Ok(autoresViewModel);
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

            // Carregar a quantidade de livros
            var qtdLivros = db.Livros.Count(l => l.AutorId == id);

            var autorViewModel = _mapper.Map<AutorViewModel>(autor);
            autorViewModel.QuantidadeLivros = qtdLivros;

            return Ok(autorViewModel);
        }

        // POST: api/autores
        [HttpPost]
        [Route("")]
        public IHttpActionResult PostAutor(AutorViewModel autorViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var autor = _mapper.Map<Autor>(autorViewModel);
                db.Autores.Add(autor);
                db.SaveChanges();

                return Ok(new { id = autor.Id });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro ao cadastrar autor: " + ex.ToString());
                return ResponseMessage(new System.Net.Http.HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new System.Net.Http.StringContent("Erro ao processar a requisição.")
                });
            }
        }

        // PUT: api/autores/5
        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult PutAutor(int id, AutorViewModel autorViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != autorViewModel.Id)
            {
                return BadRequest("O ID fornecido não corresponde ao autor");
            }

            var autor = _mapper.Map<Autor>(autorViewModel);
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
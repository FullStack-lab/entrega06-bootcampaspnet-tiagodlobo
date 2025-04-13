using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    [RoutePrefix("api/livros")]
    public class LivrosApiController : ApiController
    {
        private BibliotecaContext db = new BibliotecaContext();
        private readonly IMapper _mapper;

        public LivrosApiController()
        {
            _mapper = MapperConfig.Mapper;
        }

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

            // Usar AutoMapper para converter para ViewModel
            var livrosViewModel = _mapper.Map<List<LivroViewModel>>(livros.ToList());

            return Ok(livrosViewModel);
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

            // Usar AutoMapper para converter para ViewModel
            var livroViewModel = _mapper.Map<LivroViewModel>(livro);

            return Ok(livroViewModel);
        }

        // POST api/livros
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] LivroViewModel livroViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Converter de ViewModel para Entidade
            var livro = _mapper.Map<Livro>(livroViewModel);

            db.Livros.Add(livro);
            db.SaveChanges();

            return Created($"api/livros/{livro.Id}", new { id = livro.Id });
        }

        // PUT api/livros/5
        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult Put(int id, [FromBody] LivroViewModel livroViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != livroViewModel.Id)
                return BadRequest("ID inconsistente");

            var existingLivro = db.Livros.Find(id);
            if (existingLivro == null)
                return NotFound();

            // Converter de ViewModel para Entidade
            var livro = _mapper.Map<Livro>(livroViewModel);

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
                .ToList();

            // Usar AutoMapper para converter para ViewModel
            var livrosViewModel = _mapper.Map<List<LivroViewModel>>(livros);

            return Ok(livrosViewModel);
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
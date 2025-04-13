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
    [RoutePrefix("api/emprestimos")]
    public class EmprestimosApiController : ApiController
    {
        private BibliotecaContext db = new BibliotecaContext();
        private readonly IMapper _mapper;

        public EmprestimosApiController()
        {
            _mapper = MapperConfig.Mapper;
        }

        // GET: api/emprestimos
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetEmprestimos()
        {
            try
            {
                var emprestimos = db.Emprestimos
                    .Include(e => e.Livro)
                    .Include(e => e.Usuario)
                    .OrderByDescending(e => e.DataEmprestimo)
                    .ToList();

                var emprestimosViewModel = _mapper.Map<List<EmprestimoViewModel>>(emprestimos);

                return Ok(emprestimosViewModel);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro ao obter empréstimos: " + ex.ToString());
                return InternalServerError(ex);
            }
        }

        // GET: api/emprestimos/5
        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult GetEmprestimo(int id)
        {
            var emprestimo = db.Emprestimos
                .Include(e => e.Livro)
                .Include(e => e.Usuario)
                .FirstOrDefault(e => e.Id == id);

            if (emprestimo == null)
            {
                return NotFound();
            }

            var emprestimoViewModel = _mapper.Map<EmprestimoViewModel>(emprestimo);

            return Ok(emprestimoViewModel);
        }

        // GET: api/emprestimos/livros-disponiveis
        [HttpGet]
        [Route("livros-disponiveis")]
        public IHttpActionResult GetLivrosDisponiveis()
        {
            var livros = db.Livros
                .Include(l => l.Autor)
                .Where(l => l.Disponivel)
                .ToList();

            var livrosViewModel = _mapper.Map<List<LivroViewModel>>(livros);

            var resultado = livrosViewModel.Select(l => new
            {
                id = l.Id,
                titulo = l.Titulo,
                autor = l.AutorNome,
                nomeCompleto = $"{l.Titulo} - {l.AutorNome}"
            }).OrderBy(l => l.titulo);

            return Ok(resultado);
        }

        // GET: api/emprestimos/usuarios-ativos
        [HttpGet]
        [Route("usuarios-ativos")]
        public IHttpActionResult GetUsuariosAtivos()
        {
            var usuarios = db.Usuarios
                .Where(u => u.Ativo)
                .ToList();

            var usuariosViewModel = _mapper.Map<List<UsuarioViewModel>>(usuarios);

            var resultado = usuariosViewModel.Select(u => new
            {
                id = u.Id,
                nome = u.Nome,
                email = u.Email,
                nomeCompleto = $"{u.Nome} ({u.Email})"
            }).OrderBy(u => u.nome);

            return Ok(resultado);
        }

        // POST: api/emprestimos
        [HttpPost]
        [Route("")]
        public IHttpActionResult PostEmprestimo([FromBody] EmprestimoViewModel emprestimoViewModel)
        {
            try
            {
                if (emprestimoViewModel == null)
                {
                    return BadRequest("Dados do empréstimo não fornecidos.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var livro = db.Livros.Find(emprestimoViewModel.LivroId);
                if (livro == null || !livro.Disponivel)
                {
                    return BadRequest("Este livro não está disponível para empréstimo.");
                }

                var usuario = db.Usuarios.Find(emprestimoViewModel.UsuarioId);
                if (usuario == null || !usuario.Ativo)
                {
                    return BadRequest("O usuário selecionado não está ativo.");
                }

                var emprestimo = _mapper.Map<Emprestimo>(emprestimoViewModel);
                emprestimo.DataEmprestimo = DateTime.Now;

                livro.Disponivel = false;

                db.Emprestimos.Add(emprestimo);
                db.SaveChanges();

                return Ok(new { id = emprestimo.Id, message = "Empréstimo realizado com sucesso!" });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT: api/emprestimos/5/devolver
        [HttpPut]
        [Route("{id:int}/devolver")]
        public IHttpActionResult DevolverEmprestimo(int id)
        {
            try
            {
                var emprestimo = db.Emprestimos
                    .Include(e => e.Livro)
                    .FirstOrDefault(e => e.Id == id);

                if (emprestimo == null)
                {
                    return NotFound();
                }

                if (emprestimo.DataDevolucao.HasValue)
                {
                    return BadRequest("Este livro já foi devolvido.");
                }

                emprestimo.DataDevolucao = DateTime.Now;
                emprestimo.Livro.Disponivel = true;

                db.SaveChanges();

                // Mapear para o ViewModel para retornar os dados atualizados
                var emprestimoViewModel = _mapper.Map<EmprestimoViewModel>(emprestimo);

                return Ok(new { message = "Livro devolvido com sucesso!", emprestimo = emprestimoViewModel });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DELETE: api/emprestimos/5
        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult DeleteEmprestimo(int id)
        {
            try
            {
                var emprestimo = db.Emprestimos
                    .Include(e => e.Livro)
                    .FirstOrDefault(e => e.Id == id);

                if (emprestimo == null)
                {
                    return NotFound();
                }

                // Se o livro não foi devolvido, precisamos atualizar o status do livro
                if (!emprestimo.DataDevolucao.HasValue)
                {
                    emprestimo.Livro.Disponivel = true;
                }

                db.Emprestimos.Remove(emprestimo);
                db.SaveChanges();

                return Ok(new { message = "Empréstimo excluído com sucesso!" });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
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
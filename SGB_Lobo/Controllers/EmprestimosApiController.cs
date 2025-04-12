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
    [RoutePrefix("api/emprestimos")]
    public class EmprestimosApiController : ApiController
    {
        private BibliotecaContext db = new BibliotecaContext();

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
                    .ToList()
                    .Select(e => new
                    {
                        id = e.Id,
                        livroId = e.LivroId,
                        livroTitulo = e.Livro.Titulo,
                        usuarioId = e.UsuarioId,
                        usuarioNome = e.Usuario.Nome,
                        dataEmprestimo = e.DataEmprestimo,
                        dataPrevistaDevolucao = e.DataPrevistaDevolucao,
                        dataDevolucao = e.DataDevolucao,
                        emAtraso = !e.DataDevolucao.HasValue && DateTime.Now > e.DataPrevistaDevolucao,
                        devolvido = e.DataDevolucao.HasValue,
                        status = GetStatusEmprestimo(e),
                        diasAtraso = !e.DataDevolucao.HasValue && DateTime.Now > e.DataPrevistaDevolucao
                            ? (int)(DateTime.Now - e.DataPrevistaDevolucao).TotalDays
                            : 0
                    })
                    .ToList();

                return Ok(emprestimos);
            }
            catch (Exception ex)
            {
                // Loga o erro detalhado
                System.Diagnostics.Debug.WriteLine("Erro ao obter empréstimos: " + ex.ToString());

                // Retorna uma resposta de erro mais descritiva
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

            var resultado = new
            {
                id = emprestimo.Id,
                livroId = emprestimo.LivroId,
                livroTitulo = emprestimo.Livro.Titulo,
                usuarioId = emprestimo.UsuarioId,
                usuarioNome = emprestimo.Usuario.Nome,
                dataEmprestimo = emprestimo.DataEmprestimo,
                dataPrevistaDevolucao = emprestimo.DataPrevistaDevolucao,
                dataDevolucao = emprestimo.DataDevolucao,
                emAtraso = !emprestimo.DataDevolucao.HasValue && DateTime.Now > emprestimo.DataPrevistaDevolucao,
                devolvido = emprestimo.DataDevolucao.HasValue,
                status = GetStatusEmprestimo(emprestimo),
                diasAtraso = !emprestimo.DataDevolucao.HasValue && DateTime.Now > emprestimo.DataPrevistaDevolucao
                    ? (int)(DateTime.Now - emprestimo.DataPrevistaDevolucao).TotalDays
                    : 0
            };

            return Ok(resultado);
        }

        // GET: api/emprestimos/livros-disponiveis
        [HttpGet]
        [Route("livros-disponiveis")]
        public IHttpActionResult GetLivrosDisponiveis()
        {
            var livros = db.Livros
                .Include(l => l.Autor)
                .Where(l => l.Disponivel)
                .Select(l => new
                {
                    id = l.Id,
                    titulo = l.Titulo,
                    autor = l.Autor.Nome,
                    nomeCompleto = l.Titulo + " - " + l.Autor.Nome
                })
                .OrderBy(l => l.titulo)
                .ToList();

            return Ok(livros);
        }

        // GET: api/emprestimos/usuarios-ativos
        [HttpGet]
        [Route("usuarios-ativos")]
        public IHttpActionResult GetUsuariosAtivos()
        {
            var usuarios = db.Usuarios
                .Where(u => u.Ativo)
                .Select(u => new
                {
                    id = u.Id,
                    nome = u.Nome,
                    email = u.Email,
                    nomeCompleto = u.Nome + " (" + u.Email + ")"
                })
                .OrderBy(u => u.nome)
                .ToList();

            return Ok(usuarios);
        }

        // POST: api/emprestimos
        [HttpPost]
        [Route("")]
        public IHttpActionResult PostEmprestimo([FromBody] EmprestimoInputModel inputModel)
        {
            try
            {
                if (inputModel == null)
                {
                    return BadRequest("Dados do empréstimo não fornecidos.");
                }

                var emprestimo = new Emprestimo
                {
                    LivroId = inputModel.LivroId,
                    UsuarioId = inputModel.UsuarioId,
                    DataPrevistaDevolucao = inputModel.DataPrevistaDevolucao,
                    DataEmprestimo = DateTime.Now // Definir aqui no servidor
                };

                var livro = db.Livros.Find(emprestimo.LivroId);
                if (livro == null || !livro.Disponivel)
                {
                    return BadRequest("Este livro não está disponível para empréstimo.");
                }

                var usuario = db.Usuarios.Find(emprestimo.UsuarioId);
                if (usuario == null || !usuario.Ativo)
                {
                    return BadRequest("O usuário selecionado não está ativo.");
                }

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

        // Classe auxiliar para deserialização do JSON
        public class EmprestimoInputModel
        {
            public int LivroId { get; set; }
            public int UsuarioId { get; set; }
            public DateTime DataPrevistaDevolucao { get; set; }
            // DataEmprestimo não é necessário aqui
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

                return Ok(new { message = "Livro devolvido com sucesso!" });
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

        private static string GetStatusEmprestimo(Emprestimo emprestimo)
        {
            if (emprestimo.DataDevolucao.HasValue)
            {
                return emprestimo.DataDevolucao <= emprestimo.DataPrevistaDevolucao
                    ? "Devolvido no Prazo"
                    : "Devolvido com Atraso";
            }
            else
            {
                return DateTime.Now > emprestimo.DataPrevistaDevolucao
                    ? "Em Atraso"
                    : "Em Andamento";
            }
        }
    }
}
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
    [RoutePrefix("api/usuarios")]
    public class UsuariosApiController : ApiController
    {
        private BibliotecaContext db = new BibliotecaContext();

        // GET: api/usuarios
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetUsuarios()
        {
            var usuarios = db.Usuarios
                .Select(u => new { id = u.Id, nome = u.Nome, email = u.Email, telefone = u.Telefone, ativo = u.Ativo })
                .ToList();

            return Ok(usuarios);
        }

        // GET: api/usuarios/5
        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult GetUsuario(int id)
        {
            var usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return NotFound();
            }

            var resultado = new
            {
                id = usuario.Id,
                nome = usuario.Nome,
                email = usuario.Email,
                telefone = usuario.Telefone,
                ativo = usuario.Ativo,
                qtdEmprestimos = db.Emprestimos.Count(e => e.UsuarioId == id)
            };

            return Ok(resultado);
        }

        // POST: api/usuarios
        [HttpPost]
        [Route("")]
        public IHttpActionResult PostUsuario(Usuario usuario)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                db.Usuarios.Add(usuario);
                db.SaveChanges();

                // Retorne apenas um OK com os dados necessários
                return Ok(new { id = usuario.Id });
            }
            catch (Exception ex)
            {
                // Log do erro completo
                System.Diagnostics.Debug.WriteLine("Erro ao cadastrar usuário: " + ex.ToString());

                // Retorne uma mensagem genérica para o cliente
                return ResponseMessage(new System.Net.Http.HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new System.Net.Http.StringContent("Erro ao processar a requisição.")
                });
            }
        }

        // PUT: api/usuarios/5
        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult PutUsuario(int id, Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != usuario.Id)
            {
                return BadRequest("O ID fornecido não corresponde ao usuário");
            }

            db.Entry(usuario).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
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

        // DELETE: api/usuarios/5
        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult DeleteUsuario(int id)
        {
            var usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return NotFound();
            }

            // Verifica se o usuário possui empréstimos
            if (db.Emprestimos.Any(e => e.UsuarioId == id))
            {
                return BadRequest("Não é possível excluir o usuário pois existem empréstimos associados a ele.");
            }

            db.Usuarios.Remove(usuario);
            db.SaveChanges();

            return Ok(new { message = "Usuário excluído com sucesso" });
        }

        // GET: api/usuarios/ativos
        [HttpGet]
        [Route("ativos")]
        public IHttpActionResult GetUsuariosAtivos()
        {
            var usuariosAtivos = db.Usuarios
                .Where(u => u.Ativo)
                .Select(u => new { id = u.Id, nome = u.Nome, email = u.Email })
                .ToList();

            return Ok(usuariosAtivos);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UsuarioExists(int id)
        {
            return db.Usuarios.Count(e => e.Id == id) > 0;
        }
    }
}
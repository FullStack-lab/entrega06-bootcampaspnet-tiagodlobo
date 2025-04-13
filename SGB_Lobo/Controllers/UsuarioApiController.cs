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
    [RoutePrefix("api/usuarios")]
    public class UsuariosApiController : ApiController
    {
        private BibliotecaContext db = new BibliotecaContext();
        private readonly IMapper _mapper;

        public UsuariosApiController()
        {
            _mapper = MapperConfig.Mapper;
        }

        // GET: api/usuarios
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetUsuarios()
        {
            var usuarios = db.Usuarios.ToList();
            var usuariosViewModel = _mapper.Map<List<UsuarioViewModel>>(usuarios);

            return Ok(usuariosViewModel);
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

            var qtdEmprestimos = db.Emprestimos.Count(e => e.UsuarioId == id);

            var usuarioViewModel = _mapper.Map<UsuarioViewModel>(usuario);
            usuarioViewModel.QuantidadeEmprestimos = qtdEmprestimos;

            return Ok(usuarioViewModel);
        }

        // POST: api/usuarios
        [HttpPost]
        [Route("")]
        public IHttpActionResult PostUsuario(UsuarioViewModel usuarioViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var usuario = _mapper.Map<Usuario>(usuarioViewModel);
                db.Usuarios.Add(usuario);
                db.SaveChanges();

                usuarioViewModel.Id = usuario.Id;
                return Ok(usuarioViewModel);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro ao cadastrar usuário: " + ex.ToString());
                return ResponseMessage(new System.Net.Http.HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new System.Net.Http.StringContent("Erro ao processar a requisição.")
                });
            }
        }

        // PUT: api/usuarios/5
        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult PutUsuario(int id, UsuarioViewModel usuarioViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != usuarioViewModel.Id)
            {
                return BadRequest("O ID fornecido não corresponde ao usuário");
            }

            var usuario = _mapper.Map<Usuario>(usuarioViewModel);
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
                .ToList();

            var usuariosViewModel = _mapper.Map<List<UsuarioViewModel>>(usuariosAtivos);

            return Ok(usuariosViewModel);
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
using Blog.Application.Interfaces;
using Blog.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.Controllers
{
    [ApiController]
    [Route("api/comentarios")]
    [Authorize]
    public class ComentariosController : Controller
    {

        private readonly IComentarioService _comentarioService;
        private readonly IAutorService _autorService;
        private readonly IPostagemService _postagemService;
        private readonly UserManager<IdentityUser> _userManager;


        public ComentariosController(IComentarioService comentarioService, IAutorService autorService, IPostagemService postagemService, UserManager<IdentityUser> userManager)
        {
            _comentarioService = comentarioService;
            _autorService = autorService;
            _postagemService = postagemService;
            _userManager = userManager;
        }



        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<Comentario>), 200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<Comentario>>> GetAll()
        {
            if (_comentarioService == null) return NotFound();

            var _comentarios = await _comentarioService.GetComentarios();

            return _comentarios.ToList();
        }



        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Comentario), 200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Comentario>> GetById(int id)
        {
            if (id == 0) return NotFound();

            var comentario = await _comentarioService.GetComentarioById(id);

            return comentario == null ? NotFound() : comentario;
        }



        [HttpGet("postagem/{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<Comentario>), 200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<Comentario>>> GetByPostagemId(int id)
        {
            if (id == 0) return NotFound();

            var comentarios = await _comentarioService.GetComentariosByPostagemId(id);

            return comentarios.ToList();
        }



        [HttpPost("{idPostagem:int}")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Comentario>> Add(int idPostagem, [Bind("Conteudo")] Comentario comentario)
        {
            if (comentario == null) return BadRequest();

            var _user = _userManager.GetUserAsync(User).Result;
            Autor _autor = await _autorService.GetAutorByUserId(_user!.Id);

            if (_autor == null)
            {
                return Problem("Você precisa ser um autor cadastrado para criar uma postagem", null, 403);
            }

            ModelState.Remove("IdAutor");
            ModelState.Remove("NomeAutor");
            ModelState.Remove("PostId");
            ModelState.Remove("TituloPostagem");

            if (ModelState.IsValid)
            {
                var postagem = await _postagemService.GetPostagemById(idPostagem);

                if(postagem == null)
                {
                    return Problem("Postagem não encontrada", null, 404);
                }

                comentario.IdAutor = _autor.Id;
                comentario.NomeAutor = _autor.Nome;
                comentario.DataComentario = DateTime.Now;
                comentario.PostId = postagem.Id;
                comentario.TituloPostagem = postagem.Titulo;
                comentario.Ativo = true;

                _comentarioService.AddComentario(comentario);

                return CreatedAtAction(nameof(GetById), new { id = comentario.Id }, comentario);
            }

            return BadRequest(ModelState);

        }



        [HttpPut("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Comentario>> Update(int id, [FromBody] Comentario comentario)
        {
            if(id != comentario.Id) return BadRequest();

            if (_comentarioService == null) return NotFound();

            var _newComentario = await _comentarioService.GetComentarioById(id);
            var _user = _userManager.GetUserAsync(User).Result;
            var _isAdmin = _userManager.IsInRoleAsync(_user!, "Admin").Result;
            var _autor = await _autorService.GetAutorByUserId(_user!.Id);

            if (_autor.Id != _newComentario.IdAutor && !_isAdmin)
            {
                return Problem("Você não tem permissão para editar este comentário", null, 403);
            }

            ModelState.Remove("IdAutor");
            ModelState.Remove("NomeAutor");
            ModelState.Remove("PostId");
            ModelState.Remove("TituloPostagem");

            if(ModelState.IsValid)
            {
                _newComentario.Conteudo = comentario.Conteudo;
                _comentarioService.UpdateComentario(_newComentario);

                return NoContent();
            }

            return BadRequest(ModelState);
        }



        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(403)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Delete(int id)
        {
            if (id == 0) return NotFound();

            var _comentario = await _comentarioService.GetComentarioById(id);

            if(_comentario == null) return NotFound();

            var _user = _userManager.GetUserAsync(User).Result;
            var _isAdmin = _userManager.IsInRoleAsync(_user!, "Admin").Result;
            var _autor = await _autorService.GetAutorByUserId(_user!.Id);

            if (_autor.Id != _comentario.IdAutor && !_isAdmin)
            {
                return Problem("Você não tem permissão para excluir este comentário", null, 403);
            }

            _comentarioService.RemoveComentario(id);

            return NoContent();
        }
    }
}

using Blog.Application.Interfaces;
using Blog.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.Controllers
{
    [ApiController]
    [Route("api/postagens")]
    [Authorize]
    public class PostagensController : Controller
    {
        private readonly IPostagemService _postagemService;
        private readonly IAutorService _autorService;
        private readonly UserManager<IdentityUser> _userManager;


        public PostagensController(IPostagemService postagemService, IAutorService autorService, UserManager<IdentityUser> userManager)
        {
            _postagemService = postagemService;
            _autorService = autorService;
            _userManager = userManager;
        }


        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<Postagem>), 200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<Postagem>>> GetAll()
        {
            if (_postagemService == null) return NotFound();

            var _postagens = await _postagemService.GetPostagens();

            return _postagens.ToList();
        }



        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Postagem), 200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Postagem>> GetById(int id)
        {
            if (id == 0) return NotFound();

            var postagem = await _postagemService.GetPostagemById(id);

            return postagem == null ? NotFound() : postagem;
        }



        [HttpGet("autor/{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<Postagem>), 200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<Postagem>>> GetByAutorId(int id)
        {
            if (id == 0) return NotFound();

            var postagens = await _postagemService.GetPostagensByAutorId(id);

            return postagens == null ? NotFound() : postagens.ToList();
        }



        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Create([Bind("Titulo,Conteudo")] Postagem postagem)
        {
            ModelState.Remove("IdAutor");
            ModelState.Remove("NomeAutor");

            if (ModelState.IsValid)
            {
                var _user = _userManager.GetUserAsync(User).Result;
                Autor _autor = await _autorService.GetAutorByUserId(_user!.Id);

                if (_autor == null)
                {
                    return Problem("Você precisa ser um autor cadastrado para criar uma postagem", null, 403);
                }

                postagem.IdAutor = _autor.Id;
                postagem.NomeAutor = _autor.Nome;
                postagem.DataPublicacao = DateTime.Now;
                postagem.Ativo = true;

                _postagemService.AddPostagem(postagem);

                return CreatedAtAction(nameof(GetById), new { id = postagem.Id }, postagem);
            }

            return BadRequest(ModelState);
        }



        [HttpPut("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Update(int id, [Bind("Id,Titulo,Postagem,Ativo,")] Postagem postagem)
        {
            if (id != postagem.Id) return BadRequest();

            if (_postagemService == null) return NotFound();

            var _newPostagem = await _postagemService.GetPostagemById(id);
            var _user = _userManager.GetUserAsync(User).Result;
            var _isAdmin = _userManager.IsInRoleAsync(_user!, "Admin").Result;
            var _autor = await _autorService.GetAutorByUserId(_user!.Id);

            if(_autor.Id != _newPostagem.IdAutor && !_isAdmin)
            {
                return Problem("Você não tem permissão para editar esta postagem", null, 403);
            }

            ModelState.Remove("IdAutor");
            ModelState.Remove("NomeAutor");

            if (ModelState.IsValid)
            {

                _newPostagem.Titulo = postagem.Titulo;
                _newPostagem.Conteudo = postagem.Conteudo;
                _newPostagem.Ativo = postagem.Ativo;

                _postagemService.UpdatePostagem(_newPostagem);

                return NoContent();
            }

            return BadRequest(ModelState);
        }



        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(403)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0) return NotFound();

            var postagem = await _postagemService.GetPostagemById(id);
            var _user = _userManager.GetUserAsync(User).Result;
            var _isAdmin = _userManager.IsInRoleAsync(_user!, "Admin").Result;
            var _autor = await _autorService.GetAutorByUserId(_user!.Id);

            if (_autor.Id != postagem.IdAutor && !_isAdmin)
            {
                return Problem("Você não tem permissão para excluir esta postagem", null, 403);
            }

            _postagemService.RemovePostagem(id);

            return NoContent();

        }
    }
}

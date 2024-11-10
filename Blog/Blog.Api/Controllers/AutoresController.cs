using Blog.Application.Interfaces;
using Blog.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.Controllers
{
    [ApiController]
    [Route("api/autores")]
    [Authorize]
    public class AutoresController : Controller
    {
        private readonly IAutorService _autorService;
        private readonly UserManager<IdentityUser> _userManager;

        public AutoresController(IAutorService autorService, UserManager<IdentityUser> userManager)
        {
            _autorService = autorService;
            _userManager = userManager;
        }



        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<Autor>), 200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<Autor>>> GetAll()
        {
            if (_autorService == null) return NotFound();

            var _autores = await _autorService.GetAutores();

            return _autores.ToList();
        }

        

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Autor), 200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Autor>> GetById(int id)
        {
            if (id == 0) return NotFound();

            var autor = await _autorService.GetAutorById(id);

            return autor == null ? NotFound() : autor;
        }



        [HttpGet("usuario/{userId}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Autor), 200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Autor>> GetByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return NotFound();

            var autor = await _autorService.GetAutorByUserId(userId);

            return autor == null ? NotFound() : autor;
        }



        [HttpPost]
        [ProducesResponseType(typeof(Autor), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Autor>> Add([Bind("Nome,Email")] Autor autor)
        {
            if(_autorService == null) return Problem("Erro ao criar um autor, contate o suporte.");

            ModelState.Remove("DataCadastro");
            ModelState.Remove("IdUsuario");

            if (!ModelState.IsValid)
            {
                return ValidationProblem(new ValidationProblemDetails(ModelState)
                {
                    Title = "Um ou mais erros de validação ocorreram."
                });
            }

            var _user = await _userManager.GetUserAsync(User);
            var _isAdmin = await _userManager.IsInRoleAsync(_user!, "Admin");

            if (AutorUserExists(_user!.Id))
            {
                return Problem("Já existe um autor vinculado ao seu usuário.");
            }

            autor.IdUsuario = _isAdmin ? autor.IdUsuario : _user!.Id ;
            autor.DataCadastro = DateTime.Now;
            autor.Ativo = true;

            _autorService.AddAutor(autor);

            return CreatedAtAction(nameof(GetById), new { id = autor.Id }, autor);

        }



        [HttpPut("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(403)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Update(int id, [FromBody] Autor autor)
        {
            if (id != autor.Id) return BadRequest();

            if (_autorService == null) return NotFound();

            ModelState.Remove("DataCadastro");
            ModelState.Remove("IdUsuario");

            if (ModelState.IsValid)
            {
                var _user = _userManager.GetUserAsync(User).Result;
                var _isAdmin = _userManager.IsInRoleAsync(_user!, "Admin").Result;

                if(autor.IdUsuario.ToString() != _user!.Id && !_isAdmin)
                {
                    return Forbid();
                }

                _autorService.UpdateAutor(autor);

                return NoContent();
            }

            return ValidationProblem(new ValidationProblemDetails(ModelState)
            {
                Title = "Um ou mais erros de validação ocorreram."
            });
        }



        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(403)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0) return NotFound();

            var autor = await _autorService.GetAutorById(id);

            var _user = _userManager.GetUserAsync(User).Result;
            var _isAdmin = _userManager.IsInRoleAsync(_user!, "Admin").Result;

            if (autor.IdUsuario.ToString() != _user!.Id && !_isAdmin)
            {
                return Forbid();
            }

            _autorService.RemoveAutor(id);

            return NoContent();
        }

        private bool AutorExists(int id)
        {
            return _autorService.GetAutorById(id) != null;
        }

        private bool AutorUserExists(string userId)
        {
            return _autorService.GetAutorByUserId(userId) != null;
        }
    }
}

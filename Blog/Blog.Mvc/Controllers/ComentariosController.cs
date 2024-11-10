using Blog.Application.Interfaces;
using Blog.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Mvc.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("comentarios")]
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



        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var _user = _userManager.GetUserAsync(User) != null ? _userManager.GetUserAsync(User).Result : null;
            var _isAdmin = _user != null ? _userManager.IsInRoleAsync(_user!, "Admin").Result : false;
            var _autor = _user != null ? await _autorService.GetAutorByUserId(_user!.Id) : null;

            if (_user == null || _autor == null && !_isAdmin)
            {
                TempData["Autorizado"] = "false";
            }
            else
            {
                TempData["Autorizado"] = "true";
            }

            IEnumerable<Comentario> model = await _comentarioService.GetComentarios();
            return View(model);
        }



        [AllowAnonymous]
        [Route("detalhes/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            if (id == 0) return NotFound();

            var comentario = await _comentarioService.GetComentarioById(id);

            var _user = _userManager.GetUserAsync(User) != null ? _userManager.GetUserAsync(User).Result : null;
            var _isAdmin = _user != null ? _userManager.IsInRoleAsync(_user!, "Admin").Result : false;
            var _autor = _user != null ? await _autorService.GetAutorByUserId(_user!.Id) : null;


            if (_user == null || _autor!.Id != comentario.IdAutor && !_isAdmin)
            {
                TempData["Autorizado"] = "false";
            }
            else
            {
                TempData["Autorizado"] = "true";
            }

            return comentario == null ? NotFound() : View(comentario);
        }



        [Route("novo-comentario/{idPostagem:int}")]
        public async Task<IActionResult> Create(int idPostagem)
        {
            var _user = _userManager.GetUserAsync(User).Result;
            var _isAdmin = _userManager.IsInRoleAsync(_user!, "Admin").Result;
            Autor _autor = await _autorService.GetAutorByUserId(_user!.Id);

            if(_user == null || _autor == null && !_isAdmin)
            {
                TempData["Mensagem"] = "Você precisa ser um autor para postar comentários";    
                return RedirectToAction("Details","Postagens", new { id = idPostagem });
            }

            TempData["PostagemId"] = idPostagem;
            return View();
        }



        [HttpPost("novo-comentario/{idPostagem:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int idPostagem, [Bind("Conteudo,Id,Ativo")] Comentario comentario)
        {
            ModelState.Remove("IdAutor");
            ModelState.Remove("NomeAutor");
            ModelState.Remove("PostId");
            ModelState.Remove("TituloPostagem");

            var postagem = await _postagemService.GetPostagemById(idPostagem);
            var _user = _userManager.GetUserAsync(User).Result;
            Autor _autor = await _autorService.GetAutorByUserId(_user!.Id);

            comentario.PostId = postagem.Id;
            comentario.TituloPostagem = postagem.Titulo;
            comentario.IdAutor = _autor.Id;
            comentario.NomeAutor = _autor.Nome;
            comentario.Ativo = true;

            if (ModelState.IsValid)
            {
                _comentarioService.AddComentario(comentario);

                //return RedirectToAction(nameof(Index));
                return RedirectToAction(nameof(Details), "Postagens", new { id = idPostagem });
            }

            return View(comentario);
        }



        [Route("editar-comentario/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0) return NotFound();

            var comentario = await _comentarioService.GetComentarioById(id);
            var _user = _userManager.GetUserAsync(User).Result;
            var _isAdmin = _userManager.IsInRoleAsync(_user!, "Admin").Result;
            Autor _autor = await _autorService.GetAutorByUserId(_user!.Id);

            if (_autor != null && _autor.Id != comentario.IdAutor || !_isAdmin)
            {
                TempData["Mensagem"] = "Você não tem permissão para editar este comentário";
                return RedirectToAction(nameof(Index));
            }

            return comentario == null ? NotFound() : View(comentario);
        }



        [HttpPost("editar-comentario/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Conteudo,Id,Ativo")] Comentario comentario)
        {
            if (id != comentario.Id) return NotFound();

            var _newComentario = await _comentarioService.GetComentarioById(id);

            ModelState.Remove("IdAutor");
            ModelState.Remove("NomeAutor");
            ModelState.Remove("TituloPostagem");

            if (ModelState.IsValid)
            {
                try
                {
                    _newComentario.Conteudo = comentario.Conteudo;
                    _comentarioService.UpdateComentario(_newComentario);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComentarioExists(_newComentario.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }

                }

                return RedirectToAction(nameof(Index));
            }

            return View(_newComentario);
        }



        [Route("excluir/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if(id == 0) return NotFound();

            var comentario = await _comentarioService.GetComentarioById(id);
            var _user = _userManager.GetUserAsync(User).Result;
            var _isAdmin = _userManager.IsInRoleAsync(_user!, "Admin").Result;
            Autor _autor = await _autorService.GetAutorByUserId(_user!.Id);

            if (_autor != null && _autor.Id != comentario.IdAutor || !_isAdmin)
            {
                TempData["Mensagem"] = "Você não tem permissão para excluir este comentário";
                return RedirectToAction(nameof(Index));
            }

            return comentario == null ? NotFound() : View(comentario);

        }



        [HttpPost("excluir/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id == 0) return NotFound();

            if (!ComentarioExists(id)) return NotFound();

            _comentarioService.RemoveComentario(id);

            return RedirectToAction(nameof(Index));
        }




        private bool ComentarioExists(int id)
        {
            return _comentarioService.GetComentarioById(id) != null;
        }

    }
}

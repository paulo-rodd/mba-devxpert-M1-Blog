using Microsoft.AspNetCore.Mvc;
using Blog.Domain.Entities;
using Blog.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Blog.Mvc.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("postagens")]
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

            return View(await _postagemService.GetPostagens());
        }



        [AllowAnonymous]
        [Route("detalhes/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            if (id == 0) return NotFound();

            var postagem = await _postagemService.GetPostagemById(id);
            var _user = _userManager.GetUserAsync(User) != null ? _userManager.GetUserAsync(User).Result : null;
            var _isAdmin = _user != null ? _userManager.IsInRoleAsync(_user!, "Admin").Result : false;
            var _autor = _user != null ? await _autorService.GetAutorByUserId(_user!.Id) : null;


            if (_user == null || _autor == null && !_isAdmin || _autor != null && _autor!.Id != postagem.IdAutor && !_isAdmin)
            {
                TempData["Autorizado"] = "false";
            }
            else
            {
                TempData["Autorizado"] = "true";
            }

            return postagem == null ? NotFound() : View(postagem);
        }



        [Route("nova-postagem")]
        public async Task<IActionResult> Create()
        {
            var _user = _userManager.GetUserAsync(User).Result;
            var _isAdmin = _userManager.IsInRoleAsync(_user!, "Admin").Result;
            Autor _autor = await _autorService.GetAutorByUserId(_user!.Id);

            if (_user == null || _autor == null && !_isAdmin)
            {
                TempData["Mensagem"] = "Você precisa ser um autor para adicionar uma postagem";
                return RedirectToAction("Index", "Postagens");
            }
            return View();
        }



        [HttpPost("nova-postagem")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Titulo,Conteudo")] Postagem postagem)
        {
            ModelState.Remove("IdAutor");
            ModelState.Remove("NomeAutor");

            if (ModelState.IsValid)
            {
                var _user = _userManager.GetUserAsync(User).Result;
                Autor _autor = await _autorService.GetAutorByUserId(_user!.Id);

                if(_autor == null)
                {
                    TempData["Mensagem"] = "Você não é um autor";
                    return RedirectToAction(nameof(Index));
                }

                postagem.IdAutor = _autor.Id;
                postagem.NomeAutor = _autor.Nome;
                postagem.DataPublicacao = DateTime.Now;
                postagem.Ativo = true;

                _postagemService.AddPostagem(postagem);

                return RedirectToAction(nameof(Index));
            }

            return View(postagem);
        }



        [Route("editar/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0) return NotFound();

            var postagem = await _postagemService.GetPostagemById(id);
            var _user = _userManager.GetUserAsync(User).Result;
            var _isAdmin = _userManager.IsInRoleAsync(_user!, "Admin").Result;
            var _autor = await _autorService.GetAutorByUserId(_user!.Id);

            if(_autor != null && _autor.Id != postagem.IdAutor || !_isAdmin)
            {
                TempData["Mensagem"] = "Você não tem permissão para editar essa postagem";
                return RedirectToAction(nameof(Index));
            }

            return postagem == null ? NotFound() : View(postagem);
        }



        [HttpPost("editar/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Conteudo,Ativo,DataPublicacao,Autor")] Postagem postagem)
        {
            if (id != postagem.Id) return NotFound();
            var _newPostagem = await _postagemService.GetPostagemById(id);

            ModelState.Remove("IdAutor");
            ModelState.Remove("NomeAutor");

            if (ModelState.IsValid)
            {
                try
                {
                    _newPostagem.Titulo = postagem.Titulo;
                    _newPostagem.Conteudo = postagem.Conteudo;
                    _newPostagem.Ativo = postagem.Ativo;

                    _postagemService.UpdatePostagem(_newPostagem);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostagemExists(postagem.Id))
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

            return View(_newPostagem);
        }



        [Route("excluir/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0) return NotFound();

            var postagem = await _postagemService.GetPostagemById(id);
            var _user = _userManager.GetUserAsync(User).Result;
            var _isAdmin = _userManager.IsInRoleAsync(_user!, "Admin").Result;
            var _autor = await _autorService.GetAutorByUserId(_user!.Id);

            if (_autor.Id != postagem.IdAutor && !_isAdmin)
            {
                TempData["Mensagem"] = "Você não tem permissão para excluir essa postagem";
                return RedirectToAction(nameof(Index));
            }

            return postagem == null ? NotFound() : View(postagem);
        }



        [HttpPost("excluir/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if(id == 0) return NotFound();

            if (!PostagemExists(id)) return NotFound();

            _postagemService.RemovePostagem(id);

            return RedirectToAction(nameof(Index));
        }



        private bool PostagemExists(int id)
        {
            return _postagemService.GetPostagemById(id) != null;
        }

    }
}

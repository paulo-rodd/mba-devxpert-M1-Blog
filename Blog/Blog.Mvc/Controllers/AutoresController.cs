using Blog.Application.Interfaces;
using Blog.Application.Services;
using Blog.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Mvc.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    [Route("autores")]
    public class AutoresController : Controller
    {
        private readonly IAutorService _autorService;
        private readonly UserManager<IdentityUser> _userManager;

        public AutoresController(IAutorService autorService, UserManager<IdentityUser> userManager)
        {
            _autorService = autorService;
            _userManager = userManager;
        }



        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var _user = _userManager.GetUserAsync(User) != null ? _userManager.GetUserAsync(User).Result : null;
            var _isAdmin = _user != null ? _userManager.IsInRoleAsync(_user!, "Admin").Result : false;
            var _autor = _user != null ? await _autorService.GetAutorByUserId(_user!.Id) : null;

            if (_user == null || _autor == null)
            {
                TempData["Autorizado"] = "false";
            }
            else
            {
                TempData["Autorizado"] = "true";
            }

            IEnumerable<Autor> model = await _autorService.GetAutores();

            return View(model);
        }



        [AllowAnonymous]
        [Route("detalhes/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            if(id == 0) return NotFound();

            var autor = await _autorService.GetAutorById(id);
            var _user = _userManager.GetUserAsync(User) != null ? _userManager.GetUserAsync(User).Result : null;
            var _isAdmin = _user != null ? _userManager.IsInRoleAsync(_user!, "Admin").Result : false;
            var _autor = _user != null ? await _autorService.GetAutorByUserId(_user!.Id) : null;


            if (_user == null || _autor!.Id != autor.Id && !_isAdmin)
            {
                TempData["Autorizado"] = "false";
            }
            else
            {
                TempData["Autorizado"] = "true";
            }

            return autor == null ? NotFound() : View(autor);
        }



        [Route("novo-autor")]
        public IActionResult Create()
        {
            var _usersList = _userManager.Users;
            ViewBag.Stores = _usersList;
            return View();
        }



        [HttpPost("novo-autor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,Email,IdUsuario")] Autor autor)
        {
            ModelState.Remove("DataCadastro");
            ModelState.Remove("IdUsuario");

            if(ModelState.IsValid)
            {
                var _user = _userManager.GetUserAsync(User).Result;
                var _isAdmin = _userManager.IsInRoleAsync(_user!, "Admin").Result;

                autor.IdUsuario = _isAdmin ? autor.IdUsuario.ToUpper() : _user!.Id;
                autor.DataCadastro = DateTime.Now;
                autor.Ativo = true;

                _autorService.AddAutor(autor);
                return RedirectToAction(nameof(Index));
                
            }

            return View(autor);

        }



        [Route("editar/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0) return NotFound();

            var autor = await _autorService.GetAutorById(id);

            var _user = _userManager.GetUserAsync(User).Result;
            var isAdmin = _userManager.IsInRoleAsync(_user!, "Admin").Result;

            if(autor.IdUsuario.ToString() != _user!.Id && !isAdmin)
            {
                TempData["Mensagem"] = "Você não tem permissão para editar este autor";
                return RedirectToAction(nameof(Index));
            }

            return autor == null ? NotFound() : View(autor);
        }



        [HttpPost("editar/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Email,DataCadastro,IdUsuario,Ativo")] Autor autor)
        {
            if (id != autor.Id) return NotFound();

            ModelState.Remove("DataCadastro");
            ModelState.Remove("IdUsuario");

            if (ModelState.IsValid)
            {
                try
                {
                    var _newAutor = await _autorService.GetAutorById(id);

                    _newAutor.Nome = autor.Nome;
                    _newAutor.Email = autor.Email;
                    _newAutor.Ativo = autor.Ativo;

                    _autorService.UpdateAutor(_newAutor);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AutorExists(autor.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                TempData["Sucesso"] = "Autor atualizado com sucesso";

                return RedirectToAction(nameof(Index));
            }
            return View(autor);
        }



        [Route("excluir/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null) return NotFound();

            var autor = await _autorService.GetAutorById(id);

            var _user = _userManager.GetUserAsync(User).Result;
            var isAdmin = _userManager.IsInRoleAsync(_user!, "Admin").Result;

            if (autor.IdUsuario.ToString() != _user!.Id && !isAdmin)
            {
                TempData["Mensagem"] = "Você não tem permissão para excluir este autor";
                return RedirectToAction(nameof(Index));
            }

            return autor == null ? NotFound() : View(autor);
        }



        [HttpPost("excluir/{id:int}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var autor = await _autorService.GetAutorById(id);

            _autorService.RemoveAutor(id);

            TempData["Sucesso"] = "Autor excluído com sucesso";

            return RedirectToAction(nameof(Index));
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

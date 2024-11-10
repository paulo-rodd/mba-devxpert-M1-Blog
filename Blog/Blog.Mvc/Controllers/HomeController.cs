using Blog.Application.Interfaces;
using Blog.Application.Services;
using Blog.Mvc.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Blog.Mvc.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPostagemService _postagemService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAutorService _autorService;

        public HomeController(ILogger<HomeController> logger, IPostagemService postagemService, UserManager<IdentityUser> userManager, IAutorService autorService)
        {
            _logger = logger;
            _postagemService = postagemService;
            _userManager = userManager;
            _autorService = autorService;
        }

        public async Task<IActionResult> Index()
        {
            if(_userManager != null)
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
            }
            else
            {
                TempData["Autorizado"] = "false";
            }
            

            //return View();
            return View(await _postagemService.GetPostagens());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

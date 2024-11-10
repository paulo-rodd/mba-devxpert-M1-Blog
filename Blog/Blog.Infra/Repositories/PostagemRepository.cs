using Blog.Domain.Entities;
using Blog.Domain.Interfaces;
using Blog.Infra.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Blog.Infra.Repositories
{
    public class PostagemRepository : IPostagemRepository, IDisposable
    {
        private readonly BlogDbContext _blogContext;
        private readonly UserManager<IdentityUser> _userManager;

        public PostagemRepository(BlogDbContext blogContext, UserManager<IdentityUser> userManager)
        {
            _blogContext = blogContext;
            _userManager = userManager;
        }

        public async Task<IEnumerable<Postagem>> GetPostagens()
        {
            return await _blogContext.Postagens.ToListAsync();
        }

        public async Task<Postagem> GetPostagemById(int id)
        {
            return await _blogContext.Postagens.FindAsync(id);
        }

        public async Task<IEnumerable<Postagem>> GetPostagensByAutorId(int id)
        {
            return await _blogContext.Postagens.Where(p => p.IdAutor == id).ToListAsync();
        }

        public async void AddPostagem(Postagem postagem)
        {
            _blogContext.Postagens.Add(postagem);
            await _blogContext.SaveChangesAsync();
        }

        public async void UpdatePostagem(Postagem postagem)
        {
            _blogContext.Entry(postagem).State = EntityState.Modified;
            await _blogContext.SaveChangesAsync();
        }

        public async void RemovePostagem(int id)
        {
            Postagem postagem = _blogContext.Postagens.Find(id);
            _blogContext.Postagens.Remove(postagem);
            await _blogContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _blogContext.Dispose();
        }
    }
}
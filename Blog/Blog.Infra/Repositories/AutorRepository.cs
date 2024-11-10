using Blog.Domain.Entities;
using Blog.Domain.Interfaces;
using Blog.Infra.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Blog.Infra.Repositories
{
    public class AutorRepository : IAutorRepository, IDisposable
    {
        private readonly BlogDbContext _blogContext;

        public AutorRepository(BlogDbContext blogContext, UserManager<IdentityUser> userManager)
        {
            _blogContext = blogContext;
        }

        public async Task<IEnumerable<Autor>> GetAutores()
        {
            return await _blogContext.Autores.ToListAsync();
        }

        public async Task<Autor> GetAutorById(int id)
        {
            return await _blogContext.Autores.FindAsync(id);
        }

        public async Task<Autor> GetAutorByUserId(string userId)
        {
            return await _blogContext.Autores.FirstOrDefaultAsync(a => a.IdUsuario.ToLower().Equals(userId.ToLower()));
        }

        public async void AddAutor(Autor autor)
        {
            _blogContext.Autores.Add(autor);
            await _blogContext.SaveChangesAsync();
        }

        public async void UpdateAutor(Autor autor)
        {
            _blogContext.Entry(autor).State = EntityState.Modified;
            await _blogContext.SaveChangesAsync();
        }

        public async void RemoveAutor(int id)
        {
            Autor autor = await _blogContext.Autores.FindAsync(id);
            _blogContext.Autores.Remove(autor);
            await _blogContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _blogContext.Dispose();
        }
    }
}

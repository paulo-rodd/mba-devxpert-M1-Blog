using Blog.Domain.Entities;
using Blog.Domain.Interfaces;
using Blog.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Blog.Infra.Repositories
{
    public class ComentarioRepository : IComentarioRepository, IDisposable
    {
        private readonly BlogDbContext _blogContext;

        public ComentarioRepository(BlogDbContext context)
        {
            _blogContext = context;
        }

        public async Task<IEnumerable<Comentario>> GetComentarios()
        {
            return await _blogContext.Comentarios.ToListAsync();
        }

        public async Task<IEnumerable<Comentario>> GetComentariosByPostagemId(int id)
        {
            return await _blogContext.Comentarios.Where(c => c.PostId == id).ToListAsync(); ;
        }

        public async Task<Comentario> GetComentarioById(int id)
        {
            return await _blogContext.Comentarios.FindAsync(id);
        }

        public async void AddComentario(Comentario comentario)
        {
            comentario.DataComentario = DateTime.Now;
            _blogContext.Comentarios.Add(comentario);
            await _blogContext.SaveChangesAsync();
        }

        public async void UpdateComentario(Comentario comentario)
        {
            _blogContext.Entry(comentario).State = EntityState.Modified;
            await _blogContext.SaveChangesAsync();

        }

        public async void RemoveComentario(int id)
        {
            var comentario = _blogContext.Comentarios.Find(id);
            _blogContext.Comentarios.Remove(comentario);
            await _blogContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _blogContext.Dispose();
        }
    }
}

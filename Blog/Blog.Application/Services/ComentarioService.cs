using Blog.Application.Interfaces;
using Blog.Domain.Interfaces;
using Blog.Domain.Entities;

namespace Blog.Application.Services
{
    public class ComentarioService : IComentarioService
    {
        public IComentarioRepository _comentarioRepository;

        public ComentarioService(IComentarioRepository comentarioRepository)
        {
            _comentarioRepository = comentarioRepository;
        }

        public async Task<IEnumerable<Comentario>> GetComentarios()
        {
            return await _comentarioRepository.GetComentarios();
        }

        public async Task<IEnumerable<Comentario>> GetComentariosByPostagemId(int id)
        {
            return await _comentarioRepository.GetComentariosByPostagemId(id);
        }

        public async Task<Comentario> GetComentarioById(int id)
        {
            return await _comentarioRepository.GetComentarioById(id);
        }

        public void AddComentario(Comentario comentario)
        {
            _comentarioRepository.AddComentario(comentario);
        }

        public void UpdateComentario(Comentario comentario)
        {
            _comentarioRepository.UpdateComentario(comentario);
        }

        public void RemoveComentario(int id)
        {
            _comentarioRepository.RemoveComentario(id);
        }
    }
}

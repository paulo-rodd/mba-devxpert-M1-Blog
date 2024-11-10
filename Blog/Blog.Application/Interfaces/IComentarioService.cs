using Blog.Domain.Entities;

namespace Blog.Application.Interfaces
{
    public interface IComentarioService
    {
        Task<IEnumerable<Comentario>> GetComentarios();
        Task<IEnumerable<Comentario>> GetComentariosByPostagemId(int id);
        Task<Comentario> GetComentarioById(int id);
        void AddComentario(Comentario comentario);
        void UpdateComentario(Comentario comentario);
        void RemoveComentario(int id);
    }
}

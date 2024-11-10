using Blog.Domain.Entities;

namespace Blog.Application.Interfaces
{
    public interface IPostagemService
    {
        Task<IEnumerable<Postagem>> GetPostagens();
        Task<Postagem> GetPostagemById(int id);

        Task<IEnumerable<Postagem>> GetPostagensByAutorId(int id);
        void AddPostagem(Postagem postagem);
        void UpdatePostagem(Postagem postagem);
        void RemovePostagem(int Id);
    }
}

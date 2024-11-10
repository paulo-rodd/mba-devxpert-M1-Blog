
using Blog.Domain.Entities;

namespace Blog.Domain.Interfaces
{
    public interface IAutorRepository : IDisposable
    {
        Task<IEnumerable<Autor>> GetAutores();
        Task<Autor> GetAutorById(int id);
        Task<Autor> GetAutorByUserId(string userId);
        void AddAutor(Autor autor);
        void UpdateAutor(Autor autor);
        void RemoveAutor(int id);

    }
}

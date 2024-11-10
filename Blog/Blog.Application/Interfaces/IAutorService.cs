using Blog.Domain.Entities;

namespace Blog.Application.Interfaces
{
    public interface IAutorService
    {
        Task<IEnumerable<Autor>> GetAutores();
        Task<Autor> GetAutorById(int id);
        Task<Autor> GetAutorByUserId(string userId);
        void AddAutor(Autor autor);
        void UpdateAutor(Autor autor);
        void RemoveAutor(int Id);
    }
}

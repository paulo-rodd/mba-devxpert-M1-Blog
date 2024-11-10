using Blog.Application.Interfaces;
using Blog.Domain.Entities;
using Blog.Domain.Interfaces;
namespace Blog.Application.Services
{
    public class AutorService : IAutorService
    {
        private readonly IAutorRepository _autorRepository;
        private readonly IPostagemRepository _postagemRepository;

        public AutorService(IAutorRepository autorRepository, IPostagemRepository postagemRepository)
        {
            _autorRepository = autorRepository;
            _postagemRepository = postagemRepository;
        }

        public async Task<IEnumerable<Autor>> GetAutores()
        {
            return await _autorRepository.GetAutores();
        }

        public async Task<Autor> GetAutorById(int id)
        {
            Autor _autor = await _autorRepository.GetAutorById(id);
            _autor.Postagens = await _postagemRepository.GetPostagensByAutorId(id);
            return _autor;
            //return await _autorRepository.GetAutorById(id);
        }

        public async Task<Autor> GetAutorByUserId(string userId)
        {
            return await _autorRepository.GetAutorByUserId(userId);
        }

        public void AddAutor(Autor autor)
        {
            _autorRepository.AddAutor(autor);
        }

        public void UpdateAutor(Autor autor)
        {
            _autorRepository.UpdateAutor(autor);
        }

        public void RemoveAutor(int Id)
        {
            _autorRepository.RemoveAutor(Id);
        }

    }
}

using Blog.Application.Interfaces;
using Blog.Domain.Entities;
using Blog.Domain.Interfaces;


namespace Blog.Application.Services
{
    public class PostagemService : IPostagemService
    {
        private readonly IPostagemRepository _postagemRepository;
        private readonly IComentarioRepository _comentarioRepository;

        public PostagemService(IPostagemRepository postagemRepository, IComentarioRepository comentarioRepository)
        {
            _postagemRepository = postagemRepository;
            _comentarioRepository = comentarioRepository;
        }

        public async Task<IEnumerable<Postagem>> GetPostagens()
        {
            return await _postagemRepository.GetPostagens();
        }

        public async Task<Postagem> GetPostagemById(int id)
        {
            Postagem postagem = await _postagemRepository.GetPostagemById(id);
            if(postagem != null)
            {
                postagem.Comentarios = await _comentarioRepository.GetComentariosByPostagemId(id);
            }
            
            return postagem;
        }

        public async Task<IEnumerable<Postagem>> GetPostagensByAutorId(int id)
        {
            return await _postagemRepository.GetPostagensByAutorId(id);
        }

        public void AddPostagem(Postagem postagem)
        {
            _postagemRepository.AddPostagem(postagem);
        }

        public void UpdatePostagem(Postagem postagem)
        {
            _postagemRepository.UpdatePostagem(postagem);
        }

        public void RemovePostagem(int Id)
        {
            _postagemRepository.RemovePostagem(Id);
        }

    }
}

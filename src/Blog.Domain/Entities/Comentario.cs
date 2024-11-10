using Blog.Domain.Entities.Base;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Domain.Entities
{
    [Table("Comentarios")]
    public class Comentario : BaseModel
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(200, MinimumLength = 6, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.")]
        [DisplayName("Comentário")]
        public string Conteudo { get; set; }


        [DisplayName("Dt. Comentário")]
        [DataType(DataType.Date, ErrorMessage = "O campo {0} está em um formato inválido.")]
        public DateTime DataComentario { get; set; }

        [HiddenInput]
        public int IdAutor { get; set; }

        [HiddenInput]
        [DisplayName("AutorComentario")]
        public string NomeAutor { get; set; }

        [HiddenInput]
        public int PostId { get; set; }

        [HiddenInput]
        [DisplayName("Postagem")]
        public string TituloPostagem { get; set; }
    }
}

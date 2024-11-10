using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Blog.Domain.Entities.Base;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Domain.Entities
{
    [Table("Postagens")]
    public class Postagem : BaseModel
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.")]
        [DisplayName("Título")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(1000, MinimumLength = 30, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.")]
        [DisplayName("Postagem")]
        public string Conteudo { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DataType(DataType.Date, ErrorMessage = "O campo {0} está em um formato inválido.")]
        [DisplayName("Data da Publicação")]
        public DateTime DataPublicacao { get; set; }

        [HiddenInput]
        public int IdAutor { get; set; }

        [HiddenInput]
        [DisplayName("Autor")]
        public string NomeAutor { get; set; }

        [NotMapped]
        [DisplayName("Comentários do Post")]
        public IEnumerable<Comentario>? Comentarios { get; set; }
    }
}

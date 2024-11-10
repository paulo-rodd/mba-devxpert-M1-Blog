using Blog.Domain.Entities.Base;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Domain.Entities
{
    [Table("Autores")]
    public class Autor : BaseModel
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.")]
        [DisplayName("Nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [EmailAddress(ErrorMessage = "O campo {0} está em um formato inválido.")]
        [DisplayName("E-mail")]
        public string Email { get; set; }

        [DataType(DataType.Date, ErrorMessage = "O campo {0} está em um formato inválido.")]
        [DisplayName("Dt. Cadastro")]
        public DateTime DataCadastro { get; set; }

        [DisplayName("Usuário")]
        public string IdUsuario { get; set; }

        [NotMapped]
        [DisplayName("Postagens do Autor")]
        public IEnumerable<Postagem>? Postagens { get; set; }
    }
}

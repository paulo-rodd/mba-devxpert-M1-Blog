using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Blog.Domain.Entities.Base
{
    public class BaseModel
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Ativo")]
        public bool Ativo { get; set; }
    }
}

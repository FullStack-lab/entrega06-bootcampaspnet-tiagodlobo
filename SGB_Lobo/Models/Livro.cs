using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SGB_Lobo.Models
{
    public class Livro
    {
        public Livro()
        {
            // Inicializa a coleção de empréstimos
            Emprestimos = new HashSet<Emprestimo>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(200, ErrorMessage = "O título não pode ter mais que 200 caracteres")]
        [Display(Name = "Título")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "O autor é obrigatório")]
        [Display(Name = "Autor")]
        public int AutorId { get; set; }

        [Required(ErrorMessage = "A categoria é obrigatória")]
        [Display(Name = "Categoria")]
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "O ano de publicação é obrigatório")]
        [Range(1000, 9999, ErrorMessage = "Ano inválido")]
        [Display(Name = "Ano de Publicação")]
        public int AnoPublicacao { get; set; }

        [Display(Name = "Disponível")]
        public bool Disponivel { get; set; }

        // Propriedades de navegação
        public virtual Autor Autor { get; set; }
        public virtual Categoria Categoria { get; set; }
        public virtual ICollection<Emprestimo> Emprestimos { get; set; }
    }
}
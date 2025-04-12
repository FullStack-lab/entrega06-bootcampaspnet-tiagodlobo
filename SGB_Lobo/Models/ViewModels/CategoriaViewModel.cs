using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SGB_Lobo.Models.ViewModels
{
    public class CategoriaViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(50, ErrorMessage = "O nome não pode ter mais que 50 caracteres")]
        [Display(Name = "Nome")]
        [Remote("VerificarNome", "Categorias", AdditionalFields = "Id",
                ErrorMessage = "Esta categoria já existe")]
        public string Nome { get; set; }

        // Lista de livros nesta categoria (para detalhes)
        public ICollection<LivroViewModel> Livros { get; set; }

        // Contador de livros
        public int QuantidadeLivros { get; set; }
    }
}
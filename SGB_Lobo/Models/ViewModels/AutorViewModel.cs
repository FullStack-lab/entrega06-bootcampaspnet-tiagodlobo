using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SGB_Lobo.Models.ViewModels
{
    public class AutorViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome não pode ter mais que 100 caracteres")]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data de Nascimento")]
        public DateTime DataNascimento { get; set; }

        [Required(ErrorMessage = "A nacionalidade é obrigatória")]
        [StringLength(50, ErrorMessage = "A nacionalidade não pode ter mais que 50 caracteres")]
        [Display(Name = "Nacionalidade")]
        public string Nacionalidade { get; set; }

        // Lista de livros deste autor (para detalhes)
        public ICollection<LivroViewModel> Livros { get; set; }

        // Contador de livros
        public int QuantidadeLivros { get; set; }
    }
}
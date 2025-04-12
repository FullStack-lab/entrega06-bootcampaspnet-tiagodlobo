using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SGB_Lobo.Models
{
    // Classe para gerenciar os autores dos livros
    public class Autor
    {
        public Autor()
        {
            Livros = new HashSet<Livro>();  // Inicializa a lista de livros
        }

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

        // Lista de livros do autor
        public virtual ICollection<Livro> Livros { get; set; }
    }
}
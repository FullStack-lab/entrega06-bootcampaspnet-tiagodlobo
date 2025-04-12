using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SGB_Lobo.Models
{
    // Classe para gerenciar as categorias dos livros
    public class Categoria
    {
        public Categoria()
        {
            Livros = new HashSet<Livro>();  // Inicializa a lista de livros
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(50, ErrorMessage = "O nome não pode ter mais que 50 caracteres")]
        [Display(Name = "Nome")]
        [Remote("VerificarNome", "Categorias", AdditionalFields = "Id",
                ErrorMessage = "Esta categoria já existe")]
        public string Nome { get; set; }

        // Lista de livros da categoria
        public virtual ICollection<Livro> Livros { get; set; }
    }
}
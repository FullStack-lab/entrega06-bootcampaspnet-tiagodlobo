using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SGB_Lobo.Models.ViewModels
{
    public class UsuarioViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome não pode ter mais que 100 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "O email não pode ter mais que 100 caracteres")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Telefone inválido")]
        [StringLength(20, ErrorMessage = "O telefone não pode ter mais que 20 caracteres")]
        public string Telefone { get; set; }

        public bool Ativo { get; set; }

        // Lista de empréstimos deste usuário (para detalhes)
        public ICollection<EmprestimoViewModel> Emprestimos { get; set; }

        // Contador de empréstimos
        public int QuantidadeEmprestimos { get; set; }
    }
}
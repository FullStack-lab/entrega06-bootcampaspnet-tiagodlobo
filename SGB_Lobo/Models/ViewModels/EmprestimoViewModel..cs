using System;
using System.ComponentModel.DataAnnotations;

namespace SGB_Lobo.Models.ViewModels
{
    public class EmprestimoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O livro é obrigatório")]
        [Display(Name = "Livro")]
        public int LivroId { get; set; }

        [Required(ErrorMessage = "O usuário é obrigatório")]
        [Display(Name = "Usuário")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "A data de empréstimo é obrigatória")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data do Empréstimo")]
        public DateTime DataEmprestimo { get; set; }

        [Required(ErrorMessage = "A data prevista de devolução é obrigatória")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data Prevista para Devolução")]
        public DateTime DataPrevistaDevolucao { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data da Devolução")]
        public DateTime? DataDevolucao { get; set; }

        // Propriedades adicionais para exibição
        public string LivroTitulo { get; set; }
        public string UsuarioNome { get; set; }
        public bool EmAtraso { get; set; }
        public bool Devolvido { get; set; }
        public string Status { get; set; }
        public int DiasAtraso { get; set; }
    }
}
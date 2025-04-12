namespace SGB_Lobo.Models.ViewModels
{
    public class LivroMaisEmprestadoViewModel
    {
        public string Titulo { get; set; }
        public int QuantidadeEmprestimos { get; set; }
    }

    public class UsuarioMaisAtivoViewModel
    {
        public string Nome { get; set; }
        public int QuantidadeEmprestimos { get; set; }
    }

    public class LivroPorCategoriaViewModel
    {
        public string Nome { get; set; }
        public int QuantidadeLivros { get; set; }
    }
}
using AutoMapper;
using SGB_Lobo.Models;
using SGB_Lobo.Models.ViewModels;
using System;

namespace SGB_Lobo.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Mapeamento Livro <-> LivroViewModel
            CreateMap<Livro, LivroViewModel>()
                .ForMember(dest => dest.AutorNome, opt => opt.MapFrom(src => src.Autor != null ? src.Autor.Nome : string.Empty))
                .ForMember(dest => dest.CategoriaNome, opt => opt.MapFrom(src => src.Categoria != null ? src.Categoria.Nome : string.Empty))
                .ForMember(dest => dest.Autor, opt => opt.MapFrom(src => src.Autor))
                .ForMember(dest => dest.Categoria, opt => opt.MapFrom(src => src.Categoria));

            CreateMap<LivroViewModel, Livro>();

            // Mapeamento Autor <-> AutorViewModel
            CreateMap<Autor, AutorViewModel>()
                .ForMember(dest => dest.QuantidadeLivros, opt => opt.MapFrom(src => src.Livros != null ? src.Livros.Count : 0));

            CreateMap<AutorViewModel, Autor>();

            // Mapeamento Categoria <-> CategoriaViewModel
            CreateMap<Categoria, CategoriaViewModel>()
                .ForMember(dest => dest.QuantidadeLivros, opt => opt.MapFrom(src => src.Livros != null ? src.Livros.Count : 0));

            CreateMap<CategoriaViewModel, Categoria>();

            // Mapeamento Emprestimo <-> EmprestimoViewModel
            CreateMap<Emprestimo, EmprestimoViewModel>()
                .ForMember(dest => dest.LivroTitulo, opt => opt.MapFrom(src => src.Livro != null ? src.Livro.Titulo : string.Empty))
                .ForMember(dest => dest.UsuarioNome, opt => opt.MapFrom(src => src.Usuario != null ? src.Usuario.Nome : string.Empty))
                .ForMember(dest => dest.EmAtraso, opt => opt.MapFrom(src =>
                    !src.DataDevolucao.HasValue && DateTime.Now > src.DataPrevistaDevolucao))
                .ForMember(dest => dest.Devolvido, opt => opt.MapFrom(src => src.DataDevolucao.HasValue))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => GetStatusEmprestimo(src)))
                .ForMember(dest => dest.DiasAtraso, opt => opt.MapFrom(src =>
                    !src.DataDevolucao.HasValue && DateTime.Now > src.DataPrevistaDevolucao
                        ? (int)(DateTime.Now - src.DataPrevistaDevolucao).TotalDays
                        : 0));

            CreateMap<EmprestimoViewModel, Emprestimo>();

            // Mapeamento Usuario <-> UsuarioViewModel
            CreateMap<Usuario, UsuarioViewModel>()
                .ForMember(dest => dest.QuantidadeEmprestimos, opt => opt.MapFrom(src => src.Emprestimos != null ? src.Emprestimos.Count : 0));

            CreateMap<UsuarioViewModel, Usuario>();
        }

        private static string GetStatusEmprestimo(Emprestimo emprestimo)
        {
            if (emprestimo.DataDevolucao.HasValue)
            {
                return emprestimo.DataDevolucao <= emprestimo.DataPrevistaDevolucao
                    ? "Devolvido no Prazo"
                    : "Devolvido com Atraso";
            }
            else
            {
                return DateTime.Now > emprestimo.DataPrevistaDevolucao
                    ? "Em Atraso"
                    : "Em Andamento";
            }
        }
    }
}
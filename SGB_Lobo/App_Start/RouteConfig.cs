using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
public class RouteConfig
{
    public static void RegisterRoutes(RouteCollection routes)
    {
        routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

        // Rotas personalizadas para Livros
        routes.MapRoute(
            name: "LivrosFiltrados",
            url: "livros/{disponibilidade}",
            defaults: new { controller = "Livros", action = "Index" },
            constraints: new { disponibilidade = "disponiveis|emprestados" }
        );

        routes.MapRoute(
            name: "LivrosCategoria",
            url: "livros/categoria/{categoria}",
            defaults: new { controller = "Livros", action = "Index" }
        );

        routes.MapRoute(
            name: "LivrosBusca",
            url: "livros/busca/{searchString}",
            defaults: new { controller = "Livros", action = "Index" }
        );

        // Rota padrão
        routes.MapRoute(
            name: "Default",
            url: "{controller}/{action}/{id}",
            defaults: new { controller = "Dashboard", action = "Index", id = UrlParameter.Optional }
        );
    }
}
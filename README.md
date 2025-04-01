# Entrega 06 – Refatoração do Sistema de Gerenciamento de Biblioteca (SGB) com AutoMapper

## Objetivo da Entrega

Esta entrega tem como objetivo refatorar o projeto desenvolvido até a Entrega 05, aprimorando sua estrutura e organização por meio da aplicação do AutoMapper, uma ferramenta essencial para conversão entre entidades e ViewModels no ASP.NET MVC.

Além disso, serão avaliados conceitos relacionados à utilização correta do SQL Server e do Entity Framework, garantindo que a estrutura do banco de dados esteja implementada de forma adequada, normalizada e funcional.

A proposta desta entrega é aproximar o projeto a um padrão profissional de desenvolvimento, eliminando códigos redundantes, facilitando a manutenção e promovendo boas práticas de arquitetura.

---

## Requisitos da Entrega

### 1. Refatoração com AutoMapper

Você deverá aplicar o AutoMapper para automatizar a conversão entre suas Entidades e ViewModels. Os seguintes requisitos devem ser atendidos:

- Criação de ViewModels para Livro, Autor, Categoria e Empréstimo.
- Implementação de mapeamento explícito no arquivo de configuração do AutoMapper.
- Refatoração de todos os Controllers que realizam CRUD, removendo conversões manuais e utilizando o AutoMapper para projetar dados entre:
  - Entidade → ViewModel
  - ViewModel → Entidade
- Refatoração das Views, utilizando exclusivamente dados do ViewModel.

### 2. Avaliação dos Conceitos de SQL Server

Além da refatoração com AutoMapper, serão cobrados aspectos relacionados à correta utilização do SQL Server no projeto, como:

- Existência do script .sql atualizado na pasta `/Database` contendo:
  - Criação de tabelas normalizadas (Livro, Autor, Categoria, Empréstimo, Usuário).
  - Relacionamentos configurados com chaves primárias e estrangeiras.
  - Integridade referencial implementada.
- Configuração correta do arquivo de conexão no Web.config.
- Aplicação correta de migrações no Entity Framework.
- Base de dados populada com dados de teste.
- Relatórios SQL complementares (opcional para bônus):
  - Consulta de livros mais emprestados.
  - Consulta de usuários que mais realizaram empréstimos.

---

## Diferenciais (Bônus)

Os candidatos que implementarem os itens abaixo poderão receber pontos extras:

- Separação clara dos projetos em camadas (DataLayer, BusinessLayer, UI).
- Testes unitários utilizando as ViewModels e mapeamentos.
- Criação de Profiles do AutoMapper para organização dos mapeamentos.
- Consulta SQL adicional com indicadores ou métricas sobre os empréstimos.
- Comentários claros explicando o mapeamento feito com AutoMapper.

---

## Prazos

**Data de Entrega:** Até dia 12/04/2025 às 23:59

---

## Checklist de Desenvolvimento

- [ ] ViewModels criadas e utilizadas nas Views.
- [ ] Controllers refatorados para usar AutoMapper.
- [ ] Arquivo de configuração do AutoMapper criado e funcionando.
- [ ] Script .sql atualizado na pasta `/Database`.
- [ ] Estrutura do banco de dados normalizada.
- [ ] Migrações aplicadas e funcionando.


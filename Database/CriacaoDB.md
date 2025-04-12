Script para criação do SGB_db

CREATE TABLE Autor (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(100) NOT NULL,
    DataNascimento DATE NOT NULL,
    Nacionalidade NVARCHAR(50) NOT NULL
)
GO

CREATE TABLE Categoria (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(50) NOT NULL UNIQUE
)
GO

CREATE TABLE Livro (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Titulo NVARCHAR(200) NOT NULL,
    AutorId INT NOT NULL,
    CategoriaId INT NOT NULL,
    AnoPublicacao INT NOT NULL,
    Disponivel BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Livro_Autor FOREIGN KEY (AutorId) REFERENCES Autor(Id),
    CONSTRAINT FK_Livro_Categoria FOREIGN KEY (CategoriaId) REFERENCES Categoria(Id)
)
GO

CREATE TABLE Emprestimo (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    LivroId INT NOT NULL,
    UsuarioId NVARCHAR(128) NOT NULL,
    DataEmprestimo DATETIME NOT NULL,
    DataPrevistaDevolucao DATETIME NOT NULL,
    DataDevolucao DATETIME NULL,
    CONSTRAINT FK_Emprestimo_Livro FOREIGN KEY (LivroId) REFERENCES Livro(Id)
)
GO


-- Inserindo dados de exemplo

INSERT INTO Autor (Nome, DataNascimento, Nacionalidade) VALUES 
('Machado de Assis', '1839-06-21', 'Brasileiro'),
('Jorge Amado', '1912-08-10', 'Brasileiro'),
('Clarice Lispector', '1920-12-10', 'Brasileira')
GO

INSERT INTO Categoria (Nome) VALUES 
('Romance'),
('Ficção'),
('Literatura Brasileira'),
('Poesia')
GO

INSERT INTO Livro (Titulo, AutorId, CategoriaId, AnoPublicacao, Disponivel) VALUES 
('Dom Casmurro', 1, 1, 1899, 1),
('Capitães da Areia', 2, 1, 1937, 1),
('A Hora da Estrela', 3, 2, 1977, 1)
GO


-- Criar tabela Usuario
CREATE TABLE Usuario (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Telefone NVARCHAR(20) NULL,
    Ativo BIT NOT NULL DEFAULT 1
);
GO

-- Adicionar alguns usuários de exemplo
INSERT INTO Usuario (Nome, Email, Telefone) VALUES 
('João Silva', 'joao.silva@email.com', '(11) 99999-1111'),
('Maria Santos', 'maria.santos@email.com', '(11) 99999-2222'),
('Pedro Oliveira', 'pedro.oliveira@email.com', '(11) 99999-3333');
GO

-- Adicionar nova chave estrangeira em Emprestimo
ALTER TABLE Emprestimo
DROP COLUMN UsuarioId;

ALTER TABLE Emprestimo
ADD UsuarioId INT NOT NULL;

ALTER TABLE Emprestimo
ADD CONSTRAINT FK_Emprestimo_Usuario 
FOREIGN KEY (UsuarioId) REFERENCES Usuario(Id);
GO

-- Criar índice para melhorar performance
CREATE INDEX IX_Emprestimo_UsuarioId ON Emprestimo(UsuarioId);
GO

/****** Object:  Trigger [dbo].[trg_Emprestimo_AtualizarDisponibilidade]    Script Date: 02/02/2025 18:16:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Criar trigger para atualizar disponibilidade do livro após empréstimo/devolução
ALTER TRIGGER [dbo].[trg_Emprestimo_AtualizarDisponibilidade]
ON [dbo].[Emprestimo]
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Atualiza o livro para indisponível quando emprestado
    UPDATE l
    SET l.Disponivel = 0
    FROM Livro l
    INNER JOIN inserted i ON l.Id = i.LivroId
    WHERE i.DataDevolucao IS NULL;
    
    -- Atualiza o livro para disponível quando devolvido
    UPDATE l
    SET l.Disponivel = 1
    FROM Livro l
    INNER JOIN inserted i ON l.Id = i.LivroId
    WHERE i.DataDevolucao IS NOT NULL;
END
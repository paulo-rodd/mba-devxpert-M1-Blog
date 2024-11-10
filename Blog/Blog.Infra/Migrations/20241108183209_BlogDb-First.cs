using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog.Infra.Migrations
{
    /// <inheritdoc />
    public partial class BlogDbFirst : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Autores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IdUsuario = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Autores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Comentarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Conteudo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    DataComentario = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IdAutor = table.Column<int>(type: "INTEGER", nullable: false),
                    NomeAutor = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    PostId = table.Column<int>(type: "INTEGER", nullable: false),
                    TituloPostagem = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comentarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Postagens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Titulo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Conteudo = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    DataPublicacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IdAutor = table.Column<int>(type: "INTEGER", nullable: false),
                    NomeAutor = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Postagens", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Autores");

            migrationBuilder.DropTable(
                name: "Comentarios");

            migrationBuilder.DropTable(
                name: "Postagens");
        }
    }
}

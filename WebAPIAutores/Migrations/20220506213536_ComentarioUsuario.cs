using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIAutores.Migrations
{
    public partial class ComentarioUsuario : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsarioId",
                table: "Comentarios",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioId",
                table: "Comentarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_UsarioId",
                table: "Comentarios",
                column: "UsarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_AspNetUsers_UsarioId",
                table: "Comentarios",
                column: "UsarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_AspNetUsers_UsarioId",
                table: "Comentarios");

            migrationBuilder.DropIndex(
                name: "IX_Comentarios_UsarioId",
                table: "Comentarios");

            migrationBuilder.DropColumn(
                name: "UsarioId",
                table: "Comentarios");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Comentarios");
        }
    }
}

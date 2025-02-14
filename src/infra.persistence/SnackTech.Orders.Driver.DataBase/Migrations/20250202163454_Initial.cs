﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnackTech.Orders.Driver.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pessoa",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pessoa", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Cpf = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cliente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cliente_Pessoa_Id",
                        column: x => x.Id,
                        principalTable: "Pessoa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pedido",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PagamentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedido", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pedido_Cliente_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Cliente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PedidoItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    Observacao = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    ValorTotal = table.Column<decimal>(type: "smallmoney", nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "smallmoney", nullable: false),
                    ProdutoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PedidoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PedidoItem_Pedido_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "Pedido",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Pessoa",
                columns: new[] { "Id", "Nome" },
                values: new object[] { new Guid("6ee54a46-007f-4e4c-9fe8-1a13eadf7fd1"), "Cliente Padrão" });

            migrationBuilder.InsertData(
                table: "Cliente",
                columns: new[] { "Id", "Cpf", "Email" },
                values: new object[] { new Guid("6ee54a46-007f-4e4c-9fe8-1a13eadf7fd1"), "00000000191", "cliente.padrao@padrao.com" });

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_Cpf",
                table: "Cliente",
                column: "Cpf",
                unique: true,
                filter: "[Cpf] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_Email",
                table: "Cliente",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_ClienteId",
                table: "Pedido",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItem_PedidoId",
                table: "PedidoItem",
                column: "PedidoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PedidoItem");

            migrationBuilder.DropTable(
                name: "Pedido");

            migrationBuilder.DropTable(
                name: "Cliente");

            migrationBuilder.DropTable(
                name: "Pessoa");
        }
    }
}

﻿using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SnackTech.Orders.Driver.DataBase.Entities;

namespace SnackTech.Orders.Driver.DataBase.Configurations
{
    [ExcludeFromCodeCoverage]
    internal sealed class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable(nameof(Cliente));

            builder.Property(c => c.Email)
                .HasColumnType("varchar")
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(c => c.Cpf)
                .HasColumnType("varchar")
                .IsRequired()
                .HasMaxLength(15);

            builder
                .HasIndex(u => u.Email)
                .IsUnique();
            builder
                .HasIndex(u => u.Cpf)
                .IsUnique();

            builder.HasData(
                new
                {
                    Id = Guid.Parse("6ee54a46-007f-4e4c-9fe8-1a13eadf7fd1"),
                    Email = "cliente.padrao@padrao.com",
                    Cpf = "00000000191",
                    Nome = "Cliente Padrão"
                });
        }
    }
}

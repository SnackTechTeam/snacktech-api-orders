using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SnackTech.Orders.Driver.DataBase.Entities;

namespace SnackTech.Orders.Driver.DataBase.Configurations
{
    [ExcludeFromCodeCoverage]
    internal sealed class PedidoItemConfiguration : IEntityTypeConfiguration<PedidoItem>
    {
        public void Configure(EntityTypeBuilder<PedidoItem> builder)
        {
            builder.ToTable(nameof(PedidoItem));

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Quantidade)
                .HasColumnType("int")
                .IsRequired();

            builder.Property(p => p.Observacao)
                .HasColumnType("varchar")
                .HasMaxLength(500);

            builder.Property(p => p.ValorTotal)
                .IsRequired()
                .HasColumnType("smallmoney");

            builder.Property(p => p.ValorUnitario)
                .IsRequired()
                .HasColumnType("smallmoney");

            builder.Property(p => p.ProdutoId)
                .IsRequired();
        }
    }
}

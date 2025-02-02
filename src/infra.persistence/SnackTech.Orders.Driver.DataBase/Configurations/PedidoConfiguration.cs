using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SnackTech.Orders.Driver.DataBase.Entities;

namespace SnackTech.Orders.Driver.DataBase.Configurations
{
    [ExcludeFromCodeCoverage]
    internal sealed class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
    {
        public void Configure(EntityTypeBuilder<Pedido> builder)
        {
            builder.ToTable(nameof(Pedido));

            builder.HasKey(p => p.Id);

            builder.Property(p => p.DataCriacao)
                .IsRequired()
                .HasColumnType("datetime");

            builder.Property(p => p.Status)
                .HasColumnType("int")
                .IsRequired();

            builder.Property(p => p.PagamentoId);

            builder.Navigation(nameof(Pedido.Itens));
        }
    }
}

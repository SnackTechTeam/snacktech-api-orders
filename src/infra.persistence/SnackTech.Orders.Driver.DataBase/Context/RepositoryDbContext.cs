using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using SnackTech.Orders.Driver.DataBase.Entities;

namespace SnackTech.Orders.Driver.DataBase.Context
{
    [ExcludeFromCodeCoverage]
    public class RepositoryDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoItem> PedidoItens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) =>
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(RepositoryDbContext).Assembly);
    }
}

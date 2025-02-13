using Microsoft.EntityFrameworkCore;
using SnackTech.Orders.Common.CustomExceptions;
using SnackTech.Orders.Common.Dto.DataSource;
using SnackTech.Orders.Common.Interfaces.DataSources;
using SnackTech.Orders.Common.Enums;
using SnackTech.Orders.Driver.DataBase.Util;
using SnackTech.Orders.Driver.DataBase.Entities;
using SnackTech.Orders.Driver.DataBase.Context;

namespace SnackTech.Orders.Driver.DataBase.DataSources;

public class PedidoDataSource(RepositoryDbContext repositoryDbContext) : IPedidoDataSource
{
    public async Task<bool> AlterarItensDoPedidoAsync(PedidoDto pedidoDto)
    {
        var pedido = await repositoryDbContext.Pedidos
                .Include(p => p.Itens)
                .Where(p => p.Id == pedidoDto.Id)
                .FirstOrDefaultAsync();

        if (pedido is null)
        {
            throw new PedidoRepositoryException($"Pedido com identificacao {pedidoDto.Id} não encontrado no banco de dados.");
        }

        var itensNoBanco = pedido.Itens.ToDictionary(p => p.Id, p => p);

        foreach (var itemAtualizar in pedidoDto.Itens)
        {
            var itemEntityAtualizar = Mapping.Mapper.Map<PedidoItem>(itemAtualizar);
            if (itensNoBanco.TryGetValue(itemEntityAtualizar.Id, out var itemBanco))
            {
                itemBanco.Quantidade = itemAtualizar.Quantidade;
                itemBanco.ValorTotal = itemAtualizar.ValorTotal;
                itemBanco.ValorUnitario = itemEntityAtualizar.ValorUnitario;
                itemBanco.Observacao = itemAtualizar.Observacao;
                itemBanco.ProdutoId = itemEntityAtualizar.ProdutoId;
            }
            else
            {
                //adiocionando itens novos dessa forma evitasse que o EF tente criar um novo produto a partir do produto presente no item
                pedido.Itens.Add(itemEntityAtualizar);
                var entry = repositoryDbContext.Entry(itemEntityAtualizar);
                entry.State = EntityState.Added;
            }
        }

        //deletar itens que foram removidos
        var itensParaRemover = itensNoBanco.Where(i => !pedidoDto.Itens.Any(p => p.Id == i.Key));
        repositoryDbContext.PedidoItens.RemoveRange(itensParaRemover.Select(i => i.Value));

        await repositoryDbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> AtualizarStatusPedidoAsync(PedidoDto pedidoDto)
    {
        var pedidoBanco = await repositoryDbContext
            .Pedidos.FirstOrDefaultAsync(p => p.Id == pedidoDto.Id);

        if (pedidoBanco is null)
            return false;

        pedidoBanco.Status = (StatusPedido)pedidoDto.Status;
        await repositoryDbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> InserirPedidoAsync(PedidoDto pedidoDto)
    {
        var pedidoEntity = Mapping.Mapper.Map<Pedido>(pedidoDto);

        //Para que o EF core não tente criar novos clientes e produtos a partir dos dados presentes no pedido
        var cliente = repositoryDbContext.ChangeTracker
            .Entries<Cliente>().FirstOrDefault(c => c.Entity.Id == pedidoEntity.Cliente.Id);

        if (cliente is not null)
        {
            pedidoEntity.Cliente = cliente.Entity;
        }
        else
        {
            repositoryDbContext.Entry(pedidoEntity.Cliente).State = EntityState.Unchanged;
        }

        repositoryDbContext.Pedidos.Add(pedidoEntity);

        var resultado = await repositoryDbContext.SaveChangesAsync();

        return resultado > 0;
    }

    public async Task<IEnumerable<PedidoDto>> PesquisarPedidosPorClienteIdAsync(Guid clienteId)
    {
        var pedidosBanco = await repositoryDbContext.Pedidos
                    .AsNoTracking()
                    .Include(p => p.Cliente)
                    .Include(p => p.Itens)//.ThenInclude(i => i.Produto)
                    .Where(p => p.Cliente.Id == clienteId)
                    .ToListAsync();

        return pedidosBanco.Select(Mapping.Mapper.Map<PedidoDto>);
    }

    public async Task<IEnumerable<PedidoDto>> PesquisarPedidosPorStatusAsync(int[] valor)
    {
        var pedidosBanco = await repositoryDbContext.Pedidos
                   .AsNoTracking()
                   .Include(p => p.Cliente)
                   .Include(p => p.Itens)//.ThenInclude(i => i.Produto)
                   .Where(p => valor.Contains((int)p.Status))
                   .ToListAsync();

        return pedidosBanco.Select(Mapping.Mapper.Map<PedidoDto>);
    }

    public async Task<PedidoDto?> PesquisarPorIdentificacaoAsync(Guid identificacao)
    {
        var pedidoBanco = await repositoryDbContext.Pedidos
            .AsNoTracking()
            .Include(p => p.Cliente)
            .Include(p => p.Itens)//.ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(p => p.Id == identificacao);

        if (pedidoBanco == null)
        {
            return null;
        }

        return Mapping.Mapper.Map<PedidoDto>(pedidoBanco);
    }
}

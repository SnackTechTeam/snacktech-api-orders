﻿using Refit;
using SnackTech.Orders.Common.Dto.Api;
using SnackTech.Orders.Common.Dto.ApiSources.Payments;

namespace SnackTech.Orders.Driver.Payments.HttpApiClients
{
    public interface IPagamentoHttpClient
    {
        [Post("/pagamentos")]
        Task<ApiResponse<PagamentoDto>> CriarPagamento([Body] PedidoPagamentoDto pedido);
    }
}

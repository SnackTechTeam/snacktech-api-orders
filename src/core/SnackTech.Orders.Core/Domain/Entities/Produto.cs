using SnackTech.Orders.Core.Domain.Types;

namespace SnackTech.Orders.Core.Domain.Entities;

internal class Produto(GuidValido id, DecimalPositivo valor)
{
    internal GuidValido Id { get; private set; } = id;

    internal DecimalPositivo Valor { get; private set; } = valor;
}
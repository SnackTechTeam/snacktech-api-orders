using AutoMapper;
using SnackTech.Orders.Common.Dto.DataSource;
using SnackTech.Orders.Driver.DataBase.Entities;

namespace SnackTech.Orders.Driver.DataBase.Util;

public static class Mapping
{
    private static readonly Lazy<IMapper> Lazy = new Lazy<IMapper>(() =>
    {
        var config = new MapperConfiguration(cfg =>
        {
            // This line ensures that internal properties are also mapped over.
            cfg.ShouldMapProperty = p => p.GetMethod.IsPublic || p.GetMethod.IsAssembly;
            cfg.AddProfile<MappingProfile>();
        });
        var mapper = config.CreateMapper();
        return mapper;
    });

    public static IMapper Mapper => Lazy.Value;
}

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Cliente, ClienteDto>();
        CreateMap<ClienteDto, Cliente>();

        CreateMap<PedidoItem, PedidoItemDto>();

        CreateMap<PedidoItemDto, PedidoItem>();

        CreateMap<Pedido, PedidoDto>();
        CreateMap<PedidoDto, Pedido>();
    }
}

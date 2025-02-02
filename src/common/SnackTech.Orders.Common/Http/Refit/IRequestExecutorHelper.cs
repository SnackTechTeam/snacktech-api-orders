using Refit;
using SnackTech.Orders.Common.Dto.Api;

namespace SnackTech.Orders.Common.Http.Refit
{
    public interface IRequestExecutorHelper
    {
        Task<ResultadoOperacao<T>> Execute<T>(Func<Task<ApiResponse<T>>> func);
    }
}

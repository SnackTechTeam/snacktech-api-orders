using Refit;
using SnackTech.Orders.Common.Dto.Api;

namespace SnackTech.Orders.Common.Http.Refit
{
    public class BaseRequestExecutorHelper : IRequestExecutorHelper
    {
        public async Task<ResultadoOperacao<T>> Execute<T>(Func<Task<ApiResponse<T>>> func)
        {
            ApiResponse<T>? response;

            try
            {
                response = await func.Invoke().ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    T? data = response.Content;

                    return new ResultadoOperacao<T>(data);                    
                }
                else
                {
                    var message = "";
                    if (response.Error.Content is not null)
                    {
                        message = response.Error.Content;
                    }

                    return new ResultadoOperacao<T>(message, true);
                }
            }
            catch (Exception ex)
            {
                return new ResultadoOperacao<T>(ex);
            }
        }
    }
}

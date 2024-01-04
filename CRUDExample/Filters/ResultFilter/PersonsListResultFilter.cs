using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ResultFilter
{
    public class PersonsListResultFilter:IAsyncResultFilter
    {
        private readonly ILogger<PersonsListResultFilter> _logger;

        public PersonsListResultFilter(ILogger<PersonsListResultFilter> logger)
        {
            _logger = logger;
          
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            //To do: add before logic here
            _logger.LogInformation("{FilterName}.{MethodName} method before", nameof(PersonsListResultFilter),nameof(OnResultExecutionAsync));
            await next();
            //To do: add after logic here
            _logger.LogInformation("{FilterName}.{MethodName} method after", nameof(PersonsListResultFilter), nameof(OnResultExecutionAsync));
            context.HttpContext.Response.Headers["Last-Modified"]=DateTime.Now.ToString("yyy-MM-dd HH:mm");
        }
    }
   
}

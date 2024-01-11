using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ResourceFilters
{
    public class FeatureDisabledResourceFilter : IAsyncResourceFilter
    {
        private readonly ILogger<FeatureDisabledResourceFilter> _logger;
        private readonly bool _isFeatureEnabled = false;

        public FeatureDisabledResourceFilter(ILogger<FeatureDisabledResourceFilter> logger,bool isFeatureEnabled=false)
        {
            _logger = logger;
            _isFeatureEnabled = isFeatureEnabled;
        }   
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            //To Do:before logic here
            _logger.LogInformation("{FilterName}.{MethodName} method before", nameof(FeatureDisabledResourceFilter), nameof(OnResourceExecutionAsync));
            if(_isFeatureEnabled == false)
            {
              //context.Result = new NotFoundResult();
              context.Result = new StatusCodeResult(501);
            }else
            {
                await next();
            }
         
            //To Do:after logic here
            _logger.LogInformation("{FilterName}.{MethodName} method after", nameof(FeatureDisabledResourceFilter), nameof(OnResourceExecutionAsync));  
        }
    }
}

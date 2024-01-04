using CRUDExample.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;

namespace CRUDExample.Filters.ActionFilters
{
    public class PersonsListActionFilter : IActionFilter
    {

        private readonly ILogger<PersonsListActionFilter> _logger;
       //public int Order { get; set; }

        public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger) {
            _logger = logger;
           // Order = order;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            //To do: add after logic here
            _logger.LogInformation("{FilterName}.{MethodName} method",nameof(PersonsListActionFilter),nameof(OnActionExecuted));
            PersonsController personsController = (PersonsController)context.Controller;
            IDictionary<string, object?>? parameters = (IDictionary<string, object?>?)context.HttpContext.Items["arguments"];
            if(parameters != null)
            {
                if (parameters.ContainsKey("searchBy"))
                {
                    personsController.ViewData["CurrentSearchBy"] =Convert.ToString(parameters["searchBy"]);
                }     
                if (parameters.ContainsKey("searchString"))
                {
                    personsController.ViewData["CurrentSearchString"] = Convert.ToString(parameters["searchString"]);
                }     
                
                if (parameters.ContainsKey("sortBy"))
                {
                    personsController.ViewData["CurrentSortBy"] = Convert.ToString(parameters["sortBy"]);
                }     
                if (parameters.ContainsKey("sortOrder"))
                {
                    personsController.ViewData["CurrentSortOrder"] = Convert.ToString(parameters["sortOrder"]);
                }
            }
           // personsController.ViewData["searchBy"] = Convert.ToString(parameters["searchBy"]);
            

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //To do: add before logic here
            context.HttpContext.Items["arguments"] = context.ActionArguments;
            //Validate search by value
            if (context.ActionArguments.ContainsKey("searchBy"))
            {
                string searchBy = Convert.ToString(context.ActionArguments["searchBy"]);
                if (!string.IsNullOrEmpty(searchBy))
                {
                    var searchByOptions = new List<string>()
                    {
                        nameof(PersonResponse.PersonName),
                        nameof(PersonResponse.Email),
                        nameof(PersonResponse.DateOfBirth),
                        nameof(PersonResponse.Gender),
                        nameof(PersonResponse.CountryID),
                        nameof(PersonResponse.Address),
                    };

                    //Reset search by value
                    if (searchByOptions.Any(temp => temp == searchBy) == false)
                    {
                        _logger.LogInformation("searchBy actual value: {searchBy}",searchBy);
                        context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);
                        _logger.LogInformation("searchBy updated value: {searchBy}", searchBy);
                    }
                 }
            }
        }
    }
}

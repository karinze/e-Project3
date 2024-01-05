using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AptitudeWebApp.Models.Authentication
{
    public class Authentication:ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.HttpContext.Session.GetString("Manager")==null)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    {"Controller", "Account" },
                    {"Action", "Login" }
                });
            }
            if (context.HttpContext.Session.GetString("CompletedExamType") != null)
            {
                var completedExamTypes = context.HttpContext.Session.GetString("CompletedExamType");

                if (completedExamTypes.Contains("1"))
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary
                    {
                        {"Controller", "Applicant" },
                        {"Action", "Login" }
                    });
                }
                else if (completedExamTypes.Contains("2"))
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary
                    {
                        {"Controller", "Applicant" },
                        {"Action", "Login" }
                    });
                } else if (completedExamTypes.Contains("3"))
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary
                    {
                        {"Controller", "Applicant" },
                        {"Action", "Login" }
                    });
                }
                else
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary
                    {
                        {"Controller", "Applicant" },
                        {"Action", "Login" }
                    });
                }
            }
        }


    }
}

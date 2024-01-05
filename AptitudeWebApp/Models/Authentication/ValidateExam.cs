using AptitudeWebApp.Controllers;
using AptitudeWebApp.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace AptitudeWebApp.Models.Authentication
{
    public class ValidateExam : ActionFilterAttribute
    {
        //private readonly ILogger<ValidateExam> _logger;
        //public ValidateExam (ILogger<ValidateExam> logger)
        //{
        //    _logger = logger;
        //}
        public int CurrentExamType { get; set; }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            //try
            //{
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError($"Something went wrong while validating the CompletedExamTypes. Error: {ex.Message}");
            //    throw;
            //}
            var routeData = context.HttpContext.GetRouteData();
            if (routeData.Values.ContainsKey("examTypeId"))
            {
                if (int.TryParse(routeData.Values["examTypeId"].ToString(), out int currentExamType))
                {
                    CurrentExamType = currentExamType;
                }
            }

            if (context.HttpContext.Session.Get<List<int>>("CompletedExamType").Any())
            {
                List<int> applicantExamTypes = context.HttpContext.Session.Get<List<int>>("CompletedExamType");

                if (applicantExamTypes.Contains(CurrentExamType))
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary
                        {
                            {"Controller", "Applicant" },
                            {"Action", "ExamAlreadyCompleted" }
                        });
                }
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace AptitudeWebApp.Controllers
{
    public class RandomController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

       
        public IActionResult ExamDashboard()
        {
            if (HttpContext.Session.GetString("Applicant") == null) 
            { 
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        public IActionResult ExamPage1()
        {

            if (HttpContext.Session.GetString("Applicant") == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View("Exam");
        }

        public IActionResult ExamPage2()
        {

            if (HttpContext.Session.GetString("Applicant") == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        public IActionResult ExamPage3()
        {

            if (HttpContext.Session.GetString("Applicant") == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        public IActionResult Result()
        {

            if (HttpContext.Session.GetString("Applicant") == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
    }
}

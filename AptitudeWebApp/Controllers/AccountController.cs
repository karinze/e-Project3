using AptitudeWebApp.DAL;
using Microsoft.AspNetCore.Mvc;

namespace AptitudeWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly AptitudeContext _db;
        public AccountController(AptitudeContext db)
        {
            _db = db;
        }
        public IActionResult Login()
        {
            if(HttpContext.Session.GetString("Manager") != null)
            {
                return RedirectToAction("ApplicantDashboard", "Manager");
            }
            else if(HttpContext.Session.GetString("Applicant") != null)
            {
                return RedirectToAction("ExamDashboard", "Applicant");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public IActionResult Login(string uname, string pass)
        {
            if (HttpContext.Session.GetString("Manager") ==null)
            {
                var acc = _db.Applicants.Where(x => x.Username.ToLower().Equals(uname) && x.Password.Equals(pass)).FirstOrDefault();
                if (uname == "admin" && pass == "123")
                {
                    HttpContext.Session.SetString("Manager", "admin");
                    return RedirectToAction("ApplicantDashboard", "Manager");
                }else if (acc != null)
                {
                    HttpContext.Session.SetString("Applicant", acc.Username.ToString());
                    return RedirectToAction("ExamDashboard", "Applicant");
                }
            }
            
            return View();
            
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("Manager");
            return RedirectToAction("Login", "Account");
        }




    }
}

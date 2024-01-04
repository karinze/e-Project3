using AptitudeWebApp.DAL;
using AptitudeWebApp.Repository;
using Microsoft.AspNetCore.Mvc;

namespace AptitudeWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly AptitudeContext _db;
        private readonly IPasswordHasher _passwordHasher;

        public AccountController(AptitudeContext db, IPasswordHasher passwordHasher)
        {
            _db = db;
            _passwordHasher = passwordHasher;
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
                var applicant = _db.Applicants.Where(x => x.Username.Equals(uname)).FirstOrDefault();
                var manager = _db.Managers.Where(x => x.Username.Equals(uname)).FirstOrDefault();

                //var acc = _db.Applicants.Where(x => x.Username.Equals(uname) && x.Password.Equals(pass)).FirstOrDefault();
                //var accAd = _db.Managers.Where(x => x.Username.Equals(uname)&& x.Password.Equals(pass)).FirstOrDefault();

                if (applicant != null)
                {
                    var result = _passwordHasher.Verify(applicant.Password, pass);

                    if (result)
                    {
                        HttpContext.Session.SetString("Applicant", applicant.Username.ToString());
                        HttpContext.Session.SetString("ApplicantId", applicant.ApplicantId.ToString());
                        return RedirectToAction("ExamDashboard", "Applicant");
                    }
                    else
                    {
                        
                    }

                }
                else if (manager != null)
                {
                    //var result = _passwordHasher.Verify(manager.Password, pass);

                    if (manager.Password == pass)
                    {
                        HttpContext.Session.SetString("Manager", "admin");
                        return RedirectToAction("ApplicantDashboard", "Manager");
                    }
                    //if (result)
                    //{
                    //    HttpContext.Session.SetString("Manager", "admin");
                    //    return RedirectToAction("ApplicantDashboard", "Manager");
                    //}
                    //else
                    //{

                    //}
                }
            }
            
            return View();
            
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("Manager");
            HttpContext.Session.Remove("Applicant");

            return RedirectToAction("Login", "Account");
        }




    }
}

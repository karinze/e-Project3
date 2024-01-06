using AptitudeWebApp.DAL;
using AptitudeWebApp.Repository;
using AptitudeWebApp.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AptitudeWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly AptitudeContext _db;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<AccountController> _logger;

        public AccountController(AptitudeContext db, IPasswordHasher passwordHasher, ILogger<AccountController> logger)
        {
            _db = db;
            _passwordHasher = passwordHasher;
            _logger = logger;

        }
        public IActionResult Login()
        {
            try
            {
                if (HttpContext.Session.GetString("Manager") != null)
                {
                    return RedirectToAction("ApplicantDashboard", "Manager");
                }
                else if (HttpContext.Session.GetString("Applicant") != null)
                {
                    return RedirectToAction("ExamDashboard", "Applicant");
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred in Login. Error message: {ex.Message}");
                return View("NotFound");
            }

        }
        [HttpPost]
        public IActionResult Login(string uname, string pass)
        {
            try
            {
                if (HttpContext.Session.GetString("Manager") == null)
                {
                    var applicant = _db.Applicants.Where(x => x.Username.Equals(uname)).FirstOrDefault();
                    var manager = _db.Managers.Where(x => x.Username.Equals(uname)).FirstOrDefault();

                    List<int> examTypes = new List<int>();

                    if (applicant != null)
                    {
                        var result = _passwordHasher.Verify(applicant.Password, pass);

                        if (result)
                        {
                            examTypes = applicant.CompletedExamTypes;
                            HttpContext.Session.SetString("Applicant", applicant.Username.ToString());
                            HttpContext.Session.SetString("ApplicantId", applicant.ApplicantId.ToString());
                            if (examTypes != null)
                            {
                                HttpContext.Session.Set<List<int>>("CompletedExamTypes", applicant.CompletedExamTypes);
                                var checker = HttpContext.Session.Get<List<int>>("CompletedExamTypes");
                            }

                            return RedirectToAction("ExamDashboard", "Applicant");
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Invalid username or password.";
                            return View();
                        }

                    }
                    else if (manager != null)
                    {
                        //var result = _passwordHasher.Verify(manager.Password, pass);

                        if (manager.Password == pass)
                        {
                            HttpContext.Session.SetString("Manager", "admin");
                            return RedirectToAction("ViewApplicant", "Manager");
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Invalid username or password.";
                            return View();
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
                    else
                    {
                        ViewBag.ErrorMessage = "Invalid username or password.";
                        return View();
                    }
                }

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred in Login. Error message: {ex.Message}");
                return View("NotFound");
            }
            
            
        }

        public IActionResult LogOut()
        {
            try
            {
                HttpContext.Session.Clear();
                HttpContext.Session.Remove("Manager");
                HttpContext.Session.Remove("Applicant");
                HttpContext.Session.Remove("CompletedExamTypes");
                for (int examTypeId = 1; examTypeId <= 3; examTypeId++)
                {
                    HttpContext.Session.Remove($"CurrentExam_{examTypeId}");
                }

                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred in Login. Error message: {ex.Message}");
                return View("NotFound");
            }
            
        }




    }
}

using AptitudeWebApp.Models;
using AptitudeWebApp.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AptitudeWebApp.Controllers
{
    public class HomeController : Controller
    {
        //private readonly IGenericRepository<Applicant> _db;

        //public HomeController(IGenericRepository<Applicant> db)
        //{
        //    _db = db;
        //}
        

        public IActionResult Index()
        {
            return View(/*_db.GetAll()*/);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

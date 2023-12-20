using AptitudeWebApp.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Mail;
using System.Net;
using AptitudeWebApp.Models.Authentication;

namespace AptitudeWebApp.Controllers
{
    public class ManagerController : Controller
    {
        private readonly AptitudeContext _db;
        public ManagerController(AptitudeContext db)
        {
            _db = db;
        }
        [Authentication]
        public IActionResult ApplicantDashboard()
        {
            return View();
        }
        [Authentication]
        public IActionResult ViewApplicant()
        {
            var model = _db.Applicants.ToList();
            return View(model);
        }
        [Authentication]
        public IActionResult AddApplicant()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddApplicant(Applicant applicant, IFormFile file)
        {
            if (file != null)
            {
                // xử lý upload thumbnail
                string path = Path.Combine("wwwroot/images", file.FileName);
                var stream = new FileStream(path, FileMode.Create);
                file.CopyToAsync(stream);
                //add chuoi chua duongdan hinh vao doi tuong  newproduct
                applicant.ImagePath = "images/" + file.FileName;
            }
            _db.Applicants.Add(applicant);
            _db.SaveChanges();
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("chientestphp@gmail.com");
            mail.To.Add(applicant.Email);
            mail.IsBodyHtml = true;
            mail.Subject = "Login Account";

            string content = "<span>Username: </span>" + applicant.Username;
            content += "<br/> <span>Password: </span>" + applicant.Password;

            mail.Body = content;

            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            NetworkCredential nc = new NetworkCredential("chientestphp@gmail.com", /*"chien27112004"*/"xpbqdlofwjmenume");
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = nc;
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.Send(mail);

            return RedirectToAction("ViewApplicant", "Manager");

        }
        [Authentication]
        public IActionResult EditApplicant(Guid id)
        {
            //var model = _db.Applicants;
            //ViewBag.news = new SelectList(model, "Id", "Title");
            var item = _db.Applicants.Find(id);
            return View(item);
        }
        [HttpPost]
        public IActionResult EditApplicant(Applicant applicant, IFormFile file)
        {

            if (file != null)
            {
                // xử lý upload thumbnail
                string path = Path.Combine("wwwroot/images", file.FileName);
                var stream = new FileStream(path, FileMode.Create);
                file.CopyToAsync(stream);
                //add chuoi chua duongdan hinh vao doi tuong  newproduct
                applicant.ImagePath = "images/" + file.FileName;
            }
            _db.Applicants.Update(applicant);
            _db.SaveChanges();

           

            return RedirectToAction("ViewApplicant", "Manager");
        }

        public IActionResult DeleteApplicant(Guid id)
        {
            var item = _db.Applicants.Find(id);
            if (item != null)
            {
                _db.Applicants.Remove(item);
                _db.SaveChanges();
                return RedirectToAction("ViewApplicant", "Manager");
            }
            return View(item);
        }

        

    }
}

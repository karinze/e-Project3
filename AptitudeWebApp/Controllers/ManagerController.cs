﻿using AptitudeWebApp.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Mail;
using System.Net;
using AptitudeWebApp.Models.Authentication;
using AptitudeWebApp.Models;
using Newtonsoft.Json;
using AptitudeWebApp.Repository;
using System.Dynamic;
using Microsoft.EntityFrameworkCore;

using System.Drawing.Printing;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;

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
        public IActionResult ViewApplicant(string? txtSearch, int page = 1)
        {
            page = page < 1 ? 1 : page;
            int pageSize = 2;
            var data = (from s in _db.Applicants select s);
            if (!String.IsNullOrEmpty(txtSearch))
            {
                ViewBag.txtSearch = txtSearch;
                data = data.Where(s => s.Email.Contains(txtSearch));
            }

            if (page > 0)
            {
                page = page;
            }
            else
            {
                page = 1;
            }
            int start = (int)(page - 1) * pageSize;

            ViewBag.pageCurrent = page;
            int totalPage = data.Count();
            float totalNumsize = (totalPage / (float)pageSize);
            int numSize = (int)Math.Ceiling(totalNumsize);
            ViewBag.numSize = numSize;
            ViewBag.posts = data.OrderByDescending(x => x.ApplicantId).Skip(start).Take(pageSize);
            return View();


        }
        [Authentication]
        public IActionResult AddApplicant()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddApplicant(Applicant applicant, IFormFile file, string? education, string? education2, string? education3, DateTime? education4, DateTime? education5, string? companies, string? companies2, string? companies3, DateTime? companies4, DateTime? companies5)
        {
            ApplicantEducation applicantEducation = new ApplicantEducation();
            ApplicantCompanies applicantCompanies = new ApplicantCompanies();
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

            applicantEducation.ApplicantId = applicant.ApplicantId;
            applicantEducation.COEName = education;
            applicantEducation.Description = education2;
            applicantEducation.Notes = education3;
            applicantEducation.StartDate = education4;
            applicantEducation.EndDate = education5;

            applicantCompanies.ApplicantId = applicant.ApplicantId;
            applicantCompanies.CompanyName = companies;
            applicantCompanies.Description = companies2;
            applicantCompanies.Notes = companies3;
            applicantCompanies.StartDate = companies4;
            applicantCompanies.EndDate = companies5;

            _db.ApplicantCompanies.Add(applicantCompanies);
            _db.ApplicantEducations.Add(applicantEducation);
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
            var education = (from s in _db.ApplicantEducations where s.ApplicantId == id select s);
            ViewBag.education = education.OrderByDescending(x => x.ApplicantEducationId);
            var companies = (from s in _db.ApplicantCompanies where s.ApplicantId == id select s);
            ViewBag.companies = companies.OrderByDescending(x => x.ApplicantCompanyId);
            var item = _db.Applicants.Find(id);
            return View(item);
        }
        [HttpPost]
        public IActionResult EditApplicant(Applicant applicant, IFormFile file, string? education, string? education2, string? education3, DateTime? education4, DateTime? education5, string? companies, string? companies2, string? companies3, DateTime? companies4, DateTime? companies5)
        {
            ApplicantEducation applicantEducation = new ApplicantEducation();
            ApplicantCompanies applicantCompanies = new ApplicantCompanies();

            if (file != null)
            {
                // xử lý upload thumbnail
                string path = Path.Combine("wwwroot/images", file.FileName);
                var stream = new FileStream(path, FileMode.Create);
                file.CopyToAsync(stream);
                //add chuoi chua duongdan hinh vao doi tuong  newproduct
                applicant.ImagePath = "images/" + file.FileName;
            }
            applicantEducation.ApplicantId = applicant.ApplicantId;
            applicantEducation.COEName = education;
            applicantEducation.Description = education2;
            applicantEducation.Notes= education3;
            applicantEducation.StartDate = education4;
            applicantEducation.EndDate= education5; 

            applicantCompanies.ApplicantId= applicant.ApplicantId;
            applicantCompanies.CompanyName = companies;
            applicantCompanies.Description = companies2;
            applicantCompanies.Notes = companies3;
            applicantCompanies.StartDate = companies4;
            applicantCompanies.EndDate= companies5;

            var haha= _db.ApplicantCompanies.Where(x => x.ApplicantId == applicant.ApplicantId).FirstOrDefault();
            var hehe= _db.ApplicantEducations.Where(x=>x.ApplicantId==applicant.ApplicantId).FirstOrDefault();
            _db.ApplicantCompanies.Remove(haha);
            _db.ApplicantCompanies.Add(applicantCompanies);
            _db.ApplicantEducations.Remove(hehe);
            _db.ApplicantEducations.Add(applicantEducation);
            _db.Applicants.Update(applicant);
            _db.SaveChanges();



            return RedirectToAction("ViewApplicant", "Manager");
        }
        [Authentication]
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
        [Authentication]
        public IActionResult ViewQuestion(string? txtSearch, int page = 1)
        {
            page = page < 1 ? 1 : page;
            int pageSize = 2;
            var data = (from s in _db.ExamQuestions select s);
            if (!String.IsNullOrEmpty(txtSearch))
            {
                ViewBag.txtSearch = txtSearch;
                data = data.Where(s => s.QuestionText.Contains(txtSearch));
            }

            if (page > 0)
            {
                page = page;
            }
            else
            {
                page = 1;
            }
            int start = (int)(page - 1) * pageSize;

            ViewBag.pageCurrent = page;
            int totalPage = data.Count();
            float totalNumsize = (totalPage / (float)pageSize);
            int numSize = (int)Math.Ceiling(totalNumsize);
            ViewBag.numSize = numSize;
            ViewBag.posts = data.OrderByDescending(x => x.QuestionId).Skip(start).Take(pageSize);
            return View();
        }

        [Authentication]
        public IActionResult AddQuestion()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddQuestion(ExamQuestion exam, int btnradiotype, int btnradio)
        {
            if (btnradiotype == 1)
            {
                exam.ExamTypeId = 1;
            }
            else if (btnradiotype == 2)
            {
                exam.ExamTypeId = 2;
            }
            else if (btnradiotype == 3)
            {
                exam.ExamTypeId = 3;
            }

            if (btnradio == 1)
            {
                exam.CorrectQuestion = exam.QuestionA;
            }
            else if (btnradio == 2)
            {
                exam.CorrectQuestion = exam.QuestionB;
            }
            else if (btnradio == 3)
            {
                exam.CorrectQuestion = exam.QuestionC;
            }
            else if (btnradio == 4)
            {
                exam.CorrectQuestion = exam.QuestionD;
            }

            _db.ExamQuestions.Add(exam);
            _db.SaveChanges();
            return RedirectToAction("ViewQuestion", "Manager");

        }

        [Authentication]
        public IActionResult EditQuestion(int id)
        {
            //var model = _db.Applicants;
            //ViewBag.news = new SelectList(model, "Id", "Title");
            var item = _db.ExamQuestions.Find(id);
            return View(item);
        }
        [HttpPost]
        public IActionResult EditQuestion(ExamQuestion exam, int btnradiotype, int btnradio)
        {


            if (btnradiotype == 1)
            {
                exam.ExamTypeId = 1;
            }
            else if (btnradiotype == 2)
            {
                exam.ExamTypeId = 2;
            }
            else if (btnradiotype == 3)
            {
                exam.ExamTypeId = 3;
            }

            if (btnradio == 1)
            {
                exam.CorrectQuestion = exam.QuestionA;
            }
            else if (btnradio == 2)
            {
                exam.CorrectQuestion = exam.QuestionB;
            }
            else if (btnradio == 3)
            {
                exam.CorrectQuestion = exam.QuestionC;
            }
            else if (btnradio == 4)
            {
                exam.CorrectQuestion = exam.QuestionD;
            }

            _db.ExamQuestions.Update(exam);
            _db.SaveChanges();
            return RedirectToAction("ViewQuestion", "Manager");
        }

        [Authentication]
        public IActionResult DeleteQuestion(int id)
        {
            var item = _db.ExamQuestions.SingleOrDefault(c => c.QuestionId.Equals(id));
            if (item != null)
            {
                _db.ExamQuestions.Remove(item);
                _db.SaveChanges();
                return RedirectToAction("ViewQuestion", "Manager");
            }
            return View(item);
        }

        public IActionResult _Narbar()

        {

            return View();
        }




    }
}

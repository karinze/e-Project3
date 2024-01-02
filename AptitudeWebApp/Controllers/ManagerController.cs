using AptitudeWebApp.DAL;
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
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            if (!System.String.IsNullOrEmpty(txtSearch))
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
            const int pageSize = 10;

            // Initial query for all questions
            var query = _db.ExamQuestions.AsQueryable();

            // Apply search filter if provided
            if (!string.IsNullOrEmpty(txtSearch))
            {
                ViewBag.txtSearch = txtSearch;
                query = query.Where(q => q.QuestionText.Contains(txtSearch));
            }

            // Pagination
            page = Math.Max(page, 1);
            int start = (page - 1) * pageSize;

            // Get total number of pages
            int totalQuestions = query.Count();
            int numPages = (int)Math.Ceiling(totalQuestions / (double)pageSize);

            // Apply ordering, skip, and take for the current page
            var questions = query.OrderByDescending(q => q.QuestionId)
                                 .Skip(start)
                                 .Take(pageSize)
                                 .ToList();

            // Pass data to ViewBag
            ViewBag.Questions = questions;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = numPages;
            ViewBag.Posts = questions.OrderByDescending(x => x.QuestionId).Skip(start).Take(pageSize);

            return View();
        }

        [Authentication]
        public IActionResult AddQuestion()
        {
            var model = new ExamQuestions
            {
                Answers = new List<Answer>
                {
                    new Answer(),
                    new Answer(),
                    new Answer(),
                    new Answer()
                }
            };
            return View(model);
        }

        [HttpGet]
        [Authentication]
        public IActionResult EditQuestion(int QuestionId)
        {
            //    var existingQuestion = _db.ExamQuestions
            //.Include(q => q.Answers).ToList()
            //.FirstOrDefault(q => q.QuestionId == QuestionId);
            var existingQuestion = (from question in _db.ExamQuestions
                                   join answer in _db.Answers on question.QuestionId equals answer.QuestionId
                                   select new ExamQuestions
                                   {
                                       QuestionId = QuestionId,
                                       QuestionText = question.QuestionText,
                                       ExamTypeId = question.ExamTypeId,
                                       Answers = (from x in _db.Answers
                                                  join y in _db.ExamQuestions on x.QuestionId equals y.QuestionId select x).ToList()
                                   }).FirstOrDefault();
            if (existingQuestion == null)
            {
                // If the question with the specified QuestionId is not found, create a new instance
                var newQuestion = new ExamQuestions
                {
                    Answers = new List<Answer>() // Initialize Answers to an empty list
                };
                return View(newQuestion);
            } else
            {
                return View(existingQuestion);

            }

        }

        [HttpPost]
        public IActionResult EditQuestion(ExamQuestions question, string[] answerTexts, int correctAnswerIndex, int examTypeId)
        {
            if (ModelState.IsValid)
            {
                // Update the question details
                var existingQuestion = _db.ExamQuestions
                    .Include(q => q.Answers)
                    .FirstOrDefault(q => q.QuestionId == question.QuestionId);
                var questionAnswers = _db.Answers.Where(x => x.QuestionId == question.QuestionId).ToList();
                if (existingQuestion != null)
                {
                    existingQuestion.QuestionText = question.QuestionText;
                    existingQuestion.ExamTypeId = examTypeId;
                    _db.ExamQuestions.Update(existingQuestion);
                    _db.SaveChanges();

                    foreach (var answer in questionAnswers)
                    {
                        _db.Entry(answer).State = EntityState.Detached;
                    }
                    // Update existing answers or add new ones
                    for (int i = 0; i < answerTexts.Length; i++)
                    {
                        if (i < questionAnswers.Count())
                        {
                            // Update existing answer
                            questionAnswers[i].Text = answerTexts[i];
                            questionAnswers[i].IsCorrect = (i + 1 == correctAnswerIndex);
                            _db.Answers.Update(questionAnswers[i]);
                            _db.SaveChanges();
                        }
                        else
                        {
                            // Add new answer
                            var newAnswer = new Answer
                            {
                                Text = answerTexts[i],
                                IsCorrect = (i + 1 == correctAnswerIndex),
                                QuestionId = existingQuestion.QuestionId
                            };
                            //existingQuestion.Answers.Add(newAnswer);
                            _db.Answers.Add(newAnswer);
                            _db.SaveChanges();

                        }
                    }

                    // Remove extra answers if any
                    while (existingQuestion.Answers.Count > 4)
                    {
                        var answerToRemove = existingQuestion.Answers.Last();
                        _db.Answers.Remove(answerToRemove);
                        _db.SaveChanges();

                    }

                    _db.SaveChanges();

                    return RedirectToAction("ViewQuestion", "Manager");
                } else
                {
                    existingQuestion = new ExamQuestions
                    {
                        QuestionScore = 5, ExamTypeId = examTypeId,
                        QuestionText = question.QuestionText,
                        Answers = new List<Answer>() // Initialize Answers to an empty list
                    };
                    _db.ExamQuestions.Add(existingQuestion);
                    _db.SaveChanges();

                    foreach (var answer in existingQuestion.Answers.ToList())
                    {
                        _db.Entry(answer).State = EntityState.Detached;
                    }
                    // Update existing answers or add new ones
                    for (int i = 0; i < answerTexts.Length; i++)
                    {
                        if (i < existingQuestion.Answers.Count)
                        {
                            // Update existing answer
                            existingQuestion.Answers[i].Text = answerTexts[i];
                            existingQuestion.Answers[i].IsCorrect = (i + 1 == correctAnswerIndex);
                        }
                        else
                        {
                            // Add new answer
                            var newAnswer = new Answer
                            {
                                Text = answerTexts[i],
                                IsCorrect = (i + 1 == correctAnswerIndex),
                                QuestionId = existingQuestion.QuestionId

                            };
                            //existingQuestion.Answers.Add(newAnswer);
                            _db.Answers.Add(newAnswer);
                            _db.SaveChanges();

                        }
                    }

                    // Remove extra answers if any
                    while (existingQuestion.Answers.Count > 4)
                    {
                        var answerToRemove = existingQuestion.Answers.Last();
                        _db.Answers.Remove(answerToRemove);
                        _db.SaveChanges();

                    }
                    //_db.ExamQuestions.Add(existingQuestion);
                    _db.SaveChanges();


                    return RedirectToAction("ViewQuestion", "Manager");
                }
            }

            return View(question);
        }

        [Authentication]
        public IActionResult DeleteQuestion(int QuestionId)
        {
            var item = _db.ExamQuestions.FirstOrDefault(c => c.QuestionId.Equals(QuestionId));
            if (item != null)
            {
                _db.ExamQuestions.Remove(item);
                _db.SaveChanges();
                return RedirectToAction("ViewQuestion", "Manager");
            }
            return View();
        }

        public IActionResult _Narbar()

        {

            return View();
        }




    }
}

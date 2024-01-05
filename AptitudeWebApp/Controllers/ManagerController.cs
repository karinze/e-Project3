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
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<ManagerController> _logger;
        public ManagerController(AptitudeContext db, IPasswordHasher passwordHasher, ILogger<ManagerController> logger)
        {
            _db = db;
            _passwordHasher = passwordHasher;
            _logger = logger;
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
            ViewBag.posts = data.OrderBy(x => x.ApplicantId).Skip(start).Take(pageSize);
            return View();


        }
        [Authentication]
        public IActionResult AddApplicant()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddApplicant(ApplicantEducationCompaniesViewModel mainApplicant)
        {       
            if (ModelState.IsValid)
            {
                var applicant = mainApplicant.applicant;
                var applicantEducation = mainApplicant.applicantEducation;
                var applicantCompanies = mainApplicant.applicantCompanies;
                if (applicant.Username != null)
                {
                    var userNameCheck = _db.Applicants.Where(x => x.Username == applicant.Username).FirstOrDefault();
                    if (userNameCheck != null)
                    {
                        ViewBag.username = "This username is already taken! Please choose another one.";
                        return View();
                    }
                }
                var tempPassword = applicant.Password;

                var passwordHash = _passwordHasher.Hash(applicant.Password);
                applicant.Password = passwordHash;
                // if (file != null)
                //{
                //    // xử lý upload thumbnail
                //    string path = Path.Combine("wwwroot/images", file.FileName);
                //    var stream = new FileStream(path, FileMode.Create);
                //    file.CopyToAsync(stream);
                //    //add chuoi chua duongdan hinh vao doi tuong  newproduct
                //    applicant.ImagePath = "images/" + file.FileName;
                //}

                _db.Applicants.Add(applicant);

                applicantEducation.ApplicantId = applicant.ApplicantId;
                applicantCompanies.ApplicantId = applicant.ApplicantId;

                _db.ApplicantEducations.Add(applicantEducation);
                _db.ApplicantCompanies.Add(applicantCompanies);

                _db.SaveChanges();
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("chientestphp@gmail.com");
                mail.To.Add(applicant.Email);
                mail.IsBodyHtml = true;
                mail.Subject = "Webster Application Login Info";
                var applicantName = applicant.FirstName + " " + applicant.LastName;
                string content = "Hello " + applicantName + "! Us at Webster Corporation have created an account for you as a part of our evaluation progress. Please login to the system to start your exam at any time.";
                content += "<br/><br/> <span>Your Login Info: </span>" + applicant.Username;

                content += "<br/> <span>Username: </span>" + applicant.Username;
                content += "<br/> <span>Password: </span>" + tempPassword;
                content += "<br/> <span>----------------------------</span>";

                content += "<br/><br/> <span>Sincerely,</span>";
                content += "<br/> <span>Webster Corporation</span>";

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
            else
            {
                return View();
            }
        }
        [Authentication]
        public IActionResult EditApplicant(Guid id, ApplicantEducationCompaniesViewModel viewModel)
        {
            var item = _db.Applicants.FirstOrDefault(x => x.ApplicantId.Equals(id));
            var item1 = _db.ApplicantEducations.FirstOrDefault(x=>x.ApplicantId.Equals(id));
            var item2 = _db.ApplicantCompanies.FirstOrDefault(x => x.ApplicantId.Equals(id));
            viewModel.applicant = item;
            viewModel.applicantEducation = item1;
            viewModel.applicantCompanies = item2;

            if (viewModel == null)
            {
                return NotFound();
            }
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult EditApplicant(ApplicantEducationCompaniesViewModel mainApplicant)
        {
            if (ModelState.IsValid)
            {
                var applicant = mainApplicant.applicant;
                var applicantEducation = mainApplicant.applicantEducation;
                var applicantCompanies = mainApplicant.applicantCompanies;
                    _db.Applicants.Update(applicant);
                applicantEducation.ApplicantId = applicant.ApplicantId;
                applicantCompanies.ApplicantId = applicant.ApplicantId;
                _db.ApplicantEducations.Update(applicantEducation);
                    _db.ApplicantCompanies.Update(applicantCompanies);

                    _db.SaveChanges();
                return RedirectToAction("ViewApplicant", "Manager");
            }
            else
            {
                return View();
            }
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
            page = page < 1 ? 1 : page;
            var data = (from s in _db.Questions select s);
            if (!System.String.IsNullOrEmpty(txtSearch))
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
            ViewBag.posts = data.OrderBy(x => x.QuestionId).Skip(start).Take(pageSize);
            return View();

            //// Initial query for all questions
            //var query = _db.Questions.AsQueryable();

            //// Apply search filter if provided
            //if (!string.IsNullOrEmpty(txtSearch))
            //{
            //    ViewBag.txtSearch = txtSearch;
            //    query = query.Where(q => q.QuestionText.Contains(txtSearch));
            //}

            //// Pagination
            //page = Math.Max(page, 1);
            //int start = (page - 1) * pageSize;

            //// Get total number of pages
            //int totalQuestions = query.Count();
            //int numPages = (int)Math.Ceiling(totalQuestions / (double)pageSize);

            //// Apply ordering, skip, and take for the current page
            //var questions = query.OrderByDescending(q => q.QuestionId)
            //                     .Skip(start)
            //                     .Take(pageSize)
            //                     .ToList();

            //// Pass data to ViewBag
            //ViewBag.Questions = questions;
            //ViewBag.CurrentPage = page;
            //ViewBag.TotalPages = numPages;
            //ViewBag.Posts = questions.OrderBy(x => x.QuestionId).Skip(start).Take(pageSize);

            return View();
        }

        [Authentication]
        public IActionResult AddQuestion()
        {
            var model = new Questions
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
        public IActionResult EditQuestion(int questionId)
        {
            
            //    var existingQuestion = _db.ExamQuestions
            //.Include(q => q.Answers).ToList()
            //.FirstOrDefault(q => q.QuestionId == QuestionId);
            var existingAnswers = _db.Answers.Where(x=>x.QuestionId == questionId).ToList();
            var question = _db.Questions.FirstOrDefault(x=>x.QuestionId == questionId);
            //var existingQuestion = (from question in _db.ExamQuestions
            //                       join answer in _db.Answers on question.QuestionId equals answer.QuestionId
            //                       select new ExamQuestions
            //                       {
            //                           QuestionId = questionId,
            //                           QuestionText = question.QuestionText,
            //                           ExamTypeId = question.ExamTypeId,
            //                           Answers = existingAnswers
            //                       }).FirstOrDefault();
            if (question == null)
            {
                // If the question with the specified QuestionId is not found, create a new instance
                var newQuestion = new Questions
                {
                    Answers = new List<Answer>() // Initialize Answers to an empty list
                };
                return View(newQuestion);
            } else
            {
                question.Answers = existingAnswers;
                return View(question);

            }

        }

        [HttpPost]
        public IActionResult EditQuestion(Questions question, string[] answerTexts, int correctAnswerIndex, int examTypeId)
        {
            if (ModelState.IsValid)
            {
                // Update the question details
                var existingQuestion = _db.Questions
                    .Include(q => q.Answers)
                    .FirstOrDefault(q => q.QuestionId == question.QuestionId);
                var questionAnswers = _db.Answers.Where(x => x.QuestionId == question.QuestionId).ToList();
                if (existingQuestion != null)
                {
                    existingQuestion.QuestionText = question.QuestionText;
                    existingQuestion.ExamTypeId = examTypeId;
                    _db.Questions.Update(existingQuestion);
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
                    existingQuestion = new Questions
                    {
                        QuestionScore = 5, ExamTypeId = examTypeId,
                        QuestionText = question.QuestionText,
                        Answers = new List<Answer>() // Initialize Answers to an empty list
                    };
                    _db.Questions.Add(existingQuestion);
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
        public IActionResult DeleteQuestion(int questionId)
        {
            var item = _db.Questions.FirstOrDefault(c => c.QuestionId.Equals(questionId));
            var answers = _db.Answers.Where(x => x.QuestionId == questionId);

            foreach (var answer in answers)
            {
                _db.Answers.Remove(answer);
                _db.SaveChanges();
            }
            if (item != null)
            {
                _db.Questions.Remove(item);
                _db.SaveChanges();
                return RedirectToAction("ViewQuestion", "Manager");
            }
            return View();
        }

        public IActionResult _Narbar()
        {
            return View();
        }
        [AcceptVerbs("Get","Post")]
        public JsonResult IsUserName(string userName)
        {
            bool item = _db.Applicants.Any(x => x.Username.Equals(userName));
            if (item)
            {
                return Json(data: false);
            }
            else
            {
                return Json(data:true);
            }
            
        }
    }
}

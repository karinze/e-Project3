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
using System.Linq;
using OfficeOpenXml;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

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
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred in ApplicantDashboard. Error message: {ex.Message}");
                return View("NotFound");
            }
        }
        [Authentication]
        public IActionResult ViewApplicant(string? txtSearch, int page = 1)
        {
            try
            {
                page = page < 1 ? 1 : page;
                int pageSize = 10;
                var data = (from s in _db.Applicants select s).ToList();

                if (!System.String.IsNullOrEmpty(txtSearch))
                {
                    ViewBag.txtSearch = txtSearch;
                    data = data.Where(s => s.Email.Contains(txtSearch)
                                        || s.FirstName.Contains(txtSearch)
                                        || s.LastName.Contains(txtSearch)
                                        || s.Address.Contains(txtSearch)
                                        || s.PhoneNumber.Contains(txtSearch)
                                        || s.Age.ToString().Contains(txtSearch)
                                        ).ToList();
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

            catch (Exception ex)
            {
                _logger.LogError($"An error occurred in ViewApplicant. Error message: {ex.Message}");
                return View("NotFound");
            }



        }
        [Authentication]
        public IActionResult AddApplicant()
        {
            try
            {
                return View();

            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred in AddApplicant. Error message: {ex.Message}");
                return View("NotFound");
            }
        }
        [HttpPost]
        public IActionResult AddApplicant(ApplicantEducationCompaniesViewModel mainApplicant)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred in AddApplicant. Error message: {ex.Message}");
                return View("NotFound");
            }

        }
        [Authentication]
        public IActionResult EditApplicant(Guid id, ApplicantEducationCompaniesViewModel viewModel)
        {
            try
            {
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred in ApplicantDashboard. Error message: {ex.Message}");
                return View("NotFound");
            }
            var item = _db.Applicants.FirstOrDefault(x => x.ApplicantId.Equals(id));
            var item1 = _db.ApplicantEducations.FirstOrDefault(x => x.ApplicantId.Equals(id));
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
            try
            {
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred in ApplicantDashboard. Error message: {ex.Message}");
                return View("NotFound");
            }
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
            try
            {
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred in ApplicantDashboard. Error message: {ex.Message}");
                return View("NotFound");
            }
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
            try
            {
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred in ApplicantDashboard. Error message: {ex.Message}");
                return View("NotFound");
            }
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
        }

        [Authentication]
        public IActionResult AddQuestion()
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred in AddQuestion. Error message: {ex.Message}");
                return View("NotFound");
            }

        }

        [HttpGet]
        [Authentication]
        public IActionResult EditQuestion(int questionId)
        {
            try
            {
                var existingAnswers = _db.Answers.Where(x => x.QuestionId == questionId).ToList();
                var question = _db.Questions.FirstOrDefault(x => x.QuestionId == questionId);

                if (question == null)
                {
                    var newQuestion = new Questions
                    {
                        Answers = new List<Answer>() // Initialize Answers to an empty list
                    };
                    return View(newQuestion);
                }
                else
                {
                    question.Answers = existingAnswers;
                    return View(question);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred in ApplicantDashboard. Error message: {ex.Message}");
                return View("NotFound");
            }

        }

        [HttpPost]
        public IActionResult EditQuestion(Questions question, string[] answerTexts, int correctAnswerIndex, int examTypeId)
        {
            try
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
                    }
                    else
                    {
                        existingQuestion = new Questions
                        {
                            QuestionScore = 5,
                            ExamTypeId = examTypeId,
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
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred in EditQuestion. Error message: {ex.Message}");
                return View("NotFound");
            }

        }

        [Authentication]
        public IActionResult DeleteQuestion(int questionId)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred in DeleteQuestion. Error message: {ex.Message}");
                return View("NotFound");
            }

        }

        [Authentication]
        public IActionResult Report(string? txtSearch, string? errorMessage, int page = 1)
        {
            try
            {
                ReportViewModel reportViewModel = new ReportViewModel();

                var applicants = _db.Applicants.Where(x => x.HasPassedExam == true);

                foreach (var applicant in applicants)
                {
                    ApplicantWithScore applicantWithScore = new ApplicantWithScore();

                    var applicantExams = _db.ApplicantExams.Where(x => x.ApplicantId == applicant.ApplicantId).Select(x => x.ApplicantScore).ToList();

                    int currentApplicantScore = applicantExams.Sum().Value;

                    applicantWithScore.Applicant = applicant;
                    applicantWithScore.ApplicantScore = currentApplicantScore;
                    reportViewModel.ApplicantWithScores.Add(applicantWithScore);
                }

                page = page < 1 ? 1 : page;
                int pageSize = 10;

                List<ApplicantWithScore> data = reportViewModel.ApplicantWithScores.ToList();

                if (!System.String.IsNullOrEmpty(txtSearch))
                {
                    ViewBag.txtSearch = txtSearch;

                    if (data.Any())
                    {
                        data = (List<ApplicantWithScore>)data.Where(s =>
                                        s.Applicant.Email.Contains(txtSearch)
                                        || s.Applicant.FirstName.Contains(txtSearch)
                                        || s.Applicant.LastName.Contains(txtSearch)
                                        || s.Applicant.Address.Contains(txtSearch)
                                        || s.Applicant.PhoneNumber.Contains(txtSearch)
                                        || s.Applicant.Age.ToString().Contains(txtSearch)
                                        ).ToList();
                    }

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

                if (errorMessage != null)
                {
                    ViewBag.ErrorMessage = errorMessage;
                }
                int totalPage = data.Count();
                float totalNumsize = (totalPage / (float)pageSize);
                int numSize = (int)Math.Ceiling(totalNumsize);
                ViewBag.numSize = numSize;
                ViewBag.posts = data.OrderBy(x => x.Applicant.ApplicantId).Skip(start).Take(pageSize);
                ViewBag.ApplicantWithScoresJson = JsonConvert.SerializeObject(data.OrderBy(x => x.Applicant.ApplicantId).Skip(start).Take(pageSize));

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred in Report. Error message: {ex.Message}");
                return View("NotFound");
            }

        }
        [HttpPost]
        public IActionResult Export(string? txtSearch, string applicantWithScoresJson)
        {
            try
            {
                var applicantWithScores = JsonConvert.DeserializeObject<List<ApplicantWithScore>>(applicantWithScoresJson);

                if (applicantWithScores != null || applicantWithScores.Any())
                {
                    var stream = ExportToExcel(applicantWithScores);

                    string excelName = $"WebsterPassedApplicantReport_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";

                    // Return the file without redirecting
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
                    
                }
                else
                {
                    TempData["ErrorMessage"] = "Nothing to export!";

                }
                if (!string.IsNullOrEmpty(txtSearch))
                {
                    return RedirectToAction("Report", new { txtSearch });

                }
                else
                {
                    return RedirectToAction("Report");

                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred in Export. Error message: {ex.Message}");
                return View("Error");
            }
        }


        // Export to Excel functionality ( DO NOT TOUCH )
        private MemoryStream ExportToExcel(List<ApplicantWithScore> data)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                if (data == null || !data.Any())
                {
                    // Handle null or empty data
                    var worksheet = package.Workbook.Worksheets.Add("No Data");
                    worksheet.Cells["A1"].Value = "No data available for export. Sorry!";
                }
                else
                {
                    var worksheet = package.Workbook.Worksheets.Add("Report");

                    var headers = typeof(Applicant)
                    .GetProperties()
                    .Where(x => x.Name != "Password" && x.Name != "ImagePath" &&
                                   x.Name != "ApplicantCompanies" && x.Name != "ApplicantEducation" &&
                                   x.Name != "CompletedExamTypes")
                    .Select(x => x.Name)
                    .ToList();

                    for (int i = 0; i < headers.Count; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = headers[i];
                    }

                    // Populate data
                    int row = 2;
                    foreach (var item in data)
                    {
                        var properties = item.Applicant.GetType().GetProperties();

                        // Exclude specific properties from the list of properties
                        properties = properties
                            .Where(prop => prop.Name != "Password" && prop.Name != "ImagePath" &&
                                           prop.Name != "ApplicantCompanies" && prop.Name != "ApplicantEducation" &&
                                           prop.Name != "CompletedExamTypes")
                            .ToArray();

                        for (int i = 0; i < properties.Length; i++)
                        {
                            worksheet.Cells[row, i + 1].Value = properties[i].GetValue(item.Applicant);
                        }

                        row++;
                    }
                }

                package.Save();
            }
            stream.Position = 0;
            return stream;
        }


        private IEnumerable<string> GetPropertyDisplayNames(Type type)
        {
            return type.GetProperties().Select(prop => prop.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute)
                        .Select(displayAttr => displayAttr?.Name ?? "N/A");
        }
    }
}

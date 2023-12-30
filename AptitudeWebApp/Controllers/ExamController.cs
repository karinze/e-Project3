using AptitudeWebApp.DAL;
using AptitudeWebApp.Models;
using AptitudeWebApp.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AptitudeWebApp.Controllers
{
    [Route("exam")]
    public class ExamController : Controller
    {
        private readonly IGenericRepository<Exam> _db;
        private readonly AptitudeContext _context;
        ApplicantExam apExam = new ApplicantExam(); //initialize new ApplicantExam
        public ExamController(IGenericRepository<Exam> db)
        {
            _db = db;
        }
        [Route("index")]
        [Route("")]
        [Route("~")]

        public IActionResult Index(int? examTypeId)
        {
            if (examTypeId == 1 || examTypeId == 2 || examTypeId == 3)
            {
                Random random = new Random();
                var questions = _context.ExamQuestions.OrderBy(q => random.Next()).Where(x => x.ExamTypeId == examTypeId);
                ViewBag.Questions = questions.ToList();
            }
            
            return View(/*_db.GetAll()*/);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [HttpPost]
        [Route("submit")]
        public IActionResult Submit(List<ExamQuestion> examQuestions)
        {
            int scoreType1 = 0;
            int scoreType2 = 0;
            int scoreType3 = 0;
            List<int> scores = new List<int>();
            string correctAns = "";
            foreach(var q in examQuestions)
            {
                if (q.QuestionA != null)
                {
                    correctAns = q.QuestionA;
                }
                if (q.QuestionB != null)
                {
                    correctAns = q.QuestionB;
                }
                if (q.QuestionC != null)
                {
                    correctAns = q.QuestionC;
                }
                if (q.QuestionD != null)
                {
                    correctAns = q.QuestionD;
                }
                if (correctAns.Equals(q.CorrectQuestion))
                {
                    switch (q.ExamTypeId)
                    {
                        case 1:
                            scoreType1++;
                            break;
                        case 2:
                            scoreType2++;
                            break;
                        case 3:
                            scoreType3++;
                            break;
                    }
                }
                scores.Add(scoreType1);
                scores.Add(scoreType2);
                scores.Add(scoreType3);
            }
            return View("ExamResults", scores);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

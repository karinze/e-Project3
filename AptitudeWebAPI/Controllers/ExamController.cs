using AptitudeWebApp.Models;
using AptitudeWebApp.Repository;
using AptitudeWebApp.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace AptitudeWebApp.Controllers
{
    [ApiController]
    [Route("api/exams")]
    public class ExamController : ControllerBase
    {
        private readonly IExamService _examService;
        private readonly ILogger<ExamController> _logger;

        public ExamController(IExamService examService, ILogger<ExamController> logger)
        {
            _examService = examService;
            _logger = logger;
        }

        [HttpGet("random")]
        public IActionResult GetRandomQuestionsByExamType([FromQuery] int examTypeId, [FromQuery] int numberOfQuestions)
        {
            try
            {
                var questions = _examService.GetRandomQuestionsByExamType(examTypeId, numberOfQuestions);
                return Ok(questions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetRandomQuestionsByExamType: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("initialize")]
        public IActionResult InitializeExam([FromBody] ExamInitializationRequest request)
        {
            try
            {
                var questions = _examService.GetRandomQuestionsByExamType(request.ExamTypeId, request.NumberOfQuestions);
                var newExam = _examService.InitializeExam(request.ApplicantId, questions, request.ExamTypeId);
                return Ok(newExam);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in InitializeExam: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("nextExamType")]
        public IActionResult GetNextExamType([FromBody] List<int> completedExamTypes)
        {
            try
            {
                var nextExamType = _examService.GetNextExamType(completedExamTypes);
                return Ok(nextExamType);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetNextExamType: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("applicant/{applicantId}")]
        public IActionResult GetApplicant(Guid applicantId)
        {
            try
            {
                var applicant = _examService.GetApplicantByApplicantId(applicantId);
                return Ok(applicant);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetApplicant: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{examId}")]
        public IActionResult GetExamDetails(int examId)
        {
            try
            {
                var examDetails = _examService.GetExamDetails(examId);
                return Ok(examDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetExamDetails: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("process-answers")]
        public IActionResult ProcessAnswers([FromBody] ProcessAnswersRequest request)
        {
            try
            {
                var exam = _examService.GetExamDetails(request.ExamId);
                if (exam == null)
                {
                    return NotFound("Exam not found");
                }

                var score = _examService.ProcessAnswers(exam);
                _examService.SaveExamScore(exam, score);

                return Ok(new { Score = score });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ProcessAnswers: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // DTOs
        public class ExamInitializationRequest
        {
            public Guid ApplicantId { get; set; }
            public int ExamTypeId { get; set; }
            public int NumberOfQuestions { get; set; }
        }

        public class ProcessAnswersRequest
        {
            public int ExamId { get; set; }
            public List<int> SelectedAnswers { get; set; }
        }
    }
}

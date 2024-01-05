namespace AptitudeWebApp.Models
{
    public class ReportViewModel
    {
        public List<ApplicantWithScore> ApplicantWithScores { get; set; }

        public ReportViewModel()
        {
            ApplicantWithScores = new List<ApplicantWithScore>();
        }
        
    }
    public class ApplicantWithScore
    {
        public Applicant Applicant { get; set; }
        public int ApplicantScore { get; set; }
    }
}

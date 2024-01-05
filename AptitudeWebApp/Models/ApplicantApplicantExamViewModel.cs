namespace AptitudeWebApp.Models
{
    public class ApplicantApplicantExamViewModel
    {
        public Applicant applicant { get; set; }
        public ApplicantExam applicantExam { get; set; }

        public List<Applicant> applicants { get; set; }
        public ApplicantApplicantExamViewModel()
        {
            applicants = new List<Applicant>();
        }
    }
}

namespace CS3750Assignment1
{
    public class GradedSubmission
    {
        public string SubmissionId { get; set; }
        public int Grade { get; set; }

        public GradedSubmission(string submissionId, int grade)
        {
            SubmissionId = submissionId;
            Grade = grade;
        }



    }
}

namespace CS3750Assignment1.Classes
{
    public class RegisteredCourse
    {
        public string CourseName { get; set; }

        public int CourseNumber { get; set; }

        public IList<string> MeetingDays { get; set; }

        public string[] MeetingTime { get; set; }


        public RegisteredCourse(string name, int number, IList<string> meetingDays, string meetTimeStart, string meetTimeEnd)
        {
            CourseName = name;
            CourseNumber = number;
            MeetingDays = meetingDays;
            MeetingTime = [meetTimeStart, meetTimeEnd];
        }

    }
}

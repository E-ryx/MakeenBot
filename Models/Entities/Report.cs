namespace MakeenBot.Models.Entities
{
    public class Report
    {
        public int Id { get; private set; }
        public int ReportNumber { get; private set; }
        public int Hours { get; private set; }
        public DateTime Date { get; private set; }

        public int StudentId { get; private set; }
        public Student Student { get; private set; }

        public int CourseId { get; private set; }
        public Course Course { get; private set; }

        private Report() { }

        public Report(int reportNumber, int hours, DateTime date, int studentId, int courseId)
        {
            ReportNumber = reportNumber;
            Hours = hours;
            Date = date;
            StudentId = studentId;
            CourseId = courseId;
        }
    }
}

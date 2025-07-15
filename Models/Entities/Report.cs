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

        public bool IsFailed { get; private set; }


        private Report() { }

        public Report(int reportNumber, int hours, int studentId)
        {
            ReportNumber = reportNumber;
            Hours = hours;
            Date = DateTime.Now;
            StudentId = studentId;
            //CourseId = courseId;
            IsFailed = DateTime.Now.Hour >= 22;
        }
        public void Update(int hours)
        {
            Hours = hours;
            Date = DateTime.Now;
            IsFailed = DateTime.Now.Hour >= 22;
        }

    }
}

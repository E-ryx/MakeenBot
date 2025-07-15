namespace MakeenBot.Models.Entities
{
    public class Student
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int CourseId { get; private set; }
        public Course Course { get; private set; }

        public ICollection<Report> Reports { get; private set; } = new List<Report>();

        private Student() { }

        public Student(string name, int courseId)
        {
            Name = name;
            CourseId = courseId;
        }
    }
}

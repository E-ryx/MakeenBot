namespace MakeenBot.Models.Entities
{
        public class Course
        {
            public int Id { get; private set; }
            public string Name { get; private set; }
            public ICollection<Student> Students { get; private set; } = new List<Student>();

            private Course() { }

            public Course(string name)
            {
                Name = name;
            }
        }

    }


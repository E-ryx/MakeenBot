using MakeenBot.Models.Entities;

public interface ICourseRepository
{
    Task<Course?> GetByNameAsync(string name);
    Task<Course?> GetByIdAsync(int id);
    Task AddAsync(Course course);
    Task UpdateAsync(Course course);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(string name);
    Task UpdateStudentsAsync(Course course, List<Student> newStudents);
}

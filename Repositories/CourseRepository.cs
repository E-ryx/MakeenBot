using MakeenBot.Data;
using MakeenBot.Interfaces.Repositories;
using MakeenBot.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MakeenBot.Infrastructure.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly BotDbContext _context;

    public CourseRepository(BotDbContext context)
    {
        _context = context;
    }

    public async Task<Course?> GetByNameAsync(string name)
    {
        return await _context.Courses
            .AsNoTracking()
            .Include(c => c.Students)
                .ThenInclude(s => s.Reports)
            .FirstOrDefaultAsync(c => c.Name == name);
    }
    
    public async Task<Course?> GetByIdAsync(int id)
    {
        return await _context.Courses
            .AsNoTracking()
            .Include(c => c.Students)
                .ThenInclude(s => s.Reports)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AddAsync(Course course)
    {
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Course course)
    {
        _context.Courses.Update(course);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateStudentsAsync(Course course, List<Student> newStudents)
    {
        var existing = await _context.Courses
            .Include(c => c.Students)
            .FirstOrDefaultAsync(c => c.Id == course.Id);

        if (existing == null) return;

        _context.Students.RemoveRange(existing.Students);
        await _context.SaveChangesAsync();

        existing.Students.Clear();
        foreach (var student in newStudents)
        {
            existing.Students.Add(student);
        }

        _context.Courses.Update(existing);
        await _context.SaveChangesAsync();
    }


    public async Task DeleteAsync(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course is null) return;

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();
    }
    public async Task<bool> ExistsAsync(string name)
    {
        return await _context.Courses.AnyAsync(c => c.Name == name);
    }
}

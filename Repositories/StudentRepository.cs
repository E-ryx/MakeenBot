using System;
using MakeenBot.Data;
using MakeenBot.Interfaces.Repositories;
using MakeenBot.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MakeenBot.Repositories
{
    public class StudentRepository: IStudentRepository
    {
        private readonly BotDbContext _context;

        public StudentRepository(BotDbContext context)
        {
            _context = context;
        }

        public async Task<Student?> GetByNameAsync(string name)
        {
            return await _context.Students
                .FirstOrDefaultAsync(s => s.Name == name);
        }
    }
}

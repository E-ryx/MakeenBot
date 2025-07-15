using MakeenBot.Models.Entities;

namespace MakeenBot.Interfaces.Repositories
{
    public interface IStudentRepository
    {
        Task<Student?> GetByNameAsync(string name);
    }
}

using System.Collections.Generic;
using System.Reflection.Emit;
using System;
using Microsoft.EntityFrameworkCore;
using MakeenBot.Models.Entities;

namespace MakeenBot.Data
{
    public class BotDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Report> Reports { get; set; }
        public BotDbContext(DbContextOptions<BotDbContext> options)
            : base(options)
        {
        }
    }
}
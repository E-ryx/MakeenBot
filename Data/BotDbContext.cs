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

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    // Student -> Course (many-to-one)
        //    modelBuilder.Entity<Student>()
        //        .HasOne(s => s.Course)
        //        .WithMany(c => c.Students)
        //        .HasForeignKey(s => s.CourseId);

        //    // Report -> Student (many-to-one)
        //    modelBuilder.Entity<Report>()
        //        .HasOne(r => r.Student)
        //        .WithMany(s => s.Reports)
        //        .HasForeignKey(r => r.StudentId);

        //    // Report -> Course (many-to-one)
        //    modelBuilder.Entity<Report>()
        //        .HasOne(r => r.Course)
        //        .WithMany(c => c.Reports)
        //        .HasForeignKey(r => r.CourseId);
        //}

    }
}
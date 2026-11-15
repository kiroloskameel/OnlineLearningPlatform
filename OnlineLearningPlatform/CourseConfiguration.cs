using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLearningPlatform.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Title).IsRequired().HasMaxLength(200);
            builder.Property(c => c.Description).HasMaxLength(1500);
            builder.Property(c => c.Price).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(c => c.Level).IsRequired();
            builder.Property(c => c.IsPublished).IsRequired();

            // Relationships
            builder.HasOne(c => c.Instructor)
                   .WithMany(i => i.Courses)
                   .HasForeignKey(c => c.InstructorId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Category)
                   .WithMany(cat => cat.Courses)
                   .HasForeignKey(c => c.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Seed Data (6 Courses)
            builder.HasData(
                new Course { Id = 1, Title = "C# Fundamentals", Description = "Learn C# from scratch", Price = 49.99m, Level = CourseLevel.Beginner, IsPublished = true, PublishedDate = new DateTime(2023, 2, 1), InstructorId = 1, CategoryId = 1 },
                new Course { Id = 2, Title = "Advanced ASP.NET Core", Description = "Build enterprise web apps", Price = 99.99m, Level = CourseLevel.Advanced, IsPublished = true, PublishedDate = new DateTime(2023, 4, 10), InstructorId = 1, CategoryId = 3 },
                new Course { Id = 3, Title = "HTML & CSS Basics", Description = "Web design foundations", Price = 29.99m, Level = CourseLevel.Beginner, IsPublished = true, PublishedDate = new DateTime(2023, 6, 1), InstructorId = 2, CategoryId = 4 },
                new Course { Id = 4, Title = "React Masterclass", Description = "Modern frontend with React", Price = 79.99m, Level = CourseLevel.Intermediate, IsPublished = true, PublishedDate = new DateTime(2023, 7, 15), InstructorId = 2, CategoryId = 5 },
                new Course { Id = 5, Title = "Microservices Architecture", Description = "Design distributed systems", Price = 149.99m, Level = CourseLevel.Advanced, IsPublished = false, PublishedDate = null, InstructorId = 3, CategoryId = 2 },
                new Course { Id = 6, Title = "Clean Architecture in .NET", Description = "Write maintainable code", Price = 119.99m, Level = CourseLevel.Intermediate, IsPublished = true, PublishedDate = new DateTime(2023, 8, 20), InstructorId = 3, CategoryId = 3 }
            );
        }
    }
}

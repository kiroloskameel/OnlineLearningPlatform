using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLearningPlatform.Configurations
{
    public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
    {
        public void Configure(EntityTypeBuilder<Instructor> builder)
        {
            builder.HasKey(i => i.Id);
            builder.Property(i => i.FullName).IsRequired().HasMaxLength(150);
            builder.Property(i => i.Email).IsRequired().HasMaxLength(200);
            builder.Property(i => i.Bio).HasMaxLength(1000);
            builder.Property(i => i.JoinedDate).IsRequired();

            // Seed Data (3 Instructors)
            builder.HasData(
                new Instructor { Id = 1, FullName = "Ahmed Ali", Email = "ahmed.ali@platform.com", Bio = "Backend Expert", JoinedDate = new DateTime(2023, 1, 15) },
                new Instructor { Id = 2, FullName = "Mona Mohamed", Email = "mona.mohamed@platform.com", Bio = "Frontend Lead", JoinedDate = new DateTime(2023, 3, 10) },
                new Instructor { Id = 3, FullName = "Khaled Hassan", Email = "khaled.hassan@platform.com", Bio = "FullStack Architect", JoinedDate = new DateTime(2023, 5, 20) }
            );
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLearningPlatform.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Description).HasMaxLength(500);

            // Self-Referencing Relationship & Restrict Delete Behavior
            builder.HasOne(c => c.ParentCategory)
                   .WithMany(c => c.SubCategories)
                   .HasForeignKey(c => c.ParentCategoryId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevents deleting parent if sub-categories exist

            // Seed Data (5 Categories forming 3 levels: Programming -> Backend -> ASP.NET)
            builder.HasData(
                new Category { Id = 1, Name = "Programming", Description = "All programming languages and concepts", ParentCategoryId = null },
                new Category { Id = 2, Name = "Backend", Description = "Server-side development", ParentCategoryId = 1 },
                new Category { Id = 3, Name = "ASP.NET", Description = "ASP.NET Core web development", ParentCategoryId = 2 },
                new Category { Id = 4, Name = "Frontend", Description = "Client-side development", ParentCategoryId = 1 },
                new Category { Id = 5, Name = "React", Description = "React.js library", ParentCategoryId = 4 }
            );
        }
    }
}

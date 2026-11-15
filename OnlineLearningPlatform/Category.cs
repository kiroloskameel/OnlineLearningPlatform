using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLearningPlatform
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        // Self-Referencing (Parent -> Sub-categories)
        public int? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }
        public ICollection<Category> SubCategories { get; set; } = new List<Category>();

        // Navigation
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}

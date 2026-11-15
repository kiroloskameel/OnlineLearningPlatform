using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLearningPlatform
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public CourseLevel Level { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? PublishedDate { get; set; }

        // Foreign Keys
        public int InstructorId { get; set; }
        public Instructor Instructor { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        // Navigation
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace OnlineLearningPlatform
{
    class Program
    {
        static void Main(string[] args)
        {
            using var context = new AppDbContext();

            // Ensure database is created or migrated
            context.Database.EnsureCreated();

            // Part 02: Seeding Students and Enrollments conditionally if tables are empty
            SeedStudentsAndEnrollments(context);

            Console.WriteLine("=== 1. CREATE OPERATIONS ===");
            AddNewInstructor(context, "Sara Ahmed", "sara@platform.com", "UI/UX Specialist");
            AddNewCourse(context, "Advanced Tailwind CSS", 45.00m, CourseLevel.Beginner, 2, 4);
            AddMultipleStudents(context);
            EnrollStudent(context, studentId: 1, courseId: 1); // Should succeed (published, not enrolled)

            Console.WriteLine("\n=== 2. READ OPERATIONS ===");
            FindCourse(context, 1);
            FilterCoursesByPrice(context, 30.00m, 100.00m);
            DisplayCategories(context);
            DisplayCourseRoster(context, 1);

            Console.WriteLine("\n=== 3. UPDATE OPERATIONS ===");
            UpdateCoursePrice(context, 1, 55.00m);
            UpdateEnrollmentProgress(context, 1, 1, 100); // Should set status to Completed
            PublishCourse(context, 5); // Publishing course ID 5

            Console.WriteLine("\n=== 4. DELETE OPERATIONS ===");
            TestDeleteCategoryWithSubCategories(context, 1); // Tests safe check & exception handling
            UnenrollStudent(context, 1, 1);

            Console.WriteLine("\n=== 5. BULK OPERATIONS ===");
            BulkPriceRaise(context, CourseLevel.Beginner, 0.10m);
            BulkDeleteDroppedEnrollments(context);

            Console.WriteLine("\nAll operations completed successfully!");
        }

        // --- Seeding Method ---
        private static void SeedStudentsAndEnrollments(AppDbContext context)
        {
            if (!context.Students.Any())
            {
                var students = new[]
                {
                    new Student { FullName = "Omar Yasser", Email = "omar@gmail.com", DateOfBirth = new DateTime(2000, 5, 12) },
                    new Student { FullName = "Hana Nabil", Email = "hana@gmail.com", DateOfBirth = new DateTime(2001, 8, 22) },
                    new Student { FullName = "Ziad Tarek", Email = "ziad@gmail.com", DateOfBirth = new DateTime(1999, 2, 14) },
                    new Student { FullName = "Nourhan Adel", Email = "nourhan@gmail.com", DateOfBirth = new DateTime(2002, 11, 3) },
                    new Student { FullName = "Youssef Karim", Email = "youssef@gmail.com", DateOfBirth = new DateTime(2000, 7, 19) }
                };
                context.Students.AddRange(students);
                context.SaveChanges();
                Console.WriteLine("Students seeded successfully.");
            }

            if (!context.Enrollments.Any())
            {
                var enrollments = new[]
                {
                    new Enrollment { StudentId = 1, CourseId = 1, EnrollmentDate = DateTime.Now.AddDays(-10), ProgressPercentage = 100, Status = EnrollmentStatus.Completed, FinalGrade = 95.5m },
                    new Enrollment { StudentId = 1, CourseId = 2, EnrollmentDate = DateTime.Now.AddDays(-5), ProgressPercentage = 40, Status = EnrollmentStatus.Active },
                    new Enrollment { StudentId = 2, CourseId = 1, EnrollmentDate = DateTime.Now.AddDays(-8), ProgressPercentage = 15, Status = EnrollmentStatus.Dropped },
                    new Enrollment { StudentId = 2, CourseId = 3, EnrollmentDate = DateTime.Now.AddDays(-6), ProgressPercentage = 100, Status = EnrollmentStatus.Completed, FinalGrade = 88.0m },
                    new Enrollment { StudentId = 3, CourseId = 4, EnrollmentDate = DateTime.Now.AddDays(-3), ProgressPercentage = 60, Status = EnrollmentStatus.Active },
                    new Enrollment { StudentId = 4, CourseId = 6, EnrollmentDate = DateTime.Now.AddDays(-4), ProgressPercentage = 100, Status = EnrollmentStatus.Completed, FinalGrade = 92.0m },
                    new Enrollment { StudentId = 5, CourseId = 3, EnrollmentDate = DateTime.Now.AddDays(-2), ProgressPercentage = 10, Status = EnrollmentStatus.Dropped },
                    new Enrollment { StudentId = 5, CourseId = 6, EnrollmentDate = DateTime.Now.AddDays(-1), ProgressPercentage = 30, Status = EnrollmentStatus.Active }
                };
                context.Enrollments.AddRange(enrollments);
                context.SaveChanges();
                Console.WriteLine("Enrollments seeded successfully.");
            }
        }

        // --- Create Methods ---
        private static void AddNewInstructor(AppDbContext context, string name, string email, string bio)
        {
            var instructor = new Instructor { FullName = name, Email = email, Bio = bio, JoinedDate = DateTime.Now };
            context.Instructors.Add(instructor);
            context.SaveChanges();
            Console.WriteLine($"[Create] Instructor added with ID: {instructor.Id}");
        }

        private static void AddNewCourse(AppDbContext context, string title, decimal price, CourseLevel level, int instructorId, int categoryId)
        {
            if (!context.Instructors.Any(i => i.Id == instructorId) || !context.Categories.Any(c => c.Id == categoryId))
            {
                Console.WriteLine("[Create Error] Instructor or Category does not exist!");
                return;
            }

            var course = new Course
            {
                Title = title,
                Price = price,
                Level = level,
                IsPublished = true,
                PublishedDate = DateTime.Now,
                InstructorId = instructorId,
                CategoryId = categoryId
            };
            context.Courses.Add(course);
            context.SaveChanges();
            Console.WriteLine($"[Create] Course added with ID: {course.Id}");
        }

        private static void AddMultipleStudents(AppDbContext context)
        {
            var students = new[]
            {
                new Student { FullName = "Karim Mahmoud", Email = "karim@test.com", DateOfBirth = new DateTime(2003, 1, 1) },
                new Student { FullName = "Dina Sameh", Email = "dina@test.com", DateOfBirth = new DateTime(2002, 5, 5) },
                new Student { FullName = "Maged Nady", Email = "maged@test.com", DateOfBirth = new DateTime(2001, 9, 9) }
            };
            context.Students.AddRange(students);
            context.SaveChanges();
            Console.WriteLine($"[Create] Three students added with IDs: {string.Join(", ", students.Select(s => s.Id))}");
        }

        private static void EnrollStudent(AppDbContext context, int studentId, int courseId)
        {
            var studentExists = context.Students.Any(s => s.Id == studentId);
            var course = context.Courses.FirstOrDefault(c => c.Id == courseId);

            if (!studentExists || course == null)
            {
                Console.WriteLine("[Enroll Error] Student or Course does not exist.");
                return;
            }

            if (!course.IsPublished)
            {
                Console.WriteLine("[Enroll Refused] Course is not published yet.");
                return;
            }

            var alreadyEnrolled = context.Enrollments.Any(e => e.StudentId == studentId && e.CourseId == courseId);
            if (alreadyEnrolled)
            {
                Console.WriteLine("[Enroll Refused] Student is already enrolled in this course.");
                return;
            }

            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrollmentDate = DateTime.Now,
                ProgressPercentage = 0,
                Status = EnrollmentStatus.Active
            };

            context.Enrollments.Add(enrollment);
            context.SaveChanges();
            Console.WriteLine("[Enroll Success] Student enrolled successfully.");
        }

        // --- Read Methods ---
        private static void FindCourse(AppDbContext context, int id)
        {
            var courseById = context.Courses.Find(id);
            var courseByTitle = context.Courses.FirstOrDefault(c => c.Id == id);

            if (courseById == null)
                Console.WriteLine($"[Read] Course with ID {id} not found.");
            else
                Console.WriteLine($"[Read Found] Title: {courseById.Title}, Price: {courseById.Price}");
        }

        private static void FilterCoursesByPrice(AppDbContext context, decimal min, decimal max)
        {
            var courses = context.Courses
                .Where(c => c.Price >= min && c.Price <= max)
                .OrderByDescending(c => c.Price)
                .Select(c => new { c.Title, c.Level, c.Price })
                .ToList();

            Console.WriteLine($"[Read Filter] Courses between {min} and {max}:");
            foreach (var c in courses)
                Console.WriteLine($"- {c.Title} | Level: {c.Level} | Price: ${c.Price}");
        }

        private static void DisplayCategories(AppDbContext context)
        {
            var topCategories = context.Categories.Where(c => c.ParentCategoryId == null).ToList();
            Console.WriteLine("[Read Categories] Top-level categories listed.");
        }

        private static void DisplayCourseRoster(AppDbContext context, int courseId)
        {
            // Without joins: Get student IDs from enrollments first, then get students using Contains
            var studentIds = context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Select(e => e.StudentId)
                .ToList();

            var students = context.Students
                .Where(s => studentIds.Contains(s.Id))
                .Select(s => s.FullName)
                .ToList();

            Console.WriteLine($"[Roster] Course {courseId} has {students.Count} students enrolled: {string.Join(", ", students)}");
        }

        // --- Update Methods ---
        private static void UpdateCoursePrice(AppDbContext context, int courseId, decimal newPrice)
        {
            var course = context.Courses.Find(courseId);
            if (course != null)
            {
                course.Price = newPrice;
                context.SaveChanges();
                Console.WriteLine($"[Update] Course {courseId} price updated to ${newPrice}");
            }
        }

        private static void UpdateEnrollmentProgress(AppDbContext context, int studentId, int courseId, decimal progress)
        {
            if (progress < 0 || progress > 100)
            {
                Console.WriteLine("[Update Error] Progress must be between 0 and 100.");
                return;
            }

            var enrollment = context.Enrollments.Find(studentId, courseId);
            if (enrollment != null)
            {
                enrollment.ProgressPercentage = progress;
                if (progress == 100)
                {
                    enrollment.Status = EnrollmentStatus.Completed;
                    enrollment.FinalGrade = 95.0m; // example final grade upon completion
                }
                context.SaveChanges();
                Console.WriteLine("[Update] Enrollment progress updated successfully.");
            }
        }

        private static void PublishCourse(AppDbContext context, int courseId)
        {
            var course = context.Courses.Find(courseId);
            if (course != null && !course.IsPublished)
            {
                course.IsPublished = true;
                course.PublishedDate = DateTime.Now;
                context.SaveChanges();
                Console.WriteLine($"[Update] Course {courseId} published successfully.");
            }
        }

        // --- Delete Methods ---
        private static void UnenrollStudent(AppDbContext context, int studentId, int courseId)
        {
            var enrollment = context.Enrollments.Find(studentId, courseId);
            if (enrollment != null)
            {
                context.Enrollments.Remove(enrollment);
                context.SaveChanges();
                Console.WriteLine("[Delete] Student unenrolled successfully.");
            }
        }

        private static void TestDeleteCategoryWithSubCategories(AppDbContext context, int categoryId)
        {
            // 1. Try removing category that has sub-categories and catch DbUpdateException
            try
            {
                var category = context.Categories.Find(categoryId);
                if (category != null)
                {
                    context.Categories.Remove(category);
                    context.SaveChanges();
                }
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"[Expected Exception Caught]: {ex.InnerException?.Message ?? ex.Message}");
            }

            // 2. Safe version checking for sub-categories before deleting
            var safeCategory = context.Categories.Include(c => c.SubCategories).FirstOrDefault(c => c.Id == categoryId);
            if (safeCategory != null)
            {
                if (safeCategory.SubCategories.Any())
                {
                    Console.WriteLine("[Safe Delete] Cannot delete category because it still contains sub-categories.");
                }
                else
                {
                    context.Categories.Remove(safeCategory);
                    context.SaveChanges();
                    Console.WriteLine("[Safe Delete] Category deleted safely.");
                }
            }
        }

        // --- Bulk Operations ---
        private static void BulkPriceRaise(AppDbContext context, CourseLevel level, decimal percentage)
        {
            // Bulk update without loading entities into memory using EF Core ExecuteUpdate
            int affectedRows = context.Courses
                .Where(c => c.Level == level && c.IsPublished)
                .ExecuteUpdate(s => s.SetProperty(c => c.Price, c => c.Price * (1 + percentage)));

            Console.WriteLine($"[Bulk Update] Raised price for {affectedRows} courses of level {level}.");
        }

        private static void BulkDeleteDroppedEnrollments(AppDbContext context)
        {
            // Bulk delete without loading entities using EF Core ExecuteDelete
            int affectedRows = context.Enrollments
                .Where(e => e.Status == EnrollmentStatus.Dropped)
                .ExecuteDelete();

            Console.WriteLine($"[Bulk Delete] Deleted {affectedRows} dropped enrollments.");
        }
    }
}

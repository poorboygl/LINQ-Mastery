using System;
using System.Linq;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        var repository = new CourseRepository();

        var summaries = repository.GetStudentCourseCompletionSummary();

        Console.WriteLine("===== Student Course Completion Summary =====");

        foreach (var summary in summaries)
        {
            Console.WriteLine($"\nStudent: {summary.StudentName}");
            Console.WriteLine($"Total Courses Completed        : {summary.TotalCoursesCompleted}");
            Console.WriteLine($"Mandatory Courses Completed    : {summary.MandatoryCoursesCompleted}");
            Console.WriteLine($"Non-Mandatory Courses Completed: {summary.NonMandatoryCoursesCompleted}");
            Console.WriteLine($"Completion Percentage          : {summary.CompletionPercentage:F2}%");
        }

        Console.ReadKey();
    }
}

public class Course
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public bool Mandatory { get; set; }
}

public class Student
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<int> CompletedCourseIds { get; set; } = [];
}

public class StudentCourseCompletionSummary
{
    public required string StudentName { get; set; }
    public int TotalCoursesCompleted { get; set; }
    public int MandatoryCoursesCompleted { get; set; }
    public int NonMandatoryCoursesCompleted { get; set; }
    public double CompletionPercentage { get; set; }
}

public class CourseRepository
{
    public List<Student> Students { get; set; } =
    [
        new Student { Id = 1, Name = "Alice", CompletedCourseIds = new List<int> { 1, 2, 4 } },
        new Student { Id = 2, Name = "Bob", CompletedCourseIds = new List<int> { 1, 3 } },
        new Student { Id = 3, Name = "Charlie", CompletedCourseIds = new List<int> { 2, 4, 5 } }
    ];

    public List<Course> Courses { get; set; } =
    [
        new Course { Id = 1, Name = "Mathematics", Mandatory = true },
        new Course { Id = 2, Name = "Science", Mandatory = true },
        new Course { Id = 3, Name = "History", Mandatory = false },
        new Course { Id = 4, Name = "Art", Mandatory = false },
        new Course { Id = 5, Name = "Physical Education", Mandatory = true }
    ];

    public List<StudentCourseCompletionSummary> GetStudentCourseCompletionSummary()
    {
        var mandatoryCourseIds = Courses.Where(c => c.Mandatory).Select(c => c.Id).ToList();
        var totalMandatoryCourses = mandatoryCourseIds.Count;

        return [.. Students
            .Select(student => new StudentCourseCompletionSummary
            {
                StudentName = student.Name,
                TotalCoursesCompleted = student.CompletedCourseIds.Count,
                MandatoryCoursesCompleted = student.CompletedCourseIds.Count(id => mandatoryCourseIds.Contains(id)),
                NonMandatoryCoursesCompleted = student.CompletedCourseIds.Count(id => !mandatoryCourseIds.Contains(id)),
                CompletionPercentage = (double)student.CompletedCourseIds.Count(id => mandatoryCourseIds.Contains(id)) / totalMandatoryCourses * 100
            })
            .OrderByDescending(summary => summary.CompletionPercentage)];
    }
}

/*
 ===== Student Course Completion Summary =====

Student: Alice
Total Courses Completed        : 3
Mandatory Courses Completed    : 2
Non-Mandatory Courses Completed: 1
Completion Percentage          : 66.67%

Student: Charlie
Total Courses Completed        : 3
Mandatory Courses Completed    : 2
Non-Mandatory Courses Completed: 1
Completion Percentage          : 66.67%

Student: Bob
Total Courses Completed        : 2
Mandatory Courses Completed    : 1
Non-Mandatory Courses Completed: 1
Completion Percentage          : 33.33%
 */



/*
This exercise involves grouping and aggregating data across multiple lists to generate course completion summaries for each student.

* 1.Extracting Mandatory Course IDs:

Courses.Where(c => c.Mandatory).Select(c => c.Id).ToList() gathers IDs of all mandatory courses for easier filtering.

* 2.Calculating Course Completion Statistics:

TotalCoursesCompleted: Counts all completed courses for each student.

MandatoryCoursesCompleted: Counts only completed mandatory courses.

NonMandatoryCoursesCompleted: Counts completed non-mandatory courses.

CompletionPercentage: Calculates the percentage of mandatory courses completed by dividing completed mandatory courses by total mandatory courses.

* 3.Returning the Sorted Summary:

OrderByDescending(summary => summary.CompletionPercentage) sorts students based on their mandatory course completion rates.

 */
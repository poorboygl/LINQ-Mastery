class Program
{
    static void Main()
    {
        var repo = new AttendanceRepository();

        var summaries = repo.GetClassAttendanceSummary();

        Console.WriteLine("===== CLASS ATTENDANCE SUMMARY =====");

        foreach (var summary in summaries)
        {
            Console.WriteLine($"\nClass: {summary.ClassName}");
            Console.WriteLine($"Date               : {summary.ClassDate:yyyy-MM-dd}");
            Console.WriteLine($"Total Students     : {summary.TotalStudents}");
            Console.WriteLine($"Present Students   : {summary.PresentStudents}");
            Console.WriteLine($"Absent Students    : {summary.AbsentStudents}");
            Console.WriteLine($"Attendance %       : {summary.AttendancePercentage:F2}%");
        }

        Console.ReadKey();
    }
}

public class Class
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTime Date { get; set; }
}

public class Student
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<int> AttendedClassIds { get; set; } = [];
}

public class ClassAttendanceSummary
{
    public required string ClassName { get; set; }
    public DateTime ClassDate { get; set; }
    public int TotalStudents { get; set; }
    public int PresentStudents { get; set; }
    public int AbsentStudents { get; set; }
    public double AttendancePercentage { get; set; }
}

public class AttendanceRepository
{
    public List<Student> Students { get; set; } =
    [
        new Student { Id = 1, Name = "Alice", AttendedClassIds = [1, 2] },
        new Student { Id = 2, Name = "Bob", AttendedClassIds = [1, 3] },
        new Student { Id = 3, Name = "Charlie", AttendedClassIds = [2] },
        new Student { Id = 4, Name = "Diana", AttendedClassIds = [1, 2, 3] }
    ];

    public List<Class> Classes { get; set; } =
    [
        new Class { Id = 1, Name = "Math", Date = new DateTime(2023, 12, 1) },
        new Class { Id = 2, Name = "Science", Date = new DateTime(2023, 12, 2) },
        new Class { Id = 3, Name = "History", Date = new DateTime(2023, 12, 3) }
    ];

    public List<ClassAttendanceSummary> GetClassAttendanceSummary()
    {
        var totalStudents = Students.Count;

        return [.. Classes
            .Select(classItem => new ClassAttendanceSummary
            {
                ClassName = classItem.Name,
                ClassDate = classItem.Date,
                TotalStudents = totalStudents,
                PresentStudents = Students.Count(s => s.AttendedClassIds.Contains(classItem.Id)),
                AbsentStudents = totalStudents - Students.Count(s => s.AttendedClassIds.Contains(classItem.Id)),
                AttendancePercentage = (double)Students.Count(s => s.AttendedClassIds.Contains(classItem.Id)) / totalStudents * 100
            })
            .OrderByDescending(summary => summary.AttendancePercentage)];
    }
}

/*
     ===== CLASS ATTENDANCE SUMMARY =====

    Class: Math
    Date               : 2023-12-01
    Total Students     : 4
    Present Students   : 3
    Absent Students    : 1
    Attendance %       : 75.00%

    Class: Science
    Date               : 2023-12-02
    Total Students     : 4
    Present Students   : 3
    Absent Students    : 1
    Attendance %       : 75.00%

    Class: History
    Date               : 2023-12-03
    Total Students     : 4
    Present Students   : 2
    Absent Students    : 2
    Attendance %       : 50.00%

 */


/*
In this exercise, you group students by each class and calculate attendance-related statistics.

* 1.Setting Up Total Students:

var totalStudents = Students.Count defines the total number of students, used to calculate absent counts and percentages.

* 2.Calculating Attendance Statistics:

PresentStudents: Counts students who attended the class.

AbsentStudents: Calculates the total students minus present students.

AttendancePercentage: Computes the percentage of students in attendance.

* 3.Returning the Sorted Summary:

OrderByDescending(summary => summary.AttendancePercentage) sorts classes by the percentage of students present.

*/
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        var repo = new SchoolRepository();

        var summaries = repo.GetTopPerformingStudentsBySubject();

        Console.WriteLine("=== TOP STUDENTS BY SUBJECT ===\n");

        foreach (var summary in summaries)
        {
            Console.WriteLine($"Subject: {summary.SubjectName}");

            foreach (var student in summary.TopStudents)
            {
                Console.WriteLine($"   {student.StudentName}: {student.Score}");
            }

            Console.WriteLine(new string('-', 40));
        }

        Console.ReadKey();
    }
}


public class Subject
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class Student
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class Grade
{
    public int StudentId { get; set; }
    public int SubjectId { get; set; }
    public double Score { get; set; }
}

public class StudentPerformance
{
    public required string StudentName { get; set; }
    public double Score { get; set; }
}

public class SubjectTopStudentsSummary
{
    public required string SubjectName { get; set; }
    public List<StudentPerformance> TopStudents { get; set; } = [];
}

public class SchoolRepository
{
    public List<Subject> Subjects { get; set; } =
    [
        new Subject { Id = 1, Name = "Mathematics" },
        new Subject { Id = 2, Name = "Science" }
    ];

    public List<Student> Students { get; set; } =
    [
        new Student { Id = 1, Name = "Alice" },
        new Student { Id = 2, Name = "Bob" },
        new Student { Id = 3, Name = "Charlie" }
    ];

    public List<Grade> Grades { get; set; } =
    [
        new Grade { StudentId = 1, SubjectId = 1, Score = 95 },
        new Grade { StudentId = 2, SubjectId = 1, Score = 88 },
        new Grade { StudentId = 3, SubjectId = 1, Score = 90 },
        new Grade { StudentId = 1, SubjectId = 2, Score = 85 },
        new Grade { StudentId = 2, SubjectId = 2, Score = 92 }
    ];

    public List<SubjectTopStudentsSummary> GetTopPerformingStudentsBySubject()
    {
        //var result = Subjects.GroupJoin(Grades,
        //              subject => subject.Id,
        //              grade => grade.SubjectId,
        //              (subject, SubjectGrades) => new
        //              {
        //                  SubjectName = subject.Name,
        //                  TopStudent = SubjectGrades.Join(Students,
        //                      g => g.StudentId,
        //                      s => s.Id,
        //                      (grade, student) => new StudentPerformance
        //                      {
        //                          StudentName = student.Name,
        //                          Score = grade.Score,
        //                      })
        //                      .OrderByDescending(student => student.Score)
        //                      .Take(3)
        //                      .ToList()
        //              })
        //              .Select(summary => new SubjectTopStudentsSummary
        //              {
        //                  SubjectName = summary.SubjectName,
        //                  TopStudents = summary.TopStudent
        //              })
        //              .ToList();

        var result = Subjects.GroupJoin(Grades,
                        subject => subject.Id,
                        grade => grade.SubjectId,
                        (subject, subjectGrades) => new SubjectTopStudentsSummary
                        {
                            SubjectName = subject.Name,
                            TopStudents = [.. subjectGrades
                                .Join(Students,
                                        grade => grade.StudentId,
                                        student => student.Id,
                                        (grade, student) => new StudentPerformance
                                        {
                                            StudentName = student.Name,
                                            Score = grade.Score
                                        }
                                )
                                .OrderByDescending(sp => sp.Score)
                                .Take(3)]
                        })
                        .ToList();

        return result;
    }
}


/*
 !=== TOP STUDENTS BY SUBJECT ===

Subject: Mathematics
   Alice: 95
   Charlie: 90
   Bob: 88
----------------------------------------
Subject: Science
   Bob: 92
   Alice: 85
----------------------------------------
 */


/*
! This exercise generates a report of top-performing students by subject by combining data across subjects, students, and grades.

* 1.Grouping Grades by Subject:

GroupJoin(Grades, subject => subject.Id, grade => grade.SubjectId, ...) groups grades by each subject.

* 2.Calculating Performance Metrics:

TopStudents: subjectGrades.Join(Students, ...) matches each grade to the respective student, allowing access to StudentName and Score.

* 3.Identifying Top-Performing Students:

OrderByDescending(s => s.Score).Take(3) selects the top 3 students by score within each subject.

* 4.Returning the Report:

The result is a list of SubjectTopStudentsSummary objects, each containing the top 3 students by score in each subject.

 */
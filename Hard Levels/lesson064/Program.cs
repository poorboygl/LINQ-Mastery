class Program
{
    static void Main()
    {
        var repo = new SchoolRepository();
        var topStudents = repo.GetTopStudentsByAverageScore();

        Console.WriteLine("=== TOP STUDENTS BY AVERAGE SCORE ===\n");
        foreach (var student in topStudents)
        {
            Console.WriteLine($"Name: {student.StudentName}, Average Score: {student.AverageScore:F2}");
        }

        Console.ReadKey();
    }
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

public class StudentAverageScoreSummary
{
    public required string StudentName { get; set; }
    public double AverageScore { get; set; }
}

public class SchoolRepository
{
    public List<Student> Students { get; set; } =
    [
        new Student { Id = 1, Name = "Alice" },
        new Student { Id = 2, Name = "Bob" },
        new Student { Id = 3, Name = "Charlie" }
    ];

    public List<Grade> Grades { get; set; } =
    [
        new Grade { StudentId = 1, SubjectId = 1, Score = 90 },
        new Grade { StudentId = 1, SubjectId = 2, Score = 85 },
        new Grade { StudentId = 2, SubjectId = 1, Score = 78 },
        new Grade { StudentId = 2, SubjectId = 2, Score = 82 },
        new Grade { StudentId = 3, SubjectId = 1, Score = 95 },
        new Grade { StudentId = 3, SubjectId = 2, Score = 88 }
    ];

    public List<StudentAverageScoreSummary> GetTopStudentsByAverageScore()
    {

        /*
         !=== TOP STUDENTS BY AVERAGE SCORE ===

        Name: Charlie, Average Score: 91.50
        Name: Alice, Average Score: 87.50
        Name: Bob, Average Score: 80.00
         */

        var result = Grades
                        .GroupBy(grade => grade.StudentId)
                        .Select(group => new StudentAverageScoreSummary
                        {
                            StudentName = Students.Where(s => s.Id == group.Key).First().Name,
                            AverageScore = group.Average(g => g.Score)

                        })
                        .OrderByDescending(summary => summary.AverageScore)
                        .Take(5)
                        .ToList();
        return result;

    }

    public List<StudentAverageScoreSummary> GetTopStudentsByAverageScore_2()
    {

        /*
         !=== TOP STUDENTS BY AVERAGE SCORE ===

        Name: Charlie, Average Score: 91.50
        Name: Alice, Average Score: 87.50
        Name: Bob, Average Score: 80.00
         */

        var result = Students
                     .Select(s => new StudentAverageScoreSummary 
                     { 
                       StudentName = s.Name,
                       AverageScore = Grades.Where(grade => grade.StudentId == s.Id).Average(g => g.Score)

                     })
                    .OrderByDescending(summary => summary.AverageScore)
                    .Take(5)
                    .ToList();
        return result;

    }
}


/*
!This exercise generates a report ranking students by their average grades across all subjects.

* 1.Filtering Grades by Student:

Grades.Where(grade => grade.StudentId == student.Id) selects grades for each student.

* 2.Calculating Average Score:

AverageScore: Average(grade => grade.Score) calculates the average score for each student.

* 3.Returning the Top Students:

The result is a list of StudentAverageScoreSummary objects, showing the top 5 students by average grade.

 */
class Program
{
  static void Main()
  {
    var repo = new EmployeeRepository();
    var topEmployees = repo.GetTopPerformingEmployees();

    Console.WriteLine("=== EMPLOYEE PERFORMANCE REPORT ===\n");

    foreach (var emp in topEmployees)
    {
      Console.WriteLine($"Employee: {emp.EmployeeName}");
      Console.WriteLine($"  Average Rating: {emp.AverageRating:F2}");
      Console.WriteLine(new string('-', 40));
    }

    Console.ReadKey();
  }
}

public class Employee
{
  public int Id { get; set; }
  public required string Name { get; set; }
}

public class ProjectRating
{
  public int EmployeeId { get; set; }
  public int ProjectId { get; set; }
  public double Rating { get; set; }
}

public class EmployeePerformanceSummary
{
  public required string EmployeeName { get; set; }
  public double AverageRating { get; set; }
}

public class EmployeeRepository
{
  public List<Employee> Employees { get; set; } =
  [
      new Employee { Id = 1, Name = "Alice" },
        new Employee { Id = 2, Name = "Bob" },
        new Employee { Id = 3, Name = "Charlie" }
  ];

  public List<ProjectRating> ProjectRatings { get; set; } =
  [
      new ProjectRating { EmployeeId = 1, ProjectId = 1, Rating = 4.5 },
        new ProjectRating { EmployeeId = 1, ProjectId = 2, Rating = 4.8 },
        new ProjectRating { EmployeeId = 2, ProjectId = 3, Rating = 3.9 },
        new ProjectRating { EmployeeId = 2, ProjectId = 4, Rating = 4.2 },
        new ProjectRating { EmployeeId = 3, ProjectId = 5, Rating = 5.0 },
        new ProjectRating { EmployeeId = 3, ProjectId = 6, Rating = 4.9 }
  ];

  public List<EmployeePerformanceSummary> GetTopPerformingEmployees()
  {   /*
         !=== EMPLOYEE PERFORMANCE REPORT ===

        Employee: Charlie
          Average Rating: 4.95
        ----------------------------------------
        Employee: Alice
          Average Rating: 4.65
        ----------------------------------------
        Employee: Bob
          Average Rating: 4.05
        ----------------------------------------
      */

    var result = ProjectRatings
                 .GroupBy(p => p.EmployeeId)
                 .Select(group => new EmployeePerformanceSummary
                 {
                   EmployeeName = Employees.First(e => e.Id == group.Key).Name,
                   AverageRating = group.Sum(p => p.Rating) / group.Count()

                 })
                 .OrderByDescending(p => p.AverageRating)
                 .Take(3)
                 .ToList();

    return result;
  }

  public List<EmployeePerformanceSummary> GetTopPerformingEmployees_2()
  {   /*
         !=== EMPLOYEE PERFORMANCE REPORT ===

        Employee: Charlie
          Average Rating: 4.95
        ----------------------------------------
        Employee: Alice
          Average Rating: 4.65
        ----------------------------------------
        Employee: Bob
          Average Rating: 4.05
        ----------------------------------------
         
      */
      
    var result = Employees
                .Select(e =>
                {
                  var averageRating = ProjectRatings
                                          .Where(p => p.EmployeeId == e.Id)
                                          .Average(p => p.Rating);

                  return new EmployeePerformanceSummary
                  {
                    EmployeeName = e.Name,
                    AverageRating = averageRating
                  };
                })
                .OrderByDescending(p => p.AverageRating)
                .Take(3)
                .ToList();

    return result;
  }
}


/*
! This exercise generates a report ranking employees by their average project ratings.

* 1.Filtering Ratings by Employee:

ProjectRatings.Where(rating => rating.EmployeeId == employee.Id) selects project ratings for each employee.

* 2.Calculating Average Rating:

AverageRating: Average(rating => rating.Rating) calculates the average rating of projects for each employee.

* 3.Returning the Top Performing Employees:

The result is a list of EmployeePerformanceSummary objects, showing the top 5 employees by average rating.
 
 */
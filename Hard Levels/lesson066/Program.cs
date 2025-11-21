class Program
{
    static void Main()
    {
        var repo = new HRRepository();

        var summaries = repo.GetTopRatedEmployeeByDepartment();

        Console.WriteLine("=== TOP EMPLOYEE BY DEPARTMENT ===\n");

        foreach (var s in summaries)
        {
            Console.WriteLine($"Department: {s.DepartmentName}");
            Console.WriteLine($"  Top Employee: {s.EmployeeName}");
            Console.WriteLine($"  Average Rating: {s.AverageRating:F2}\n");
        }

        Console.ReadKey();
    }
}

public class Department
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class Employee
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int DepartmentId { get; set; }
}

public class PerformanceRating
{
    public int EmployeeId { get; set; }
    public DateTime RatingDate { get; set; }
    public double Score { get; set; }
}

public class DepartmentTopEmployeeSummary
{
    public string DepartmentName { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public double AverageRating { get; set; }
}

public class HRRepository
{
    public List<Department> Departments { get; set; } =
    [
        new Department { Id = 1, Name = "IT" },
        new Department { Id = 2, Name = "HR" }
    ];

    public List<Employee> Employees { get; set; } =
    [
        new Employee { Id = 1, Name = "Alice", DepartmentId = 1 },
        new Employee { Id = 2, Name = "Bob", DepartmentId = 1 },
        new Employee { Id = 3, Name = "Charlie", DepartmentId = 2 }
    ];

    public List<PerformanceRating> PerformanceRatings { get; set; } =
    [
        new PerformanceRating { EmployeeId = 1, RatingDate = new DateTime(2023, 1, 15), Score = 4.5 },
        new PerformanceRating { EmployeeId = 1, RatingDate = new DateTime(2023, 2, 10), Score = 4.0 },
        new PerformanceRating { EmployeeId = 2, RatingDate = new DateTime(2023, 1, 20), Score = 4.2 },
        new PerformanceRating { EmployeeId = 3, RatingDate = new DateTime(2023, 1, 25), Score = 3.8 }
    ];

    public List<DepartmentTopEmployeeSummary> GetTopRatedEmployeeByDepartment()
    {

        /*
        === TOP EMPLOYEE BY DEPARTMENT ===

        Department: IT
          Top Employee: Alice
          Average Rating: 4.25

        Department: HR
          Top Employee: Charlie
          Average Rating: 3.80

         */

        var result = Departments
                    .GroupJoin(Employees,
                    d => d.Id,
                    emp => emp.DepartmentId,
                    (department, departmentEmployees) =>
                    {
                        var departmentName = department.Name;
                        var scoreEmployee = departmentEmployees
                                          .Join(PerformanceRatings,
                                          d => d.Id,
                                          p => p.EmployeeId,
                                          (employee, rating) => new
                                          // This is a join in which the rating value is not used.
                                          // rating is not used here, meaning this join is redundant and the query is not optimized
                                          {
                                              employeeName = employee.Name,
                                              averageScore = PerformanceRatings
                                                            .Where(p => p.EmployeeId == employee.Id)
                                                            .Average(p => p.Score)
                                          })
                                          .OrderByDescending(p => p.averageScore)
                                          .FirstOrDefault();
                        ;
                        return new DepartmentTopEmployeeSummary
                        {
                            DepartmentName = departmentName,
                            EmployeeName = scoreEmployee?.employeeName ?? "No employees in this department",
                            AverageRating = scoreEmployee?.averageScore ?? 0
                        };
                    })
                    .OrderBy(summary => summary.DepartmentName)
                    .ToList();
        return result;
    }

    public List<DepartmentTopEmployeeSummary> GetTopRatedEmployeeByDepartment_2()
    {

        /*
         !=== TOP EMPLOYEE BY DEPARTMENT ===

        Department: HR
          Top Employee: Charlie
          Average Rating: 3.80

        Department: IT
          Top Employee: Alice
          Average Rating: 4.25
         */

        return [.. Departments
               .Select(department =>
               {
                   var topEmployee = Employees
                       .Where(employee => employee.DepartmentId == department.Id)
                       .Select(employee => new
                       {
                           EmployeeName = employee.Name,
                           AverageRating = PerformanceRatings
                               .Where(rating => rating.EmployeeId == employee.Id)
                               .Average(rating => rating.Score)
                       })
                       .OrderByDescending(e => e.AverageRating)
                       .FirstOrDefault();

                   return new DepartmentTopEmployeeSummary
                   {
                       DepartmentName = department.Name,
                       EmployeeName = topEmployee?.EmployeeName ?? "No employees in this department",
                       AverageRating = topEmployee?.AverageRating ?? 0
                   };
               })
               .OrderBy(summary => summary.DepartmentName)];
    }
}

/*
! This exercise generates a report showing the average customer rating for each product category.

* 1.Filtering Products by Category:

Products.Where(product => product.CategoryId == category.Id) selects products within each category.

* 2.Calculating Average Rating:

AverageRating: Join(Ratings, product => product.Id, rating => rating.ProductId, (product, rating) => rating.Score).Average() calculates the average rating for all products within a category.

* 3.Returning the Report:

The result is a list of CategoryRatingSummary objects, showing each category’s average rating.
*/
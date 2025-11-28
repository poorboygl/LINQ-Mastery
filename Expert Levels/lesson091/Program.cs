using System.Collections.Concurrent;

public class Program
{
    static void Main()
    {
        var repo = new PerformanceRepository();

        var topEmployees = repo.GetTopPerformingEmployees_Parallel();

        Console.WriteLine("=== Top Performing Employees ===");
        foreach (var emp in topEmployees)
        {
            Console.WriteLine($"Name: {emp.EmployeeName}");
            Console.WriteLine($"Average Rating: {emp.AverageRating:F2}");
            Console.WriteLine($"Projects Rated: {emp.ProjectCount}");
            Console.WriteLine();
        }

        Console.ReadKey();
    }
}

public class Employee
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class Project
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class ProjectRating
{
    public int EmployeeId { get; set; }
    public int ProjectId { get; set; }
    public int Rating { get; set; } // Rating from 1 to 5
}

public class TopEmployeeSummary
{
    public required string EmployeeName { get; set; }
    public double AverageRating { get; set; }
    public int ProjectCount { get; set; }
}

public class PerformanceRepository
{
    public List<Employee> Employees { get; set; } =
    [
        new Employee { Id = 1, Name = "Alice" },
        new Employee { Id = 2, Name = "Bob" },
        new Employee { Id = 3, Name = "Charlie" }
    ];

    public List<Project> Projects { get; set; } =
    [
        new Project { Id = 1, Name = "Project A" },
        new Project { Id = 2, Name = "Project B" },
        new Project { Id = 3, Name = "Project C" }
    ];

    public List<ProjectRating> ProjectRatings { get; set; } =
    [
        new ProjectRating { EmployeeId = 1, ProjectId = 1, Rating = 5 },
        new ProjectRating { EmployeeId = 1, ProjectId = 2, Rating = 4 },
        new ProjectRating { EmployeeId = 1, ProjectId = 3, Rating = 5 },
        new ProjectRating { EmployeeId = 2, ProjectId = 1, Rating = 3 },
        new ProjectRating { EmployeeId = 2, ProjectId = 2, Rating = 4 },
        new ProjectRating { EmployeeId = 3, ProjectId = 1, Rating = 5 },
        new ProjectRating { EmployeeId = 3, ProjectId = 2, Rating = 5 },
        new ProjectRating { EmployeeId = 3, ProjectId = 3, Rating = 4 }
    ];

    public List<TopEmployeeSummary> GetTopPerformingEmployees()
    {
        var topPerformers = ProjectRatings
            .GroupBy(pr => pr.EmployeeId)
            .Select(g => new
            {
                EmployeeId = g.Key,
                AverageRating = g.Average(pr => pr.Rating),
                ProjectCount = g.Count()
            })
            .OrderByDescending(e => e.AverageRating)
            .Take(3)
            .Join(Employees,
                  e => e.EmployeeId,
                  emp => emp.Id,
                  (e, emp) => new TopEmployeeSummary
                  {
                      EmployeeName = emp.Name,
                      AverageRating = e.AverageRating,
                      ProjectCount = e.ProjectCount
                  })
            .ToList();

        return topPerformers;
    }

    public List<TopEmployeeSummary> GetTopPerformingEmployees_Optimized()
    {
        // 1) Tạo dictionary để gom rating theo EmployeeId
        var ratingDict = new Dictionary<int, (int totalRating, int count)>();

        foreach (var pr in ProjectRatings)
        {
            if (!ratingDict.ContainsKey(pr.EmployeeId))
            {
                ratingDict[pr.EmployeeId] = (pr.Rating, 1);
            }
            else
            {
                var current = ratingDict[pr.EmployeeId];
                ratingDict[pr.EmployeeId] = (current.totalRating + pr.Rating, current.count + 1);
            }
        }

        // 2) Cache Employees vào dictionary để lookup nhanh
        var employeeDict = Employees.ToDictionary(e => e.Id);

        // 3) Convert về summary list
        var results = ratingDict
            .Select(kv =>
            {
                var employee = employeeDict[kv.Key];

                double average = (double)kv.Value.totalRating / kv.Value.count;

                return new TopEmployeeSummary
                {
                    EmployeeName = employee.Name,
                    AverageRating = average,
                    ProjectCount = kv.Value.count
                };
            })
            .OrderByDescending(e => e.AverageRating)
            .Take(3)
            .ToList();

        return results;
    }

    public List<TopEmployeeSummary> GetTopPerformingEmployees_Parallel()
    {
        // 1) Gom rating theo EmployeeId bằng thread-safe dictionary
        var ratingDict = new ConcurrentDictionary<int, (int totalRating, int count)>();

        Parallel.ForEach(ProjectRatings, pr =>
        {
            ratingDict.AddOrUpdate(
                pr.EmployeeId,
                (pr.Rating, 1), // nếu chưa có
                (key, oldValue) => (oldValue.totalRating + pr.Rating, oldValue.count + 1)
            );
        });

        // 2) Cache employees vào dictionary để lookup nhanh
        var employeeDict = Employees.ToDictionary(e => e.Id);

        // 3) Convert kết quả
        var results = ratingDict
            .Select(kv =>
            {
                var employee = employeeDict[kv.Key];

                double avg = (double)kv.Value.totalRating / kv.Value.count;

                return new TopEmployeeSummary
                {
                    EmployeeName = employee.Name,
                    AverageRating = avg,
                    ProjectCount = kv.Value.count
                };
            })
            .OrderByDescending(e => e.AverageRating)
            .Take(3)
            .ToList();

        return results;
    }

    public List<TopEmployeeSummary> GetTopPerformingEmployees_PLINQ()
    {
        // 1) Group và aggregate song song bằng PLINQ
        var aggregated = ProjectRatings
            .AsParallel()
            .GroupBy(pr => pr.EmployeeId)
            .Select(g => new
            {
                EmployeeId = g.Key,
                TotalRating = g.Sum(pr => pr.Rating),
                Count = g.Count()
            })
            .ToList();

        // 2) Cache employees để lookup nhanh
        var employeeDict = Employees.ToDictionary(e => e.Id);

        // 3) Map sang summary + sắp xếp
        var results = aggregated
            .Select(a =>
            {
                var emp = employeeDict[a.EmployeeId];
                double average = (double)a.TotalRating / a.Count;

                return new TopEmployeeSummary
                {
                    EmployeeName = emp.Name,
                    AverageRating = average,
                    ProjectCount = a.Count
                };
            })
            .OrderByDescending(e => e.AverageRating)
            .Take(3)
            .ToList();

        return results;
    }
}

/*
 !=== Top Performing Employees ===
    Name: Alice
    Average Rating: 4.67
    Projects Rated: 3

    Name: Charlie
    Average Rating: 4.67
    Projects Rated: 3

    Name: Bob
    Average Rating: 3.50
    Projects Rated: 2
 */


/*
 !This exercise identifies the top-performing employees based on their average project rating.

* 1.Grouping by Employee:

GroupBy(pr => pr.EmployeeId) groups project ratings by each employee.

* 2.Calculating Average Rating and Project Count:

Average(pr => pr.Rating) calculates each employee’s average rating across all projects, and Count() gets the number of projects rated for each employee.

* 3.Selecting Top Performers:

OrderByDescending(e => e.AverageRating).Take(3) selects the top 3 employees with the highest average ratings.

* 4.Returning the Summary:

The result is a list of TopEmployeeSummary objects, sorted by AverageRating in descending order.
 
 */
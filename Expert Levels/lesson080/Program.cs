public class Program
{
    static void Main()
    {
        var repo = new OptimizationRepository();

        var minimalEmployees = repo.GetMinimalEmployeeSetForCoverage_Optimized();

        Console.WriteLine("=== Minimal Employee Set For Project Coverage ===\n");

        foreach (var summary in minimalEmployees)
        {
            Console.WriteLine($"Employee: {summary.EmployeeName}");
            Console.WriteLine("  Covers Projects: " + string.Join(", ", summary.ProjectsCovered));
            Console.WriteLine();
        }

        Console.ReadKey();
    }
}

public class Project
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class Employee
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required List<int> ProjectsContributedTo { get; set; }
}

public class ProjectDependency
{
    public int ProjectId1 { get; set; }
    public int ProjectId2 { get; set; }
}

public class EmployeeCoverageSummary
{
    public required string EmployeeName { get; set; }
    public required List<int> ProjectsCovered { get; set; }
}

public class OptimizationRepository
{
    public List<Project> Projects { get; set; } =
    [
        new Project { Id = 1, Name = "Alpha" },
        new Project { Id = 2, Name = "Beta" },
        new Project { Id = 3, Name = "Gamma" },
        new Project { Id = 4, Name = "Delta" }
    ];

    public List<Employee> Employees { get; set; } =
    [
        new Employee { Id = 1, Name = "Alice", ProjectsContributedTo = [1, 2] },
        new Employee { Id = 2, Name = "Bob", ProjectsContributedTo = [2, 3] },
        new Employee { Id = 3, Name = "Charlie", ProjectsContributedTo = [3, 4] },
        new Employee { Id = 4, Name = "Diana", ProjectsContributedTo = [1, 4] }
    ];

    public List<ProjectDependency> ProjectDependencies { get; set; } =
    [
        new ProjectDependency { ProjectId1 = 1, ProjectId2 = 2 },
        new ProjectDependency { ProjectId1 = 2, ProjectId2 = 3 },
        new ProjectDependency { ProjectId1 = 3, ProjectId2 = 4 },
        new ProjectDependency { ProjectId1 = 1, ProjectId2 = 4 }
    ];

    public List<EmployeeCoverageSummary> GetMinimalEmployeeSetForCoverage()
    {
        /*
         !=== Minimal Employee Set For Project Coverage ===

            Employee: Alice
              Covers Projects: 1, 2

            Employee: Charlie
              Covers Projects: 3, 4 
         */

        //Chuyển ProjectDependency to int
        var requiredProjects = ProjectDependencies
            .SelectMany(dep => new[] { dep.ProjectId1, dep.ProjectId2 })
            .ToHashSet();

        var minimalSet = new List<EmployeeCoverageSummary>();

        var uncoveredProjects = new HashSet<int>(requiredProjects);

        while (uncoveredProjects.Count > 0)
        {
            var bestEmployee = Employees
                .Select(employee => new
                {
                    Employee = employee,
                    CoveredProjects = employee.ProjectsContributedTo
                        .Intersect(uncoveredProjects)
                        .ToList()
                })
                .OrderByDescending(e => e.CoveredProjects.Count)
                .FirstOrDefault(e => e.CoveredProjects.Count > 0);

            if (bestEmployee == null)
                break;

            uncoveredProjects.ExceptWith(bestEmployee.CoveredProjects);

            minimalSet.Add(new EmployeeCoverageSummary
            {
                EmployeeName = bestEmployee.Employee.Name,
                ProjectsCovered = bestEmployee.CoveredProjects
            });
        }

        return minimalSet
            .OrderByDescending(summary => summary.ProjectsCovered.Count)
            .ToList();
    }

    public List<EmployeeCoverageSummary> GetMinimalEmployeeSetForCoverage_Optimized()
    {
        // 1. Lấy danh sách project cần cover
        var requiredProjects = ProjectDependencies
            .SelectMany(dep => new[] { dep.ProjectId1, dep.ProjectId2 })
            .ToHashSet();

        // 2. Convert ProjectsContributedTo → HashSet để tra cứu nhanh
        var employeeProjectSets = Employees.ToDictionary(
            e => e,
            e => e.ProjectsContributedTo.ToHashSet()
        );

        // 3. Kết quả cuối
        var result = new List<EmployeeCoverageSummary>();

        // 4. Tập project chưa cover
        var uncovered = new HashSet<int>(requiredProjects);

        while (uncovered.Count > 0)
        {
            Employee? bestEmployee = null;
            int bestCoverCount = 0;
            List<int>? bestCoveredProjects = null;

            // 5. Tìm employee cover nhiều nhất mà không sort
            foreach (var kvp in employeeProjectSets)
            {
                var employee = kvp.Key;
                var projectSet = kvp.Value;

                // Lấy các project employee này cover còn chưa được cover
                var covered = projectSet.Where(p => uncovered.Contains(p)).ToList();

                if (covered.Count > bestCoverCount)
                {
                    bestCoverCount = covered.Count;
                    bestEmployee = employee;
                    bestCoveredProjects = covered;
                }
            }

            if (bestEmployee == null)
                break;

            // 6. Remove các project đã cover
            foreach (var p in bestCoveredProjects!)
                uncovered.Remove(p);

            // 7. Lưu vào kết quả
            result.Add(new EmployeeCoverageSummary
            {
                EmployeeName = bestEmployee.Name,
                ProjectsCovered = bestCoveredProjects
            });
        }

        return result
            .OrderByDescending(r => r.ProjectsCovered.Count)
            .ToList();
    }

    public List<EmployeeCoverageSummary> GetMinimalEmployeeSetForCoverage_Optimized_2()
    {
        // 1) Lấy tất cả project cần cover từ Dependency
        var requiredProjects = ProjectDependencies
            .SelectMany(dep => new[] { dep.ProjectId1, dep.ProjectId2 })
            .ToHashSet();

        var uncoveredProjects = new HashSet<int>(requiredProjects);
        var minimalSet = new List<EmployeeCoverageSummary>();

        // 2) Precompute: Convert ProjectsContributedTo to HashSet
        //    This avoids repeated ToList() and Intersect() cost.
        var employeeCoverage = Employees
            .Select(e => new
            {
                Employee = e,
                ProjectSet = e.ProjectsContributedTo.ToHashSet()
            })
            .ToList();

        // 3) Main greedy loop
        while (uncoveredProjects.Count > 0)
        {
            int maxCoverage = 0;
            var best = null as (Employee employee, List<int> covers)?;

            foreach (var item in employeeCoverage)
            {
                // Compute coverage manually → faster than Intersect LINQ
                var covers = item.ProjectSet.Where(p => uncoveredProjects.Contains(p)).ToList();

                if (covers.Count > maxCoverage)
                {
                    maxCoverage = covers.Count;
                    best = (item.Employee, covers);
                }
            }

            if (best == null)
                break;

            // Remove covered projects
            foreach (var p in best.Value.covers)
                uncoveredProjects.Remove(p);

            minimalSet.Add(new EmployeeCoverageSummary
            {
                EmployeeName = best.Value.employee.Name,
                ProjectsCovered = best.Value.covers
            });
        }

        return minimalSet
            .OrderByDescending(x => x.ProjectsCovered.Count)
            .ToList();
    }
}

/*
   !This exercise generates a report of the minimum set of employees required to cover all projects with dependencies.

    * 1.Identify Required Projects:

    SelectMany(dep => new[] { dep.ProjectId1, dep.ProjectId2 }).Distinct() gathers a distinct list of all projects involved in dependencies.

    * 2.Determine Minimal Coverage:

    In each iteration, the employee covering the most uncovered projects is added to the minimal set. The uncoveredProjects set is updated by removing projects covered by the selected employee.

    * 3.Return the Report:

    The result is a list of EmployeeCoverageSummary objects, showing the minimal set of employees required to cover all dependencies.
 */
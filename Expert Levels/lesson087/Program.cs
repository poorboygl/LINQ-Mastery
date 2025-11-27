public class Program
{
    static void Main()
    {
        var repo = new EmployeeRepository();

        Console.WriteLine("=== Top Employee Contributors ===\n");

        var contributors = repo.GetTopContributors();

        foreach (var c in contributors)
        {
            Console.WriteLine($"Employee: {c.EmployeeName}");
            Console.WriteLine($"  Department: {c.DepartmentName}");
            Console.WriteLine($"  Projects Contributed: {c.ProjectCount}");
            Console.WriteLine();
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

public class ProjectAssignment
{
    public int EmployeeId { get; set; }
    public int ProjectId { get; set; }
}

public class EmployeeContributionSummary
{
    public required string EmployeeName { get; set; }
    public required string DepartmentName { get; set; }
    public int ProjectCount { get; set; }
}

public class EmployeeRepository
{
    public List<Department> Departments { get; set; } =
    [
        new Department { Id = 1, Name = "Engineering" },
        new Department { Id = 2, Name = "Marketing" },
        new Department { Id = 3, Name = "Sales" }
    ];

    public List<Employee> Employees { get; set; } =
    [
        new Employee { Id = 1, Name = "Alice", DepartmentId = 1 },
        new Employee { Id = 2, Name = "Bob", DepartmentId = 1 },
        new Employee { Id = 3, Name = "Charlie", DepartmentId = 2 },
        new Employee { Id = 4, Name = "Diana", DepartmentId = 3 }
    ];

    public List<ProjectAssignment> ProjectAssignments { get; set; } =
    [
        new ProjectAssignment { EmployeeId = 1, ProjectId = 101 },
        new ProjectAssignment { EmployeeId = 1, ProjectId = 102 },
        new ProjectAssignment { EmployeeId = 2, ProjectId = 101 },
        new ProjectAssignment { EmployeeId = 2, ProjectId = 103 },
        new ProjectAssignment { EmployeeId = 3, ProjectId = 201 },
        new ProjectAssignment { EmployeeId = 3, ProjectId = 202 },
        new ProjectAssignment { EmployeeId = 4, ProjectId = 301 },
        new ProjectAssignment { EmployeeId = 1, ProjectId = 103 }
    ];

    public List<EmployeeContributionSummary> GetTopContributors()
    {
        var topContributors = ProjectAssignments
            .GroupBy(pa => pa.EmployeeId)
            .Select(g => new
            {
                EmployeeId = g.Key,
                ProjectCount = g.Select(pa => pa.ProjectId).Distinct().Count()
            })
            .OrderByDescending(e => e.ProjectCount)
            .Take(5)
            .Join(Employees,
                  e => e.EmployeeId,
                  emp => emp.Id,
                  (e, emp) => new
                  {
                      EmployeeName = emp.Name,
                      DepartmentId = emp.DepartmentId,
                      ProjectCount = e.ProjectCount
                  })
            .Join(Departments,
                  e => e.DepartmentId,
                  dept => dept.Id,
                  (e, dept) => new EmployeeContributionSummary
                  {
                      EmployeeName = e.EmployeeName,
                      DepartmentName = dept.Name,
                      ProjectCount = e.ProjectCount
                  })
            .ToList();

        return topContributors;
    }

    public List<EmployeeContributionSummary> GetTopContributors_Optimized()
    {
        // Dictionary để đếm distinct project mỗi employee
        var projectMap = new Dictionary<int, HashSet<int>>();

        foreach (var pa in ProjectAssignments)
        {
            if (!projectMap.ContainsKey(pa.EmployeeId))
            {
                projectMap[pa.EmployeeId] = new HashSet<int>();
            }

            projectMap[pa.EmployeeId].Add(pa.ProjectId); // HashSet => đảm bảo distinct
        }

        // Tạo dictionary để lookup nhanh employee và department
        var employeeMap = Employees.ToDictionary(e => e.Id);
        var departmentMap = Departments.ToDictionary(d => d.Id);

        // Tạo danh sách kết quả
        var results = projectMap
            .Select(entry =>
            {
                var employeeId = entry.Key;
                var distinctProjectCount = entry.Value.Count;

                var emp = employeeMap[employeeId];
                var dept = departmentMap[emp.DepartmentId];

                return new EmployeeContributionSummary
                {
                    EmployeeName = emp.Name,
                    DepartmentName = dept.Name,
                    ProjectCount = distinctProjectCount
                };
            })
            .OrderByDescending(e => e.ProjectCount)
            .Take(5)
            .ToList();

        return results;
    }
}

/*
 !=== Top Employee Contributors ===

Employee: Alice
  Department: Engineering
  Projects Contributed: 3

Employee: Bob
  Department: Engineering
  Projects Contributed: 2

Employee: Charlie
  Department: Marketing
  Projects Contributed: 2

Employee: Diana
  Department: Sales
  Projects Contributed: 1
 
 */

/*
!This exercise identifies the employees who have contributed to the most projects.

* 1.Grouping by Employee:

GroupBy(pa => pa.EmployeeId) groups project assignments by each employee.

* 2.Counting Unique Projects:

Select(g => g.ProjectId).Distinct().Count() calculates the number of unique projects each employee has contributed to.

* 3.Selecting Top Contributors:

OrderByDescending(e => e.ProjectCount).Take(5) selects the top 5 contributors.

* 4.Returning the Summary:

The result is a list of EmployeeContributionSummary objects, including each employee's name, department, and project count.
 
 */
class Program
{
    static void Main()
    {
        var repo = new ContributionRepository();
        var topContributors = repo.GetTopContributorsByDepartment();

        Console.WriteLine("=== TOP CONTRIBUTORS BY DEPARTMENT ===\n");

        foreach (var summary in topContributors)
        {
            Console.WriteLine($"Department: {summary.DepartmentName}");
            Console.WriteLine($"  Employee: {summary.EmployeeName}");
            Console.WriteLine($"  Total Contribution Hours: {summary.TotalContributionHours}\n");
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

public class ProjectContribution
{
    public int EmployeeId { get; set; }
    public int ProjectId { get; set; }
    public int ContributionHours { get; set; }
}

public class TopContributorSummary
{
    public required string DepartmentName { get; set; }
    public required string EmployeeName { get; set; }
    public int TotalContributionHours { get; set; }
}

public class ContributionRepository
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

    public List<ProjectContribution> ProjectContributions { get; set; } =
    [
        new ProjectContribution { EmployeeId = 1, ProjectId = 101, ContributionHours = 20 },
        new ProjectContribution { EmployeeId = 1, ProjectId = 102, ContributionHours = 15 },
        new ProjectContribution { EmployeeId = 2, ProjectId = 101, ContributionHours = 10 },
        new ProjectContribution { EmployeeId = 3, ProjectId = 201, ContributionHours = 25 },
        new ProjectContribution { EmployeeId = 4, ProjectId = 301, ContributionHours = 30 }
    ];

    public List<TopContributorSummary> GetTopContributorsByDepartment()
    {
        /*
         !=== TOP CONTRIBUTORS BY DEPARTMENT ===

            Department: Engineering
              Employee: Alice
              Total Contribution Hours: 35

            Department: Engineering
              Employee: Bob
              Total Contribution Hours: 10

            Department: Marketing
              Employee: Charlie
              Total Contribution Hours: 25

            Department: Sales
              Employee: Diana
              Total Contribution Hours: 30
        */
        //! go over this code again to understand.
        var topContributors = Employees
            .GroupBy(emp => emp.DepartmentId)
            .SelectMany(group =>
                group.Join(ProjectContributions,
                           emp => emp.Id,
                           contrib => contrib.EmployeeId,
                           (emp, contrib) => new
                           {
                               DepartmentId = emp.DepartmentId,
                               EmployeeName = emp.Name,
                               ContributionHours = contrib.ContributionHours
                           })
                      .GroupBy(e => e.EmployeeName)
                      .Select(e => new
                      {
                          DepartmentId = group.Key,
                          EmployeeName = e.Key,
                          TotalContributionHours = e.Sum(c => c.ContributionHours)
                      })
                      .OrderByDescending(e => e.TotalContributionHours)
                      .Take(3)
            )
            .Join(Departments,
                  e => e.DepartmentId,
                  dept => dept.Id,
                  (e, dept) => new TopContributorSummary
                  {
                      DepartmentName = dept.Name,
                      EmployeeName = e.EmployeeName,
                      TotalContributionHours = e.TotalContributionHours
                  })
            .ToList();

        return topContributors;
    }

    public List<TopContributorSummary> GetTopContributorsByDepartment_2()
    {
        /*
         !=== TOP CONTRIBUTORS BY DEPARTMENT ===

            Department: Engineering
              Employee: Alice
              Total Contribution Hours: 35

            Department: Engineering
              Employee: Bob
              Total Contribution Hours: 10

            Department: Marketing
              Employee: Charlie
              Total Contribution Hours: 25

            Department: Sales
              Employee: Diana
              Total Contribution Hours: 30
         */
        // trước khi  SelectMany flatten
        /*
        [
            { DepartmentName = "Engineering", EmployeeName = "Alice", TotalContributionHours = 35 },
            { DepartmentName = "Engineering", EmployeeName = "Bob", TotalContributionHours = 10 }
        ],
        [
            { DepartmentName = "Marketing", EmployeeName = "Charlie", TotalContributionHours = 25 }
        ],
        [
            { DepartmentName = "Sales", EmployeeName = "Diana", TotalContributionHours = 30 }
        ]     
         */

        /*
          [
            [ Alice, Bob ],       // Engineering
            [ Charlie ],          // Marketing
            [ Diana ]             // Sales
          ]
                    ==============>>
            [
              Alice, Bob, Charlie, Diana
            ]
         */
        var result = Departments
                        .SelectMany(department =>
                        {
                            // Nhân viên Phân ra trong phòng ban sử dụng Where
                            var employeesInDept = Employees
                                .Where(e => e.DepartmentId == department.Id);


                            // Tính tổng giờ của từng nhân viên
                            var contributors = employeesInDept
                                .Select(emp => new TopContributorSummary
                                {
                                    DepartmentName = department.Name,
                                    EmployeeName = emp.Name,
                                    TotalContributionHours = ProjectContributions
                                        .Where(p => p.EmployeeId == emp.Id)
                                        .Sum(p => p.ContributionHours)
                                })
                                .OrderByDescending(x => x.TotalContributionHours); // sort từng phòng ban


                            return contributors;
                        })
                        .ToList();

        return result;
    }
}

/*
 !This exercise identifies the top 3 contributors by department based on total project contribution hours.

* 1.Grouping by Department:

GroupBy(emp => emp.DepartmentId) groups employees by their department.

* 2.Joining with Contributions:

For each department group, employees are joined with ProjectContributions to sum up their total contribution hours.

* 3.Selecting the Top 3 Contributors:

OrderByDescending(e => e.TotalContributionHours).Take(3) selects the top contributors for each department.

* 4.Returning the Summary:

The result is a list of TopContributorSummary objects, showing each department’s top 3 contributors.
*/
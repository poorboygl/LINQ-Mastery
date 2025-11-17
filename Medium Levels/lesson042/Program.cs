class Program
{
    static void Main()
    {
        var repo = new ProjectRepository();
        var reports = repo.GetEmployeeProjectReport(DateTime.Now);

        Console.WriteLine("=== EMPLOYEE PROJECT REPORT ===\n");

        foreach (var r in reports)
        {
            Console.WriteLine($"Employee: {r.EmployeeName}");
            Console.WriteLine($"  Total Projects: {r.TotalProjects}");
            Console.WriteLine($"  Overdue Projects: {r.OverdueProjects}");
            Console.WriteLine("  Current Projects:");

            if (r.CurrentProjects.Count == 0)
            {
                Console.WriteLine("    (none)");
            }
            else
            {
                foreach (var name in r.CurrentProjects)
                {
                    Console.WriteLine($"    - {name}");
                }
            }

            Console.WriteLine();
        }

        Console.ReadLine();
    }
}

public class Employee
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<int> ProjectIds { get; set; } = [];
}

public class Project
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTime Deadline { get; set; }
}

public class EmployeeProjectReport
{
    public required string EmployeeName { get; set; }
    public int TotalProjects { get; set; }
    public int OverdueProjects { get; set; }
    public List<string> CurrentProjects { get; set; } = [];
}

public class ProjectRepository
{
    public List<Employee> Employees { get; set; } =
    [
        new Employee { Id = 1, Name = "Alice", ProjectIds = new List<int> { 1, 2, 3 } },
        new Employee { Id = 2, Name = "Bob", ProjectIds = new List<int> { 2, 4 } },
        new Employee { Id = 3, Name = "Charlie", ProjectIds = new List<int> { 3, 5 } }
    ];

    public static int CurrentYear => DateTime.Now.Year;

    public List<Project> Projects { get; set; } =
    [
        new Project { Id = 1, Name = "Project Alpha", Deadline = new DateTime(CurrentYear, 1, 15) },
        new Project { Id = 2, Name = "Project Beta", Deadline = new DateTime(CurrentYear, 12, 10) },
        new Project { Id = 3, Name = "Project Gamma", Deadline = new DateTime(CurrentYear, 11, 5) },
        new Project { Id = 4, Name = "Project Delta", Deadline = new DateTime(CurrentYear, 2, 20) },
        new Project { Id = 5, Name = "Project Epsilon", Deadline = new DateTime(CurrentYear, 10, 30) }
    ];

    public List<EmployeeProjectReport> GetEmployeeProjectReport(DateTime currentDate)
    {
        return [.. Employees
            .SelectMany(emp => emp.ProjectIds
                .Join(Projects, id => id, proj => proj.Id, (id, proj) => new { Employee = emp, Project = proj }))
            .GroupBy(item => item.Employee.Name)
            .Select(group => new EmployeeProjectReport
            {
                EmployeeName = group.Key,
                TotalProjects = group.Count(),
                OverdueProjects = group.Count(p => p.Project.Deadline < currentDate),
                CurrentProjects = [.. group
                    .Where(p => p.Project.Deadline >= currentDate)
                    .Select(p => p.Project.Name)]
            })];
    }

    //public List<EmployeeProjectReport> GetEmployeeProjectReport(DateTime currentDate)
    //{
    //    return Employees
    //        .SelectMany(emp => emp.ProjectIds
    //            .Join(Projects, id => id, proj => proj.Id, (id, proj) => new { Employee = emp, Project = proj }))
    //        .GroupBy(item => item.Employee.Name)
    //        .Select(group => new EmployeeProjectReport
    //        {
    //            EmployeeName = group.Key,
    //            TotalProjects = group.Count(),
    //            OverdueProjects = group.Count(p => p.Project.Deadline < currentDate),
    //            CurrentProjects = group
    //                .Where(p => p.Project.Deadline >= currentDate)
    //                .Select(p => p.Project.Name)
    //                .ToList()
    //        })
    //        .ToList();
    //}

}

/*
 === EMPLOYEE PROJECT REPORT ===

Employee: Alice
  Total Projects: 3
  Overdue Projects: 2
  Current Projects:
    - Project Beta

Employee: Bob
  Total Projects: 2
  Overdue Projects: 1
  Current Projects:
    - Project Beta

Employee: Charlie
  Total Projects: 2
  Overdue Projects: 2
  Current Projects:
    (none)
 
 */
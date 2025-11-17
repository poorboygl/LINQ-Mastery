class Program
{
    static void Main()
    {
        var repo = new ProjectRepository();

        var results = repo.GetEmployeeProjectEligibility();

        Console.WriteLine("=== Employee Project Eligibility ===\n");

        foreach (var r in results)
        {
            Console.WriteLine(
                $"{r.EmployeeName,-10} | Project: {r.ProjectName,-20} | Eligible: {(r.IsEligible ? "YES" : "NO")}");
        }

        Console.ReadKey();
    }
}

public class Skill
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class Employee
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<int> SkillIds { get; set; } = [];
}

public class Project
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<int> RequiredSkillIds { get; set; } = [];
}

public class ProjectEligibility
{
    public required string EmployeeName { get; set; }
    public required string ProjectName { get; set; }
    public bool IsEligible { get; set; }
}

public class ProjectRepository
{
    public List<Employee> Employees { get; set; } =
    [
        new Employee { Id = 1, Name = "Alice", SkillIds = [1, 2, 3] },
        new Employee { Id = 2, Name = "Bob", SkillIds = [1, 4] },
        new Employee { Id = 3, Name = "Charlie", SkillIds = [2, 3] }
    ];

    public List<Skill> Skills { get; set; } =
    [
        new Skill { Id = 1, Name = "C#" },
        new Skill { Id = 2, Name = "SQL" },
        new Skill { Id = 3, Name = "JavaScript" },
        new Skill { Id = 4, Name = "Python" }
    ];

    public List<Project> Projects { get; set; } =
    [
        new Project { Id = 1, Name = "Website Development", RequiredSkillIds = [1, 3] },
        new Project { Id = 2, Name = "Data Analysis", RequiredSkillIds = [2, 4] },
        new Project { Id = 3, Name = "Database Management", RequiredSkillIds = [2] }
    ];

    public List<ProjectEligibility> GetEmployeeProjectEligibility()
    {
        return [.. Employees
            .SelectMany(emp => Projects.Select(proj => new ProjectEligibility
            {
                EmployeeName = emp.Name,
                ProjectName = proj.Name,
                IsEligible = proj.RequiredSkillIds.All(skillId => emp.SkillIds.Contains(skillId))
            }))];
    }

    //public List<ProjectEligibility> GetEmployeeProjectEligibility()
    //{
    //    var list = new List<ProjectEligibility>();

    //    foreach (var emp in Employees)
    //    {
    //        foreach (var proj in Projects)
    //        {
    //            var elig = new ProjectEligibility
    //            {
    //                EmployeeName = emp.Name,
    //                ProjectName = proj.Name,
    //                IsEligible = proj.RequiredSkillIds.All(skill => emp.SkillIds.Contains(skill))
    //            };

    //            list.Add(elig);
    //        }
    //    }

    //    return list;
    //}

}

/*
 === Employee Project Eligibility ===

Alice      | Project: Website Development  | Eligible: YES
Alice      | Project: Data Analysis        | Eligible: NO
Alice      | Project: Database Management  | Eligible: YES
Bob        | Project: Website Development  | Eligible: NO
Bob        | Project: Data Analysis        | Eligible: NO
Bob        | Project: Database Management  | Eligible: NO
Charlie    | Project: Website Development  | Eligible: NO
Charlie    | Project: Data Analysis        | Eligible: NO
Charlie    | Project: Database Management  | Eligible: YES
 */


/*
In this exercise, you group orders by customer and calculate a summary for each customer.

* 1.Grouping Orders by Customer:

GroupJoin(Orders, customer => customer.Id, order => order.CustomerId, ...) associates each customer with their respective orders.

* 2.Calculating Order Summary Data:

TotalOrders: Counts all orders for the customer.

TotalSpent: Sums the order amounts for each customer.

LastOrderDate: Finds the most recent order date with Max.

AverageOrderAmount: Calculates the average order amount.

* 3.Sorting the Results:

OrderByDescending(summary => summary.TotalSpent) sorts customers by the total amount spent in descending order.
 */
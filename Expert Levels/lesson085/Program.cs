public class Program
{
    static void Main()
    {
        var repo = new DependencyRepository();

        Console.WriteLine("=== Deepest Dependency Chain ===\n");

        var chain = repo.GetDeepestDependencyChain();

        foreach (var step in chain)
        {
            Console.WriteLine($"Department: {step.DepartmentName} | Team: {step.TeamName} -> {step.DependentTeamName} | Level: {step.DependencyLevel}");
        }

        Console.ReadKey();
    }
}

public class Department
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class Team
{
    public int Id { get; set; }
    public int DepartmentId { get; set; }
    public required string Name { get; set; }
}

public class InterDependency
{
    public int TeamId { get; set; }
    public int DependentTeamId { get; set; }
    public int DependencyLevel { get; set; } // Level of dependency
}

public class DependencyChainSummary
{
    public required string DepartmentName { get; set; }
    public required string TeamName { get; set; }
    public required string DependentTeamName { get; set; }
    public int DependencyLevel { get; set; }
}

public class DependencyRepository
{
    public List<Department> Departments { get; set; } =
    [
        new Department { Id = 1, Name = "Engineering" },
        new Department { Id = 2, Name = "Marketing" },
        new Department { Id = 3, Name = "Sales" }
    ];

    public List<Team> Teams { get; set; } =
    [
        new Team { Id = 1, DepartmentId = 1, Name = "Backend" },
        new Team { Id = 2, DepartmentId = 1, Name = "Frontend" },
        new Team { Id = 3, DepartmentId = 2, Name = "SEO" },
        new Team { Id = 4, DepartmentId = 3, Name = "Sales Ops" }
    ];

    public List<InterDependency> InterDependencies { get; set; } =
    [
        new InterDependency { TeamId = 1, DependentTeamId = 2, DependencyLevel = 1 },
        new InterDependency { TeamId = 2, DependentTeamId = 3, DependencyLevel = 2 },
        new InterDependency { TeamId = 3, DependentTeamId = 4, DependencyLevel = 3 },
        new InterDependency { TeamId = 4, DependentTeamId = 1, DependencyLevel = 4 } // Circular dependency
    ];

    public List<DependencyChainSummary> GetDeepestDependencyChain()
    {
        var dependencyChains = new List<List<int>>();

        foreach (var startTeam in Teams)
        {
            var chain = new List<int> { startTeam.Id };
            var currentTeamId = startTeam.Id;

            while (InterDependencies.Any(dep => dep.TeamId == currentTeamId &&
                                                !chain.Contains(dep.DependentTeamId)))
            {
                var nextDependency = InterDependencies.First(dep => dep.TeamId == currentTeamId &&
                                                                    !chain.Contains(dep.DependentTeamId));
                currentTeamId = nextDependency.DependentTeamId;
                chain.Add(currentTeamId);
            }

            dependencyChains.Add(chain);
        }

        var deepestChain = dependencyChains
            .OrderByDescending(chain => chain.Count)
            .ThenByDescending(chain => chain.Sum(teamId => InterDependencies
                .Where(dep => dep.TeamId == teamId)
                .Sum(dep => dep.DependencyLevel)))
            .First();

        return deepestChain
            .Zip(deepestChain.Skip(1), (teamId, dependentTeamId) =>
            {
                var dependency = InterDependencies.First(dep => dep.TeamId == teamId && dep.DependentTeamId == dependentTeamId);
                var team = Teams.First(t => t.Id == teamId);
                var dependentTeam = Teams.First(t => t.Id == dependentTeamId);
                var department = Departments.First(d => d.Id == team.DepartmentId);
                return new DependencyChainSummary
                {
                    DepartmentName = department.Name,
                    TeamName = team.Name,
                    DependentTeamName = dependentTeam.Name,
                    DependencyLevel = dependency.DependencyLevel
                };
            })
            .ToList();
    }

}

/*
!=== Deepest Dependency Chain ===

    Department: Engineering | Team: Backend -> Frontend | Level: 1
    Department: Engineering | Team: Frontend -> SEO | Level: 2
    Department: Marketing | Team: SEO -> Sales Ops | Level: 3
 */

/*
!This exercise identifies the deepest multi-level dependency chain across teams and departments by analyzing sequential dependencies and depth levels.

* 1.Building Dependency Chains:

For each starting team, a chain is constructed by recursively finding teams they depend on, avoiding cycles by ensuring no team appears more than once.

* 2.Identifying the Deepest Chain:

Chains are sorted first by length and then by cumulative dependency level to prioritize the most complex and deepest chain.

* 3.Returning the Deepest Chain with Dependency Levels:

The result is a list of DependencyChainSummary objects, showing each department, team, dependent team, and dependency level in sequence.
 */
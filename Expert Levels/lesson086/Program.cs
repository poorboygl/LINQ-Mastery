public class Program
{
    static void Main()
    {
        var repo = new BottleneckRepository();

        Console.WriteLine("=== Critical Bottlenecks ===\n");

        var bottlenecks = repo.GetCriticalBottlenecks_Optimized();

        foreach (var b in bottlenecks)
        {
            Console.WriteLine($"Department: {b.DepartmentName} | Team: {b.TeamName} | Impacted Teams: {b.ImpactedTeamsCount}");
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

public class TeamDependency
{
    public int TeamId { get; set; }
    public int DependentTeamId { get; set; }
}

public class BottleneckImpactSummary
{
    public required string TeamName { get; set; }
    public required string DepartmentName { get; set; }
    public int ImpactedTeamsCount { get; set; }
}

public class BottleneckRepository
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
        new Team { Id = 4, DepartmentId = 3, Name = "Sales Ops" },
        new Team { Id = 5, DepartmentId = 3, Name = "Customer Support" }
    ];

    public List<TeamDependency> TeamDependencies { get; set; } =
    [
        new TeamDependency { TeamId = 1, DependentTeamId = 2 },
        new TeamDependency { TeamId = 2, DependentTeamId = 3 },
        new TeamDependency { TeamId = 3, DependentTeamId = 4 },
        new TeamDependency { TeamId = 1, DependentTeamId = 4 },
        new TeamDependency { TeamId = 4, DependentTeamId = 5 }
    ];

    public List<BottleneckImpactSummary> GetCriticalBottlenecks()
    {
        var bottleneckImpacts = Teams.Select(team => new BottleneckImpactSummary
        {
            TeamName = team.Name,
            DepartmentName = Departments.First(d => d.Id == team.DepartmentId).Name,
            ImpactedTeamsCount = CalculateImpactedTeams(team.Id, new HashSet<int>())
        })
        .OrderByDescending(summary => summary.ImpactedTeamsCount)
        .ToList();

        return bottleneckImpacts;
    }

    private int CalculateImpactedTeams(int teamId, HashSet<int> visitedTeams)
    {
        if (!visitedTeams.Add(teamId))
            return 0; // Avoid cycles

        var directDependents = TeamDependencies
            .Where(dep => dep.TeamId == teamId)
            .Select(dep => dep.DependentTeamId)
            .ToList();

        int impactCount = directDependents.Count;

        foreach (var dependentTeam in directDependents)
        {
            impactCount += CalculateImpactedTeams(dependentTeam, visitedTeams);
        }

        visitedTeams.Remove(teamId); // Backtrack for other paths

        return impactCount;
    }

    public List<BottleneckImpactSummary> GetCriticalBottlenecks_Optimized()
    {
        var teamLookup = Teams.ToDictionary(t => t.Id, t => t);
        var departmentLookup = Departments.ToDictionary(d => d.Id, d => d);

        // Build adjacency list: TeamId -> list of DependentTeamId
        var adj = TeamDependencies
            .GroupBy(d => d.TeamId)
            .ToDictionary(g => g.Key, g => g.Select(dep => dep.DependentTeamId).ToList());

        // Memoization cache: TeamId -> Impacted count
        var memo = new Dictionary<int, int>();

        int Dfs(int teamId, HashSet<int> visited)
        {
            if (!visited.Add(teamId))
                return 0; // avoid cycles

            if (memo.TryGetValue(teamId, out var cached))
                return cached;

            int impactCount = 0;

            if (adj.TryGetValue(teamId, out var dependents))
            {
                impactCount += dependents.Count; // direct dependents
                foreach (var dep in dependents)
                {
                    impactCount += Dfs(dep, visited);
                }
            }

            visited.Remove(teamId); // backtrack
            memo[teamId] = impactCount; // cache result
            return impactCount;
        }

        var bottleneckImpacts = Teams
            .Select(team => new BottleneckImpactSummary
            {
                TeamName = team.Name,
                DepartmentName = departmentLookup[team.DepartmentId].Name,
                ImpactedTeamsCount = Dfs(team.Id, new HashSet<int>())
            })
            .OrderByDescending(summary => summary.ImpactedTeamsCount)
            .ToList();

        return bottleneckImpacts;
    }
}

/*
 !=== Critical Bottlenecks ===

    Department: Engineering | Team: Backend | Impacted Teams: 6
    Department: Engineering | Team: Frontend | Impacted Teams: 3
    Department: Marketing | Team: SEO | Impacted Teams: 2
    Department: Sales | Team: Sales Ops | Impacted Teams: 1
    Department: Sales | Team: Customer Support | Impacted Teams: 0
 
 */

/*
 !This exercise calculates the critical bottlenecks by finding teams with the highest reach across departments in terms of downstream dependencies.

* 1.Calculating Impacted Teams:

For each team, CalculateImpactedTeams uses recursion to find all teams directly or indirectly impacted by a delay in the current team, avoiding cycles by keeping track of visited teams.

* 2.Identifying the Top Bottlenecks:

The bottlenecks are sorted by ImpactedTeamsCount to prioritize teams whose delay would affect the most teams.

* 3.Returning the Bottleneck Summary:

The result is a list of BottleneckImpactSummary objects, showing each team’s bottleneck impact in terms of affected teams across departments.
 
 */
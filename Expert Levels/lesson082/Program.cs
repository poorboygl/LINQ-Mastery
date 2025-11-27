public class Program
{
    static void Main()
    {
        var repo = new TeamDependencyRepository();

        Console.WriteLine("=== LONGEST TEAM DEPENDENCY CHAIN ===\n");

        var chain = repo.GetLongestTeamDependencyChain();

        foreach (var step in chain)
        {
            Console.WriteLine($"{step.TeamName}  -->  {step.DependentTeamName}  (Project {step.ProjectId})");
        }

        Console.WriteLine($"\nTotal chain length: {chain.Count + 1} teams");

        Console.ReadKey();
    }
}

public class Team
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class ProjectDependency
{
    public int TeamId { get; set; }
    public int DependentTeamId { get; set; }
    public int ProjectId { get; set; }
}

public class TeamDependencyChain
{
    public required string TeamName { get; set; }
    public required string DependentTeamName { get; set; }
    public int ProjectId { get; set; }
}

public class TeamDependencyRepository
{
    public List<Team> Teams { get; set; } =
    [
        new() { Id = 1, Name = "Team A" },
        new Team { Id = 2, Name = "Team B" },
        new Team { Id = 3, Name = "Team C" },
        new Team { Id = 4, Name = "Team D" }
    ];

    public List<ProjectDependency> ProjectDependencies { get; set; } =
    [
        new ProjectDependency { TeamId = 1, DependentTeamId = 2, ProjectId = 101 },
        new ProjectDependency { TeamId = 2, DependentTeamId = 3, ProjectId = 102 },
        new ProjectDependency { TeamId = 3, DependentTeamId = 4, ProjectId = 103 },
        new ProjectDependency { TeamId = 4, DependentTeamId = 1, ProjectId = 104 } // Circular dependency
    ];

    public List<TeamDependencyChain> GetLongestTeamDependencyChain()
    {
        var dependencyChains = new List<List<int>>();

        foreach (var startDependency in ProjectDependencies)
        {
            var chain = new List<int> { startDependency.TeamId };
            var currentDependency = startDependency;

            while (ProjectDependencies.Any(d => d.TeamId == currentDependency.DependentTeamId &&
                                                !chain.Contains(d.DependentTeamId)))
            {
                currentDependency = ProjectDependencies.First(d => d.TeamId == currentDependency.DependentTeamId &&
                                                                  !chain.Contains(d.DependentTeamId));
                chain.Add(currentDependency.TeamId);
            }

            dependencyChains.Add(chain);
        }

        var longestChain = dependencyChains.OrderByDescending(chain => chain.Count).First();

        return longestChain
            .Zip(longestChain.Skip(1), (teamId, dependentTeamId) =>
            {
                var projectDependency = ProjectDependencies.First(d => d.TeamId == teamId && d.DependentTeamId == dependentTeamId);
                return new TeamDependencyChain
                {
                    TeamName = Teams.First(t => t.Id == teamId).Name,
                    DependentTeamName = Teams.First(t => t.Id == dependentTeamId).Name,
                    ProjectId = projectDependency.ProjectId
                };
            })
            .ToList();
    }

    public List<TeamDependencyChain> GetLongestTeamDependencyChain_Optimized()
    {
        // Map TeamId -> list of dependencies
        var next = ProjectDependencies
            .GroupBy(d => d.TeamId)
            .ToDictionary(g => g.Key, g => g.ToList());

        // Fast lookup for team names
        var teamName = Teams.ToDictionary(t => t.Id, t => t.Name);

        var longestChain = new List<int>();
        var visitedGlobal = new HashSet<int>();   // avoid recomputing

        foreach (var startTeam in Teams)
        {
            if (visitedGlobal.Contains(startTeam.Id)) continue;

            var path = new List<int>();
            var visitedLocal = new HashSet<int>();

            DFS(startTeam.Id, next, visitedLocal, visitedGlobal, path, ref longestChain);
        }

        // Convert chain to TeamDependencyChain
        return longestChain
            .Zip(longestChain.Skip(1), (teamId, dependentTeamId) =>
            {
                var dep = ProjectDependencies.First(d => d.TeamId == teamId &&
                                                         d.DependentTeamId == dependentTeamId);

                return new TeamDependencyChain
                {
                    TeamName = teamName[teamId],
                    DependentTeamName = teamName[dependentTeamId],
                    ProjectId = dep.ProjectId
                };
            })
            .ToList();
    }

    private void DFS(
        int currentTeam,
        Dictionary<int, List<ProjectDependency>> next,
        HashSet<int> visitedLocal,
        HashSet<int> visitedGlobal,
        List<int> currentPath,
        ref List<int> longestPath)
    {
        if (visitedLocal.Contains(currentTeam))
            return; // cycle detected → stop

        visitedLocal.Add(currentTeam);
        visitedGlobal.Add(currentTeam);
        currentPath.Add(currentTeam);

        if (!next.ContainsKey(currentTeam))
        {
            if (currentPath.Count > longestPath.Count)
                longestPath = new List<int>(currentPath);

            currentPath.RemoveAt(currentPath.Count - 1);
            visitedLocal.Remove(currentTeam);
            return;
        }

        foreach (var dep in next[currentTeam])
        {
            DFS(dep.DependentTeamId, next, visitedLocal, visitedGlobal, currentPath, ref longestPath);
        }

        currentPath.RemoveAt(currentPath.Count - 1);
        visitedLocal.Remove(currentTeam);
    }
}

/*
 !=== LONGEST TEAM DEPENDENCY CHAIN ===

Team A  -->  Team B  (Project 101)
Team B  -->  Team C  (Project 102)

Total chain length: 3 teams
 
*/

/*
 !This exercise generates the longest sequence of team dependencies by following project dependencies across teams.

* 1.Building Dependency Chains:

For each starting dependency, a chain is constructed by iteratively finding teams that depend on the current team, avoiding cycles.

* 2.Identifying the Longest Chain:

OrderByDescending(chain => chain.Count).First() identifies the chain with the maximum number of dependencies.

* 3.Returning the Longest Chain in Sequence:

The result is a list of TeamDependencyChain objects, showing team dependencies and their respective project IDs in sequential order.
 
 */
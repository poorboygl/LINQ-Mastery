public class Program
{
    static void Main()
    {
        var repo = new ProjectRepository();
        var criticalPath = repo.GetCriticalPath();

        Console.WriteLine("=== Critical Path ===");
        foreach (var step in criticalPath)
        {
            Console.WriteLine($"{step.ProjectName}: {step.Duration} days");
        }

        Console.ReadKey();
    }
}

public class Project
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Duration { get; set; } // Duration in days
}

public class ProjectDependency
{
    public int ProjectId { get; set; }
    public int DependentProjectId { get; set; }
}

public class ProjectPathSummary
{
    public required string ProjectName { get; set; }
    public int Duration { get; set; }
}

public class ProjectRepository
{
    public List<Project> Projects { get; set; } =
    [
        new Project { Id = 1, Name = "Alpha", Duration = 10 },
        new Project { Id = 2, Name = "Beta", Duration = 20 },
        new Project { Id = 3, Name = "Gamma", Duration = 15 },
        new Project { Id = 4, Name = "Delta", Duration = 30 }
    ];

    public List<ProjectDependency> ProjectDependencies { get; set; } =
    [
        new ProjectDependency { ProjectId = 1, DependentProjectId = 2 },
        new ProjectDependency { ProjectId = 2, DependentProjectId = 3 },
        new ProjectDependency { ProjectId = 3, DependentProjectId = 4 }
    ];

    public List<ProjectPathSummary> GetCriticalPath()
    {
        var paths = new List<List<int>>();

        foreach (var startProject in Projects.Where(p => !ProjectDependencies.Any(d => d.DependentProjectId == p.Id)))
        {
            var path = new List<int> { startProject.Id };
            var currentProjectId = startProject.Id;

            while (ProjectDependencies.Any(d => d.ProjectId == currentProjectId))
            {
                var nextDependency = ProjectDependencies.First(d => d.ProjectId == currentProjectId);
                currentProjectId = nextDependency.DependentProjectId;
                path.Add(currentProjectId);
            }

            paths.Add(path);
        }

        var criticalPath = paths
            .OrderByDescending(path => path.Sum(projectId => Projects.First(p => p.Id == projectId).Duration))
            .First();

        var totalDuration = criticalPath.Sum(projectId => Projects.First(p => p.Id == projectId).Duration);

        return criticalPath
            .Select(projectId => new ProjectPathSummary
            {
                ProjectName = Projects.First(p => p.Id == projectId).Name,
                Duration = Projects.First(p => p.Id == projectId).Duration
            })
            .Concat(new[] { new ProjectPathSummary { ProjectName = "Total Duration", Duration = totalDuration } })
            .ToList();
    }

    // ============================
    //  TỐI ƯU
    // ============================
    public List<ProjectPathSummary> GetCriticalPath_optimaze()
    {
        // Map project -> next project (dictionary for O(1) lookup)
        var nextProject = ProjectDependencies.ToDictionary(d => d.ProjectId, d => d.DependentProjectId);

        // Map projectId -> Project for O(1)
        var projectMap = Projects.ToDictionary(p => p.Id, p => p);

        // Find starting nodes (projects that are NOT dependentProject)
        var dependentSet = ProjectDependencies.Select(d => d.DependentProjectId).ToHashSet();
        var startProjects = Projects.Where(p => !dependentSet.Contains(p.Id));

        List<int> bestPath = [];
        int bestDuration = 0;

        foreach (var start in startProjects)
        {
            var path = new List<int> { start.Id };
            int current = start.Id;
            int duration = start.Duration;

            while (nextProject.TryGetValue(current, out int next))
            {
                path.Add(next);
                duration += projectMap[next].Duration;
                current = next;
            }

            if (duration > bestDuration)
            {
                bestDuration = duration;
                bestPath = path;
            }
        }

        return bestPath
            .Select(id => new ProjectPathSummary
            {
                ProjectName = projectMap[id].Name,
                Duration = projectMap[id].Duration
            })
            .Append(new ProjectPathSummary
            {
                ProjectName = "Total Duration",
                Duration = bestDuration
            })
            .ToList();
    }
}

/*
   !=== Critical Path ===
    Alpha: 10 days
    Beta: 20 days
    Gamma: 15 days
    Delta: 30 days
    Total Duration: 75 days
 */

/*
 !This exercise generates the critical path by finding the longest sequence of dependencies in terms of duration.

* 1.Building Paths:

For each starting project (one with no dependencies), a path is constructed by iteratively finding dependent projects until no further dependencies are found.

* 2.Identifying the Critical Path:

OrderByDescending(path => path.Sum(projectId => Projects.First(p => p.Id == projectId).Duration)).First() identifies the path with the maximum cumulative duration.

* 3.Returning the Critical Path with Total Duration:

The result is a list of ProjectPathSummary objects, showing each project and a summary of the total path duration.
 
 */
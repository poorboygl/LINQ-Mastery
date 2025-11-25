public class Program
{
    static void Main()
    {
        var repo = new CollaborationRepository();
        var results = repo.GetTopCollaborativeTeams_Optimize();

        Console.WriteLine("=== TEAM COLLABORATION SUMMARY ===");
        foreach (var summary in results)
        {
            Console.WriteLine($"{summary.TeamName} | Collaborations: {summary.CollaborationCount}");
        }

        Console.ReadKey();
    }
}

public class Project
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class Team
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<int> ProjectsWorkedOn { get; set; } = [];
}

public class ProjectDependency
{
    public int ProjectId1 { get; set; }
    public int ProjectId2 { get; set; }
}

public class TeamCollaborationSummary
{
    public required string TeamName { get; set; }
    public int CollaborationCount { get; set; }
}

public class CollaborationRepository
{
    public List<Project> Projects { get; set; } =
    [
        new Project { Id = 1, Name = "Alpha" },
        new Project { Id = 2, Name = "Beta" },
        new Project { Id = 3, Name = "Gamma" },
        new Project { Id = 4, Name = "Delta" },
        new Project { Id = 5, Name = "NguyenVu" },
        new Project { Id = 6, Name = "Wife" },
    ];

    public List<Team> Teams { get; set; } =
    [
        new Team { Id = 1, Name = "Engineering", ProjectsWorkedOn = [1, 2] },
        new Team { Id = 2, Name = "Marketing", ProjectsWorkedOn = [2, 3, 6] },
        new Team { Id = 3, Name = "Sales", ProjectsWorkedOn = [3, 4,6] },
        new Team { Id = 4, Name = "HR", ProjectsWorkedOn = [1, 4] },
        new Team { Id = 5, Name = "Construction", ProjectsWorkedOn = [5] }
    ];

    public List<ProjectDependency> ProjectDependencies { get; set; } =
    [
        new ProjectDependency { ProjectId1 = 1, ProjectId2 = 2 },
        new ProjectDependency { ProjectId1 = 2, ProjectId2 = 3 },
        new ProjectDependency { ProjectId1 = 3, ProjectId2 = 4 },
        new ProjectDependency { ProjectId1 = 1, ProjectId2 = 4 },
        new ProjectDependency { ProjectId1 = 1, ProjectId2 = 5 },
    ];

    public List<TeamCollaborationSummary> GetTopCollaborativeTeams()
    {

        /*
          !=== TEAM COLLABORATION SUMMARY ===
            Engineering | Collaborations: 3
            Marketing | Collaborations: 3
            Sales | Collaborations: 3
            HR | Collaborations: 3
         */

        var result = Teams
                  .Select(team => new TeamCollaborationSummary
                  {
                      TeamName = team.Name,
                      CollaborationCount = ProjectDependencies
                          .Where(dep => team.ProjectsWorkedOn.Contains(dep.ProjectId1) || team.ProjectsWorkedOn.Contains(dep.ProjectId2))
                          .SelectMany(dep => Teams
                              .Where(otherTeam => otherTeam.Id != team.Id &&
                                                  (otherTeam.ProjectsWorkedOn.Contains(dep.ProjectId1) ||
                                                   otherTeam.ProjectsWorkedOn.Contains(dep.ProjectId2))))
                          .Distinct()
                          .Count()
                  })
                  .OrderByDescending(summary => summary.CollaborationCount)
                  .ToList();

        return result;
    }

    public List<TeamCollaborationSummary> GetTopCollaborativeTeams_Optimize()
    {

        /*
          !=== TEAM COLLABORATION SUMMARY ===
            Engineering | Collaborations: 3
            Marketing | Collaborations: 3
            Sales | Collaborations: 3
            HR | Collaborations: 3
         */

        // 1) Chuyển list project của team thành HashSet lookup O(1)
        var teamLookUp = Teams.ToDictionary(
                t => t.Id,
                t => t.ProjectsWorkedOn.ToHashSet()
            );

        // 2) Tạo Inverted Index: ProjectId → List TeamId
        var teamProjects = Teams.SelectMany(t => t.ProjectsWorkedOn.Select(projectId => new { ProjectId = projectId, TeamId = t.Id }))
                                .GroupBy(l => l.ProjectId)
                                .ToDictionary(group => group.Key,
                                            group => group.Select(e => e.TeamId).ToList()
                                );


        // 3) Tính toán collaboration
        var result = Teams.Select(team =>
        {
            var collaborators = new HashSet<int>();

            foreach (var dep in ProjectDependencies)
            {
                // team này có tham gia vào dep nào không?
                if (teamLookUp[team.Id].Contains(dep.ProjectId1)
                    || teamLookUp[team.Id].Contains(dep.ProjectId2))
                {
                    // Tìm các team còn lại
                    if (teamProjects.TryGetValue(dep.ProjectId1, out var team01))
                        foreach (var otherId in team01)
                            if (otherId != team.Id) collaborators.Add(otherId);

                    if (teamProjects.TryGetValue(dep.ProjectId2, out var team02))
                        foreach (var otherId in team02)
                            if (otherId != team.Id) collaborators.Add(otherId);
                }
            }

            return new TeamCollaborationSummary
            {
                TeamName = team.Name,
                CollaborationCount = collaborators.Count
            };
        })
        .OrderByDescending(x => x.CollaborationCount)
        .ToList();

        return result;
    }
}

/*
!This exercise generates a report showing each team’s number of unique collaboration connections based on shared project dependencies.

    * 1.Filtering Dependencies by Team Projects:

    ProjectDependencies.Where(dep => team.ProjectsWorkedOn.Contains(dep.ProjectId1) || team.ProjectsWorkedOn.Contains(dep.ProjectId2)) filters dependencies that involve projects the team worked on.

    * 2.Calculating Collaboration Connections:

    CollaborationCount: SelectMany and Distinct are used to count unique connections with other teams on shared dependencies.

    * 3.Returning the Report:

    The result is a list of TeamCollaborationSummary objects, showing each team’s collaboration count.
 */
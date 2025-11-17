class Program
{
    static void Main()
    {
        var repo = new ProjectRepository();

        //Check
        //List<object> test = [];
        //repo.GetAllTaskByTeam(out test);

        var summaries = repo.GetTaskCompletionByTeam();

        Console.WriteLine("Team Task Completion Summary:\n");

        foreach (var summary in summaries)
        {
            Console.WriteLine($"Team: {summary.TeamName}");
            Console.WriteLine($"  Total Tasks       : {summary.TotalTasks}");
            Console.WriteLine($"  Completed Tasks   : {summary.CompletedTasks}");
            Console.WriteLine($"  Completion %      : {summary.CompletionPercentage:F2}%");
            Console.WriteLine();
        }

        Console.ReadKey();
    }
}


public class Team
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class Project
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int TeamId { get; set; }
}

public class Task
{
    public int ProjectId { get; set; }
    public required string Description { get; set; }
    public bool IsCompleted { get; set; }
}

public class TeamTaskCompletionSummary
{
    public required string TeamName { get; set; }
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public double CompletionPercentage { get; set; }
}

public class ProjectRepository
{
    public List<Team> Teams { get; set; } =
    [
        new Team { Id = 1, Name = "Alpha Team" },
        new Team { Id = 2, Name = "Beta Team" }
    ];

    public List<Project> Projects { get; set; } =
    [
        new Project { Id = 1, Name = "Project A", TeamId = 1 },
        new Project { Id = 2, Name = "Project B", TeamId = 1 },
        new Project { Id = 3, Name = "Project C", TeamId = 2 }
    ];

    public List<Task> Tasks { get; set; } =
    [
        new Task { ProjectId = 1, Description = "Task 1", IsCompleted = true },
        new Task { ProjectId = 1, Description = "Task 2", IsCompleted = false },
        new Task { ProjectId = 2, Description = "Task 3", IsCompleted = true },
        new Task { ProjectId = 3, Description = "Task 4", IsCompleted = false },
        new Task { ProjectId = 3, Description = "Task 5", IsCompleted = true }
    ];

    public List<TeamTaskCompletionSummary> GetTaskCompletionByTeam()
    {
        return [.. Teams
            .GroupJoin(Projects,
                team => team.Id,
                project => project.TeamId,
                (team, teamProjects) => new
                {
                    TeamName = team.Name,
                    Tasks = teamProjects.SelectMany(project => Tasks.Where(task => task.ProjectId == project.Id))
                })
            .Select(group => new TeamTaskCompletionSummary
            {
                TeamName = group.TeamName,
                TotalTasks = group.Tasks.Count(),
                CompletedTasks = group.Tasks.Count(task => task.IsCompleted),
                CompletionPercentage = group.Tasks.Any() ? (double)group.Tasks.Count(task => task.IsCompleted) / group.Tasks.Count() * 100 : 0
            })
            .OrderByDescending(summary => summary.CompletionPercentage)];
    }
    /*
        var temp = teamProjects.Select(project => Tasks.Where(task => task.ProjectId == project.Id));
        Select
           [
               IEnumerable<Task> { Task 1, Task 2 },
               IEnumerable<Task> { Task 3 }
           ]     
       Tasks = teamProjects.SelectMany(project => Tasks.Where(task => task.ProjectId == project.Id));
       SelectMany
           [ Task 1, Task 2, Task 3 ]

       Team	        Projects	    Tasks.Where(...) per project	Sau SelectMany (flatten)
       Alpha Team	Project A, B	[Task1, Task2], [Task3]	        [Task1, Task2, Task3]
       Beta Team	Project C	    [Task4]	                        [Task4]
     */

    // public void GetAllTaskByTeam(out List<object> result)
    // {
    //     result = [.. Teams
    //         .GroupJoin(Projects,
    //             team => team.Id,
    //             project => project.TeamId,
    //             (team, teamProjects) => new
    //             {
    //                 TeamName = team.Name,
    //                 Tasks = teamProjects.SelectMany(project => Tasks.Where(task => task.ProjectId == project.Id)).ToList()
    //             })];      
    // }
}

/*
 Team Task Completion Summary:

Team: Alpha Team
  Total Tasks       : 3
  Completed Tasks   : 2
  Completion %      : 66.67%

Team: Beta Team
  Total Tasks       : 2
  Completed Tasks   : 1
  Completion %      : 50.00%
 
 */

/*
This exercise involves analyzing task completion for each team by aggregating data across multiple levels (teams, projects, tasks).

* 1.Grouping Projects and Tasks by Team:

GroupJoin(Projects, team => team.Id, project => project.TeamId, ...) groups projects by team.

SelectMany(project => Tasks.Where(task => task.ProjectId == project.Id)) collects tasks for each project within a team.

* 2.Calculating Task Completion Metrics:

TotalTasks: Counts all tasks associated with the team’s projects.

CompletedTasks: Counts tasks marked as completed.

CompletionPercentage: Calculates the percentage of tasks completed.

* 3.Returning the Report:

The result is a list of TeamTaskCompletionSummary objects, sorted by CompletionPercentage in descending order.
 
 */
using System.Collections.Concurrent;

public class Program
{
    static void Main()
    {
        var repo = new TaskRepository();

        var start = new DateTime(2024, 1, 1);
        var end = new DateTime(2024, 1, 15);

        var results = repo.GetMostActiveEmployees_PLINQ(start, end);

        Console.WriteLine("=== Most Active Employees ===\n");

        foreach (var r in results)
        {
            Console.WriteLine($"Name: {r.EmployeeName}");
            Console.WriteLine($"Completed Tasks: {r.CompletedTasksCount}");
            Console.WriteLine($"Period: {r.TimePeriod}");
            Console.WriteLine();
        }

        Console.ReadKey();
    }
}

public class Employee
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class Task
{
    public int EmployeeId { get; set; }
    public DateTime TaskDate { get; set; }
    public required string Status { get; set; } // "Completed" or "Pending"
}

public class ActiveEmployeeSummary
{
    public required string EmployeeName { get; set; }
    public int CompletedTasksCount { get; set; }
    public required string TimePeriod { get; set; }
}

public class TaskRepository
{
    public List<Employee> Employees { get; set; } =
    [
        new Employee { Id = 1, Name = "Alice" },
        new Employee { Id = 2, Name = "Bob" },
        new Employee { Id = 3, Name = "Charlie" }
    ];

    public List<Task> Tasks { get; set; } =
    [
        new Task { EmployeeId = 1, TaskDate = new DateTime(2024, 1, 5), Status = "Completed" },
        new Task { EmployeeId = 1, TaskDate = new DateTime(2024, 1, 6), Status = "Completed" },
        new Task { EmployeeId = 1, TaskDate = new DateTime(2024, 1, 7), Status = "Pending" },
        new Task { EmployeeId = 2, TaskDate = new DateTime(2024, 1, 10), Status = "Completed" },
        new Task { EmployeeId = 3, TaskDate = new DateTime(2024, 1, 11), Status = "Completed" },
        new Task { EmployeeId = 3, TaskDate = new DateTime(2024, 1, 12), Status = "Completed" }
    ];

    public List<ActiveEmployeeSummary> GetMostActiveEmployees(DateTime startDate, DateTime endDate)
    {
        var activeEmployees = Tasks
            .Where(task => task.TaskDate >= startDate && task.TaskDate <= endDate && task.Status == "Completed")
            .GroupBy(task => task.EmployeeId)
            .Select(g => new
            {
                EmployeeId = g.Key,
                CompletedTasksCount = g.Count()
            })
            .OrderByDescending(e => e.CompletedTasksCount)
            .Take(5)
            .Join(Employees,
                  e => e.EmployeeId,
                  emp => emp.Id,
                  (e, emp) => new ActiveEmployeeSummary
                  {
                      EmployeeName = emp.Name,
                      CompletedTasksCount = e.CompletedTasksCount,
                      TimePeriod = $"{startDate:MMMM dd, yyyy} - {endDate:MMMM dd, yyyy}"
                  })
            .ToList();

        return activeEmployees;
    }

    public List<ActiveEmployeeSummary> GetMostActiveEmployees_Dictionary(DateTime startDate, DateTime endDate)
    {
        // 1) Cache employee lookup O(1)
        var employeeDict = Employees.ToDictionary(e => e.Id, e => e.Name);

        // 2) Tạo dictionary đếm số tasks completed cho mỗi employee
        var completedTasks = new Dictionary<int, int>();

        foreach (var task in Tasks)
        {
            if (task.TaskDate >= startDate && task.TaskDate <= endDate && task.Status == "Completed")
            {
                if (!completedTasks.ContainsKey(task.EmployeeId))
                    completedTasks[task.EmployeeId] = 0;

                completedTasks[task.EmployeeId]++;
            }
        }

        // 3) Chọn top 5 employee theo số task completed
        var topEmployees = completedTasks
            .OrderByDescending(kvp => kvp.Value)
            .Take(5)
            .Select(kvp => new ActiveEmployeeSummary
            {
                EmployeeName = employeeDict[kvp.Key],
                CompletedTasksCount = kvp.Value,
                TimePeriod = $"{startDate:MMMM dd, yyyy} - {endDate:MMMM dd, yyyy}"
            })
            .ToList();

        return topEmployees;
    }

    public List<ActiveEmployeeSummary> GetMostActiveEmployees_Parallel(DateTime startDate, DateTime endDate)
    {
        // 1) Cache employee lookup O(1)
        var employeeDict = Employees.ToDictionary(e => e.Id, e => e.Name);

        // 2) Thread-safe dictionary đếm completed tasks
        var completedTasks = new ConcurrentDictionary<int, int>();

        // 3) Duyệt song song các task
        Parallel.ForEach(Tasks, task =>
        {
            if (task.TaskDate >= startDate && task.TaskDate <= endDate && task.Status == "Completed")
            {
                completedTasks.AddOrUpdate(task.EmployeeId, 1, (key, oldValue) => oldValue + 1);
            }
        });

        // 4) Chọn top 5 employee
        var topEmployees = completedTasks
            .OrderByDescending(kvp => kvp.Value)
            .Take(5)
            .Select(kvp => new ActiveEmployeeSummary
            {
                EmployeeName = employeeDict[kvp.Key],
                CompletedTasksCount = kvp.Value,
                TimePeriod = $"{startDate:MMMM dd, yyyy} - {endDate:MMMM dd, yyyy}"
            })
            .ToList();

        return topEmployees;
    }

    public List<ActiveEmployeeSummary> GetMostActiveEmployees_PLINQ(DateTime startDate, DateTime endDate)
    {
        // 1) Cache employee lookup O(1)
        var employeeDict = Employees.ToDictionary(e => e.Id, e => e.Name);

        // 2) Duyệt Tasks song song, group & count
        var completedTasks = Tasks
            .AsParallel()
            .Where(task => task.TaskDate >= startDate && task.TaskDate <= endDate && task.Status == "Completed")
            .GroupBy(task => task.EmployeeId)
            .Select(g => new
            {
                EmployeeId = g.Key,
                CompletedTasksCount = g.Count()
            })
            .ToList();

        // 3) Chọn top 5 employee
        var topEmployees = completedTasks
            .OrderByDescending(x => x.CompletedTasksCount)
            .Take(5)
            .Select(x => new ActiveEmployeeSummary
            {
                EmployeeName = employeeDict[x.EmployeeId],
                CompletedTasksCount = x.CompletedTasksCount,
                TimePeriod = $"{startDate:MMMM dd, yyyy} - {endDate:MMMM dd, yyyy}"
            })
            .ToList();

        return topEmployees;
    }
}

/*
 !=== Most Active Employees ===

    Name: Alice
    Completed Tasks: 2
    Period: January 01, 2024 - January 15, 2024

    Name: Charlie
    Completed Tasks: 2
    Period: January 01, 2024 - January 15, 2024

    Name: Bob
    Completed Tasks: 1
    Period: January 01, 2024 - January 15, 2024
 */

/*
 !This exercise identifies the most active employees based on completed tasks within a specified date range.

* 1.Filtering Tasks by Date and Status:

Where(task => task.TaskDate >= startDate && task.TaskDate <= endDate && task.Status == "Completed") filters tasks within the specified date range and only includes completed tasks.

* 2.Grouping by Employee:

GroupBy(task => task.EmployeeId) groups tasks by each employee.

* 3.Counting Completed Tasks:

Count() calculates the number of completed tasks for each employee.

* 4.Selecting Top Active Employees:

OrderByDescending(e => e.CompletedTasksCount).Take(5) selects the top 5 employees by the number of completed tasks.

* 5.Returning the Summary:

The result is a list of ActiveEmployeeSummary objects, sorted by CompletedTasksCount in
 */
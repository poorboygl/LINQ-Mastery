class Program
{
    static void Main()
    {
        var repo = new EmployeeRepository();
        var summaries = repo.GetConsistentEmployees();

        Console.WriteLine("=== EMPLOYEE CONSISTENCY REPORT ===\n");

        foreach (var summary in summaries)
        {
            Console.WriteLine($"Employee: {summary.EmployeeName}");
            Console.WriteLine($"  Total Tasks Assigned:   {summary.TotalTasksAssigned}");
            Console.WriteLine($"  Total Tasks Completed:  {summary.TotalTasksCompleted}");
            Console.WriteLine($"  Consistently Completes: {summary.ConsistentlyCompletesTasks}");
            Console.WriteLine(new string('-', 40));
        }

        Console.ReadKey();
    }
}


public class Employee
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class Project
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<int> AssignedEmployeeIds { get; set; } = [];
}

public class Task
{
    public int ProjectId { get; set; }
    public int AssignedEmployeeId { get; set; }
    public bool IsCompleted { get; set; }
    public int HoursSpent { get; set; }
}

public class EmployeeConsistencySummary
{
    public required string EmployeeName { get; set; }
    public int TotalTasksAssigned { get; set; }
    public int TotalTasksCompleted { get; set; }
    public bool ConsistentlyCompletesTasks { get; set; }
}

public class EmployeeRepository
{
    public List<Employee> Employees { get; set; } =
    [
        new Employee { Id = 1, Name = "Alice" },
        new Employee { Id = 2, Name = "Bob" }
    ];

    public List<Project> Projects { get; set; } =
    [
        new Project { Id = 1, Name = "Project Alpha", AssignedEmployeeIds = [1, 2] },
        new Project { Id = 2, Name = "Project Beta", AssignedEmployeeIds = [1] }
    ];

    public List<Task> Tasks { get; set; } =
    [
        new Task { ProjectId = 1, AssignedEmployeeId = 1, IsCompleted = true, HoursSpent = 10 },
        new Task { ProjectId = 1, AssignedEmployeeId = 1, IsCompleted = true, HoursSpent = 15 },
        new Task { ProjectId = 1, AssignedEmployeeId = 2, IsCompleted = false, HoursSpent = 5 },
        new Task { ProjectId = 2, AssignedEmployeeId = 1, IsCompleted = true, HoursSpent = 20 }
    ];

    public List<EmployeeConsistencySummary> GetConsistentEmployees()
    {
        /*
        !=== EMPLOYEE CONSISTENCY REPORT ===

         Employee: Alice
           Total Tasks Assigned:   3
           Total Tasks Completed:  3
           Consistently Completes: True
         ----------------------------------------
         Employee: Bob
           Total Tasks Assigned:   1
           Total Tasks Completed:  0
           Consistently Completes: False
         ----------------------------------------
          */


        return Employees
            .Select(employee =>
            {
                var assignedTasks = Tasks.Where(task => task.AssignedEmployeeId == employee.Id).ToList();
                var totalTasksAssigned = assignedTasks.Count;
                var totalTasksCompleted = assignedTasks.Count(task => task.IsCompleted);

                return new EmployeeConsistencySummary
                {
                    EmployeeName = employee.Name,
                    TotalTasksAssigned = totalTasksAssigned,
                    TotalTasksCompleted = totalTasksCompleted,
                    ConsistentlyCompletesTasks = totalTasksAssigned > 0 && assignedTasks.All(task => task.IsCompleted)
                };
            })
            .ToList();
    }

    public List<EmployeeConsistencySummary> GetConsistentEmployees_2()
    {
        /*
        !=== EMPLOYEE CONSISTENCY REPORT ===

         Employee: Alice
           Total Tasks Assigned:   3
           Total Tasks Completed:  3
           Consistently Completes: True
         ----------------------------------------
         Employee: Bob
           Total Tasks Assigned:   1
           Total Tasks Completed:  0
           Consistently Completes: False
         ----------------------------------------
          */


        return Employees
                .GroupJoin(
                    Tasks,
                    employee => employee.Id,
                    task => task.AssignedEmployeeId,
                    (employee, employeeTasks) =>
                    {
                        var list = employeeTasks.ToList();
                        return new EmployeeConsistencySummary
                        {
                            EmployeeName = employee.Name,
                            TotalTasksAssigned = list.Count,
                            TotalTasksCompleted = list.Count(t => t.IsCompleted),
                            ConsistentlyCompletesTasks = list.Count > 0 && list.All(t => t.IsCompleted)
                        };
                    }
                )
                .ToList();
    }
    
}

//If Task hasn't AssignedEmployeeId
//var result =
//    Employees.GroupJoin(
//        Projects,
//        emp => emp.Id,
//        proj => proj.AssignedEmployeeIds.Contains(emp.Id),
//        (emp, empProjects) =>
//        {
//            // gather all tasks from all projects assigned to this employee
//            var tasks = empProjects
//                .SelectMany(p => Tasks.Where(t => t.ProjectId == p.Id))
//                .ToList();

//            return new EmployeeConsistencySummary
//            {
//                EmployeeName = emp.Name,
//                TotalTasksAssigned = tasks.Count,
//                TotalTasksCompleted = tasks.Count(t => t.IsCompleted),
//                ConsistentlyCompletesTasks = tasks.Count > 0 && tasks.All(t => t.IsCompleted)
//            };
//        }
//    )
//    .ToList();


/*
   !This exercise generates a consistency report for employees by analyzing whether they consistently complete all assigned tasks.

    * 1.Filtering Tasks by Employee:

    Tasks.Where(task => task.AssignedEmployeeId == employee.Id) selects tasks assigned to each employee.

    * 2.Calculating Completion Metrics:

    TotalTasksAssigned: Counts tasks assigned to the employee.

    TotalTasksCompleted: Counts tasks marked as completed.

    ConsistentlyCompletesTasks: Uses All to verify if all assigned tasks are completed.

    * 3.Returning the Report:

    The result is a list of EmployeeConsistencySummary objects, each showing completion consistency for an employee.
 
 */
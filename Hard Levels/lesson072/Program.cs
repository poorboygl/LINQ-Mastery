class Program
{
    static void Main()
    {
        var repo = new AttendanceRepository();

        var perfectList = repo.GetEmployeesWithPerfectAttendance_AuthorWritting();

        Console.WriteLine("=== EMPLOYEES WITH PERFECT ATTENDANCE ===\n");

        if (perfectList.Count == 0)
        {
            Console.WriteLine("No employee has perfect attendance.");
        }
        else
        {
            foreach (var summary in perfectList)
            {
                Console.WriteLine($"Name: {summary.EmployeeName}");
                Console.WriteLine($"Attendance Count: {summary.AttendanceCount}");
                Console.WriteLine();
            }
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
public class Employee
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class Workday
{
    public DateTime Date { get; set; }
}

public class AttendanceRecord
{
    public int EmployeeId { get; set; }
    public DateTime Date { get; set; }
}

public class AttendanceSummary
{
    public required string EmployeeName { get; set; }
    public int AttendanceCount { get; set; }
}

public class AttendanceRepository
{
    public List<Employee> Employees { get; set; } =
    [
        new Employee { Id = 1, Name = "Alice" },
        new Employee { Id = 2, Name = "Bob" },
        new Employee { Id = 3, Name = "Charlie" }
    ];

    public List<Workday> Workdays { get; set; } =
    [
        new Workday { Date = new DateTime(2024, 1, 1) },
        new Workday { Date = new DateTime(2024, 1, 2) },
        new Workday { Date = new DateTime(2024, 1, 3) }
    ];

    public List<AttendanceRecord> AttendanceRecords { get; set; } =
    [
        new AttendanceRecord { EmployeeId = 1, Date = new DateTime(2024, 1, 1) },
        new AttendanceRecord { EmployeeId = 1, Date = new DateTime(2024, 1, 2) },
        new AttendanceRecord { EmployeeId = 1, Date = new DateTime(2024, 1, 3) },
        new AttendanceRecord { EmployeeId = 2, Date = new DateTime(2024, 1, 1) },
        new AttendanceRecord { EmployeeId = 2, Date = new DateTime(2024, 1, 3) }
    ];

    public List<AttendanceSummary> GetEmployeesWithPerfectAttendance()
    {
        /*
         === EMPLOYEES WITH PERFECT ATTENDANCE ===

            Name: Alice
            Attendance Count: 3
         */
        var requiredDates = Workdays.Select(w => w.Date.Date).ToList();

        var result = AttendanceRecords
                    .GroupBy(r => r.EmployeeId)
                    .Where(g => requiredDates.All(d => g.Any(r => r.Date.Date == d)))
                    .Select(group => new AttendanceSummary
                    {
                        EmployeeName = Employees.First(e => e.Id == group.Key).Name,
                        AttendanceCount = group.Count()

                    })
                    .OrderByDescending(summary => summary.AttendanceCount)
                    .ToList();

        return result;
    }

    public List<AttendanceSummary> GetEmployeesWithPerfectAttendance_AuthorWritting()
    {
        //Với mỗi ngày bắt buộc, kiểm tra xem nhân viên có đi làm ít nhất 1 lần hay không.
        var requiredDates = Workdays.Select(w => w.Date).ToList();

        var perfectAttendance = Employees
            .Where(employee => requiredDates
                .All(date => AttendanceRecords
                    .Any(record => record.EmployeeId == employee.Id && record.Date == date)))
            .Select(employee => new AttendanceSummary
            {
                EmployeeName = employee.Name,
                AttendanceCount = AttendanceRecords.Count(record => record.EmployeeId == employee.Id)
            })
            .OrderBy(summary => summary.EmployeeName)
            .ToList();

        return perfectAttendance;
    }

}
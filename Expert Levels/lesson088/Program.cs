using System.Collections.Concurrent;

public class Program
{
    static void Main()
    {
        var repo = new ShiftRepository();

        Console.WriteLine("=== Employees With Required Consecutive Shifts ===\n");

        int requiredDays = 2;  // muốn lọc nhân viên làm tối thiểu 2 ngày liên tục

        var results = repo.GetEmployeesWithConsecutiveShifts_Parallel(requiredDays);

        foreach (var r in results)
        {
            Console.WriteLine($"Employee: {r.EmployeeName}");
            Console.WriteLine($"  Consecutive Days Worked: {r.ConsecutiveDaysWorked}");
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

public class WorkShift
{
    public int EmployeeId { get; set; }
    public DateTime ShiftDate { get; set; }
}

public class ConsecutiveShiftSummary
{
    public required string EmployeeName { get; set; }
    public int ConsecutiveDaysWorked { get; set; }
}

public class ShiftRepository
{
    public List<Employee> Employees { get; set; } =
    [
        new Employee { Id = 1, Name = "Alice" },
        new Employee { Id = 2, Name = "Bob" },
        new Employee { Id = 3, Name = "Charlie" }
    ];

    public List<WorkShift> WorkShifts { get; set; } =
    [
        new WorkShift { EmployeeId = 1, ShiftDate = new DateTime(2024, 1, 1) },
        new WorkShift { EmployeeId = 1, ShiftDate = new DateTime(2024, 1, 2) },
        new WorkShift { EmployeeId = 1, ShiftDate = new DateTime(2024, 1, 3) },
        new WorkShift { EmployeeId = 2, ShiftDate = new DateTime(2024, 1, 5) },
        new WorkShift { EmployeeId = 2, ShiftDate = new DateTime(2024, 1, 6) },
        new WorkShift { EmployeeId = 3, ShiftDate = new DateTime(2024, 1, 7) }
    ];

    public List<ConsecutiveShiftSummary> GetEmployeesWithConsecutiveShifts(int requiredConsecutiveDays)
    {
        var employeesWithConsecutiveShifts = WorkShifts
            .GroupBy(ws => ws.EmployeeId)
            .Select(g =>
            {
                var employee = Employees.First(e => e.Id == g.Key);
                var orderedDates = g.Select(ws => ws.ShiftDate).OrderBy(date => date).ToList();

                int maxConsecutiveDays = 1;
                int currentStreak = 1;

                for (int i = 1; i < orderedDates.Count; i++)
                {
                    if ((orderedDates[i] - orderedDates[i - 1]).Days == 1)
                    {
                        currentStreak++;
                    }
                    else
                    {
                        currentStreak = 1;
                    }

                    if (currentStreak > maxConsecutiveDays)
                    {
                        maxConsecutiveDays = currentStreak;
                    }
                }

                return new
                {
                    EmployeeName = employee.Name,
                    ConsecutiveDaysWorked = maxConsecutiveDays
                };
            })
            .Where(e => e.ConsecutiveDaysWorked >= requiredConsecutiveDays)
            .Select(e => new ConsecutiveShiftSummary
            {
                EmployeeName = e.EmployeeName,
                ConsecutiveDaysWorked = e.ConsecutiveDaysWorked
            })
            .OrderBy(e => e.EmployeeName)
            .ToList();

        return employeesWithConsecutiveShifts;
    }

    public List<ConsecutiveShiftSummary> GetEmployeesWithConsecutiveShifts_Fast(int requiredConsecutiveDays)
    {
        // Dictionary gom ngày làm theo từng nhân viên
        var shiftMap = new Dictionary<int, List<DateTime>>();

        foreach (var shift in WorkShifts)
        {
            if (!shiftMap.ContainsKey(shift.EmployeeId))
                shiftMap[shift.EmployeeId] = new List<DateTime>();

            shiftMap[shift.EmployeeId].Add(shift.ShiftDate);
        }

        // Dictionary lookup nhanh employee name
        var employeeMap = Employees.ToDictionary(e => e.Id);

        var results = new List<ConsecutiveShiftSummary>();

        foreach (var entry in shiftMap)
        {
            int employeeId = entry.Key;
            var dates = entry.Value;

            // Sort ngày
            dates.Sort();

            // Tính streak liên tục
            int maxStreak = 1;
            int currentStreak = 1;

            for (int i = 1; i < dates.Count; i++)
            {
                if ((dates[i] - dates[i - 1]).Days == 1)
                {
                    currentStreak++;
                }
                else
                {
                    currentStreak = 1;
                }

                if (currentStreak > maxStreak)
                    maxStreak = currentStreak;
            }

            // Nếu đạt yêu cầu streak thì add
            if (maxStreak >= requiredConsecutiveDays)
            {
                results.Add(new ConsecutiveShiftSummary
                {
                    EmployeeName = employeeMap[employeeId].Name,
                    ConsecutiveDaysWorked = maxStreak
                });
            }
        }

        // Sắp xếp cho đẹp
        return results.OrderBy(r => r.EmployeeName).ToList();
    }

    public List<ConsecutiveShiftSummary> GetEmployeesWithConsecutiveShifts_Parallel(int requiredConsecutiveDays)
    {
        // 1) Gom shift theo employee bằng dictionary
        var shiftMap = new Dictionary<int, List<DateTime>>();

        foreach (var shift in WorkShifts)
        {
            if (!shiftMap.ContainsKey(shift.EmployeeId))
                shiftMap[shift.EmployeeId] = new List<DateTime>();

            shiftMap[shift.EmployeeId].Add(shift.ShiftDate);
        }

        // 2) Employee lookup nhanh
        var employeeMap = Employees.ToDictionary(e => e.Id);

        // 3) Kết quả thread-safe
        var results = new ConcurrentBag<ConsecutiveShiftSummary>();

        // 4) Xử lý song song
        Parallel.ForEach(shiftMap, entry =>
        {
            int employeeId = entry.Key;
            var dates = entry.Value;

            // Sort ngày (sort nhỏ, nên đủ nhẹ để chạy song song ổn)
            dates.Sort();

            int maxStreak = 1;
            int currentStreak = 1;

            for (int i = 1; i < dates.Count; i++)
            {
                if ((dates[i] - dates[i - 1]).Days == 1)
                {
                    currentStreak++;
                }
                else
                {
                    currentStreak = 1;
                }

                if (currentStreak > maxStreak)
                    maxStreak = currentStreak;
            }

            if (maxStreak >= requiredConsecutiveDays)
            {
                results.Add(new ConsecutiveShiftSummary
                {
                    EmployeeName = employeeMap[employeeId].Name,
                    ConsecutiveDaysWorked = maxStreak
                });
            }
        });

        return results.OrderBy(r => r.EmployeeName).ToList();
    }
}

/*
 !=== Employees With Required Consecutive Shifts ===

Employee: Alice
  Consecutive Days Worked: 3

Employee: Bob
  Consecutive Days Worked: 2
 
 */

/*
 !This exercise identifies employees who worked a specified number of consecutive days.

* 1.Grouping by Employee:

GroupBy(ws => ws.EmployeeId) groups work shifts by each employee.

* 2.Sorting and Counting Consecutive Days:

Shifts are ordered by date, and a loop checks if each day is consecutive with the previous day. Streaks are counted and reset when a gap is detected.

* 3.Filtering by Required Consecutive Days:

Only employees who meet or exceed requiredConsecutiveDays are included.

* 4.Returning the Summary:

The result is a list of ConsecutiveShiftSummary objects, sorted by EmployeeName.
 */
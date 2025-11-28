using System.Collections.Concurrent;

public class Program
{
    static void Main()
    {
        var repo = new EventRepository();
        var topAttendees = repo.GetTopAttendeesByAttendanceFrequency_PLINQ();

        Console.WriteLine("=== Top Attendees by Attendance Frequency ===");

        foreach (var attendee in topAttendees)
        {
            Console.WriteLine(
                $"{attendee.AttendeeName} - Events: {attendee.TotalEventsAttended}, " +
                $"First Attendance: {attendee.FirstAttendanceDate:yyyy-MM-dd}"
            );
        }

        Console.ReadKey();
    }
}

public class Attendee
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class EventRecord
{
    public int AttendeeId { get; set; }
    public int EventId { get; set; }
    public DateTime AttendanceDate { get; set; }
}

public class TopAttendeeSummary
{
    public required string AttendeeName { get; set; }
    public int TotalEventsAttended { get; set; }
    public DateTime FirstAttendanceDate { get; set; }
}

public class EventRepository
{
    public List<Attendee> Attendees { get; set; } =
    [
        new Attendee { Id = 1, Name = "Alice" },
        new Attendee { Id = 2, Name = "Bob" },
        new Attendee { Id = 3, Name = "Charlie" }
    ];

    public List<EventRecord> EventRecords { get; set; } =
    [
        new EventRecord { AttendeeId = 1, EventId = 101, AttendanceDate = new DateTime(2024, 1, 10) },
        new EventRecord { AttendeeId = 1, EventId = 102, AttendanceDate = new DateTime(2024, 1, 20) },
        new EventRecord { AttendeeId = 1, EventId = 103, AttendanceDate = new DateTime(2024, 1, 30) },
        new EventRecord { AttendeeId = 2, EventId = 101, AttendanceDate = new DateTime(2024, 1, 10) },
        new EventRecord { AttendeeId = 2, EventId = 104, AttendanceDate = new DateTime(2024, 1, 15) },
        new EventRecord { AttendeeId = 3, EventId = 101, AttendanceDate = new DateTime(2024, 2, 5) }
    ];

    public List<TopAttendeeSummary> GetTopAttendeesByAttendanceFrequency()
    {
        var topAttendees = EventRecords
            .GroupBy(record => record.AttendeeId)
            .Select(g => new
            {
                AttendeeId = g.Key,
                TotalEventsAttended = g.Count(),
                FirstAttendanceDate = g.Min(record => record.AttendanceDate)
            })
            .OrderByDescending(attendee => attendee.TotalEventsAttended)
            .Take(5)
            .Join(Attendees,
                  attendee => attendee.AttendeeId,
                  attendeeInfo => attendeeInfo.Id,
                  (attendee, attendeeInfo) => new TopAttendeeSummary
                  {
                      AttendeeName = attendeeInfo.Name,
                      TotalEventsAttended = attendee.TotalEventsAttended,
                      FirstAttendanceDate = attendee.FirstAttendanceDate
                  })
            .ToList();

        return topAttendees;
    }

    public List<TopAttendeeSummary> GetTopAttendeesByAttendanceFrequency_Dictionary()
    {
        // Tạo Dictionary lookup cho Attendees
        var attendeeNames = Attendees.ToDictionary(a => a.Id, a => a.Name);

        // Group theo AttendeeId
        var grouped = EventRecords
            .GroupBy(r => r.AttendeeId)
            .Select(g => new
            {
                AttendeeId = g.Key,
                TotalEventsAttended = g.Count(),
                FirstAttendanceDate = g.Min(r => r.AttendanceDate)
            });

        // Chọn top 5 và ánh xạ bằng dictionary lookup
        var topAttendees = grouped
            .OrderByDescending(a => a.TotalEventsAttended)
            .Take(5)
            .Select(a => new TopAttendeeSummary
            {
                AttendeeName = attendeeNames[a.AttendeeId],
                TotalEventsAttended = a.TotalEventsAttended,
                FirstAttendanceDate = a.FirstAttendanceDate
            })
            .ToList();

        return topAttendees;
    }

    public List<TopAttendeeSummary> GetTopAttendeesByAttendanceFrequency_Parallel()
    {
        // Dictionary lookup để lấy tên nhanh
        var attendeeNames = Attendees.ToDictionary(a => a.Id, a => a.Name);

        // Group theo AttendeeId
        var grouped = EventRecords
            .GroupBy(r => r.AttendeeId)
            .ToList(); // Chuyển sang list để Parallel.ForEach

        var results = new ConcurrentBag<TopAttendeeSummary>();

        Parallel.ForEach(grouped, g =>
        {
            var summary = new TopAttendeeSummary
            {
                AttendeeName = attendeeNames[g.Key],
                TotalEventsAttended = g.Count(),
                FirstAttendanceDate = g.Min(r => r.AttendanceDate)
            };

            results.Add(summary);
        });

        // Sắp xếp top 5
        return results
            .OrderByDescending(a => a.TotalEventsAttended)
            .Take(5)
            .ToList();
    }

    public List<TopAttendeeSummary> GetTopAttendeesByAttendanceFrequency_PLINQ()
    {
        // Dictionary lookup để lấy tên nhanh
        var attendeeNames = Attendees.ToDictionary(a => a.Id, a => a.Name);

        var topAttendees = EventRecords
            .AsParallel() // Bật PLINQ
            .GroupBy(r => r.AttendeeId)
            .Select(g => new TopAttendeeSummary
            {
                AttendeeName = attendeeNames[g.Key],
                TotalEventsAttended = g.Count(),
                FirstAttendanceDate = g.Min(r => r.AttendanceDate)
            })
            .OrderByDescending(a => a.TotalEventsAttended)
            .Take(5)
            .ToList();

        return topAttendees;
    }
}

/*
 !=== Top Attendees by Attendance Frequency ===
Alice - Events: 3, First Attendance: 2024-01-10
Bob - Events: 2, First Attendance: 2024-01-10
Charlie - Events: 1, First Attendance: 2024-02-05
 */

/*
 !This exercise identifies the most engaged attendees by their attendance frequency.

* 1.Grouping by Attendee:

GroupBy(record => record.AttendeeId) groups attendance records by each attendee.

* 2.Counting Events Attended and Finding First Attendance Date:

Count() calculates the total events attended for each attendee.

Min(record => record.AttendanceDate) finds the first attendance date for each attendee.

* 3.Selecting Top Attendees:

OrderByDescending(attendee => attendee.TotalEventsAttended).Take(5) selects the top 5 attendees based on attendance frequency.

* 4.Returning the Summary:

The result is a list of TopAttendeeSummary objects, sorted by TotalEventsAttended in descending order.
 
 */
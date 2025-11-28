using System.Collections.Concurrent;

public class Program
{
    static void Main()
    {
        var repo = new ServiceRepository();
        var results = repo.GetTopIssuesByMonth_Dictionary();

        Console.WriteLine("=== Monthly Top Issues ===");
        foreach (var summary in results)
        {
            Console.WriteLine($"{summary.Month}: {summary.IssueDescription} (Occurrences: {summary.OccurrenceCount})");
        }

        Console.ReadKey();
    }
}

public class Issue
{
    public int Id { get; set; }
    public required string Description { get; set; }
}

public class ServiceRecord
{
    public int IssueId { get; set; }
    public DateTime ServiceDate { get; set; }
    public int CustomerId { get; set; }
}

public class MonthlyTopIssueSummary
{
    public required string Month { get; set; }
    public required string IssueDescription { get; set; }
    public int OccurrenceCount { get; set; }
}

public class ServiceRepository
{
    public List<Issue> Issues { get; set; } =
    [
        new Issue { Id = 1, Description = "Billing Error" },
        new Issue { Id = 2, Description = "Technical Support" },
        new Issue { Id = 3, Description = "Account Access" }
    ];

    public List<ServiceRecord> ServiceRecords { get; set; } =
    [
        new ServiceRecord { IssueId = 1, ServiceDate = new DateTime(2024, 1, 10), CustomerId = 101 },
        new ServiceRecord { IssueId = 1, ServiceDate = new DateTime(2024, 1, 15), CustomerId = 102 },
        new ServiceRecord { IssueId = 2, ServiceDate = new DateTime(2024, 1, 20), CustomerId = 103 },
        new ServiceRecord { IssueId = 2, ServiceDate = new DateTime(2024, 2, 5), CustomerId = 101 },
        new ServiceRecord { IssueId = 3, ServiceDate = new DateTime(2024, 2, 10), CustomerId = 104 },
        new ServiceRecord { IssueId = 1, ServiceDate = new DateTime(2024, 2, 15), CustomerId = 105 }
    ];

    public List<MonthlyTopIssueSummary> GetTopIssuesByMonth()
    {
        var topIssues = ServiceRecords
            .GroupBy(record => new { record.ServiceDate.Year, record.ServiceDate.Month, record.IssueId })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                IssueId = g.Key.IssueId,
                OccurrenceCount = g.Count()
            })
            .GroupBy(g => new { g.Year, g.Month })
            .Select(monthGroup => monthGroup
                .OrderByDescending(g => g.OccurrenceCount)
                .First())
            .Join(Issues,
                  monthIssue => monthIssue.IssueId,
                  issue => issue.Id,
                  (monthIssue, issue) => new MonthlyTopIssueSummary
                  {
                      Month = new DateTime(monthIssue.Year, monthIssue.Month, 1).ToString("MMMM yyyy"),
                      IssueDescription = issue.Description,
                      OccurrenceCount = monthIssue.OccurrenceCount
                  })
            .OrderBy(summary => summary.Month)
            .ToList();

        return topIssues;
    }

    public List<MonthlyTopIssueSummary> GetTopIssuesByMonth_Dictionary()
    {
        // Dictionary: Year-Month → Dictionary<IssueId, Count>
        var monthIssueCounter = new Dictionary<(int Year, int Month), Dictionary<int, int>>();

        foreach (var record in ServiceRecords)
        {
            var key = (record.ServiceDate.Year, record.ServiceDate.Month);

            if (!monthIssueCounter.ContainsKey(key))
                monthIssueCounter[key] = new Dictionary<int, int>();

            if (!monthIssueCounter[key].ContainsKey(record.IssueId))
                monthIssueCounter[key][record.IssueId] = 0;

            monthIssueCounter[key][record.IssueId]++;
        }

        // Mapping Issues để truy cập nhanh
        var issueLookup = Issues.ToDictionary(i => i.Id, i => i.Description);

        var results = new List<MonthlyTopIssueSummary>();

        foreach (var monthEntry in monthIssueCounter)
        {
            var year = monthEntry.Key.Year;
            var month = monthEntry.Key.Month;

            // Lấy Issue có count cao nhất
            var topIssue = monthEntry.Value
                .OrderByDescending(x => x.Value)
                .First();

            results.Add(new MonthlyTopIssueSummary
            {
                Month = new DateTime(year, month, 1).ToString("MMMM yyyy"),
                IssueDescription = issueLookup[topIssue.Key],
                OccurrenceCount = topIssue.Value
            });
        }

        // Sort theo thứ tự Month
        return results
            .OrderBy(r => DateTime.Parse(r.Month))
            .ToList();
    }

}

/*
 !=== Monthly Top Issues ===
    February 2024: Technical Support (Occurrences: 1)
    January 2024: Billing Error (Occurrences: 2)

 */

/*
 !This exercise identifies the most common issue each month based on the number of occurrences.

* 1.Grouping by Month and Issue:

GroupBy(record => new { record.ServiceDate.Year, record.ServiceDate.Month, record.IssueId }) groups records by month and issue type.

* 2.Counting Issue Occurrences:

Count() calculates the number of occurrences of each issue for each month.

* 3.Selecting Most Frequent Issue per Month:

OrderByDescending(g => g.OccurrenceCount).First() selects the most common issue for each month.

* 4.Returning the Summary:

The result is a list of MonthlyTopIssueSummary objects, sorted by Month in ascending order.
 */
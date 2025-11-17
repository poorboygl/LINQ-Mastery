class Program
{
    static void Main()
    {
        var repo = new SalesRepository();
        var reports = repo.GetSalesPerformanceReport();

        Console.WriteLine("=== SALES PERFORMANCE REPORT ===\n");

        foreach (var r in reports)
        {
            Console.WriteLine($"Month: {r.Month}");
            Console.WriteLine($"  Target:       {r.SalesTarget}");
            Console.WriteLine($"  Actual:       {r.ActualSales}");
            Console.WriteLine($"  Difference:   {r.Difference}");
            Console.WriteLine();
        }

        Console.ReadLine();
    }
}

public class MonthlyPerformance
{
    public required string Month { get; set; }
    public decimal SalesTarget { get; set; }
    public decimal ActualSales { get; set; }
    public decimal Difference { get; set; }
}

public class SalesRepository
{
    public List<string> Months { get; set; } =
    [
        "January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    ];

    public List<decimal> SalesTargets { get; set; } =
    [
        1000, 1500, 1300, 1200, 1600, 1400,
        1350, 1550, 1450, 1100, 1250, 1700
    ];

    public List<decimal> ActualSales { get; set; } =
    [
        1100, 1400, 1200, 1300, 1550, 1380,
        1300, 1600, 1500, 1000, 1200, 1800
    ];

    public List<MonthlyPerformance> GetSalesPerformanceReport()
    {
        return [.. Months
            .Zip(SalesTargets, (month, target) => new { month, target })
            .Zip(ActualSales, (monthTarget, actual) => new MonthlyPerformance
            {
                Month = monthTarget.month,
                SalesTarget = monthTarget.target,
                ActualSales = actual,
                Difference = actual - monthTarget.target
            })
            .OrderByDescending(performance => Math.Abs(performance.Difference))
            .ThenBy(performance => performance.Month)];
    }
}

/*
 === SALES PERFORMANCE REPORT ===

Month: April
  Target:       1200
  Actual:       1300
  Difference:   100

Month: December
  Target:       1700
  Actual:       1800
  Difference:   100

Month: February
  Target:       1500
  Actual:       1400
  Difference:   -100

Month: January
  Target:       1000
  Actual:       1100
  Difference:   100

Month: March
  Target:       1300
  Actual:       1200
  Difference:   -100

Month: October
  Target:       1100
  Actual:       1000
  Difference:   -100

Month: August
  Target:       1550
  Actual:       1600
  Difference:   50

Month: July
  Target:       1350
  Actual:       1300
  Difference:   -50

Month: May
  Target:       1600
  Actual:       1550
  Difference:   -50

Month: November
  Target:       1250
  Actual:       1200
  Difference:   -50

Month: September
  Target:       1450
  Actual:       1500
  Difference:   50

Month: June
  Target:       1400
  Actual:       1380
  Difference:   -20

 */

/*
This method, GetSalesPerformanceReport, generates a monthly sales performance report by combining data from three lists (Months, SalesTargets, and ActualSales).

* 1.First Zip Operation: The method starts by using the Zip function to pair each month with its corresponding sales target. This creates an intermediate collection where each element holds a month and target value, representing the month name and the target sales for that month.

* 2.Second Zip Operation: Next, it performs a second Zip operation that combines this intermediate result (containing month and target pairs) with ActualSales, the actual sales for each month. This second Zip creates a MonthlyPerformance object for each month, assigning values for Month, SalesTarget, ActualSales, and Difference. The Difference is calculated by subtracting SalesTarget from ActualSales, giving the performance deviation for that month.

* 3.Sorting: After creating the list of MonthlyPerformance objects, the method sorts the data. The sorting is done in two steps:

First, it orders the results by the absolute value of Difference in descending order, showing the months with the largest deviations (either positive or negative) first.

Then, it applies a secondary ordering by Month to resolve ties in deviation by alphabetically sorting months with the same deviation.

* 4.ToList Conversion: Finally, the sorted data is converted to a List<MonthlyPerformance>, which the method returns as the monthly performance report.
 
*/
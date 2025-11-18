using System.Diagnostics.Metrics;
using System.Xml;

class Program
{
    static void Main()
    {
        var customerRepository = new CustomerRepository();

        var summaries = customerRepository.GetPurchasingTrendsByRegion();

        Console.WriteLine("=== Purchasing Trends By Region ===");

        foreach (var summary in summaries)
        {
            Console.WriteLine($"\nRegion: {summary.RegionName}");
            Console.WriteLine($"Total Customers: {summary.TotalCustomers}");
            Console.WriteLine($"Total Purchases: {summary.TotalPurchases}");
            Console.WriteLine($"Total Amount Spent: {summary.TotalAmountSpent}");
            Console.WriteLine($"Average Spending / Customer: {summary.AverageSpendingPerCustomer}");
        }

        Console.ReadKey();
    }
}
public class Region
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class Customer
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int RegionId { get; set; }
}

public class Purchase
{
    public int CustomerId { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
}

public class RegionalPurchaseSummary
{
    public required string RegionName { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalPurchases { get; set; }
    public decimal TotalAmountSpent { get; set; }
    public decimal AverageSpendingPerCustomer { get; set; }
}

public class CustomerRepository
{
    public List<Region> Regions { get; set; } =
    [
        new Region { Id = 1, Name = "North" },
        new Region { Id = 2, Name = "South" }
    ];

    public List<Customer> Customers { get; set; } =
    [
        new Customer { Id = 1, Name = "Alice", RegionId = 1 },
        new Customer { Id = 2, Name = "Bob", RegionId = 1 },
        new Customer { Id = 3, Name = "Charlie", RegionId = 2 }
    ];

    public List<Purchase> Purchases { get; set; } =
    [
        new Purchase { CustomerId = 1, Date = new DateTime(2023, 10, 5), Amount = 300m },
        new Purchase { CustomerId = 1, Date = new DateTime(2023, 10, 10), Amount = 150m },
        new Purchase { CustomerId = 2, Date = new DateTime(2023, 10, 15), Amount = 200m },
        new Purchase { CustomerId = 3, Date = new DateTime(2023, 11, 20), Amount = 500m }
    ];

    public List<RegionalPurchaseSummary> GetPurchasingTrendsByRegion()
    {
        return [.. Regions
            .GroupJoin(Customers,
                region => region.Id,
                customer => customer.RegionId,
                (region, regionCustomers) => new
                {
                    RegionName = region.Name,
                    Customers = regionCustomers.Select(c => c.Id).Distinct(),
                    Purchases = regionCustomers
                        .SelectMany(customer => Purchases.Where(p => p.CustomerId == customer.Id))
                })
            .Select(summary => new RegionalPurchaseSummary
            {
                RegionName = summary.RegionName,
                TotalCustomers = summary.Customers.Count(),
                TotalPurchases = summary.Purchases.Count(),
                TotalAmountSpent = summary.Purchases.Sum(p => p.Amount),
                AverageSpendingPerCustomer = summary.Customers.Any() ? summary.Purchases.Sum(p => p.Amount) / summary.Customers.Count() : 0
            })
            .OrderByDescending(summary => summary.TotalAmountSpent)];
    }
}


/*
 !=== Purchasing Trends By Region ===

Region: North
Total Customers: 2
Total Purchases: 3
Total Amount Spent: 650
Average Spending / Customer: 325

Region: South
Total Customers: 1
Total Purchases: 1
Total Amount Spent: 500
Average Spending / Customer: 500
 
 */


/*
 ! This exercise generates a regional purchase summary by combining data across regions, customers, and purchases.

* 1.Grouping Purchases by Region:

GroupJoin(Customers, ...) links each region to its customers.

SelectMany(customer => Purchases.Where(p => p.CustomerId == customer.Id)) collects all purchases for customers in the region.

* 2.Calculating Regional Purchase Metrics:

TotalCustomers: Counts the unique customers in each region.

TotalPurchases: Counts all purchases made in each region.

TotalAmountSpent: Sums the amount of all purchases in each region.

AverageSpendingPerCustomer: Calculates average spending per customer.

* 3. Returning the Report:

The result is a list of RegionalPurchaseSummary objects, sorted by TotalAmountSpent in descending order.
*/
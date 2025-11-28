using System.Collections.Concurrent;

public class Program
{
    static void Main()
    {
        var repo = new SalesRepository();
        var topSpenders = repo.GetTopSpendersByQuarter_PLINQ();

        Console.WriteLine("=== Quarterly Top Spenders ===");
        foreach (var summary in topSpenders)
        {
            Console.WriteLine($"{summary.Quarter}: {summary.CustomerName} (Total Spent: {summary.TotalAmountSpent})");
        }

        Console.ReadKey();
    }
}

public class Customer
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class Sale
{
    public int CustomerId { get; set; }
    public DateTime PurchaseDate { get; set; }
    public decimal Amount { get; set; }
}

public class QuarterlyTopSpenderSummary
{
    public required string Quarter { get; set; }
    public required string CustomerName { get; set; }
    public decimal TotalAmountSpent { get; set; }
}

public class SalesRepository
{
    public List<Customer> Customers { get; set; } =
    [
        new Customer { Id = 1, Name = "Alice" },
        new Customer { Id = 2, Name = "Bob" },
        new Customer { Id = 3, Name = "Charlie" }
    ];

    public List<Sale> Sales { get; set; } =
    [
        new Sale { CustomerId = 1, PurchaseDate = new DateTime(2024, 1, 15), Amount = 500 },
        new Sale { CustomerId = 1, PurchaseDate = new DateTime(2024, 2, 20), Amount = 300 },
        new Sale { CustomerId = 2, PurchaseDate = new DateTime(2024, 3, 10), Amount = 400 },
        new Sale { CustomerId = 3, PurchaseDate = new DateTime(2024, 4, 5), Amount = 700 },
        new Sale { CustomerId = 2, PurchaseDate = new DateTime(2024, 4, 25), Amount = 600 }
    ];

    public List<QuarterlyTopSpenderSummary> GetTopSpendersByQuarter()
    {
        var topSpenders = Sales
            .GroupBy(sale => new
            {
                sale.PurchaseDate.Year,
                Quarter = (sale.PurchaseDate.Month - 1) / 3 + 1,
                sale.CustomerId
            })
            .Select(g => new
            {
                Year = g.Key.Year,
                Quarter = g.Key.Quarter,
                CustomerId = g.Key.CustomerId,
                TotalAmountSpent = g.Sum(sale => sale.Amount)
            })
            .GroupBy(g => new { g.Year, g.Quarter })
            .Select(quarterGroup => quarterGroup
                .OrderByDescending(g => g.TotalAmountSpent)
                .First())
            .Join(Customers,
                  quarterSpender => quarterSpender.CustomerId,
                  customer => customer.Id,
                  (quarterSpender, customer) => new QuarterlyTopSpenderSummary
                  {
                      Quarter = $"Q{quarterSpender.Quarter} {quarterSpender.Year}",
                      CustomerName = customer.Name,
                      TotalAmountSpent = quarterSpender.TotalAmountSpent
                  })
            .OrderBy(summary => summary.Quarter)
            .ToList();

        return topSpenders;
    }

    public List<QuarterlyTopSpenderSummary> GetTopSpendersByQuarter_Dictionary()
    {
        // 1) Tạo lookup Customer O(1)
        var customerDict = Customers.ToDictionary(c => c.Id, c => c.Name);

        // 2) Gom Sales theo (Year, Quarter, CustomerId)
        var salesDict = new Dictionary<(int Year, int Quarter, int CustomerId), decimal>();

        foreach (var sale in Sales)
        {
            int quarter = (sale.PurchaseDate.Month - 1) / 3 + 1;
            var key = (sale.PurchaseDate.Year, quarter, sale.CustomerId);

            if (!salesDict.ContainsKey(key))
                salesDict[key] = 0;

            salesDict[key] += sale.Amount;
        }

        // 3) Tìm top spender mỗi Quarter
        var topSpendersPerQuarter = new Dictionary<(int Year, int Quarter), (int CustomerId, decimal TotalAmount)>();

        foreach (var kvp in salesDict)
        {
            var yearQuarter = (kvp.Key.Year, kvp.Key.Quarter);
            var customerId = kvp.Key.CustomerId;
            var total = kvp.Value;

            if (!topSpendersPerQuarter.ContainsKey(yearQuarter))
            {
                topSpendersPerQuarter[yearQuarter] = (customerId, total);
            }
            else
            {
                var existing = topSpendersPerQuarter[yearQuarter];

                // nếu total lớn hơn → cập nhật
                // nếu bằng → giữ CustomerId nhỏ hơn (giống LINQ First())
                if (total > existing.TotalAmount ||
                    (total == existing.TotalAmount && customerId < existing.CustomerId))
                {
                    topSpendersPerQuarter[yearQuarter] = (customerId, total);
                }
            }
        }

        // 4) Chuyển sang danh sách QuarterlyTopSpenderSummary
        var result = new List<QuarterlyTopSpenderSummary>();

        foreach (var kvp in topSpendersPerQuarter)
        {
            result.Add(new QuarterlyTopSpenderSummary
            {
                Quarter = $"Q{kvp.Key.Quarter} {kvp.Key.Year}",
                CustomerName = customerDict[kvp.Value.CustomerId],
                TotalAmountSpent = kvp.Value.TotalAmount
            });
        }

        // Không OrderBy → giữ thứ tự giống code gốc
        return result;
    }

    public List<QuarterlyTopSpenderSummary> GetTopSpendersByQuarter_Parallel()
    {
        // 1) Cache Customer lookup O(1)
        var customerDict = Customers.ToDictionary(c => c.Id, c => c.Name);

        // 2) Gom Sales theo (Year, Quarter, CustomerId) song song
        var salesDict = new ConcurrentDictionary<(int Year, int Quarter, int CustomerId), decimal>();

        Parallel.ForEach(Sales, sale =>
        {
            int quarter = (sale.PurchaseDate.Month - 1) / 3 + 1;
            var key = (sale.PurchaseDate.Year, quarter, sale.CustomerId);

            salesDict.AddOrUpdate(key, sale.Amount, (k, old) => old + sale.Amount);
        });

        // 3) Tìm top spender mỗi Quarter
        var topSpendersPerQuarter = new ConcurrentDictionary<(int Year, int Quarter), (int CustomerId, decimal TotalAmount)>();

        foreach (var kvp in salesDict)
        {
            var yearQuarter = (kvp.Key.Year, kvp.Key.Quarter);
            var customerId = kvp.Key.CustomerId;
            var total = kvp.Value;

            topSpendersPerQuarter.AddOrUpdate(
                yearQuarter,
                (customerId, total),
                (k, existing) =>
                {
                    if (total > existing.TotalAmount ||
                        (total == existing.TotalAmount && customerId < existing.CustomerId))
                    {
                        return (customerId, total);
                    }
                    return existing;
                });
        }

        // 4) Chuyển sang danh sách QuarterlyTopSpenderSummary
        var result = new List<QuarterlyTopSpenderSummary>();
        foreach (var kvp in topSpendersPerQuarter)
        {
            result.Add(new QuarterlyTopSpenderSummary
            {
                Quarter = $"Q{kvp.Key.Quarter} {kvp.Key.Year}",
                CustomerName = customerDict[kvp.Value.CustomerId],
                TotalAmountSpent = kvp.Value.TotalAmount
            });
        }

        // Không OrderBy → giữ thứ tự xuất hiện giống code gốc
        return result;
    }

    public List<QuarterlyTopSpenderSummary> GetTopSpendersByQuarter_PLINQ()
    {
        // 1) Lookup Customer O(1)
        var customerDict = Customers.ToDictionary(c => c.Id, c => c.Name);

        // 2) Gom Sales theo (Year, Quarter, CustomerId) song song
        var salesGrouped = Sales
            .AsParallel()
            .GroupBy(sale => (sale.PurchaseDate.Year, Quarter: (sale.PurchaseDate.Month - 1) / 3 + 1, sale.CustomerId))
            .Select(g => new
            {
                Year = g.Key.Year,
                Quarter = g.Key.Quarter,
                CustomerId = g.Key.CustomerId,
                TotalAmount = g.Sum(s => s.Amount)
            })
            .ToList(); // chuyển sang danh sách để dùng tiếp

        // 3) Tìm top spender mỗi Quarter
        var topSpendersPerQuarter = salesGrouped
            .GroupBy(x => (x.Year, x.Quarter))
            .Select(g =>
                g.OrderByDescending(x => x.TotalAmount)
                 .ThenBy(x => x.CustomerId) // nếu bằng → ProductId nhỏ hơn (giống LINQ First())
                 .First()
            )
            .ToList();

        // 4) Chuyển sang danh sách QuarterlyTopSpenderSummary
        var result = topSpendersPerQuarter
            .Select(x => new QuarterlyTopSpenderSummary
            {
                Quarter = $"Q{x.Quarter} {x.Year}",
                CustomerName = customerDict[x.CustomerId],
                TotalAmountSpent = x.TotalAmount
            })
            .ToList();

        // Không OrderBy cuối → giữ thứ tự giống code gốc
        return result;
    }
}

/*
 !=== Quarterly Top Spenders ===
    Q1 2024: Alice (Total Spent: 800)
    Q2 2024: Charlie (Total Spent: 700)
 */

/*
 !This exercise identifies the highest-spending customers by quarter.

* 1.Grouping by Quarter and Customer:

GroupBy(sale => new { sale.PurchaseDate.Year, Quarter = (sale.PurchaseDate.Month - 1) / 3 + 1, sale.CustomerId }) groups sales by quarter and customer.

* 2.Calculating Total Amount Spent:

Sum(sale => sale.Amount) calculates total spending for each customer within each quarter.

* 3.Selecting Top Spender per Quarter:

OrderByDescending(g => g.TotalAmountSpent).First() selects the highest spender for each quarter.

Returning the Summary:

* 4.The result is a list of QuarterlyTopSpenderSummary objects, sorted by Quarter in ascending order.
 
 */


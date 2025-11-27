using System.Collections.Concurrent;

public class Program
{
    static void Main()
    {
        var repo = new TransactionRepository();

        // Chọn khoảng thời gian để lọc giao dịch
        DateTime start = new(2024, 1, 1);
        DateTime end = new(2024, 1, 31);

        var results = repo.GetTopSpendersInDateRange(start, end);

        Console.WriteLine("=== Top Spenders ===");
        Console.WriteLine($"From {start:yyyy-MM-dd} to {end:yyyy-MM-dd}");
        Console.WriteLine();

        foreach (var r in results)
        {
            Console.WriteLine($"Customer: {r.CustomerName}");
            Console.WriteLine($"  Total Spent: ${r.TotalSpent}");
            Console.WriteLine($"  Transactions: {r.TransactionCount}");
            Console.WriteLine();
        }

        Console.ReadKey();
    }
}

public class Customer
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class Transaction
{
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
}

public class CustomerSpendingSummary
{
    public required string CustomerName { get; set; }
    public decimal TotalSpent { get; set; }
    public int TransactionCount { get; set; }
}

public class TransactionRepository
{
    public List<Customer> Customers { get; set; } =
    [
        new Customer { Id = 1, Name = "Alice" },
        new Customer { Id = 2, Name = "Bob" },
        new Customer { Id = 3, Name = "Charlie" }
    ];

    public List<Transaction> Transactions { get; set; } =
    [
        new Transaction { CustomerId = 1, Amount = 150, TransactionDate = new DateTime(2024, 1, 10) },
        new Transaction { CustomerId = 1, Amount = 350, TransactionDate = new DateTime(2024, 1, 15) },
        new Transaction { CustomerId = 2, Amount = 200, TransactionDate = new DateTime(2024, 1, 20) },
        new Transaction { CustomerId = 3, Amount = 120, TransactionDate = new DateTime(2024, 1, 25) },
        new Transaction { CustomerId = 3, Amount = 300, TransactionDate = new DateTime(2024, 1, 30) },
        new Transaction { CustomerId = 1, Amount = 100, TransactionDate = new DateTime(2024, 2, 1) }
    ];

    public List<CustomerSpendingSummary> GetTopSpendersInDateRange(DateTime startDate, DateTime endDate)
    {
        var topSpenders = Transactions
            .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
            .GroupBy(t => t.CustomerId)
            .Select(g => new
            {
                CustomerId = g.Key,
                TotalSpent = g.Sum(t => t.Amount),
                TransactionCount = g.Count()
            })
            .OrderByDescending(c => c.TotalSpent)
            .Take(3)
            .Join(Customers,
                  c => c.CustomerId,
                  customer => customer.Id,
                  (c, customer) => new CustomerSpendingSummary
                  {
                      CustomerName = customer.Name,
                      TotalSpent = c.TotalSpent,
                      TransactionCount = c.TransactionCount
                  })
            .ToList();

        return topSpenders;
    }

    public List<CustomerSpendingSummary> GetTopSpendersInDateRange_Optimized(
    DateTime startDate, DateTime endDate)
    {
        // Dictionary để lưu tổng chi tiêu và số giao dịch
        var spendingDict = new Dictionary<int, (decimal totalSpent, int count)>();

        foreach (var t in Transactions)
        {
            // Bỏ qua giao dịch nằm ngoài thời gian
            if (t.TransactionDate < startDate || t.TransactionDate > endDate)
                continue;

            if (!spendingDict.ContainsKey(t.CustomerId))
            {
                spendingDict[t.CustomerId] = (t.Amount, 1);
            }
            else
            {
                var entry = spendingDict[t.CustomerId];
                spendingDict[t.CustomerId] = (entry.totalSpent + t.Amount, entry.count + 1);
            }
        }

        // Lấy dictionary khách hàng để truy xuất O(1)
        var customerDict = Customers.ToDictionary(c => c.Id, c => c.Name);

        // Chuyển sang danh sách kết quả
        var summaries = spendingDict
            .Select(kvp => new CustomerSpendingSummary
            {
                CustomerName = customerDict[kvp.Key],
                TotalSpent = kvp.Value.totalSpent,
                TransactionCount = kvp.Value.count
            })
            .OrderByDescending(s => s.TotalSpent)
            .Take(3)
            .ToList();

        return summaries;
    }

    public List<CustomerSpendingSummary> GetTopSpendersInDateRange_Parallel(
    DateTime startDate, DateTime endDate)
    {
        var spendingDict = new ConcurrentDictionary<int, (decimal totalSpent, int count)>();

        Parallel.ForEach(Transactions, t =>
        {
            if (t.TransactionDate < startDate || t.TransactionDate > endDate)
                return;

            spendingDict.AddOrUpdate(
                t.CustomerId,
                (t.Amount, 1),
                (id, oldValue) => (
                    oldValue.totalSpent + t.Amount,
                    oldValue.count + 1
                )
            );
        });

        var customerDict = Customers.ToDictionary(c => c.Id, c => c.Name);

        var results = spendingDict
            .Select(kvp => new CustomerSpendingSummary
            {
                CustomerName = customerDict[kvp.Key],
                TotalSpent = kvp.Value.totalSpent,
                TransactionCount = kvp.Value.count
            })
            .OrderByDescending(s => s.TotalSpent)
            .Take(3)
            .ToList();

        return results;
    }

    public List<CustomerSpendingSummary> GetTopSpendersInDateRange_PLINQ(
    DateTime startDate, DateTime endDate)
    {
        var customerDict = Customers.ToDictionary(c => c.Id, c => c.Name);

        var results = Transactions
            .AsParallel() // chạy song song ở đây
            .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
            .GroupBy(t => t.CustomerId)
            .Select(g => new
            {
                CustomerId = g.Key,
                TotalSpent = g.Sum(t => t.Amount),
                Count = g.Count()
            })
            .OrderByDescending(x => x.TotalSpent)
            .Take(3)
            .Select(x => new CustomerSpendingSummary
            {
                CustomerName = customerDict[x.CustomerId],
                TotalSpent = x.TotalSpent,
                TransactionCount = x.Count
            })
            .ToList();

        return results;
    }

}

/*
 !=== Top Spenders ===
    From 2024-01-01 to 2024-01-31

    Customer: Alice
      Total Spent: $500
      Transactions: 2

    Customer: Charlie
      Total Spent: $420
      Transactions: 2

    Customer: Bob
      Total Spent: $200
      Transactions: 1
 */

/*
 !This exercise identifies the top-spending customers within a specified date range.

    * 1.Filtering Transactions by Date:

    Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate) filters transactions within the specified date range.

    * 2.Grouping by Customer:

    GroupBy(t => t.CustomerId) groups transactions by each customer.

    * 3.Calculating Total Spending:

    Sum(t => t.Amount) calculates total spending for each customer.

    * 4.Selecting Top Spenders:

    OrderByDescending(c => c.TotalSpent).Take(3) selects the top 3 customers by total spending.

    * 5.Returning the Summary:

    The result is a list of CustomerSpendingSummary objects, sorted by TotalSpent in descending order.
 */
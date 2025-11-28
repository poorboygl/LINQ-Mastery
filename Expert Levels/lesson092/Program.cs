public class Program
{
    static void Main()
    {
        var repo = new PurchaseRepository();

        var results = repo.GetLongestPurchaseStreaks();

        Console.WriteLine("=== Longest Purchase Streaks ===\n");

        foreach (var r in results)
        {
            Console.WriteLine($"Customer: {r.CustomerName}");
            Console.WriteLine($"  Longest Streak: {r.LongestStreakDays} days");
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

public class Purchase
{
    public int CustomerId { get; set; }
    public DateTime PurchaseDate { get; set; }
}

public class CustomerStreakSummary
{
    public required string CustomerName { get; set; }
    public int LongestStreakDays { get; set; }
}

public class PurchaseRepository
{
    public List<Customer> Customers { get; set; } =
    [
        new Customer { Id = 1, Name = "Alice" },
        new Customer { Id = 2, Name = "Bob" },
        new Customer { Id = 3, Name = "Charlie" }
    ];

    public List<Purchase> Purchases { get; set; } =
    [
        new Purchase { CustomerId = 1, PurchaseDate = new DateTime(2024, 1, 1) },
        new Purchase { CustomerId = 1, PurchaseDate = new DateTime(2024, 1, 2) },
        new Purchase { CustomerId = 1, PurchaseDate = new DateTime(2024, 1, 4) },
        new Purchase { CustomerId = 2, PurchaseDate = new DateTime(2024, 1, 5) },
        new Purchase { CustomerId = 2, PurchaseDate = new DateTime(2024, 1, 6) },
        new Purchase { CustomerId = 3, PurchaseDate = new DateTime(2024, 1, 7) },
        new Purchase { CustomerId = 3, PurchaseDate = new DateTime(2024, 1, 8) },
        new Purchase { CustomerId = 3, PurchaseDate = new DateTime(2024, 1, 9) }
    ];

    public List<CustomerStreakSummary> GetLongestPurchaseStreaks()
    {
        var streaks = Purchases
            .GroupBy(p => p.CustomerId)
            .Select(g =>
            {
                var orderedDates = g.Select(p => p.PurchaseDate).OrderBy(date => date).ToList();

                int maxStreak = 1;
                int currentStreak = 1;

                for (int i = 1; i < orderedDates.Count; i++)
                {
                    if ((orderedDates[i] - orderedDates[i - 1]).Days == 1)
                    {
                        currentStreak++;
                        maxStreak = Math.Max(maxStreak, currentStreak);
                    }
                    else
                    {
                        currentStreak = 1;
                    }
                }

                var customerName = Customers.First(c => c.Id == g.Key).Name;

                return new CustomerStreakSummary
                {
                    CustomerName = customerName,
                    LongestStreakDays = maxStreak
                };
            })
            .OrderBy(summary => summary.CustomerName)
            .ToList();

        return streaks;
    }

    public List<CustomerStreakSummary> GetLongestPurchaseStreaks_Dictionary()
    {
        // 1) Cache customers vào dictionary O(1)
        var customerDict = Customers.ToDictionary(c => c.Id, c => c.Name);

        // 2) Gom purchase dates theo customerId
        var purchaseDict = new Dictionary<int, List<DateTime>>();

        foreach (var p in Purchases)
        {
            if (!purchaseDict.ContainsKey(p.CustomerId))
                purchaseDict[p.CustomerId] = new List<DateTime>();

            purchaseDict[p.CustomerId].Add(p.PurchaseDate);
        }

        // 3) Tính streak cho từng customer
        var results = new List<CustomerStreakSummary>();

        foreach (var kvp in purchaseDict)
        {
            int customerId = kvp.Key;
            List<DateTime> dates = kvp.Value;

            // Sort ngày
            dates.Sort();

            int maxStreak = 1;
            int currentStreak = 1;

            for (int i = 1; i < dates.Count; i++)
            {
                if ((dates[i] - dates[i - 1]).Days == 1)
                {
                    currentStreak++;
                    if (currentStreak > maxStreak)
                        maxStreak = currentStreak;
                }
                else
                {
                    currentStreak = 1;
                }
            }

            results.Add(new CustomerStreakSummary
            {
                CustomerName = customerDict[customerId],
                LongestStreakDays = maxStreak
            });
        }

        // 4) Sort theo tên khách hàng
        return results.OrderBy(r => r.CustomerName).ToList();
    }


}

/*
 !=== Longest Purchase Streaks ===

Customer: Alice
  Longest Streak: 2 days

Customer: Bob
  Longest Streak: 2 days

Customer: Charlie
  Longest Streak: 3 days
 */

/*
 !This exercise identifies the longest streak of consecutive purchase days for each customer.

    * 1.Grouping by Customer:

    GroupBy(p => p.CustomerId) groups purchases by each customer.

    * 2.Sorting and Counting Consecutive Days:

    OrderBy(date => date) orders purchase dates. A loop then calculates the longest streak of consecutive days, resetting the count when a gap is detected.

    * 3.Returning the Summary:

    The result is a list of CustomerStreakSummary objects, sorted alphabetically by CustomerName.
 
 */
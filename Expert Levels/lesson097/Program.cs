using System.Collections.Concurrent;

public class Program
{
    static void Main()
    {
        var repo = new SubscriptionRepository();
        var results = repo.GetLongestSubscriptionStreaks();

        Console.WriteLine("=== Longest Subscription Streaks ===\n");

        foreach (var r in results)
        {
            Console.WriteLine($"{r.CustomerName}: {r.LongestStreakDays} days");
        }

        Console.ReadKey();
    }
}

public class Customer
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class Subscription
{
    public int CustomerId { get; set; }
    public DateTime SubscriptionStartDate { get; set; }
    public DateTime SubscriptionEndDate { get; set; }
}

public class SubscriptionStreakSummary
{
    public required string CustomerName { get; set; }
    public int LongestStreakDays { get; set; }
}

public class SubscriptionRepository
{
    public List<Customer> Customers { get; set; } =
    [
        new Customer { Id = 1, Name = "Alice" },
        new Customer { Id = 2, Name = "Bob" },
        new Customer { Id = 3, Name = "Charlie" }
    ];

    public List<Subscription> Subscriptions { get; set; } =
    [
        new Subscription { CustomerId = 1, SubscriptionStartDate = new DateTime(2024, 1, 1), SubscriptionEndDate = new DateTime(2024, 1, 15) },
        new Subscription { CustomerId = 1, SubscriptionStartDate = new DateTime(2024, 1, 16), SubscriptionEndDate = new DateTime(2024, 1, 30) },
        new Subscription { CustomerId = 2, SubscriptionStartDate = new DateTime(2024, 2, 1), SubscriptionEndDate = new DateTime(2024, 2, 5) },
        new Subscription { CustomerId = 2, SubscriptionStartDate = new DateTime(2024, 2, 10), SubscriptionEndDate = new DateTime(2024, 2, 20) },
        new Subscription { CustomerId = 3, SubscriptionStartDate = new DateTime(2024, 1, 1), SubscriptionEndDate = new DateTime(2024, 1, 10) },
        new Subscription { CustomerId = 3, SubscriptionStartDate = new DateTime(2024, 1, 11), SubscriptionEndDate = new DateTime(2024, 1, 25) }
    ];

    public List<SubscriptionStreakSummary> GetLongestSubscriptionStreaks()
    {
        var longestStreaks = Subscriptions
            .GroupBy(sub => sub.CustomerId)
            .Select(g =>
            {
                var orderedPeriods = g.OrderBy(sub => sub.SubscriptionStartDate).ToList();

                int longestStreak = 0;
                int currentStreak = 0;
                DateTime? previousEndDate = null;

                foreach (var period in orderedPeriods)
                {
                    if (previousEndDate != null && period.SubscriptionStartDate <= previousEndDate.Value.AddDays(1))
                    {
                        currentStreak += (period.SubscriptionEndDate - period.SubscriptionStartDate).Days + 1;
                    }
                    else
                    {
                        currentStreak = (period.SubscriptionEndDate - period.SubscriptionStartDate).Days + 1;
                    }

                    if (currentStreak > longestStreak)
                    {
                        longestStreak = currentStreak;
                    }

                    previousEndDate = period.SubscriptionEndDate;
                }

                var customerName = Customers.First(c => c.Id == g.Key).Name;

                return new SubscriptionStreakSummary
                {
                    CustomerName = customerName,
                    LongestStreakDays = longestStreak
                };
            })
            .OrderByDescending(summary => summary.LongestStreakDays)
            .ToList();

        return longestStreaks;
    }

    public List<SubscriptionStreakSummary> GetLongestSubscriptionStreaks_Dictionary()
    {
        // Map CustomerId -> CustomerName (O(1) lookup)
        var customerNames = Customers.ToDictionary(c => c.Id, c => c.Name);

        // Map CustomerId -> list of subscriptions
        var subsByCustomer = new Dictionary<int, List<Subscription>>();

        foreach (var sub in Subscriptions)
        {
            if (!subsByCustomer.ContainsKey(sub.CustomerId))
                subsByCustomer[sub.CustomerId] = new List<Subscription>();

            subsByCustomer[sub.CustomerId].Add(sub);
        }

        var results = new List<SubscriptionStreakSummary>();

        foreach (var kv in subsByCustomer)
        {
            int customerId = kv.Key;
            var periods = kv.Value.OrderBy(s => s.SubscriptionStartDate).ToList();

            int longest = 0;
            int current = 0;
            DateTime? previousEnd = null;

            foreach (var p in periods)
            {
                int duration = (p.SubscriptionEndDate - p.SubscriptionStartDate).Days + 1;

                if (previousEnd != null && p.SubscriptionStartDate <= previousEnd.Value.AddDays(1))
                {
                    current += duration;
                }
                else
                {
                    current = duration;
                }

                if (current > longest)
                    longest = current;

                previousEnd = p.SubscriptionEndDate;
            }

            results.Add(new SubscriptionStreakSummary
            {
                CustomerName = customerNames[customerId],
                LongestStreakDays = longest
            });
        }

        return results
            .OrderByDescending(r => r.LongestStreakDays)
            .ToList();
    }
    public List<SubscriptionStreakSummary> GetLongestSubscriptionStreaks_Parallel()
    {
        // Map CustomerId -> CustomerName (O(1) lookup)
        var customerNames = Customers.ToDictionary(c => c.Id, c => c.Name);

        // Map CustomerId -> list of subscriptions
        var subsByCustomer = new Dictionary<int, List<Subscription>>();

        foreach (var sub in Subscriptions)
        {
            if (!subsByCustomer.ContainsKey(sub.CustomerId))
                subsByCustomer[sub.CustomerId] = new List<Subscription>();

            subsByCustomer[sub.CustomerId].Add(sub);
        }

        // Thread-safe result collection
        var results = new ConcurrentBag<SubscriptionStreakSummary>();

        // Process each customer in parallel
        Parallel.ForEach(subsByCustomer, kv =>
        {
            int customerId = kv.Key;
            var periods = kv.Value.OrderBy(s => s.SubscriptionStartDate).ToList();

            int longest = 0;
            int current = 0;
            DateTime? previousEnd = null;

            foreach (var p in periods)
            {
                int duration = (p.SubscriptionEndDate - p.SubscriptionStartDate).Days + 1;

                if (previousEnd != null && p.SubscriptionStartDate <= previousEnd.Value.AddDays(1))
                {
                    current += duration;
                }
                else
                {
                    current = duration;
                }

                if (current > longest)
                    longest = current;

                previousEnd = p.SubscriptionEndDate;
            }

            results.Add(new SubscriptionStreakSummary
            {
                CustomerName = customerNames[customerId],
                LongestStreakDays = longest
            });
        });

        return results
            .OrderByDescending(r => r.LongestStreakDays)
            .ToList();
    }

    public List<SubscriptionStreakSummary> GetLongestSubscriptionStreaks_PLINQ()
    {
        // Customer lookup O(1)
        var customerNames = Customers.ToDictionary(c => c.Id, c => c.Name);

        // Group subscriptions by customer (normal, not parallel)
        var grouped = Subscriptions
            .GroupBy(s => s.CustomerId)
            .ToDictionary(g => g.Key, g => g.OrderBy(s => s.SubscriptionStartDate).ToList());

        // Parallel processing each customer streak
        var results = grouped
            .AsParallel()
            .Select(kv =>
            {
                int customerId = kv.Key;
                var periods = kv.Value;

                int longest = 0;
                int current = 0;
                DateTime? previousEnd = null;

                foreach (var p in periods)
                {
                    int duration = (p.SubscriptionEndDate - p.SubscriptionStartDate).Days + 1;

                    if (previousEnd != null && p.SubscriptionStartDate <= previousEnd.Value.AddDays(1))
                    {
                        current += duration;
                    }
                    else
                    {
                        current = duration;
                    }

                    if (current > longest)
                        longest = current;

                    previousEnd = p.SubscriptionEndDate;
                }

                return new SubscriptionStreakSummary
                {
                    CustomerName = customerNames[customerId],
                    LongestStreakDays = longest
                };
            })
            .ToList();

        return results
            .OrderByDescending(r => r.LongestStreakDays)
            .ToList();
    }
}

/*
 !=== Longest Subscription Streaks ===

    Alice: 30 days
    Charlie: 25 days
    Bob: 11 days
 */

/*
 !This exercise identifies the longest continuous subscription streak for each customer.

* 1.Grouping by Customer:

GroupBy(sub => sub.CustomerId) groups subscriptions by each customer.

* 2.Sorting and Counting Consecutive Days:

Subscriptions are ordered by SubscriptionStartDate. A loop then checks if each period is consecutive with the previous period, accumulating the days if they are.

* 3.Calculating Longest Streak:

longestStreak is updated whenever currentStreak exceeds it.

* 4.Returning the Summary:

The result is a list of SubscriptionStreakSummary objects, sorted by LongestStreakDays in descending order.
 
 */
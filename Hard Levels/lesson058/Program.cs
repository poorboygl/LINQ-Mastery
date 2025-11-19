class Program
{
    static void Main()
    {
        var repo = new CustomerRepository();

        // var test = new Dictionary<object, object>();

        // repo.Test(out test);

        Console.WriteLine("=== CUSTOMER SPENDING REPORT ===\n");

        var results = repo.GetMonthlySpendingTrends();

        foreach (var customer in results)
        {
            Console.WriteLine($"Customer: {customer.CustomerName}");

            foreach (var growth in customer.MonthlyGrowth)
            {
                Console.WriteLine($"  Month: {growth.Month} | Growth: {growth.GrowthRate:F2}%");
            }

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
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
}

public class MonthlySpendingGrowth
{
    public required string Month { get; set; }
    public decimal GrowthRate { get; set; }
}

public class CustomerSpendingTrendSummary
{
    public required string CustomerName { get; set; }
    public List<MonthlySpendingGrowth> MonthlyGrowth { get; set; } = [];
}

public class CustomerRepository
{
    public List<Customer> Customers { get; set; } =
    [
        new Customer { Id = 1, Name = "Alice" },
        new Customer { Id = 2, Name = "Bob" }
    ];

    public List<Transaction> Transactions { get; set; } =
    [
        new Transaction { CustomerId = 1, TransactionDate = new DateTime(2023, 1, 15), Amount = 100 },
        new Transaction { CustomerId = 1, TransactionDate = new DateTime(2023, 2, 15), Amount = 150 },
        new Transaction { CustomerId = 1, TransactionDate = new DateTime(2023, 3, 15), Amount = 120 },
        new Transaction { CustomerId = 2, TransactionDate = new DateTime(2023, 1, 20), Amount = 80 },
        new Transaction { CustomerId = 2, TransactionDate = new DateTime(2023, 2, 20), Amount = 90 }
    ];

    public List<CustomerSpendingTrendSummary> GetMonthlySpendingTrends_2()
    {

        /*
         ! === CUSTOMER SPENDING REPORT ===

        Customer: Alice
          Month: January 2023 | Growth: 0.00%
          Month: February 2023 | Growth: 50.00%
          Month: March 2023 | Growth: -20.00%

        Customer: Bob
          Month: January 2023 | Growth: 0.00%
          Month: February 2023 | Growth: 12.50%
        */
        var result = Customers.GroupJoin(
                       Transactions,
                       c => c.Id,
                       t => t.CustomerId,
                       (customer, trans) => new CustomerSpendingTrendSummary
                       {
                           CustomerName = customer.Name,
                           MonthlyGrowth = trans
                               .GroupBy(t => new { t.TransactionDate.Year, t.TransactionDate.Month })
                               .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                               .Select((g, i) => new MonthlySpendingGrowth
                               {
                                   Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy"),
                                   GrowthRate = i == 0 ? 0 : ((g.Sum(t => t.Amount) - trans.GroupBy(t2 => new { t2.TransactionDate.Year, t2.TransactionDate.Month }).OrderBy(x => x.Key.Year).ThenBy(x => x.Key.Month).ElementAt(i - 1).Sum(t => t.Amount)) /
                                                            trans.GroupBy(t2 => new { t2.TransactionDate.Year, t2.TransactionDate.Month }).OrderBy(x => x.Key.Year).ThenBy(x => x.Key.Month).ElementAt(i - 1).Sum(t => t.Amount)) * 100
                               }).ToList()
                       }).ToList();

        return result;

    }

    public List<CustomerSpendingTrendSummary> GetMonthlySpendingTrends()
    {
        /*
            !=== CUSTOMER SPENDING REPORT ===

            Customer: Alice
            Month: October 2025 | Growth: 50.00%
            Month: October 2025 | Growth: -20.00%

            Customer: Bob
            Month: October 2025 | Growth: 12.50%
        */


        var result = Customers
                    .Select(customer =>
                    {
                        //CustomerId = 1
                        //Nhóm 1: Key = { Year = 2023, Month = 3 }, Transactions = [120]
                        //Nhóm 2: Key = { Year = 2023, Month = 1 }, Transactions = [100]
                        //Nhóm 3: Key = { Year = 2023, Month = 2 }, Transactions = [150]

                        // 1. Gom giao dịch theo tháng
                        var monthlyTotals = Transactions
                            .Where(t => t.CustomerId == customer.Id)
                            .GroupBy(t => new { t.TransactionDate.Year, t.TransactionDate.Month })
                            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                            .Select(g => new
                            {
                                Month = new DateTime(g.Key.Year, g.Key.Month, 1),
                                Total = g.Sum(t => t.Amount)
                            })
                            .ToList();

                        // 2. Tính tăng trưởng
                        var monthlyGrowth = monthlyTotals
                            .Zip(monthlyTotals.Skip(1), (prev, current) => new MonthlySpendingGrowth
                            {
                                Month = current.Month.ToString("MMMM yyyy"),
                                GrowthRate = prev.Total > 0 ? ((current.Total - prev.Total) / prev.Total) * 100 : 0
                            })
                            .ToList();

                        // 3. Trả về summary
                        return new CustomerSpendingTrendSummary
                        {
                            CustomerName = customer.Name,
                            MonthlyGrowth = monthlyGrowth
                        };
                    })
                    .ToList();

        return result;
    }
    public void Test(out Dictionary<object,object> result)
    {
        var id = 1;

        result = Transactions.Where(t => t.CustomerId == id)
                     .GroupBy(t => new { t.TransactionDate.Year, t.TransactionDate.Month })
                     .ToDictionary(
                         g => (object)g.Key,
                         g => (object)g.ToList()
                    );
    }

}


/*
! This exercise generates a report on monthly spending growth for each customer by tracking changes in monthly spending.

* 1.Grouping Transactions by Month:

GroupBy(new { t.TransactionDate.Year, t.TransactionDate.Month }) groups transactions by month for each customer.

* 2.Calculating Monthly Spending Growth:

Zip: monthlySpendings.Zip(monthlySpendings.Skip(1), (prev, current) => ...) calculates the growth rate between consecutive monthly totals.

* 3.Returning the Report:

The result is a list of CustomerSpendingTrendSummary objects, each containing a list of growth percentages for consecutive months
 
 */

class Program
{
    static void Main()
    {
        var repo = new CustomerRepository();

        var loyalCustomers = repo.GetLoyalCustomers();

        Console.WriteLine("Loyal Customers (5+ purchases):\n");

        foreach (var c in loyalCustomers)
        {
            Console.WriteLine($"Customer: {c.CustomerName}");
            Console.WriteLine($"  Total Purchases: {c.TotalPurchases}");
            Console.WriteLine($"  Total Spent: {c.TotalSpent:C}");
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
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}

public class LoyalCustomerSummary
{
    public required string CustomerName { get; set; }
    public int TotalPurchases { get; set; }
    public decimal TotalSpent { get; set; }
}

public class CustomerRepository
{
    public List<Customer> Customers { get; set; } =
    [
        new Customer { Id = 1, Name = "Alice" },
        new Customer { Id = 2, Name = "Bob" },
        new Customer { Id = 3, Name = "Charlie" }
    ];

    public List<Purchase> Purchases { get; set; } =
    [
        new Purchase { CustomerId = 1, Amount = 100m, Date = new DateTime(2023, 11, 1) },
        new Purchase { CustomerId = 1, Amount = 150m, Date = new DateTime(2023, 11, 10) },
        new Purchase { CustomerId = 1, Amount = 200m, Date = new DateTime(2023, 12, 1) },
        new Purchase { CustomerId = 1, Amount = 50m, Date = new DateTime(2023, 12, 5) },
        new Purchase { CustomerId = 1, Amount = 75m, Date = new DateTime(2023, 12, 10) },
        new Purchase { CustomerId = 2, Amount = 100m, Date = new DateTime(2023, 10, 15) },
        new Purchase { CustomerId = 2, Amount = 250m, Date = new DateTime(2023, 11, 20) },
        new Purchase { CustomerId = 3, Amount = 80m, Date = new DateTime(2023, 10, 10) }
    ];

    public List<LoyalCustomerSummary> GetLoyalCustomers()
    {
        return [.. Customers
            .GroupJoin(Purchases,
                customer => customer.Id,
                purchase => purchase.CustomerId,
                (customer, customerPurchases) => new LoyalCustomerSummary
                {
                    CustomerName = customer.Name,
                    TotalPurchases = customerPurchases.Count(),
                    TotalSpent = customerPurchases.Sum(p => p.Amount)
                })
            .Where(summary => summary.TotalPurchases >= 5)
            .OrderByDescending(summary => summary.TotalSpent)];
    }
}


/*
 Loyal Customers (5+ purchases):

Customer: Alice
  Total Purchases: 5
  Total Spent: $575.00

*/

/*
This exercise focuses on identifying loyal customers based on their purchase frequency.

* 1.Grouping Purchases by Customer:

GroupJoin(Purchases, customer => customer.Id, purchase => purchase.CustomerId, ...) groups purchases under each customer.

* 2.Calculating Loyalty Criteria:

TotalPurchases: Counts all purchases for each customer.

TotalSpent: Sums the amounts of all purchases for each customer.

Filters customers with 5 or more purchases to identify loyal customers.

* 3.Returning the Report:

The result is a list of LoyalCustomerSummary objects, sorted by TotalSpent in descending order.
 
 */

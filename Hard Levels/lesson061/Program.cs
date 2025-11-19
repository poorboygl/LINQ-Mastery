class Program
{
    static void Main()
    {
        var repo = new CustomerRepository();
        var topCustomers = repo.GetTopSpendingCustomers();

        Console.WriteLine("=== CUSTOMER SPENDING REPORT ===\n");

        foreach (var customer in topCustomers)
        {
            Console.WriteLine($"Customer: {customer.CustomerName}");
            Console.WriteLine($"  Total Spent: {customer.TotalSpent:C}");
            Console.WriteLine(new string('-', 40));
        }

        Console.ReadKey();
    }
}

public class Customer
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class Order
{
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
}

public class CustomerSpendingSummary
{
    public required string CustomerName { get; set; }
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

    public List<Order> Orders { get; set; } =
    [
        new Order { CustomerId = 1, OrderDate = new DateTime(2023, 10, 5), TotalAmount = 500 },
        new Order { CustomerId = 1, OrderDate = new DateTime(2023, 10, 10), TotalAmount = 300 },
        new Order { CustomerId = 2, OrderDate = new DateTime(2023, 10, 15), TotalAmount = 700 },
        new Order { CustomerId = 3, OrderDate = new DateTime(2023, 10, 20), TotalAmount = 400 },
        new Order { CustomerId = 3, OrderDate = new DateTime(2023, 10, 25), TotalAmount = 600 }
    ];

    public List<CustomerSpendingSummary> GetTopSpendingCustomers()
    {

        /*
         ! === CUSTOMER SPENDING REPORT ===

        Customer: Charlie
          Total Spent: $1,000.00
        ----------------------------------------
        Customer: Alice
          Total Spent: $800.00
        ----------------------------------------
        Customer: Bob
          Total Spent: $700.00
        ----------------------------------------
         */

        var result = Customers.Select( c => new CustomerSpendingSummary
        {
            CustomerName = c.Name,
            TotalSpent = Orders
                        .Where(order => order.CustomerId == c.Id)
                        .Sum(order => order.TotalAmount)
        })
         .OrderByDescending(summary => summary.TotalSpent)
         .Take(3)
         .ToList();
        
        return result;
    }

    public List<CustomerSpendingSummary> GetTopSpendingCustomers_2()
    {

        /*
         ! === CUSTOMER SPENDING REPORT ===

        Customer: Charlie
          Total Spent: $1,000.00
        ----------------------------------------
        Customer: Alice
          Total Spent: $800.00
        ----------------------------------------
        Customer: Bob
          Total Spent: $700.00
        ----------------------------------------
         */

        var result = Orders
            .GroupBy(o => o.CustomerId)
            .Select(g => new CustomerSpendingSummary
            {
                CustomerName = Customers.First(c => c.Id == g.Key).Name,
                TotalSpent = g.Sum(o => o.TotalAmount)
            })
            .OrderByDescending(s => s.TotalSpent)
            .Take(3)
            .ToList();

        return result;
    }
}

/*
 !This exercise generates a report that ranks customers by their total spending.

* 1.Filtering Orders by Customer:

Orders.Where(order => order.CustomerId == customer.Id) selects orders for each customer.

* 2.Calculating Total Spending:

TotalSpent: Sum(order => order.TotalAmount) calculates the total amount spent by each customer.

* 3.Returning the Top Spending Customers:

The result is a list of CustomerSpendingSummary objects, showing the top 3 customers by spending
 
 */
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        var repo = new OrderRepository();

        var topCustomers = repo.GetMostLoyalCustomers();

        Console.WriteLine("=== TOP 3 MOST LOYAL CUSTOMERS ===\n");

        foreach (var c in topCustomers)
        {
            Console.WriteLine($"Customer: {c.CustomerName}");
            Console.WriteLine($"  Order Count: {c.OrderCount}");
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

public class Order
{
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
}

public class CustomerOrderSummary
{
    public required string CustomerName { get; set; }
    public int OrderCount { get; set; }
}

public class OrderRepository
{
    public List<Customer> Customers { get; set; } =
    [
        new Customer { Id = 1, Name = "Alice" },
        new Customer { Id = 2, Name = "Bob" },
        new Customer { Id = 3, Name = "Charlie" },
        new Customer { Id = 4, Name = "Diana" }
    ];

    public List<Order> Orders { get; set; } =
    [
        new Order { CustomerId = 1, OrderDate = new DateTime(2024, 1, 15) },
        new Order { CustomerId = 1, OrderDate = new DateTime(2024, 2, 10) },
        new Order { CustomerId = 2, OrderDate = new DateTime(2024, 1, 22) },
        new Order { CustomerId = 3, OrderDate = new DateTime(2024, 3, 5) },
        new Order { CustomerId = 1, OrderDate = new DateTime(2024, 3, 15) },
        new Order { CustomerId = 2, OrderDate = new DateTime(2024, 3, 20) }
    ];

    public List<CustomerOrderSummary> GetMostLoyalCustomers()
    {

        /*
         === TOP 3 MOST LOYAL CUSTOMERS ===

        Customer: Alice
          Order Count: 3

        Customer: Bob
          Order Count: 2

        Customer: Charlie
          Order Count: 1
         */

        var result = Orders
                    .GroupBy(o => o.CustomerId)
                    .Select(group => new CustomerOrderSummary
                    {
                        CustomerName = Customers.First(c => c.Id == group.Key).Name,
                        OrderCount = group.Count()
                    })
                    .OrderBy(o => o.CustomerName)
                    .Take(3)
                    .ToList();
        return result;
    }

    public List<CustomerOrderSummary> GetMostLoyalCustomers_AuthorWritting()
    {
        var loyalCustomers = Orders
            .GroupBy(order => order.CustomerId)
            .Select(group => new
            {
                CustomerId = group.Key,
                OrderCount = group.Count()
            })
            .OrderByDescending(c => c.OrderCount)
            .Take(3)
            .Join(Customers,
                  c => c.CustomerId,
                  customer => customer.Id,
                  (c, customer) => new CustomerOrderSummary
                  {
                      CustomerName = customer.Name,
                      OrderCount = c.OrderCount
                  })
            .ToList();

        return loyalCustomers;
    }
}

/*
 ! This exercise identifies the customers with the highest order frequency by counting orders per customer and selecting the top results.

* 1.Grouping by Customer:

GroupBy(order => order.CustomerId) groups orders by each customer.

* 2.Counting Orders per Customer:

Within each group, Count() calculates the number of orders each customer has placed.

* 3.Selecting Top Customers:

OrderByDescending(c => c.OrderCount).Take(3) selects the top 3 customers by order frequency.

* 4.Returning the Summary:

The result is a list of CustomerOrderSummary objects, sorted by OrderCount in descending order.
*/
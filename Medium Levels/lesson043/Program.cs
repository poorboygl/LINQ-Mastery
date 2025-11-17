class Program
{
    static void Main()
    {
        var repository = new OrderRepository();
        var summaries = repository.GetCustomerOrderSummary();

        Console.WriteLine("Customer Order Summary:");
        Console.WriteLine("--------------------------------------------------------");
        Console.WriteLine("{0,-10} {1,10} {2,15} {3,15} {4,20}", "Customer", "Orders", "Total Spent", "Avg Order", "Last Order Date");

        foreach (var summary in summaries)
        {
            Console.WriteLine("{0,-10} {1,10} {2,15:C} {3,15:C} {4,20:yyyy-MM-dd}",
                summary.CustomerName,
                summary.TotalOrders,
                summary.TotalSpent,
                summary.AverageOrderAmount,
                summary.LastOrderDate);
        }

        Console.ReadLine();
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
    public decimal Amount { get; set; }
}

public class CustomerOrderSummary
{
    public required string CustomerName { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalSpent { get; set; }
    public DateTime LastOrderDate { get; set; }
    public decimal AverageOrderAmount { get; set; }
}

public class OrderRepository
{
    public List<Customer> Customers { get; set; } =
    [
        new Customer { Id = 1, Name = "Alice" },
        new Customer { Id = 2, Name = "Bob" },
        new Customer { Id = 3, Name = "Charlie" },
        new Customer { Id = 4, Name = "Tom" }
    ];

    public List<Order> Orders { get; set; } =
    [
        new Order { CustomerId = 1, OrderDate = new DateTime(2023, 12, 1), Amount = 500.00m },
        new Order { CustomerId = 1, OrderDate = new DateTime(2023, 11, 10), Amount = 700.00m },
        new Order { CustomerId = 1, OrderDate = new DateTime(2023, 10, 5), Amount = 300.00m },
        new Order { CustomerId = 2, OrderDate = new DateTime(2023, 11, 15), Amount = 450.00m },
        new Order { CustomerId = 3, OrderDate = new DateTime(2023, 11, 20), Amount = 200.00m },
        new Order { CustomerId = 3, OrderDate = new DateTime(2023, 12, 5), Amount = 300.00m }
    ];

    //? with GroupJoin
    public List<CustomerOrderSummary> GetCustomerOrderSummary()
    {
        return [.. Customers
            .GroupJoin(Orders,
                customer => customer.Id,
                order => order.CustomerId,
                (customer, customerOrders) => new CustomerOrderSummary
                {
                    CustomerName = customer.Name,
                    TotalOrders = customerOrders.Count(),
                    TotalSpent = customerOrders.Sum(o => o.Amount),
                    LastOrderDate =customerOrders.Any() ? customerOrders.Max(o => o.OrderDate) : DateTime.MinValue,
                    AverageOrderAmount = customerOrders.Any() ? customerOrders.Average(o => o.Amount) : 0
                })
            .OrderByDescending(summary => summary.TotalSpent)];
    }

    //? with Join
    //public List<CustomerOrderSummary> GetCustomerOrderSummary()
    //{
    //    return [.. Customers
    //        .Join(Orders,
    //            customer => customer.Id,
    //            order => order.CustomerId,
    //            (customer, customerOrder) => new
    //            {
    //                customer.Name,
    //                customerOrder.Amount,
    //                customerOrder.OrderDate
    //            })
    //        .GroupBy(x => x.Name)
    //        .Select(g => new CustomerOrderSummary
    //        {
    //            CustomerName = g.Key,
    //            TotalOrders = g.Count(),
    //            TotalSpent = g.Sum(x => x.Amount),
    //            LastOrderDate = g.Max(x => x.OrderDate),
    //            AverageOrderAmount = g.Average(x => x.Amount),                
    //        })];
    //}
}

/*
 ? with Group Join
    Customer Order Summary:
    --------------------------------------------------------
    Customer       Orders     Total Spent       Avg Order      Last Order Date
    Alice               3       $1,500.00         $500.00           2023-12-01
    Charlie             2         $500.00         $250.00           2023-12-05
    Bob                 1         $450.00         $450.00           2023-11-15
  ! Tom                 0           $0.00           $0.00           0001-01-01

 ? with Join
    Customer Order Summary:
    --------------------------------------------------------
    Customer       Orders     Total Spent       Avg Order      Last Order Date
    Alice               3       $1,500.00         $500.00           2023-12-01
    Bob                 1         $450.00         $450.00           2023-11-15
    Charlie             2         $500.00         $250.00           2023-12-05
 */


/*
In this exercise, you group orders by customer and calculate a summary for each customer.

* 1.Grouping Orders by Customer:

GroupJoin(Orders, customer => customer.Id, order => order.CustomerId, ...) associates each customer with their respective orders.

* 2.Calculating Order Summary Data:

TotalOrders: Counts all orders for the customer.

TotalSpent: Sums the order amounts for each customer.

LastOrderDate: Finds the most recent order date with Max.

AverageOrderAmount: Calculates the average order amount.

* 3.Sorting the Results:

OrderByDescending(summary => summary.TotalSpent) sorts customers by the total amount spent in descending order.
 
 
 */
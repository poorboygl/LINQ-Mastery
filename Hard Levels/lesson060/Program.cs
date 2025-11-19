class Program
{
    static void Main()
    {
        var repo = new OrderRepository();
        var summaries = repo.GetProductsByCustomerReach();

        Console.WriteLine("=== PRODUCT CUSTOMER REACH REPORT ===\n");

        foreach (var summary in summaries)
        {
            Console.WriteLine($"Product: {summary.ProductName}");
            Console.WriteLine($"  Distinct Customers: {summary.DistinctCustomerCount}");
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

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class Order
{
    public int ProductId { get; set; }
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public int Quantity { get; set; }
}

public class ProductCustomerReachSummary
{
    public required string ProductName { get; set; }
    public int DistinctCustomerCount { get; set; }
}

public class OrderRepository
{
    public List<Customer> Customers { get; set; } =
    [
        new Customer { Id = 1, Name = "Alice" },
        new Customer { Id = 2, Name = "Bob" },
        new Customer { Id = 3, Name = "Charlie" }
    ];

    public List<Product> Products { get; set; } =
    [
        new Product { Id = 1, Name = "Laptop" },
        new Product { Id = 2, Name = "Monitor" }
    ];

    public List<Order> Orders { get; set; } =
    [
        new Order { ProductId = 1, CustomerId = 1, OrderDate = new DateTime(2023, 10, 5), Quantity = 1 },
        new Order { ProductId = 1, CustomerId = 2, OrderDate = new DateTime(2023, 10, 10), Quantity = 2 },
        new Order { ProductId = 1, CustomerId = 3, OrderDate = new DateTime(2023, 10, 15), Quantity = 1 },
        new Order { ProductId = 2, CustomerId = 1, OrderDate = new DateTime(2023, 10, 20), Quantity = 1 },
        new Order { ProductId = 2, CustomerId = 2, OrderDate = new DateTime(2023, 10, 25), Quantity = 1 }
    ];


    public List<ProductCustomerReachSummary> GetProductsByCustomerReach()
    {
        /*
         ! === PRODUCT CUSTOMER REACH REPORT ===

            Product: Laptop
              Distinct Customers: 3
            ----------------------------------------
            Product: Monitor
              Distinct Customers: 2
            ----------------------------------------
        */

        return [.. Products
                .Select(product => new ProductCustomerReachSummary
                {
                    ProductName = product.Name,
                    DistinctCustomerCount = Orders
                        .Where(order => order.ProductId == product.Id)
                        .Select(order => order.CustomerId)
                        .Distinct()
                        .Count()
                })
                .OrderByDescending(summary => summary.DistinctCustomerCount)];
    }

    public List<ProductCustomerReachSummary> GetProductsByCustomerReach_2()
    {
        /*
         ! === PRODUCT CUSTOMER REACH REPORT ===

            Product: Laptop
              Distinct Customers: 3
            ----------------------------------------
            Product: Monitor
              Distinct Customers: 2
            ----------------------------------------
        */

        var result = Products
                    .Select( product =>
                    {
                        var productOrders = Orders.Where(o => o.ProductId == product.Id).ToList();
                        var customerCount = productOrders.Select(o => o.CustomerId).Distinct().Count();
                        return new ProductCustomerReachSummary
                        {
                            ProductName = product.Name,
                            DistinctCustomerCount = customerCount

                        };
                    })
                    .OrderByDescending(summary  => summary.DistinctCustomerCount)
                    .ToList();
        return result;

    }
}

/*
    ! This exercise generates a report ranking products by the number of distinct customers who ordered them.

    * 1.Filtering Orders by Product:

    Orders.Where(order => order.ProductId == product.Id) selects orders for each product.

    * 2.Calculating Distinct Customer Reach:

    DistinctCustomerCount: Select(order => order.CustomerId).Distinct().Count() calculates the number of unique customers for each product.

    * 3.Returning the Report:

    The result is a list of ProductCustomerReachSummary objects, sorted by DistinctCustomerCount in descending order.
 
 */
class Program
{
    static void Main()
    {
        var repo = new OrderRepository();

        var result = repo.GetOrdersWithProducts();

        Console.WriteLine("== Orders with Products ==");

        foreach (var order in result)
        {
            Console.WriteLine($"\nOrder {order.OrderId} - Date: {order.OrderDate:yyyy-MM-dd}");

            foreach (var product in order.Products)
            {
                Console.WriteLine($"   - {product.Name} (${product.Price})");
            }
        }

        Console.ReadLine();
    }
}

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public int OrderId { get; set; } // Foreign key to Order
}

public class Order
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
}

public class OrderWithProducts
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public List<Product> Products { get; set; } = [];
}

public class OrderRepository
{
    public List<Order> Orders { get; set; } =
    [
        new Order { OrderId = 1, OrderDate = new DateTime(2024, 1, 15) },
        new Order { OrderId = 2, OrderDate = new DateTime(2024, 2, 10) }
    ];

    public List<Product> Products { get; set; } =
    [
        new Product { Id = 1, Name = "Laptop", Price = 1200.00m, OrderId = 1 }, //Have Order ID
        new Product { Id = 2, Name = "Mouse", Price = 25.00m, OrderId = 1 },
        new Product { Id = 3, Name = "Keyboard", Price = 45.00m, OrderId = 2 },
        new Product { Id = 4, Name = "Monitor", Price = 200.00m, OrderId = 2 }
    ];

    // Complete this method
    public List<OrderWithProducts> GetOrdersWithProducts()
    {

        return [.. Orders
            .GroupJoin(
                Products,
                order => order.OrderId,
                product => product.OrderId,
                (order, products) => new OrderWithProducts
                {
                    OrderId = order.OrderId,
                    OrderDate = order.OrderDate,
                    Products = products.ToList()
                })];
    }
}

/*
 == Orders with Products ==

Order 1 - Date: 2024-01-15
   - Laptop ($1200.00)
   - Mouse ($25.00)

Order 2 - Date: 2024-02-10
   - Keyboard ($45.00)
   - Monitor ($200.00)
 
 */


/*
In this exercise, you are asked to complete the GetOrdersWithProducts method by using GroupJoin. Here’s a breakdown of the solution:

* 1.Using GroupJoin for Hierarchical Grouping:

GroupJoin(Products, order => order.OrderId, product => product.OrderId, ...) groups Products by OrderId and joins them to Orders.

* 2.Creating OrderWithProducts Objects for Each Order:

Each OrderWithProducts object contains an OrderId, OrderDate, and the list of grouped Products.

* 3.Example Execution:

Calling GetOrdersWithProducts() returns a list of OrderWithProducts objects, each containing an OrderId, OrderDate, and list of Products associated with that order.
 
 */
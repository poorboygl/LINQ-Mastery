class Program
{
    static void Main()
    {
        var orderRepo = new OrderRepository();

        var orderDetails = orderRepo.GetOrderDetailsWithProducts();

        foreach (var order in orderDetails)
        {
            Console.WriteLine($"OrderId: {order.OrderId}, Date: {order.OrderDate:yyyy-MM-dd}");
            foreach (var product in order.Products)
            {
                Console.WriteLine($"  - ProductId: {product.Id}, Name: {product.Name}, Price: {product.Price}");
            }
            Console.WriteLine();
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

public class OrderDetailWithProducts
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public required List<Product> Products { get; set; }
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
        new Product { Id = 1, Name = "Laptop", Price = 1200.00m, OrderId = 1 }, //have oderID
        new Product { Id = 2, Name = "Mouse", Price = 25.00m, OrderId = 1 },
        new Product { Id = 3, Name = "Keyboard", Price = 45.00m, OrderId = 2 },
        new Product { Id = 4, Name = "Monitor", Price = 200.00m, OrderId = 2 }
    ];


    public List<OrderDetailWithProducts> GetOrderDetailsWithProducts()
    {
        return [.. Orders
            .GroupJoin(
                Products,
                order => order.OrderId,
                product => product.OrderId,
                (order, products) => new OrderDetailWithProducts
                {
                    OrderId = order.OrderId,
                    OrderDate = order.OrderDate,
                    Products = [.. products]
                }
            )];
    }

    //public List<OrderDetailWithProducts> GetOrderDetailsWithProducts()
    //{
    //    return Orders
    //        .GroupJoin(
    //            Products,
    //            order => order.OrderId,
    //            product => product.OrderId,
    //            (order, products) => new OrderDetailWithProducts
    //            {
    //                OrderId = order.OrderId,
    //                OrderDate = order.OrderDate,
    //                Products = products.ToList()
    //            }
    //        ).ToList();
    //}
}

/*
 OrderId: 1, Date: 2024-01-15
  - ProductId: 1, Name: Laptop, Price: 1200.00
  - ProductId: 2, Name: Mouse, Price: 25.00

OrderId: 2, Date: 2024-02-10
  - ProductId: 3, Name: Keyboard, Price: 45.00
  - ProductId: 4, Name: Monitor, Price: 200.00
 
 */


/*
In this exercise, you are asked to complete the GetOrderDetailsWithProducts method in the OrderRepository class. Here’s a breakdown of the solution:

1.Using GroupJoin to Group Products by Order:

GroupJoin is used to associate each Order with a collection of related Products based on OrderId.

Orders.GroupJoin(Products, order => order.OrderId, product => product.OrderId, ...) performs the join and groups the products by each order.

2.Selecting Data into OrderDetailWithProducts Objects:

In the result selector, a new OrderDetailWithProducts object is created with properties from Order (OrderId, OrderDate) and the list of Products.

This structure provides a combined view where each order contains its associated products.

3.Converting to List of OrderDetailWithProducts:

The result of GroupJoin is converted to List<OrderDetailWithProducts> using ToList() to match the return type.

4.Example Execution:

Calling GetOrderDetailsWithProducts() will return a list where each object represents an order, and each order includes a list of associated products.
 
 
 */
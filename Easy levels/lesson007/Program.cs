class Program
{
    static void Main()
    {
        var orderRepository = new OrderRepository();

        var details = orderRepository.GetOrderProductDetails();

        Console.WriteLine("Order - Product Details:");
        foreach (var d in details)
        {
            Console.WriteLine(
                $"OrderId: {d.OrderId}, " +
                $"Date: {d.OrderDate:yyyy-MM-dd}, " +
                $"Product: {d.ProductName}, " +
                $"Price: {d.ProductPrice}"
            );
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

public class OrderRepository
{
    public List<Order> Orders { get; set; } =
    [
        new Order { OrderId = 1, OrderDate = new DateTime(2024, 1, 15) },
        new Order { OrderId = 2, OrderDate = new DateTime(2024, 2, 10) }
    ];

    public List<Product> Products { get; set; } =
    [
        new Product { Id = 1, Name = "Laptop", Price = 1200.00m, OrderId = 1 },
        new Product { Id = 2, Name = "Mouse", Price = 25.00m, OrderId = 1 },
        new Product { Id = 3, Name = "Keyboard", Price = 45.00m, OrderId = 2 }
    ];

    public class OrderProductDetail
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public required string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
    }

    public List<OrderProductDetail> GetOrderProductDetails()
    {
        return [.. Orders
            .Join(
                Products,
                order => order.OrderId,
                product => product.OrderId,
                (order, product) => new OrderProductDetail
                {
                    OrderId = order.OrderId,
                    OrderDate = order.OrderDate,
                    ProductName = product.Name,
                    ProductPrice = product.Price
                }
            )];
    }

    /*
     Projection
     (order, product) => new OrderProductDetail
        {
            OrderId = order.OrderId,
            OrderDate = order.OrderDate,
            ProductName = product.Name,
            ProductPrice = product.Price
        }
     */

    //public List<OrderProductDetail> GetOrderProductDetails()
    //{
    //    return Orders
    //        .Join(
    //            Products,
    //            order => order.OrderId,
    //            product => product.OrderId,
    //            (order, product) => new OrderProductDetail
    //            {
    //                OrderId = order.OrderId,
    //                OrderDate = order.OrderDate,
    //                ProductName = product.Name,
    //                ProductPrice = product.Price
    //            }
    //        ).ToList();
    //}
}

/*
 Order - Product Details:
OrderId: 1, Date: 2024-01-15, Product: Laptop, Price: 1200.00
OrderId: 1, Date: 2024-01-15, Product: Mouse, Price: 25.00
OrderId: 2, Date: 2024-02-10, Product: Keyboard, Price: 45.00 
 */


/*
 In this exercise, you are asked to complete the GetOrderProductDetails method in the OrderRepository class by using a join. Here’s a breakdown of the solution:

Using Join for an Inner Join:

The Join method is used to combine Orders and Products based on a matching OrderId between both lists.

Orders.Join(Products, order => order.OrderId, product => product.OrderId, ...) specifies the join keys and the result projection.

Selecting Data into OrderProductDetail Objects:

In the result selector, a new OrderProductDetail object is created with properties from both Order (OrderId and OrderDate) and Product (Name and Price).

This structure provides a combined view of Order and Product information in each result.
 
 
 
 */
class Program
{
    static void Main()
    {
        var orderRepo = new OrderRepository();

        var allProducts = orderRepo.GetAllProducts();

        Console.WriteLine("All Products from All Orders:");
        foreach (var p in allProducts)
        {
            Console.WriteLine($"Id: {p.Id}, Name: {p.Name}, Price: {p.Price}");
        }

        Console.ReadLine();
    }
}

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
}

public class Order
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public List<Product> Products { get; set; } = new List<Product>(); // List of products in the order
}

public class OrderRepository
{
    public List<Order> Orders { get; set; } =
    [
        new Order
        {
            OrderId = 1,
            OrderDate = new DateTime(2024, 1, 15),
            Products =
            [
                new Product { Id = 1, Name = "Laptop", Price = 1200.00m },
                new Product { Id = 2, Name = "Mouse", Price = 25.00m }
            ]
        },
        new Order
        {
            OrderId = 2,
            OrderDate = new DateTime(2024, 2, 10),
            Products =
            [
                new Product { Id = 3, Name = "Keyboard", Price = 45.00m },
                new Product { Id = 4, Name = "Monitor", Price = 200.00m }
            ]
        }
    ];

    public List<Product> GetAllProducts()
    {
        return [.. Orders.SelectMany(order => order.Products)];
    }


    //public List<Product> GetAllProducts()
    //{
    //    return Orders.SelectMany(order => order.Products).ToList();
    //}
}


/*
All Products from All Orders:
Id: 1, Name: Laptop, Price: 1200.00
Id: 2, Name: Mouse, Price: 25.00
Id: 3, Name: Keyboard, Price: 45.00
Id: 4, Name: Monitor, Price: 200.00
 */


/*
In this exercise, you are asked to complete the GetAllProducts method in the OrderRepository class by using SelectMany. Here’s a breakdown of the solution:

Using SelectMany for Flattening:

SelectMany is used to flatten a collection of collections into a single sequence.

Orders.SelectMany(order => order.Products) flattens all Products from each Order into a single sequence of Product objects.

Converting to a List of Products:

The result of SelectMany is converted to a List<Product> using ToList() to match the return type of the method.

Example Execution:

Calling GetAllProducts() will return a single list of all products across all orders, such as [Product(Id = 1, ...), Product(Id = 2, ...), ...].
 
 */
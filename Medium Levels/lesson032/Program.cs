class Program
{
    static void Main()
    {
        var repo = new ProductRepository();

        Console.WriteLine("== Last 3 Products ==");
        var last3 = repo.GetLastNProducts(3);
        foreach (var product in last3)
        {
            Console.WriteLine($"Id: {product.Id}, Name: {product.Name}, Price: ${product.Price}");
        }

        Console.WriteLine("\n== Products after skipping last 2 ==");
        var skipLast2 = repo.SkipLastNProducts(2);
        foreach (var product in skipLast2)
        {
            Console.WriteLine($"Id: {product.Id}, Name: {product.Name}, Price: ${product.Price}");
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

public class ProductRepository
{
    public List<Product> Products { get; set; } =
    [
        new Product { Id = 1, Name = "Laptop", Price = 1200.00m },
        new Product { Id = 2, Name = "Mouse", Price = 25.00m },
        new Product { Id = 3, Name = "Keyboard", Price = 45.00m },
        new Product { Id = 4, Name = "Monitor", Price = 200.00m },
        new Product { Id = 5, Name = "Desk", Price = 300.00m },
        new Product { Id = 6, Name = "Chair", Price = 85.00m }
    ];

    // Complete this method
    public List<Product> GetLastNProducts(int n)
    {
        // Use TakeLast to get the last n products
        return [.. Products.TakeLast(n)];
    }

    // Complete this method
    public List<Product> SkipLastNProducts(int n)
    {
        // Use SkipLast to skip the last n products and get the rest
        return [.. Products.SkipLast(n)];
    }
}


/*
 == Last 3 Products ==
Id: 4, Name: Monitor, Price: $200.00
Id: 5, Name: Desk, Price: $300.00
Id: 6, Name: Chair, Price: $85.00

== Products after skipping last 2 ==
Id: 1, Name: Laptop, Price: $1200.00
Id: 2, Name: Mouse, Price: $25.00
Id: 3, Name: Keyboard, Price: $45.00
Id: 4, Name: Monitor, Price: $200.00
 */


/*
In this exercise, you are asked to complete two methods using TakeLast and SkipLast. Here’s a breakdown of the solution:

* 1.Using TakeLast to Retrieve the Last n Elements:

TakeLast(n) retrieves the last n products from the list.

* 2.Using SkipLast to Skip the Last n Elements:

SkipLast(n) skips the last n products and retrieves the rest.

* 3.Example Execution:

Calling GetLastNProducts(3) returns the last three products in the list.

Calling SkipLastNProducts(2) returns all products except the last two.
 
 */
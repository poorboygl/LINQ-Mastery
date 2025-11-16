class Program
{
    static void Main()
    {
        var productRepo = new ProductRepository();

        decimal minPrice = 50;
        decimal maxPrice = 250;

        var filtered = productRepo.GetFilteredProducts(minPrice, maxPrice);

        Console.WriteLine($"Products with Price between {minPrice} and {maxPrice} starting with 'M':");
        foreach (var p in filtered)
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

    public List<Product> GetFilteredProducts(decimal minPrice, decimal maxPrice)
    {

        return [.. Products.Where(p => p.Price >= minPrice && p.Price <= maxPrice && p.Name.StartsWith("M", StringComparison.OrdinalIgnoreCase))];
    }

    //public List<Product> GetFilteredProducts(decimal minPrice, decimal maxPrice)
    //{

    //    return Products
    //        .Where(p => p.Price >= minPrice && p.Price <= maxPrice && p.Name.StartsWith("M", StringComparison.OrdinalIgnoreCase))
    //        .ToList();
    //}
}


/*
Products with Price between 50 and 250 starting with 'M':
Id: 4, Name: Monitor, Price: 200.00
*/


/*
In this exercise, you are asked to complete the GetFilteredProducts method in the ProductRepository class using Where with multiple conditions. Here’s a breakdown of the solution:

* 1.Using Where for Multiple Conditions:

Where allows you to apply multiple conditions to filter a collection.

p.Price >= minPrice && p.Price <= maxPrice ensures that only products within the specified price range are included.

p.Name.StartsWith("M", StringComparison.OrdinalIgnoreCase) filters products with names starting with "M", ignoring case.

* 2.Converting to a List of Products:

The result of Where is converted to List<Product> using ToList() to match the return type of the method.

* 3. Example Execution:

Calling GetFilteredProducts(20, 300) will return a list of products with prices between $20 and $300, where the name starts with "M".
 
 */
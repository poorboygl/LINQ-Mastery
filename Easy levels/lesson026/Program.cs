class Program
{
    static void Main()
    {
        var repo = new ProductRepository();

        Console.WriteLine("=== Test HasAnyProductAbovePrice ===");
        Console.WriteLine(repo.HasAnyProductAbovePrice(1000));   // true (Laptop 1200)
        Console.WriteLine(repo.HasAnyProductAbovePrice(2000));   // false


        Console.WriteLine("\n=== Test AreAllProductsBelowPrice ===");
        Console.WriteLine(repo.AreAllProductsBelowPrice(5000));  // true (tất cả < 5000)
        Console.WriteLine(repo.AreAllProductsBelowPrice(100));   // false (Laptop 1200)


        Console.WriteLine("\n=== Test ContainsProductByName ===");
        Console.WriteLine(repo.ContainsProductByName("Mouse"));  // true
        Console.WriteLine(repo.ContainsProductByName("mouse"));  // true (ignore case)
        Console.WriteLine(repo.ContainsProductByName("Table"));  // false

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


    public bool HasAnyProductAbovePrice(decimal priceThreshold)
    {
        return Products.Any(p => p.Price > priceThreshold); // Add your code here
    }


    public bool AreAllProductsBelowPrice(decimal priceLimit)
    {
        return Products.All(p => p.Price < priceLimit); // Add your code here
    }


    public bool ContainsProductByName(string productName)
    {
        return Products.Any(p => string.Equals(p.Name, productName, StringComparison.OrdinalIgnoreCase)); // Add your code here
    }
}


/*
 === Test HasAnyProductAbovePrice ===
True
False

=== Test AreAllProductsBelowPrice ===
True
False

=== Test ContainsProductByName ===
True
True
False 
 */


/*
 In this exercise, you are asked to complete three methods using Any, All, and Contains. Here’s a breakdown of the solution:

* 1.Using Any for Conditional Checks:

Any(p => p.Price > priceThreshold) checks if there’s any product with a Price above priceThreshold.

* 2.Using All to Verify All Elements Meet a Condition:

All(p => p.Price < priceLimit) checks if every product’s Price is below priceLimit.

* 3.Using Any with Case-Insensitive string.Equals for Name Matching:

Any(p => string.Equals(p.Name, productName, StringComparison.OrdinalIgnoreCase)) checks if any product’s Name matches productName case-insensitively.

* 4.Example Execution:

Calling HasAnyProductAbovePrice(1000) returns true if there’s a product with a price above $1000.

Calling AreAllProductsBelowPrice(5000) returns true if all products are priced below $5000.

Calling ContainsProductByName("Laptop") returns true if there’s a product named "Laptop" (ignores case)
 
 */
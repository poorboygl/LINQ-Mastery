class Program
{
    static void Main()
    {
        var repo = new ProductRepository();

        Console.WriteLine("=== GetFirstNProducts(3) ===");
        foreach (var p in repo.GetFirstNProducts(3))
            Console.WriteLine($"{p.Id} - {p.Name} - {p.Price}");

        Console.WriteLine("\n=== GetProductsAfterSkipping(2) ===");
        foreach (var p in repo.GetProductsAfterSkipping(2))
            Console.WriteLine($"{p.Id} - {p.Name} - {p.Price}");

        Console.WriteLine("\n=== GetProductsWhilePriceBelow(100) ===");
        foreach (var p in repo.GetProductsWhilePriceBelow(100))
            Console.WriteLine($"{p.Id} - {p.Name} - {p.Price}");

        Console.WriteLine("\n=== SkipProductsWhilePriceBelow(100) ===");
        foreach (var p in repo.SkipProductsWhilePriceBelow(100))
            Console.WriteLine($"{p.Id} - {p.Name} - {p.Price}");

        Console.ReadLine();
    }
}


public class Product
{
    public int Id { get; set; }
    public required string  Name { get; set; }
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

    public List<Product> GetFirstNProducts(int n)
    {
        // Use Take to get the first n products
        return [.. Products.Take(n)];
    }

    public List<Product> GetProductsAfterSkipping(int n)
    {
        // Use Skip to get all products after the first n products
        return [.. Products.Skip(n)];
    }

    public List<Product> GetProductsWhilePriceBelow(decimal priceThreshold)
    {
        // Take products while their price is below the threshold
        return [.. Products.TakeWhile(p => p.Price < priceThreshold)];
    }

    public List<Product> SkipProductsWhilePriceBelow(decimal priceThreshold)
    {
        // Skip products while their price is below the threshold, then return the rest
        return [.. Products.SkipWhile(p => p.Price < priceThreshold)];
    }
}

/*
 === GetFirstNProducts(3) ===
1 - Laptop - 1200.00
2 - Mouse - 25.00
3 - Keyboard - 45.00

=== GetProductsAfterSkipping(2) ===
3 - Keyboard - 45.00
4 - Monitor - 200.00
5 - Desk - 300.00
6 - Chair - 85.00

=== GetProductsWhilePriceBelow(100) ===

=== SkipProductsWhilePriceBelow(100) ===
1 - Laptop - 1200.00
2 - Mouse - 25.00
3 - Keyboard - 45.00
4 - Monitor - 200.00
5 - Desk - 300.00
6 - Chair - 85.00
 
 */


/*
 In this exercise, you are asked to complete four methods using Take, Skip, TakeWhile, and SkipWhile. Here’s a breakdown of the solution:

* 1.Using Take to Get the First n Elements:

Take(n) retrieves the first n products from the list.

* 2.Using Skip to Skip the First n Elements:

Skip(n) skips the first n products and retrieves the rest.

* 3.Using TakeWhile to Retrieve Items While a Condition Is Met:

TakeWhile(p => p.Price < priceThreshold) takes products from the start until a product with Price >= priceThreshold is found.

* 4.Using SkipWhile to Skip Items While a Condition Is Met:

SkipWhile(p => p.Price < priceThreshold) skips products with Price < priceThreshold and returns the rest.

* 5.Example Execution:

Calling GetFirstNProducts(3) returns the first three products in the list.

Calling GetProductsWhilePriceBelow(100) will return products until a product priced $100 or above is encountered.
 
 
 */
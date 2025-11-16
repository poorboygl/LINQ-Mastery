class Program
{
    static void Main()
    {
        var productRepository = new ProductRepository();

        Console.WriteLine("=== Test Any ===");
        Console.WriteLine(productRepository.HasAnyProductAbovePrice(100));  // true
        Console.WriteLine(productRepository.HasAnyProductAbovePrice(1500)); // false

        Console.WriteLine("=== Test All ===");
        Console.WriteLine(productRepository.AreAllProductsAbovePrice(10));  // true
        Console.WriteLine(productRepository.AreAllProductsAbovePrice(100)); // false

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
            new Product { Id = 4, Name = "Monitor", Price = 200.00m }
        ];

    // Complete this method
    public bool HasAnyProductAbovePrice(decimal priceThreshold)
    {
        // Use Any to check if there is any product with Price > priceThreshold
        return Products.Any(p => p.Price > priceThreshold); // Add your code here
    }

    // Complete this method
    public bool AreAllProductsAbovePrice(decimal priceThreshold)
    {
        // Use All to check if all products have Price > priceThreshold
        return Products.All(p => p.Price > priceThreshold); // Add your code here
    }
}

/*
=== Test Any ===
True
False
=== Test All ===
True
False
*/

/*

In this exercise, you are asked to complete two methods in the ProductRepository class to check conditions using Any and All. Here’s a breakdown of each solution:

Using Any in HasAnyProductAbovePrice:

The Any method returns true if at least one item in the collection matches the condition; otherwise, it returns false.

Products.Any(p => p.Price > priceThreshold) checks if there is at least one product with a Price greater than the specified priceThreshold.

Using All in AreAllProductsAbovePrice:

The All method returns true only if every item in the collection matches the condition; otherwise, it returns false.

Products.All(p => p.Price > priceThreshold) checks if all products have a Price greater than the specified priceThreshold.

Example Execution:

HasAnyProductAbovePrice(50) will return true because there are products above $50.

AreAllProductsAbovePrice(50) will return false because some products are priced below $50.
 
 */

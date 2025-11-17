class Program
{
    static void Main()
    {
        var repo = new ProductRepository();

        var lookup = repo.GetProductLookupByCategory();

        Console.WriteLine("== Products Grouped by Category (Lookup) ==");

        foreach (var group in lookup)
        {
            Console.WriteLine($"\nCategory: {group.Key}");

            foreach (var product in group)
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
    public required string Category { get; set; } // Category of the product
}

public class ProductRepository
{
    public List<Product> Products { get; set; } =
    [
        new Product { Id = 1, Name = "Laptop", Price = 1200.00m, Category = "Electronics" },
        new Product { Id = 2, Name = "Mouse", Price = 25.00m, Category = "Electronics" },
        new Product { Id = 3, Name = "Keyboard", Price = 45.00m, Category = "Electronics" },
        new Product { Id = 4, Name = "Desk", Price = 300.00m, Category = "Furniture" },
        new Product { Id = 5, Name = "Chair", Price = 85.00m, Category = "Furniture" }
    ];

    // Complete this method
    public ILookup<string, Product> GetProductLookupByCategory()
    {
        // Use ToLookup to group products by Category
        return Products.ToLookup(p => p.Category); // Add your code here
    }
}

/*
     == Products Grouped by Category (Lookup) ==

    Category: Electronics
       - Laptop ($1200.00)
       - Mouse ($25.00)
       - Keyboard ($45.00)

    Category: Furniture
       - Desk ($300.00)
       - Chair ($85.00)
 */


/*
 In this exercise, you are asked to complete the GetProductLookupByCategory method by using ToLookup. Here’s a breakdown of the solution:

* 1.Using ToLookup to Group by Category:

ToLookup(p => p.Category) creates a lookup where each key is a Category, and each value is a collection of products in that category.

* 2.Example Execution:

Calling GetProductLookupByCategory() returns an ILookup<string, Product> with categories as keys, allowing easy access to each list of products by category.
 
*/
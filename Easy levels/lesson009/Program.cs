class Program
{
    static void Main()
    {
        var productRepository = new ProductRepository();

        var categories = productRepository.GetUniqueCategories();

        Console.WriteLine("Unique Categories:");
        foreach (var c in categories)
        {
            Console.WriteLine($"- {c}");
        }

        Console.ReadLine();
    }
}
public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public required string Category { get; set; } // Product category
}

public class ProductRepository
{
    public List<Product> Products { get; set; } =
    [
        new Product { Id = 1, Name = "Laptop", Price = 1200.00m, Category = "Electronics" },
        new Product { Id = 2, Name = "Mouse", Price = 25.00m, Category = "Electronics" },
        new Product { Id = 3, Name = "Desk", Price = 300.00m, Category = "Furniture" },
        new Product { Id = 4, Name = "Monitor", Price = 200.00m, Category = "Electronics" },
        new Product { Id = 5, Name = "Chair", Price = 85.00m, Category = "Furniture" },
        new Product { Id = 6, Name = "Pen", Price = 1.50m, Category = "Office Supplies" }
    ];


    public List<string> GetUniqueCategories()
    {
        return [.. Products.Select(p => p.Category).Distinct()];
    }

    //public List<string> GetUniqueCategories()
    //{
    //    return Products.Select(p => p.Category).Distinct().ToList();
    //}
}


/*
 Unique Categories:
- Electronics
- Furniture
- Office Supplies
 */


/*
 In this exercise, you are asked to complete the GetUniqueCategories method in the ProductRepository class. Here’s a breakdown of the solution:

Using Select and Distinct for Unique Categories:

Select(p => p.Category) creates a collection of category names from the Products list.

Distinct() is then used to remove duplicates from this collection, ensuring only unique category names remain.

Converting to a List of Strings:

The result of Distinct() is converted to a list of strings using ToList(), which matches the return type of the method.

Example Execution:

Calling GetUniqueCategories() will return a list of unique category names, such as ["Electronics", "Furniture", "Office Supplies"].
 
 
 */
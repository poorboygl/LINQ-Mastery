class Program
{
    static void Main()
    {
        var repo = new ProductRepository();

        var result = repo.GetProductsGroupedByCategoryAndPriceRange();

        foreach (var category in result)
        {
            Console.WriteLine($"Category: {category.Key}");

            foreach (var priceRange in category.Value)
            {
                Console.WriteLine($"  Price Range: {priceRange.Key}");

                foreach (var product in priceRange.Value)
                {
                    Console.WriteLine($"    Id: {product.Id}, Name: {product.Name}, Price: {product.Price}");
                }
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
    public required string Category { get; set; }
}

public class ProductRepository
{
    public List<Product> Products { get; set; } =
    [
        new Product { Id = 1, Name = "Laptop", Price = 1200.00m, Category = "Electronics" },
        new Product { Id = 2, Name = "Mouse", Price = 25.00m, Category = "Electronics" },
        new Product { Id = 3, Name = "Keyboard", Price = 45.00m, Category = "Electronics" },
        new Product { Id = 4, Name = "Desk", Price = 300.00m, Category = "Furniture" },
        new Product { Id = 5, Name = "Chair", Price = 85.00m, Category = "Furniture" },
        new Product { Id = 6, Name = "Lamp", Price = 20.00m, Category = "Furniture" }
    ];

    // Complete this method
    public Dictionary<string, Dictionary<string, List<Product>>> GetProductsGroupedByCategoryAndPriceRange()
    {
        // Group products by Category and then by Price range
        return Products
            .GroupBy(p => p.Category)
            .ToDictionary(
                categoryGroup => categoryGroup.Key,
                categoryGroup => categoryGroup
                    .GroupBy(p => 
                        p.Price <= 50 ? "Low" :
                        p.Price <= 200 ? "Medium" : "High")
                    .ToDictionary(
                        priceRangeGroup => priceRangeGroup.Key,
                        priceRangeGroup => priceRangeGroup.ToList()
                    )
            );  
    }
}

/*
 Category: Electronics
  Price Range: High
    Id: 1, Name: Laptop, Price: 1200.00
  Price Range: Low
    Id: 2, Name: Mouse, Price: 25.00
    Id: 3, Name: Keyboard, Price: 45.00

Category: Furniture
  Price Range: High
    Id: 4, Name: Desk, Price: 300.00
  Price Range: Medium
    Id: 5, Name: Chair, Price: 85.00
  Price Range: Low
    Id: 6, Name: Lamp, Price: 20.00
 */

/*
In this exercise, you are asked to complete the GetProductsGroupedByCategoryAndPriceRange method by using nested GroupBy operations.Here’s a breakdown of the solution:

* 1.Using GroupBy for Hierarchical Grouping:

GroupBy(p => p.Category) groups products by Category.

Within each Category group, another GroupBy is used to categorize products by price range (e.g., "Low", "Medium", "High").

* 2.Creating Nested Dictionaries:

The outer dictionary has categories as keys and inner dictionaries as values.

The inner dictionaries map each price range to lists of products.

* 3.Example Execution:

Calling GetProductsGroupedByCategoryAndPriceRange() returns a nested dictionary, providing hierarchical access to products by category and price range. 

 */

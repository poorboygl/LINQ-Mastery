class Program
{
    static void Main()
    {
        var productRepo = new ProductRepository();

        var grouped = productRepo.GetProductsGroupedByPriceRange();

        foreach (var group in grouped)
        {
            Console.WriteLine($"{group.Key} Price Products:");
            foreach (var p in group.Value)
            {
                Console.WriteLine($"  Id: {p.Id}, Name: {p.Name}, Price: {p.Price}");
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


    public Dictionary<string, List<Product>> GetProductsGroupedByPriceRange()
    {

        return Products
            .GroupBy(p => p.Price <= 50 ? "Low" : p.Price <= 200 ? "Medium" : "High")
            .ToDictionary(g => g.Key, g => g.ToList());
    }
}

/*
 High Price Products:
  Id: 1, Name: Laptop, Price: 1200.00
  Id: 5, Name: Desk, Price: 300.00

Low Price Products:
  Id: 2, Name: Mouse, Price: 25.00
  Id: 3, Name: Keyboard, Price: 45.00

Medium Price Products:
  Id: 4, Name: Monitor, Price: 200.00
  Id: 6, Name: Chair, Price: 85.00 
 */

/*
In this exercise, you are asked to complete the GetProductsGroupedByPriceRange method in the ProductRepository class. Here’s a breakdown of the solution:

* 1.Using GroupBy to Categorize by Price Range:

GroupBy is used to categorize products based on their Price.

The condition p.Price <= 50 ? "Low" : p.Price <= 200 ? "Medium" : "High" groups products into "Low", "Medium", or "High" price ranges.

* 2.Converting to a Dictionary:

ToDictionary is used to convert the grouped result into a dictionary, where the key is the price range and the value is a list of products in that range.

* 3.Example Execution:

Calling GetProductsGroupedByPriceRange() will return a dictionary with three keys ("Low", "Medium", "High") and lists of products corresponding to each range.
 
 */

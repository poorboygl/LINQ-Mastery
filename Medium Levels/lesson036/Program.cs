class Program
{
    static void Main()
    {
        var repo = new ProductRepository();

        var uniqueProducts = repo.GetUniqueProductsByName();

        Console.WriteLine("Unique products by Name:");
        foreach (var product in uniqueProducts)
        {
            Console.WriteLine($"Id: {product.Id}, Name: {product.Name}, Price: {product.Price}");
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
        new Product { Id = 6, Name = "Laptop", Price = 1000.00m }, // Duplicate name
        new Product { Id = 7, Name = "Mouse", Price = 30.00m } // Duplicate name
    ];

    // Complete this method
    public List<Product> GetUniqueProductsByName()
    {
        // Use DistinctBy to get unique products by Name, keeping only the first occurrence of each name
        return [.. Products.DistinctBy(p => p.Name)];
    }
}

/*
Unique products by Name:
Id: 1, Name: Laptop, Price: 1200.00
Id: 2, Name: Mouse, Price: 25.00
Id: 3, Name: Keyboard, Price: 45.00
Id: 4, Name: Monitor, Price: 200.00
Id: 5, Name: Desk, Price: 300.00
 */


/*
In this exercise, you are asked to complete the GetUniqueProductsByName method by using DistinctBy to filter out products with duplicate names. Here’s a breakdown of the solution:

* 1.Using DistinctBy for Unique Names:

DistinctBy(p => p.Name) removes duplicate products based on the Name property, keeping only the first occurrence of each unique name.

* 2.Converting to a List of Products:

The result of DistinctBy is converted to List<Product> using ToList() to match the return type.

* 3.Example Execution:

Calling GetUniqueProductsByName() returns a list of products with unique names, such as ["Laptop", "Mouse", "Keyboard", "Monitor", "Desk"].
 */
class Program
{
    static void Main()
    {
        var productRepo = new ProductRepository();

        var uniqueProducts = productRepo.GetUniqueProducts();

        Console.WriteLine("Unique products by Name:");
        foreach (var p in uniqueProducts)
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
        new Product { Id = 3, Name = "Laptop", Price = 1150.00m },
        new Product { Id = 4, Name = "Keyboard", Price = 45.00m },
        new Product { Id = 5, Name = "Mouse", Price = 30.00m },
        new Product { Id = 3, Name = "Laptop", Price = 80.00m },
    ];


    public List<Product> GetUniqueProducts()
    {

        return [.. Products.DistinctBy(p => p.Name, StringComparer.OrdinalIgnoreCase)];
    }

    //public List<Product> GetUniqueProducts()
    //{

    //    return Products
    //        .DistinctBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
    //        .ToList();
    //}
}

/*
Unique products by Name:
Id: 1, Name: Laptop, Price: 1200.00
Id: 2, Name: Mouse, Price: 25.00
Id: 4, Name: Keyboard, Price: 45.00
 */

/*
 In this exercise, you are asked to complete the GetUniqueProducts method using Distinct. Here’s a breakdown of the solution:

* 1.Using DistinctBy for Uniqueness Based on Name:

DistinctBy(p => p.Name, StringComparer.OrdinalIgnoreCase) returns unique products by ignoring duplicates with the same Name (case-insensitive).

* 2.Converting to a List of Products:

The result of DistinctBy is converted to List<Product> using ToList() to match the return type.

* 3.Example Execution:

Calling GetUniqueProducts() will return a list with only one instance of each product name, like [Product(Id=1, ...), Product(Id=2, ...), Product(Id=4, ...)].
 
 */

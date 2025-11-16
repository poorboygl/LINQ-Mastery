class Program
{
    static void Main()
    {
        var productRepository = new ProductRepository();

        var grouped = productRepository.GroupProductsByPriceRange();

        Console.WriteLine("Grouped Products by Price Range:");

        foreach (var group in grouped)
        {
            Console.WriteLine($"\n=== {group.Key} Price Products ===");
            foreach (var product in group.Value)
            {
                Console.WriteLine($"Id: {product.Id}, Name: {product.Name}, Price: {product.Price}");
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

    public Dictionary<string, List<Product>> GroupProductsByPriceRange()
    {
        return Products
            .GroupBy(p => p.Price <= 50 ? "Low" : p.Price <= 200 ? "Medium" : "High")
            .ToDictionary(g => g.Key, g => g.ToList()); // Add your code here
    }
}


/*
     Grouped Products by Price Range:

    === High Price Products ===
    Id: 1, Name: Laptop, Price: 1200.00

    === Low Price Products ===
    Id: 2, Name: Mouse, Price: 25.00
    Id: 3, Name: Keyboard, Price: 45.00

    === Medium Price Products ===
    Id: 4, Name: Monitor, Price: 200.00

 */
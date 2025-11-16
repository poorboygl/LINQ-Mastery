class Program
{
    static void Main()
    {
        var productRepo = new ProductRepository();

        var uniqueProducts = productRepo.GetAllUniqueProductsFromBothStores();

        Console.WriteLine("All unique products from both stores:");
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
    public List<Product> StoreAProducts { get; set; } =
    [
        new Product { Id = 1, Name = "Laptop", Price = 1200.00m },
        new Product { Id = 2, Name = "Mouse", Price = 25.00m },
        new Product { Id = 3, Name = "Keyboard", Price = 45.00m }
    ];

    public List<Product> StoreBProducts { get; set; } =
    [
        new Product { Id = 4, Name = "Laptop", Price = 1150.00m },
        new Product { Id = 5, Name = "Monitor", Price = 200.00m },
        new Product { Id = 6, Name = "Keyboard", Price = 40.00m }
    ];


    public List<Product> GetAllUniqueProductsFromBothStores()
    {
        return [.. StoreAProducts.Union(StoreBProducts, new ProductNameComparer())];
    }

    //public List<Product> GetAllUniqueProductsFromBothStores()
    //{
    //    return StoreAProducts
    //        .Union(StoreBProducts, new ProductNameComparer())
    //        .ToList();
    //}
}

// Provides a case-insensitive comparison for Product objects based on their Name property.
public class ProductNameComparer : IEqualityComparer<Product>
{
    public bool Equals(Product x, Product y)
    {
        return string.Equals(x?.Name, y?.Name, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(Product obj)
    {
        return obj.Name?.ToLower().GetHashCode() ?? 0;
    }
}


/*
All unique products from both stores:
Id: 1, Name: Laptop, Price: 1200.00
Id: 2, Name: Mouse, Price: 25.00
Id: 3, Name: Keyboard, Price: 45.00
Id: 5, Name: Monitor, Price: 200.00
 */

/*
In this exercise, you are asked to complete the GetAllUniqueProductsFromBothStores method using Union. Here’s a breakdown of the solution:

* 1.Using Union to Combine Collections:

Union(StoreBProducts, new ProductNameComparer()) combines both lists, keeping only unique products based on Name (case-insensitive).

* 2.Using ProductNameComparer for Custom Equality:

This comparer ensures that only one instance of each product name appears in the final result, regardless of case.

* 3.Example Execution:

Calling GetAllUniqueProductsFromBothStores() will return a list of all unique products from both stores, with duplicates removed by name.
 
 */
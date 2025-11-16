class Program
{
    static void Main()
    {
        var productRepo = new ProductRepository();

        var exclusiveA = productRepo.GetExclusiveProductsInStoreA();

        Console.WriteLine("Products exclusive to Store A:");
        foreach (var p in exclusiveA)
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

    // Complete this method
    public List<Product> GetExclusiveProductsInStoreA()
    {
        // Use Except to find products only in StoreA by Name
        return [.. StoreAProducts.Except(StoreBProducts, new ProductNameComparer())]; // Add your code here
    }

    //public List<Product> GetExclusiveProductsInStoreA()
    //{

    //    return StoreAProducts
    //        .Except(StoreBProducts, new ProductNameComparer())
    //        .ToList();
    //}
}

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
    Products exclusive to Store A:
    Id: 2, Name: Mouse, Price: 25.00
 */


/*
In this exercise, you are asked to complete the GetExclusiveProductsInStoreA method using Except. Here’s a breakdown of the solution:

* 1.Using Except for Finding Exclusive Items:

Except(StoreBProducts, new ProductNameComparer()) returns products that are in StoreAProducts but not in StoreBProducts, based on Name.

* 2.Using ProductNameComparer for Custom Equality:

The custom comparer ensures that the comparison is based on Name, ignoring case.

* 3.Example Execution:

Calling GetExclusiveProductsInStoreA() will return a list of products exclusive to StoreAProducts, such as [Product(Id = 2, ...)].
 
 */
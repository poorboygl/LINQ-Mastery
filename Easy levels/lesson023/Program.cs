class Program
{
    static void Main()
    {
        var productRepo = new ProductRepository();

        var commonProducts = productRepo.GetCommonProductsBetweenStores();

        Console.WriteLine("Products common to both stores:");
        foreach (var p in commonProducts)
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


    public List<Product> GetCommonProductsBetweenStores()
    {

        return [.. StoreAProducts.Intersect(StoreBProducts, new ProductNameComparer())]; // Add your code here
    }

    //public List<Product> GetCommonProductsBetweenStores()
    //{

    //    return StoreAProducts
    //        .Intersect(StoreBProducts, new ProductNameComparer())
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
 Products common to both stores:
Id: 1, Name: Laptop, Price: 1200.00
Id: 3, Name: Keyboard, Price: 45.00
*/

/*
In this exercise, you are asked to complete the GetCommonProductsBetweenStores method using Intersect. Here’s a breakdown of the solution:

* 1.Using Intersect for Finding Common Items:

Intersect(StoreBProducts, new ProductNameComparer()) finds products with matching Name in both lists.

* 2.Using ProductNameComparer for Custom Equality:

The custom comparer ensures that Name is the basis for comparison, ignoring case.

* 3.Example Execution:

Calling GetCommonProductsBetweenStores() will return a list of products present in both stores, such as [Product(Id = 1, ...), Product(Id = 3, ...)]. 
*/
class Program
{
    static void Main()
    {
        var productRepo = new ProductRepository();

        var sortedProducts = productRepo.GetProductsSortedByPriceAndName();

        Console.WriteLine("Products sorted by Price and Name:");
        foreach (var p in sortedProducts)
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
        new Product { Id = 3, Name = "Keyboard", Price = 45.00m },
        new Product { Id = 4, Name = "Monitor", Price = 200.00m },
        new Product { Id = 5, Name = "Desk", Price = 300.00m },
        new Product { Id = 6, Name = "Chair", Price = 85.00m }
    ];


    public List<Product> GetProductsSortedByPriceAndName()
    {

        return [.. Products
            .OrderBy(p => p.Price)
            .ThenBy(p => p.Name)];
    }

    //public List<Product> GetProductsSortedByPriceAndName()
    //{

    //    return Products
    //        .OrderBy(p => p.Price)
    //        .ThenBy(p => p.Name)
    //        .ToList();
    //}
}

/*
Products sorted by Price and Name:
Id: 2, Name: Mouse, Price: 25.00
Id: 3, Name: Keyboard, Price: 45.00
Id: 6, Name: Chair, Price: 85.00
Id: 4, Name: Monitor, Price: 200.00
Id: 5, Name: Desk, Price: 300.00
Id: 1, Name: Laptop, Price: 1200.00
 */

/*
 In this exercise, you are asked to complete the GetProductsSortedByPriceAndName method in the ProductRepository class by using OrderBy and ThenBy. Here’s a breakdown of the solution:

* 1.Sorting by Price with OrderBy:

OrderBy(p => p.Price) sorts products by Price in ascending order.

* 2.Sorting by Name with ThenBy:

ThenBy(p => p.Name) ensures that products with the same Price are sorted alphabetically by Name.

* 3.Converting to a List of Products:

The result of OrderBy and ThenBy is converted to List<Product> using ToList() to match the return type.

* 4.Example Execution:

Calling GetProductsSortedByPriceAndName() will return a list of products sorted first by price, and then by name if prices are equal.
 */
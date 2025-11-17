class Program
{
    static void Main()
    {
        var repo = new ProductRepository();

        Console.WriteLine("== Products sorted by Price DESC, Name DESC ==");
        var sortedProducts = repo.GetProductsSortedByPriceAndNameDescending();

        foreach (var product in sortedProducts)
        {
            Console.WriteLine($"Id: {product.Id}, Name: {product.Name}, Price: ${product.Price}");
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

    // Complete this method
    public List<Product> GetProductsSortedByPriceAndNameDescending()
    {
        // Use OrderByDescending and ThenByDescending to sort by Price and Name
        return [.. Products
            .OrderByDescending(p => p.Price)
            .ThenByDescending(p => p.Name)];
    }
}

/*
 == Products sorted by Price DESC, Name DESC ==
Id: 1, Name: Laptop, Price: $1200.00
Id: 5, Name: Desk, Price: $300.00
Id: 4, Name: Monitor, Price: $200.00
Id: 6, Name: Chair, Price: $85.00
Id: 3, Name: Keyboard, Price: $45.00
Id: 2, Name: Mouse, Price: $25.00
 */

/*
In this exercise, you are asked to complete the GetProductsSortedByPriceAndNameDescending method in the ProductRepository class by using OrderByDescending and ThenByDescending. Here’s a breakdown of the solution:

* 1.Using OrderByDescending for Primary Sorting:

OrderByDescending(p => p.Price) sorts products by Price in descending order.

* 2.Using ThenByDescending for Secondary Sorting:

ThenByDescending(p => p.Name) ensures that products with the same Price are sorted in descending alphabetical order by Name.

* 3.Example Execution:

Calling GetProductsSortedByPriceAndNameDescending() will return a list of products sorted first by descending price, then by descending name if prices are equal.
 
*/
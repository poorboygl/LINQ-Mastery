class Program
{
    static void Main()
    {
        var productRepo = new ProductRepository();

        decimal minPrice = 100;
        var firstAbove = productRepo.GetFirstProductAbovePrice(minPrice);
        if (firstAbove != null)
            Console.WriteLine($"First product with Price > {minPrice}: Id={firstAbove.Id}, Name={firstAbove.Name}, Price={firstAbove.Price}");
        else
            Console.WriteLine($"No product found with Price > {minPrice}");

        decimal maxPrice = 250;
        var lastBelow = productRepo.GetLastProductBelowPrice(maxPrice);
        if (lastBelow != null)
            Console.WriteLine($"Last product with Price < {maxPrice}: Id={lastBelow.Id}, Name={lastBelow.Name}, Price={lastBelow.Price}");
        else
            Console.WriteLine($"No product found with Price < {maxPrice}");

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


    public Product? GetFirstProductAbovePrice(decimal minPrice)
    {
        return Products.FirstOrDefault(p => p.Price > minPrice);
    }


    public Product? GetLastProductBelowPrice(decimal maxPrice)
    {
        return Products.LastOrDefault(p => p.Price < maxPrice);
    }
}

/*
    First product with Price > 100: Id=1, Name=Laptop, Price=1200.00
    Last product with Price < 250: Id=6, Name=Chair, Price=85.00
 */



/*
In this exercise, you are asked to complete two methods in the ProductRepository class using FirstOrDefault and LastOrDefault. Here’s a breakdown of the solution:

* 1.Using FirstOrDefault in GetFirstProductAbovePrice:

FirstOrDefault(p => p.Price > minPrice) returns the first Product with a Price greater than minPrice.

If no products match, FirstOrDefault returns null.

* 2.Using LastOrDefault in GetLastProductBelowPrice:

LastOrDefault(p => p.Price < maxPrice) returns the last Product with a Price below maxPrice.

If no products match, LastOrDefault also returns null.

* 3. Example Execution:

Calling GetFirstProductAbovePrice(50) will return the first product above $50.

Calling GetLastProductBelowPrice(1000) will return the last product below $1000.
 
 
 */
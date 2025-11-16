class Program
{
    static void Main()
    {
        var productRepo = new ProductRepository();

        decimal minPrice = 100;

        decimal total = productRepo.GetTotalPriceAboveMin(minPrice);

        Console.WriteLine($"Total price of products with Price > {minPrice}: {total}");

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


    public decimal GetTotalPriceAboveMin(decimal minPrice)
    {

        return Products
            .Where(p => p.Price > minPrice)
            .Sum(p => p.Price);
    }
}

/*
 Total price of products with Price > 100: 1700.00
 */


/*
In this exercise, you are asked to complete the GetTotalPriceAboveMin method in the ProductRepository class. Here’s a breakdown of the solution:

* 1.Filtering with Where:

The Where clause filters products to include only those with a Price greater than minPrice.

* 2.Using Sum for Aggregation:

After filtering, Sum(p => p.Price) calculates the total Price of the filtered products.

* 3.Example Execution:

Calling GetTotalPriceAboveMin(50) will return the sum of prices for products priced above $100.
 
 */
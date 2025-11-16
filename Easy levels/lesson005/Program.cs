class Program
{
    static void Main()
    {
        var productRepository = new ProductRepository();

        var info = new
        {
             total = productRepository.GetTotalProductCount(),
             lPrice = productRepository.GetTotalPrice(),
             minPrice = productRepository.GetMinPrice(),
             maxPrice = productRepository.GetMaxPrice(),
             averagePrice = productRepository.GetAveragePrice(),
        };

        Console.WriteLine("Product Statistics:");
        Console.WriteLine($"Total Products: {info.total}");
        Console.WriteLine($"Total Price: {info.lPrice}");
        Console.WriteLine($"Minimum Price: {info.minPrice}");
        Console.WriteLine($"Maximum Price: {info.maxPrice}");
        Console.WriteLine($"Average Price: {info.averagePrice}");

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

    // Complete this method
    public int GetTotalProductCount()
    {
        // Use Count to return the total number of products
        return Products.Count();
    }

    // Complete this method
    public decimal GetTotalPrice()
    {
        // Use Sum to return the total price of all products
        return Products.Sum(p => p.Price);
    }

    // Complete this method
    public decimal GetMinPrice()
    {
        // Use Min to return the minimum price among products
        return Products.Min(p => p.Price);
    }

    // Complete this method
    public decimal GetMaxPrice()
    {
        // Use Max to return the maximum price among products
        return Products.Max(p => p.Price);
    }

    // Complete this method
    public decimal GetAveragePrice()
    {
        // Use Average to return the average price of all products
        return Products.Average(p => p.Price);
    }
}

/*
Solution Explanation

In this exercise, you are asked to complete five methods in the ProductRepository class to perform aggregations. Here’s a breakdown of each solution:

GetTotalProductCount: Uses Count to return the total number of products in the Products list.

GetTotalPrice: Uses Sum to add up the Price values of all products.

GetMinPrice: Uses Min to find the lowest Price among all products.

GetMaxPrice: Uses Max to find the highest Price among all products.

GetAveragePrice: Uses Average to calculate the mean of all Price values.
 
*/
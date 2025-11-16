using System.Dynamic;
using System.Numerics;

class Program
{
    static void Main()
    {
        var productRepo = new ProductRepository();

        decimal productOfPrices = productRepo.GetProductOfAllPrices();

        Console.WriteLine($"Product of all product prices: {productOfPrices}");

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


    public decimal GetProductOfAllPrices()
    {
        return Products
           .Select(p => p.Price)
           .Aggregate(1m, (acc, price) => acc * price);
    }
}


/*
    
 Product of all product prices: 6885000000000.000000000000
 */

/*
 In this exercise, you are asked to complete the GetProductOfAllPrices method in the ProductRepository class by using Aggregate. Here’s a breakdown of the solution:

 * 1.Selecting Price Values:

Select(p => p.Price) projects only the Price values from the Products list.

 * 2.Using Aggregate for Custom Aggregation:

Aggregate(1m, (acc, price) => acc * price) starts with an initial accumulator of 1m (a decimal value).

The lambda function (acc, price) => acc * price multiplies each Price by the accumulated value, resulting in the product of all prices.

* 3.Example Execution:

Calling GetProductOfAllPrices() with the sample data will return the product of all prices in the list

acc = 1
acc = 1 * 1200 = 1200
acc = 1200 * 25 = 30,000
acc = 30,000 * 45 = 1,350,000
acc = 1,350,000 * 200 = 270,000,000
acc = 270,000,000 * 300 = 81,000,000,000
acc = 81,000,000,000 * 85 = 6,885,000,000,000
 
 */

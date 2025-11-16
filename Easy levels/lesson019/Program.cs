class Program
{
    static void Main()
    {
        var productRepo = new ProductRepository();

        decimal minPrice = 50;
        decimal maxPrice = 300;
        int countInRange = productRepo.CountProductsInRange(minPrice, maxPrice);
        Console.WriteLine($"Number of products with Price between {minPrice} and {maxPrice}: {countInRange}");

        decimal minCountPrice = 200;
        long expensiveCount = productRepo.CountExpensiveProducts(minCountPrice);
        Console.WriteLine($"Number of products with Price >= {minCountPrice}: {expensiveCount}");

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


    public int CountProductsInRange(decimal minPrice, decimal maxPrice)
    {
        return Products.Count(p => p.Price >= minPrice && p.Price <= maxPrice);
    }


    public long CountExpensiveProducts(decimal minCountPrice)
    {

        return Products.LongCount(p => p.Price >= minCountPrice);
    }
}

/*
Number of products with Price between 50 and 300: 3
Number of products with Price >= 200: 3
*/

/*
In this exercise, you are asked to complete two methods in the ProductRepository class using Count and LongCount. Here’s a breakdown of the solution:

* 1.Using Count in CountProductsInRange:

Count(p => p.Price >= minPrice && p.Price <= maxPrice) counts products within the specified price range.

This method returns an int because it counts a moderate collection of items.

* 2.Using LongCount in CountExpensiveProducts:

LongCount(p => p.Price >= minCountPrice) counts products with prices above or equal to minCountPrice.

This method returns a long, which can handle larger counts.

* 3.Example Execution:

Calling CountProductsInRange(50, 200) will return the number of products priced between $50 and $200.

Calling CountExpensiveProducts(100) will return the count of products priced at $100 or above.
 

Count(predicate) → đếm số phần tử thỏa điều kiện, trả về int.

LongCount(predicate) → đếm số phần tử thỏa điều kiện, trả về long, dùng khi danh sách rất lớn.

 */